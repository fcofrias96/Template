import { Routes } from '@angular/router';
import { UserComponent } from './features/user/user.component';
import { ResourceComponent } from './features/resource/resource.component';
import { RoleComponent } from './features/role/role.component';
import { PermissionComponent } from './features/permission/permission.component';

export const routes: Routes = [
  {
    path: 'configuracion/usuarios',
    component: UserComponent,
  },
  {
    path: 'configuracion/roles',
    component: RoleComponent,
  },
  {
    path: 'configuracion/permisos',
    component: PermissionComponent,
  },
  {
    path: 'configuracion/recursos',
    component: ResourceComponent,
  },
  {
    path: 'categorias',
    component: ResourceComponent,
  },
  {
    path: 'productos',
    component: ResourceComponent,
  },

];
