import {
  ApplicationConfig,
  importProvidersFrom,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { provideNgxMask } from 'ngx-mask';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { apiResponseInterceptor } from './core/interceptors/api-response.interceptor';
import { retryInterceptor } from './core/interceptors/retry.interceptor';
import { tokenRefreshInterceptor } from './core/interceptors/token-refresh.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        authInterceptor,
        loadingInterceptor,
        apiResponseInterceptor,
        errorInterceptor,
        tokenRefreshInterceptor,
        retryInterceptor,
      ])
    ),
    importProvidersFrom(ModalModule.forRoot(), BsModalService),
    provideNgxMask(),
  ],
};
