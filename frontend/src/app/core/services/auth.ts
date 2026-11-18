import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

import { LoginRequest } from '../../features/auth/models/login-request';
import { LoginResponse } from '../../features/auth/models/login-response';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private http = inject(HttpClient);
  private router = inject(Router);

  private apiUrl = `${environment.apiUrl}/auth`;

  login(credentials: LoginRequest): Observable<LoginResponse> {

    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      credentials
    );

  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }

  getUser(): LoginResponse['user'] | null {
    const storedUser = localStorage.getItem('user');
    if (!storedUser) return null;

    try {
      return JSON.parse(storedUser) as LoginResponse['user'];
    } catch {
      return null;
    }
  }

  storeSession(response: LoginResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(response.user));
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    void this.router.navigate(['/login']);
  }
}
