import { AfterViewInit, Component, NgZone, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CredentialResponse } from 'google-one-tap';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatProgressSpinnerModule,
    MatIconModule,
    MatToolbarModule,
    MatMenuModule,
    MatSnackBarModule,
    RouterLink,
  ],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements AfterViewInit {
  private _snackBar: MatSnackBar = inject(MatSnackBar);
  constructor(private _ngZone: NgZone, private router: Router) {}

  clientId: string =
    '154973198055-rtm0rkdp2vvrujg5hq41h6n86ogch6hs.apps.googleusercontent.com';

  authService: AuthService = inject(AuthService);

  userService: UserService = inject(UserService);

  isLoading: boolean = false;

  openMenuClass =
    'flex flex-col gap-3 fixed top-0 left-0 h-screen w-[200px] text-white px-5 duration-700 ease-in-out bg-gray-900 md:hidden';
  closeMenuClass =
    'flex flex-col gap-3 fixed top-0 left-[-100%] h-screen w-[200px] text-white px-5 duration-700 ease-in-out bg-gray-900 md:hidden';

  isOpen: boolean = false;

  toggleMenu() {
    this.isOpen = !this.isOpen;
    console.log(this.isOpen);
  }
  scroolToPricing() {
    document.getElementById('pricing')?.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
      inline: 'nearest',
    });
  }

  scroolToFeaturse() {
    document.getElementById('features')?.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
      inline: 'nearest',
    });
  }

  scroolToHome() {
    document.getElementById('home')?.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
      inline: 'nearest',
    });
  }

  ngAfterViewInit(): void {
    //@ts-ignore
    google.accounts.id.initialize({
      client_id: this.clientId,
      callback: this.handleCredentialResponse.bind(this),
      auto_select: false,
      cancel_on_tap_outside: true,
    });
    //@ts-ignore
    google.accounts.id.renderButton(document.getElementById('buttonDiv')!, {
      theme: 'outline',
      size: 'medium',
      text: 'signin_with',
      logo_alignment: 'center',
    });
  }

  async handleCredentialResponse(response: CredentialResponse) {
    this.isLoading = true;
    try {
      this._ngZone.run(() => {
        this.authService.loginWithGoogle(response.credential).subscribe({
          next: (r: any) => {
            this.router.navigate(['/documents']);
            this.isLoading = false;
          },
          error: (err: HttpErrorResponse) => {
            this.isLoading = false;
            if (err.status == 400) {
              this._snackBar.open('Error occured please try again', 'close');
            }
          },
        });
      });
    } catch (err) {
      this.isLoading = false;
    }
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: (res: any) => {
        this.userService.user.set(null);
        this.router.navigate(['/home']);
      },
    });
  }
}
