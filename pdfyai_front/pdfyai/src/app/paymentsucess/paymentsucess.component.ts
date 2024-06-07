import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink, RouterOutlet } from '@angular/router';
@Component({
  selector: 'app-paymentsucess',
  standalone: true,
  imports: [MatIconModule, RouterLink],
  templateUrl: './paymentsucess.component.html',
  styleUrl: './paymentsucess.component.css',
})
export class PaymentsucessComponent {}
