import { Routes } from '@angular/router';

import { Login } from './features/auth/pages/login/login';
import { Home } from './features/dashboard/pages/home/home';
import { authGuard, guestGuard } from './core/guards/auth.guard';
import { AppLayout } from './shared/components/app-layout/app-layout';
import { Categories } from './features/categories/pages/categories/categories';
import { Tasks } from './features/tasks/pages/tasks/tasks';

export const routes: Routes = [

  {
    path: 'login',
    component: Login,
    canActivate: [guestGuard]
  },

  { path: '', component: AppLayout, canActivate: [authGuard], children: [
    { path: 'home', component: Home },
    { path: 'categories', component: Categories },
    { path: 'tasks', component: Tasks },
    { path: '', redirectTo: 'home', pathMatch: 'full' }
  ] },

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: '**',
    redirectTo: 'login'
  }

];
