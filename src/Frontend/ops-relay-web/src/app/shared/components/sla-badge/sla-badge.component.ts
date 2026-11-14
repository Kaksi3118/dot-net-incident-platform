import { Component, input } from '@angular/core';

@Component({
  selector: 'app-sla-badge',
  standalone: true,
  template: `<span class="sla-badge sla-green">{{ slaDeadline() }}</span>`
})
export class SlaBadgeComponent {
  readonly slaDeadline = input.required<string>();
  readonly reportedAt = input<string>();
}
