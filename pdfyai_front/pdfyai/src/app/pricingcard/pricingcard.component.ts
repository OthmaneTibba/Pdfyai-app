import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-pricingcard',
  standalone: true,
  imports: [MatIconModule, MatButtonModule],
  templateUrl: './pricingcard.component.html',
  styleUrl: './pricingcard.component.css',
})
export class PricingcardComponent {
  @Input() title: string = '';
  @Input() price: string = '';
  @Input() features: string[] = [];
  @Input() isPremium: boolean = false;
  @Input() buttonTitle = '';

  premiumClass =
    'w-[350px] h-[380px] bg-white shadow-xl my-5 py-10 px-8 flex flex-col justify-between duration-500 scale-100 ease-in-out hover:scale-105 hover:cursor-pointer border border-gray-900 relative';
  freeClass =
    'w-[350px] h-[380px] bg-white shadow-xl my-5 py-10 px-8 flex flex-col justify-between duration-500 scale-100 ease-in-out hover:scale-105 hover:cursor-pointer border border-gray-900';

  @Output() onSubscribeEvent = new EventEmitter();

  onSubscribeClicked(): void {
    this.onSubscribeEvent.emit();
  }
}
