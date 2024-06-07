import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BASE_URL } from '../env/constants';
import { Observable } from 'rxjs';
import { UserResponseDto } from '../models/user_response_dto';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http: HttpClient = inject(HttpClient);
  private userService: UserService = inject(UserService);

  loginWithGoogle(credential: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    return this.http.post(
      BASE_URL + 'auth/google-signup',
      JSON.stringify(credential),
      { headers: headers }
    );
  }

  getUserInfo(): Observable<UserResponseDto> {
    return this.http.get<UserResponseDto>(BASE_URL + 'user-info');
  }

  healthCheck() {
    return this.http.get(BASE_URL + 'health-check');
  }

  isLogged(): boolean {
    return this.userService.user() != null;
  }

  logout() {
    //@ts-ignore
    google.accounts.id.disableAutoSelect();

    return this.http.post(BASE_URL + 'signout', null);
  }
}
