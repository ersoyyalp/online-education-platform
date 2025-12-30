import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const instructorGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken();
  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  const payload = JSON.parse(atob(token.split('.')[1]));
  const role =
    payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

  if (role !== 'Instructor') {
    router.navigate(['/login']);
    return false;
  }

  return true;
};
