import { HttpInterceptorFn } from '@angular/common/http';
import { HttpErrorResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';
import { inject } from '@angular/core';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const errorMessage = error.error.message;

      if (error.error instanceof ErrorEvent) {
        notificationService.showWarning(errorMessage);
        console.warn('An error EVENT occurred:', error.error.message);
      } else {
        if (error.status >= 400 && error.status < 500) {
          notificationService.showWarning(errorMessage);
        } else {
          notificationService.showError(errorMessage);
        }
      }

      return throwError(() => new Error(errorMessage));
    })
  );
};
