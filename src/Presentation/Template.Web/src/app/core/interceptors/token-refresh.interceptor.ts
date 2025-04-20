import { inject } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn,
  HttpErrorResponse
} from '@angular/common/http';
import { AuthService } from '@features/auth/shared/services/auth.service';
import { BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take, tap } from 'rxjs/operators';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const tokenRefreshInterceptor: HttpInterceptorFn =
  (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
    const authService = inject(AuthService);

    return next(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status !== 401) {
          return throwError(() => error);
        }

        if (!isRefreshing) {
          isRefreshing = true;
          refreshTokenSubject.next(null);

          return authService.refreshToken().pipe(
            tap((newToken: string) => {
              isRefreshing = false;
              authService.saveAccessToken(newToken);
              refreshTokenSubject.next(newToken);
            }),
            switchMap((newToken: string) =>
              next(req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` }
              }))
            ),
            catchError(err => {
              isRefreshing = false;
              authService.logout();
              refreshTokenSubject.error(err);
              return throwError(() => err);
            })
          );
        }

        return refreshTokenSubject.pipe(
          filter(token => token != null),
          take(1),
          switchMap(token =>
            next(req.clone({
              setHeaders: { Authorization: `Bearer ${token}` }
            }))
          ),
          catchError(err => {

            return throwError(() => err);
          })
        );
      })
    );
  };
