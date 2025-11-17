import { HttpInterceptorFn } from '@angular/common/http';
import { HttpResponse } from '@angular/common/http';
import { ApiResponse } from '@models/api-response.interface';
import { map } from 'rxjs';

export const apiResponseInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    map((event) => {
      if (event instanceof HttpResponse) {
        const contentType = event.headers.get('Content-Type') || '';
        // Solo mapea si el Content-Type indica JSON
        if (contentType.includes('application/json')) {
          const body = event.body as ApiResponse<unknown>;
          if (body && 'success' in body && body.success) {
            return event.clone({ body: body.data });
          }
        }
      }
      return event;
    })
  );
};
