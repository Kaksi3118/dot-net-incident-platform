import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';

import {
  INCIDENT_STATUSES,
  Incident,
  IncidentStatus,
  Priority
} from '../../../core/models/incident.model';
import { IncidentService } from '../../../core/services/incident.service';
import { SlaBadgeComponent } from '../../../shared/components/sla-badge/sla-badge.component';

const PRIORITY_RANK: Record<Priority, number> = {
  Low: 1,
  Medium: 2,
  High: 3,
  Critical: 4
};

@Component({
  selector: 'app-incident-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSelectModule,
    MatFormFieldModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    SlaBadgeComponent
  ],
  templateUrl: './incident-list.component.html',
  styleUrl: './incident-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class IncidentListComponent implements OnInit {
  private readonly incidentService = inject(IncidentService);

  readonly incidents = signal<Incident[]>([]);
  readonly loading = signal<boolean>(false);
  readonly totalCount = signal<number>(0);
  readonly pageIndex = signal<number>(0);
  readonly pageSize = signal<number>(10);
  readonly selectedStatus = signal<IncidentStatus | null>(null);

  readonly statuses = INCIDENT_STATUSES;
  readonly displayedColumns: string[] = [
    'title',
    'priority',
    'status',
    'reportedAt',
    'slaDeadline',
    'assignedTeamId'
  ];

  private readonly sortState = signal<{
    active: string;
    direction: 'asc' | 'desc' | '';
  }>({ active: '', direction: '' });

  readonly sortedIncidents = computed(() => {
    const incidents = [...this.incidents()];
    const { active, direction } = this.sortState();

    if (!active || !direction) {
      return incidents;
    }

    const factor = direction === 'asc' ? 1 : -1;

    return incidents.sort(
      (left, right) => factor * this.compare(left, right, active)
    );
  });

  ngOnInit(): void {
    this.loadIncidents();
  }

  loadIncidents(): void {
    this.loading.set(true);

    this.incidentService
      .getIncidents({
        pageNumber: this.pageIndex() + 1,
        pageSize: this.pageSize(),
        status: this.selectedStatus()
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: page => {
          this.incidents.set(page.items);
          this.totalCount.set(page.totalCount);
        },
        error: () => {
          this.incidents.set([]);
          this.totalCount.set(0);
        }
      });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadIncidents();
  }

  onStatusFilterChange(status: IncidentStatus | null): void {
    this.selectedStatus.set(status);
    this.pageIndex.set(0);
    this.loadIncidents();
  }

  onSortChange(sort: Sort): void {
    this.sortState.set({
      active: sort.active,
      direction: sort.direction
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

  private compare(left: Incident, right: Incident, column: string): number {
    switch (column) {
      case 'title':
        return left.title.localeCompare(right.title);
      case 'priority':
        return PRIORITY_RANK[left.priority] - PRIORITY_RANK[right.priority];
      case 'status':
        return left.status.localeCompare(right.status);
      case 'reportedAt':
        return (
          new Date(left.reportedAt).getTime() -
          new Date(right.reportedAt).getTime()
        );
      case 'slaDeadline':
        return (
          new Date(left.slaDeadline).getTime() -
          new Date(right.slaDeadline).getTime()
        );
      case 'assignedTeamId':
        return left.assignedTeamId.localeCompare(right.assignedTeamId);
      default:
        return 0;
    }
  }
}
