import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { KeycloakService } from '../../services/keycloak.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
  imports: [RouterLink],
})
export class NavbarComponent {
  public readonly keycloakService: KeycloakService = inject(KeycloakService);

  fixed = false;
  menuOpen = false;

  public toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  public closeMenu(): void {
    this.menuOpen = false;
  }

  public login(): void {
    this.keycloakService.login();
  }

  public logout(): void {
    this.keycloakService.logout();
  }
}
