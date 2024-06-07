import { Component, OnInit, inject } from '@angular/core';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { UserResponseDto } from '../models/user_response_dto';
import { HttpErrorResponse } from '@angular/common/http';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [MatListModule, MatIconModule, MatButtonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  userService: UserService = inject(UserService);
  authService: AuthService = inject(AuthService);
  isLoading: boolean = false;
  user!: UserResponseDto;
  private router: Router = inject(Router);

  ngOnInit(): void {
    this.getUserInfo();
  }

  getUserInfo(): void {
    this.isLoading = true;
    this.authService.getUserInfo().subscribe({
      next: (user: UserResponseDto) => {
        this.isLoading = false;
        this.user = user;
        console.log(user);
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = true;
        console.log(error);
      },
    });
  }

  signOut() {
    this.authService.logout().subscribe({
      next: (data: any) => {
        this.userService.user.set(null);
        this.router.navigate(['/home']);
      },
    });
  }

  deleteAccount() {}
}
