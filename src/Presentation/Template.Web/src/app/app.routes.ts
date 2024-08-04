import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { LoggedGuard } from './core/guards/logged.guard';
import { NotFoundComponent } from '@shared/components/not-found/not-found.component';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
    canActivate: [LoggedGuard],
  },
  {
    path: 'admin',
    loadChildren: () =>
      import('./features/admin/admin.routes').then((m) => m.ADMIN_ROUTES),
    canActivate: [AuthGuard],
  },

  {
    path: 'categories',
    loadChildren: () =>
      import('./features/category/category.routes').then(
        (m) => m.CATEGORY_ROUTES
      ),
    canActivate: [AuthGuard],
  },
  { path: '', redirectTo: '/admin', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent },
];
