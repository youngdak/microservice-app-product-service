export const environment = {
  production: true,
  baseUrl: 'http://localhost:7890/',
  authority: 'http://localhost:8080',
  clientId: 'angular-product-service',
  redirectUri: 'http://localhost:4200',
  postLogoutRedirectUri:'http://localhost:4200',
  scope:'openid profile offline_access',
  responseType:'code',
  realm: 'microservice-app',
};
