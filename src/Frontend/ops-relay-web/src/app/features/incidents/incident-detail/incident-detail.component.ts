import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  signal
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatStepperModule } from '@angular/material/stepper';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';

import {
  Incident,
  IncidentStatus,
  Priority
} from '../../../core/models/incident.model';
import { IncidentService } from '../../../core/services/incident.service';
import { NotificationService } from '../../../core/services/notification.service';
import { SlaBadgeComponent } from '../../../shared/components/sla-badge/sla-badge.component';
import {
  StatusTransitionDialogComponent,
  StatusTransitionDialogData,
  StatusTransitionResult
} from '../dialogs/status-transition-dialog.component';

const LIFECYCLE_STEPS: readonly IncidentStatus[] = [
  'Reported',
  'Triaged',
  'InProgress',
  'Escalated',
  'Resolved',
  'Closed'
];

const ALLOWED_TRANSITIONS: Record<IncidentStatus, IncidentStatus[]> = {
  Reported: ['Triaged', 'Closed'],
  Triaged: ['InProgress', 'Escalated', 'Closed'],
  InProgress: ['Escalated', 'Resolved', 'Triaged'],
  Escalated: ['InProgress', 'Resolved'],
  Resolved: ['Closed', 'InProgress'],
  Closed: []
};

@Component({
  selector: 'app-incident-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatStepperModule,
    SlaBadgeComponent
  ],
  templateUrl: './incident-detail.component.html',
  styleUrl: './incident-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class IncidentDetailComponent {
  private readonly incidentService = inject(IncidentService);
  private readonly dialog = inject(MatDialog);
  private readonly notificationService = inject(NotificationService);

  readonly id = input.required<string>();
  readonly incident = signal<Incident | null>(null);
  readonly loading = signal(false);
  readonly lifecycleSteps = LIFECYCLE_STEPS;

  readonly currentStepIndex = computed(() => {
    const incident = this.incident();

    if (!incident) {
      return 0;
    }

    const index = LIFECYCLE_STEPS.indexOf(incident.status);
    return index >= 0 ? index : 0;
  });

  readonly allowedStatuses = computed(() => {
    const incident = this.incident();
    return incident ? ALLOWED_TRANSITIONS[incident.status] : [];
  });

  constructor() {
    effect(() => {
      const id = this.id();
      this.loadIncident(id);
    });
  }

  loadIncident(id: string = this.id()): void {
    this.loading.set(true);

    this.incidentService
      .getIncidentById(id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: incident => this.incident.set(incident),
        error: () => this.incident.set(null)
      });
  }

  openStatusDialog(): void {
    const incident = this.incident();

    if (!incident || this.allowedStatuses().length === 0) {
      return;
    }

    const data: StatusTransitionDialogData = {
      allowedStatuses: [...this.allowedStatuses()]
    };

    this.dialog
      .open<
        StatusTransitionDialogComponent,
        StatusTransitionDialogData,
        StatusTransitionResult
      >(StatusTransitionDialogComponent, {
        width: '440px',
        data
      })
      .afterClosed()
      .subscribe(result => {
        if (!result) {
          return;
        }

        this.incidentService.updateStatus(incident.id, result).subscribe({
          next: () => {
            this.notificationService.showSuccess(
              `Incident moved to ${result.newStatus}.`
            );
            this.loadIncident();
          }
        });
      });
  }

  priorityClass(priority: Priority): string {
    switch (priority) {
      case 'Critical':
        return 'bg-red-100 text-red-800';
      case 'High':
        return 'bg-orange-100 text-orange-800';
      case 'Medium':
        return 'bg-amber-100 text-amber-800';
      default:
        return 'bg-emerald-100 text-emerald-800';
    }
  }

  statusClass(status: IncidentStatus): string {
    switch (status) {
      case 'Resolved':
      case 'Closed':
        return 'bg-slate-100 text-slate-700';
      case 'Escalated':
        return 'bg-red-100 text-red-800';
      case 'InProgress':
        return 'bg-indigo-100 text-indigo-800';
      case 'Triaged':
        return 'bg-amber-100 text-amber-800';
      default:
        return 'bg-sky-100 text-sky-800';
    }
  }

  stepDescription(step: IncidentStatus): string {
    switch (step) {
      case 'Reported':
        return 'Incident has been reported and is waiting for triage.';
      case 'Triaged':
        return 'The incident has been reviewed and assigned to a team.';
      case 'InProgress':
        return 'The team is actively working on remediation.';
      case 'Escalated':
        return 'The incident requires additional attention or ownership.';
      case 'Resolved':
        return 'A fix has been applied and the incident is awaiting closure.';
      case 'Closed':
        return 'The incident is closed and no further action is required.';
      default:
        return '';
    }
  }
}
