import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { KeycloakService } from '../services/keycloak.service';

export const authGuard: CanActivateFn = () => {
  const keycloak = inject(KeycloakService);
  const router = inject(Router);

  if (keycloak.isLoggedIn()) {
    return true;
  }

  keycloak.login();
  return false;
};
