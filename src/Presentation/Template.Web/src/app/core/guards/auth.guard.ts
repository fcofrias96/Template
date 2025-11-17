import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from 'app/features/auth/shared/services/auth.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated) {
    return true;
  } else {
    authService.setRedirectUrl(state.url);
    router.navigate(['/login']);
    return false;
  }
};
