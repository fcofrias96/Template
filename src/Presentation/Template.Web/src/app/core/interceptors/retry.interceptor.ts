import { HttpInterceptorFn } from '@angular/common/http';
import { retry } from 'rxjs/operators';
import { timer } from 'rxjs';

export const retryInterceptor: HttpInterceptorFn = (req, next) => {
  const maxRetries = 2;
  const baseDelayMs = 1000;

  if (req.method !== 'GET') {
    return next(req);
  }

  return next(req).pipe(
    retry({
      count: maxRetries,
      delay: (_error, retryCount) => timer((retryCount + 1) * baseDelayMs),
    })
  );
};
