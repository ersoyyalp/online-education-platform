import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginRequest, LoginResponse } from './auth.models';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private readonly baseUrl = 'https://localhost:7050/api/auth';
  private readonly TOKEN_KEY = 'access_token';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest) {
    return this.http
      .post<LoginResponse>(`${this.baseUrl}/login`, request)
      .pipe(
        tap(res => {
          localStorage.setItem(this.TOKEN_KEY, res.token);
        })
      );
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  }
}
