import { Component, OnInit, inject } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PricingcardComponent } from '../pricingcard/pricingcard.component';
import { Router, RouterLink } from '@angular/router';
import { FaqcardComponent } from '../faqcard/faqcard.component';
declare var google: any;
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    HeaderComponent,
    MatIconModule,
    MatButtonModule,
    PricingcardComponent,
    RouterLink,
    FaqcardComponent,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  howItWorksSection: any;
  private router: Router = inject(Router);

  currentYear = new Date().getFullYear();
  email: string = 'email';

  freePlanFeatures = ['Document max size (10mb)', '50 questions', '1 document'];

  premiumPlanFeatures = [
    'Document max size (20mb)',
    '+6000 questions',
    '+100 document',
    'Genrate pdf from your answers',
  ];

  onStartFreeClicked() {
    this.router.navigate(['/start']);
  }

  onFileSelected(event: any) {
    let files = event.target.files;

    if (files) {
      let file: File = files[0];

      console.log(file);
    }
  }

  ngOnInit(): void {}
}
