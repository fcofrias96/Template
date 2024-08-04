import { Routes } from '@angular/router';
import { CategoryDetailComponent } from './category-detail/category-detail.component';
import { CategoryFormComponent } from './category-form/category-form.component';
import { CategoryListComponent } from './category-list/category-list.component';


export const CATEGORY_ROUTES: Routes = [
  { path: '', component: CategoryListComponent },
  { path: 'detail/:id', component: CategoryDetailComponent },
  { path: 'create', component: CategoryFormComponent },
  { path: 'edit/:id', component: CategoryFormComponent }
];
