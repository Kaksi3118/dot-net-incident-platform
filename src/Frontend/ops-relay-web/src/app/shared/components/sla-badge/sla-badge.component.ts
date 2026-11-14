import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { interval, map } from 'rxjs';

@Component({
  selector: 'app-sla-badge',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sla-badge.component.html',
  styleUrl: './sla-badge.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SlaBadgeComponent {
  readonly slaDeadline = input.required<string>();
  readonly reportedAt = input<string | null>(null);

  private readonly clock = toSignal(
    interval(60000).pipe(map(() => Date.now())),
    { initialValue: Date.now() }
  );

  readonly remainingMinutes = computed(() => {
    const deadline = new Date(this.slaDeadline()).getTime();
    return Math.ceil((deadline - this.clock()) / 60000);
  });

  private readonly totalMinutes = computed(() => {
    const reportedAt = this.reportedAt();

    if (!reportedAt) {
      return 0;
    }

    const total =
      (new Date(this.slaDeadline()).getTime() -
        new Date(reportedAt).getTime()) /
      60000;

    return total > 0 ? total : 0;
  });

  readonly badgeClass = computed(() => {
    const remaining = this.remainingMinutes();

    if (remaining <= 0) {
      return 'sla-breached';
    }

    const total = this.totalMinutes();

    if (total > 0) {
      const percentage = (remaining / total) * 100;

      if (percentage > 50) {
        return 'sla-green';
      }

      if (percentage >= 10) {
        return 'sla-amber';
      }

      return 'sla-red';
    }

    if (remaining < 60) {
      return 'sla-red';
    }

    if (remaining < 240) {
      return 'sla-amber';
    }

    return 'sla-green';
  });

  readonly badgeText = computed(() => {
    const remaining = this.remainingMinutes();

    if (remaining <= 0) {
      return `Breached by ${formatDuration(Math.abs(remaining))}`;
    }

    return `${formatDuration(remaining)} left`;
  });
}

function formatDuration(minutes: number): string {
  const total = Math.max(Math.round(minutes), 0);
  const days = Math.floor(total / 1440);
  const hours = Math.floor((total % 1440) / 60);
  const remainingMinutes = total % 60;

  if (days > 0) {
    return remainingMinutes > 0 ? `${days}d ${hours}h` : `${days}d`;
  }

  if (hours > 0) {
    return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
  }

  return `${remainingMinutes}m`;
}
