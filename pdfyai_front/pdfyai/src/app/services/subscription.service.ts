import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { PaymentDto } from '../models/payment_dto';
import { BASE_URL } from '../env/constants';

@Injectable({
  providedIn: 'root',
})
export class SubscriptionService {
  http: HttpClient = inject(HttpClient);

  healthCheck() {
    return this.http.get(BASE_URL + 'health-check');
  }

  invokePayment(paymentDto: PaymentDto) {
    return this.http.post(BASE_URL + 'paypal-webhook', paymentDto);
  }
}
