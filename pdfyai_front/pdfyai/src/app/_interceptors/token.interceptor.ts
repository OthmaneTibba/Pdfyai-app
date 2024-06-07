import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { UserService } from '../services/user.service';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  let userService = inject(UserService);
  let router: Router = inject(Router);

  req = req.clone({
    withCredentials: true,
  });

  return next(req).pipe(
    catchError((erorr: HttpErrorResponse) => {
      if (erorr.status === 401) {
        userService.user.set(null);
        router.navigate(['/home']);
      }
      return throwError(() => erorr);
    })
  );
};
