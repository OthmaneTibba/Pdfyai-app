import {
  AfterViewInit,
  Component,
  NgZone,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserModel } from '../models/user_model';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { CredentialResponse } from 'google-one-tap';

declare var google: any;
@Component({
  selector: 'app-start',
  standalone: true,
  imports: [],
  templateUrl: './start.component.html',
  styleUrl: './start.component.css',
})
export class StartComponent implements OnInit {
  clientId: string = 'google_client_id';

  isLoading: boolean = false;
  private _ngZone: NgZone = inject(NgZone);
  private authService: AuthService = inject(AuthService);
  private userService: UserService = inject(UserService);
  private router: Router = inject(Router);

  ngOnInit(): void {
    //@ts-ignore
    google.accounts.id.initialize({
      client_id: this.clientId,
      callback: this.handleCredentialResponse.bind(this),
      auto_select: false,
      cancel_on_tap_outside: true,
    });

    //@ts-ignore
    google.accounts.id.renderButton(document.getElementById('google-login')!, {
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
          error: (err) => {
            this.isLoading = false;
          },
        });
      });
    } catch (err) {
      this.isLoading = false;
    }
  }
}
