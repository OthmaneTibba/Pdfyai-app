import {
  Component,
  ElementRef,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import { SubscriptionService } from '../services/subscription.service';
import { PaymentDto } from '../models/payment_dto';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-subscription',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: './subscription.component.html',
  styleUrl: './subscription.component.css',
})
export class SubscriptionComponent implements OnInit, OnDestroy {
  @ViewChild('paypal', { static: true }) paypalBtn!: ElementRef;

  subscriptionService: SubscriptionService = inject(SubscriptionService);

  healthCheckSub$!: Subscription;
  confirmPatmentSub$!: Subscription;
  subscriptions: Subscription[] = [];
  private router: Router = inject(Router);
  private _ngZone: NgZone = inject(NgZone);
  isProssessing: boolean = false;

  premiumPlanFeatures = [
    'Document max size (20mb)',
    '+6000 questions',
    '+100 document',
    'Genrate pdf from your answers',
  ];

  ngOnInit(): void {
    this.healthCheckSub$ = this.subscriptionService.healthCheck().subscribe({
      next: (data: any) => {
        this.subscriptions.push(this.healthCheckSub$);
        window.paypal
          .Buttons({
            style: {
              color: 'blue',
              shape: 'rect',
              label: 'paypal',
            },
            createOrder: (data: any, actions: any) => {
              return actions.order.create({
                purchase_units: [
                  {
                    amount: {
                      value: '10',
                      currency_code: 'USD',
                    },
                  },
                ],
              });
            },
            onApprove: (data: any, actions: any) => {
              return actions.order.capture().then((details: any) => {
                this._ngZone.run(() => {
                  let paymentDto: PaymentDto = {
                    paymentId: details.id,
                  };
                  this.isProssessing = true;
                  this.confirmPatmentSub$ = this.subscriptionService
                    .invokePayment(paymentDto)
                    .subscribe({
                      next: (data: any) => {
                        this.isProssessing = false;
                        this.subscriptions.push(this.confirmPatmentSub$);
                        this.router.navigate(['/payment-confirmed']);
                      },
                      error: (error) => {
                        this.isProssessing = false;
                      },
                    });
                });
              });
            },
            onError: (error: any) => {
              console.log(error);
            },
          })
          .render(this.paypalBtn.nativeElement);
      },
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub$: Subscription) => {
      sub$.unsubscribe();
    });
  }
}
