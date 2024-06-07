import { Injectable, signal } from '@angular/core';
import { UserModel } from '../models/user_model';
import { UserResponseDto } from '../models/user_response_dto';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  public user = signal<UserResponseDto | null>(null);
}
