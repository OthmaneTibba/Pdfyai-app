import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ChatComponent } from './chat/chat.component';
import { loginGuard } from './_guards/login.guard';
import { DocumentsComponent } from './documents/documents.component';
import { MypdfeditorComponent } from './mypdfeditor/mypdfeditor.component';
import { SubscriptionComponent } from './subscription/subscription.component';
import { authGuard } from './_guards/auth.guard';
import { PaymentsucessComponent } from './paymentsucess/paymentsucess.component';
import { StartComponent } from './start/start.component';
import { PrivacyComponent } from './privacy/privacy.component';

export const routes: Routes = [
  {
    path: 'home',
    component: LoginComponent,
    canActivate: [loginGuard],
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'documents',
    component: DocumentsComponent,
  },
  {
    path: 'chat/:id',
    component: ChatComponent,
  },
  {
    path: 'payment-confirmed',
    component: PaymentsucessComponent,
    canActivate: [authGuard],
  },
  {
    path: 'editor',
    component: MypdfeditorComponent,
  },
  {
    path: 'subscription',
    component: SubscriptionComponent,
  },
  {
    path: 'start',
    component: StartComponent,
  },
  {
    path: 'profile',
    loadComponent: () =>
      import('./profile/profile.component').then((c) => c.ProfileComponent),
  },
  {
    path: 'privacy-policy',
    component: PrivacyComponent,
  },
  {
    path: '**',
    component: LoginComponent,
    canActivate: [authGuard],
  },
];
