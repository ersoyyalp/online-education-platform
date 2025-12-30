import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  styleUrls: ['./login.component.scss'],
  template: `
    <div class="login-page">
      <div class="login-card">
        <!-- SOL: FORM -->
        <div class="login-form">
          <h2>LOGIN</h2>
          <p class="subtitle">Hi! How was your day?</p>

          <input type="email" placeholder="Email" [(ngModel)]="email" />

          <input type="password" placeholder="Password" [(ngModel)]="password" />

          <button class="primary-btn" (click)="login()">Login Now</button>

          <div class="remember-row">
            <label class="remember-toggle">
              <input type="checkbox" [(ngModel)]="rememberMe" />
              <span class="slider"></span>
            </label>
            <span class="remember-text">Remember Me</span>
          </div>
        </div>

        <!-- SAĞ: GÖRSEL -->
        <div class="login-visual">
          <img src="/login-teacher3.png" />
        </div>
      </div>
    </div>
  `,
})
export class LoginComponent {
  email = '';
  password = '';
  rememberMe = false;
  isLoading = false;
  constructor(
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}
  login() {
    if (!this.email || !this.password) {
      this.toastr.warning('Please enter both email and password', 'Missing Information');
      return;
    }
    this.isLoading = true;

    this.authService
      .login({
        email: this.email,
        password: this.password,
      })
      .subscribe({
        next: () => {
          this.isLoading = false;

          const role = this.authService.getRole();

          this.toastr.success('Welcome back 👋', 'Login Successful');

          role === 'Instructor'
            ? this.router.navigateByUrl('/instructor/schedule')
            : this.router.navigateByUrl('/participant/schedule');
        },
        error: () => {
          this.isLoading = false;

          this.toastr.error('Email or password is incorrect', 'Login Failed');
        },
      });
  }
}
