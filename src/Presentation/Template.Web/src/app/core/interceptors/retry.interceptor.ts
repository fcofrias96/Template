import { HttpErrorResponse } from '@angular/common/http';
import { HttpInterceptorFn } from '@angular/common/http';
import { retry, timer, throwError } from 'rxjs';

export const retryInterceptor: HttpInterceptorFn = (req, next) => {
  const maxRetries = 2;
  const baseDelayMs = 1000;

  if (req.method !== 'GET') {
    return next(req);
  }

  return next(req).pipe(
    retry({
      count: maxRetries,
      delay: (error: HttpErrorResponse, retryCount: number) => {
        const { status } = error;
        if (status < 500 || status >= 600) {
          return throwError(() => error);
        }
        return timer((retryCount + 1) * baseDelayMs);
      },
    })
  );
};
