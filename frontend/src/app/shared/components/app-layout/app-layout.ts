import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Auth } from '../../../core/services/auth';

@Component({ standalone: true, imports: [RouterLink, RouterLinkActive, RouterOutlet], selector: 'app-layout', templateUrl: './app-layout.html', styleUrl: './app-layout.css' })
export class AppLayout {
  private readonly auth = inject(Auth);
  get user() { return this.auth.getUser(); }
  logout(): void { this.auth.logout(); }
}
