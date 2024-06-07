import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-faqcard',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: './faqcard.component.html',
  styleUrl: './faqcard.component.css',
})
export class FaqcardComponent {
  open: boolean = false;

  @Input() question: string = '';
  @Input() answer: string = '';

  onClick() {
    this.open = !this.open;
  }
}
