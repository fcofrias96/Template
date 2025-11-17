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
    data: {
      title: 'Configuración',
    },
    children: [
      {
        path: 'usuarios',
        component: UserComponent,
        data: {
          title: 'Usuarios',
        },
      },
      {
        path: 'roles',
        component: RoleComponent,
        data: {
          title: 'Roles',
        },
      },
      {
        path: 'permisos',
        component: PermissionComponent,
        data: {
          title: 'Permisos',
        },
      },
      {
        path: 'recursos',
        component: ResourceComponent,
        data: {
          title: 'Recursos',
        },
      },
    ],
  },

  {
    path: 'categorias',
    component: CategoryComponent,
    canActivate: [authGuard],
    data: {
      title: 'Categorias',
    },
  },
  {
    path: 'productos',
    component: ProductComponent,
    canActivate: [authGuard],
    data: {
      title: 'Productos',
    },
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [guestGuard],
    data: { title: 'Iniciar Sesión' },
  },
  { path: 'register', component: RegisterComponent, canActivate: [guestGuard], data: { title: 'Registrar' } },
  { path: 'dashboard', component: DashboardComponent, data: { title: 'Dashboard' } },
  { path: 'forgot-password', component: ForgotPasswordComponent, data: { title: 'Recuperar Contraseña' } },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full', },
  { path: '**', component: NotFoundComponent, data: { title: '404 - Not Found' } },
];
