import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  let authService: AuthService = inject(AuthService);
  let router = inject(Router);
  let res: boolean = authService.isLogged();

  if (!res) {
    router.navigate(['/login']);
  }

  return res;
};
