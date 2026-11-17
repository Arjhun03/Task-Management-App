import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../../../../core/services/auth';
import { LoginRequest } from '../../models/login-request';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  private fb = inject(FormBuilder);
  private auth = inject(Auth);
  private router = inject(Router);

  showPassword = signal(false);
  isLoading = signal(false);
  loginError = signal('');

  loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [false]
  });

  onSubmit(): void {

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loginError.set('');
    this.isLoading.set(true);

    const credentials: LoginRequest = {
      email: this.loginForm.controls.email.value,
      password: this.loginForm.controls.password.value
    };

    this.auth.login(credentials).subscribe({

      next: (response) => {

        // Store JWT token
        localStorage.setItem('token', response.token);

        // Store user information
        localStorage.setItem(
          'user',
          JSON.stringify(response.user)
        );

        console.log('Login successful!');
        console.log('Logged in user:', response.user);

        // Navigate to home page
        this.router.navigate(['/home']);

      },

      error: (error) => {

        console.error('Login failed:', error);

        this.loginError.set(
          error.error?.message || 'Invalid email or password.'
        );

        this.isLoading.set(false);
      },

      complete: () => {
        this.isLoading.set(false);
      }

    });

  }

  togglePassword(): void {
    this.showPassword.update(value => !value);
  }
}
