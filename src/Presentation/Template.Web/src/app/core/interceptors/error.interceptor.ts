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
      let errorMessage = 'An unknown error occurred!';

      if (error.error instanceof ErrorEvent) {
        errorMessage = `Client error: ${error.error.message}`;
        notificationService.showWarning(errorMessage);
      } else {
        if (error.status >= 400 && error.status < 500) {
          errorMessage = `Warning: ${
            error.error?.message || 'Request could not be processed.'
          }`;
          notificationService.showWarning(errorMessage);
        } else {
          errorMessage = `Server error: ${error.message}`;
          notificationService.showError(errorMessage);
        }
      }

      console.error('An error occurred:', error);
      return throwError(() => new Error(errorMessage));
    })
  );
};
