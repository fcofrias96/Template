import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { ApiResponse } from '@models/wrappers';
import { MessageService } from '@services/message.service';
import { map } from 'rxjs';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    map(event => {
      if (event instanceof HttpResponse) {
        const responseBody = event.body as ApiResponse<any>;
        if (responseBody?.success) {
          if (responseBody.data) {
            return event.clone({ body: responseBody.data });
          } else {
            messageService.showSuccess(responseBody.message || 'Success');
          }
        }
      }
      return event;
    })
  );
};

