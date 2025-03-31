import { Routes } from '@angular/router';
import { UserComponent } from './features/user/user.component';
import { ResourceComponent } from './features/resource/resource.component';
import { RoleComponent } from './features/role/role.component';
import { PermissionComponent } from './features/permission/permission.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ProductComponent } from './features/product/product.component';
import { CategoryComponent } from './features/category/category.component';
import { NotFoundComponent } from '@shared/components/not-found/not-found.component';

export const routes: Routes = [
  {
    path: 'configuracion',
    canActivate: [authGuard],
    children: [
      {
        path: 'usuarios',
        component: UserComponent,
      },
      {
        path: 'roles',
        component: RoleComponent,
      },
      {
        path: 'permisos',
        component: PermissionComponent,
      },
      {
        path: 'recursos',
        component: ResourceComponent,
      },
    ],
  },

  {
    path: 'categorias',
    component: CategoryComponent,
    canActivate: [authGuard],
  },
  {
    path: 'productos',
    component: ProductComponent,
    canActivate: [authGuard],
  },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [guestGuard] },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent  },
];
