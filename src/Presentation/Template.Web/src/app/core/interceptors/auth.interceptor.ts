import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { refreshToken } from '@features/auth/models/refresh-token';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from 'src/app/features/auth/services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const token = authService.getToken();

  const authReq = req.clone({
    setHeaders: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  });

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        return authService.refreshToken().pipe(
          switchMap((response: refreshToken) => {
            localStorage.setItem('token', response.accessToken);
            const newAuthReq = req.clone({
              setHeaders: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${response.accessToken}`,
              },
            });
            return next(newAuthReq);
          }),
          catchError((refreshTokenError: HttpErrorResponse) => {
            authService.logout().subscribe(() => {
              router.navigate(['/auth/login']);
            });
            return throwError(() => refreshTokenError);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
