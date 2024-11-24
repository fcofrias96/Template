import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from '../services/loading.service';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  const noLoading = req.headers.has('X-No-Loading')
  if (noLoading) {
    return next(req);
  }

  loadingService.show();

  return next(req).pipe(finalize(() => loadingService.hide()));
};
