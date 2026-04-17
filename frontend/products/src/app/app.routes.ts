import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home/home') },
  { path: 'health', loadComponent: () => import('./pages/health/health') },
  {
    path: 'products',
    loadComponent: () => import('./pages/products/products'),
    canActivate: [authGuard],
  }
];
