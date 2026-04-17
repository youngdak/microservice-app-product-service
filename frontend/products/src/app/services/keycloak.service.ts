import Keycloak from 'keycloak-js';
import { inject, Injectable } from '@angular/core';
import { EnvService } from './env.service';

@Injectable({
  providedIn: 'root',
})
export class KeycloakService {
  private readonly keycloak: Keycloak;
  private readonly envService = inject(EnvService);

  constructor() {
    this.keycloak = new Keycloak({
      url: this.envService.authority,
      realm: this.envService.realm,
      clientId: this.envService.clientId,
    });
  }

  init(): Promise<boolean> {
    return this.keycloak.init({
      onLoad: 'check-sso',
      pkceMethod: 'S256',
      checkLoginIframe: false,
      redirectUri: this.envService.redirectUri
    });
  }

  login(): Promise<void> {
    return this.keycloak.login();
  }

  logout(): Promise<void> {
    return this.keycloak.logout({
      redirectUri: this.envService.redirectUri,
    });
  }

  getToken(): string | undefined {
    return this.keycloak.token;
  }

  isLoggedIn(): boolean {
    return !!this.keycloak.token;
  }

  getUsername(): string {
    return this.keycloak.tokenParsed?.['preferred_username'];
  }
}
