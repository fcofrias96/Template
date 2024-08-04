import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from '@services/message.service';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const userMessage = createUserFriendlyMessage(error);
      messageService.showError(userMessage);
      return throwError(() => new Error(userMessage));
    })
  );
};

function createUserFriendlyMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Could not connect to the server. Please check your network connection.';
  } else if (error.error instanceof ErrorEvent) {
    return `An error occurred: ${error.error.message}`;
  } else {
    const message = error.error.message || 'Something went wrong';
    return `${error.status}: ${message}`;
  }
}
