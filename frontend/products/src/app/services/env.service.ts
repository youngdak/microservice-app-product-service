import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment.prod";

declare global {
  interface Window {
    __env?: any;
  }
}

@Injectable({
  providedIn: 'root',
})
export class EnvService {
  production: string =
    window.__env?.PRODUCTION ||
    environment.production;

  baseUrl: string =
    window.__env?.BASE_URL ||
    environment.baseUrl;

  authority: string =
    window.__env?.AUTHORITY ||
    environment.authority;

  clientId: string =
    window.__env?.CLIENT_ID ||
    environment.clientId;

  redirectUri: string =
    window.__env?.REDIRECT_URI ||
    environment.redirectUri;

  postLogoutRedirectUri: string =
    window.__env?.POST_LOGOUT_REDIRECT_URI ||
    environment.postLogoutRedirectUri;

  scope: string =
    window.__env?.SCOPE ||
    environment.scope;

  responseType: string =
    window.__env?.RESPONSE_TYPE ||
    environment.responseType;

  realm: string =
    window.__env?.REALM ||
    environment.realm;
}
