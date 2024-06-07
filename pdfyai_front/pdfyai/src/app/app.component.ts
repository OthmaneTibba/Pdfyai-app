import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { UserService } from './services/user.service';
import { AuthService } from './services/auth.service';
import { UserResponseDto } from './models/user_response_dto';
import { routes } from './app.routes';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  title = 'pdfyai';

  message: string = '';
  private router: Router = inject(Router);

  userService: UserService = inject(UserService);
  authService: AuthService = inject(AuthService);

  ngOnInit(): void {
    if (this.userService.user() == null) {
      this.authService.getUserInfo().subscribe({
        next: (user: UserResponseDto) => {
          this.userService.user.set(user);
          this.router.navigate(['/documents']);
        },
      });
    }
  }
}
