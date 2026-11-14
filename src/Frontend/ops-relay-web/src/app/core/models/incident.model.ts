export type Priority = 'Low' | 'Medium' | 'High' | 'Critical';

export type IncidentStatus =
  | 'Reported'
  | 'Triaged'
  | 'InProgress'
  | 'Escalated'
  | 'Resolved'
  | 'Closed';

export interface Incident {
  id: string;
  title: string;
  description: string;
  priority: Priority;
  status: IncidentStatus;
  reportedAt: string;
  resolvedAt: string | null;
  slaDeadline: string;
  assignedTeamId: string;
}

export const PRIORITIES: readonly Priority[] = [
  'Low',
  'Medium',
  'High',
  'Critical'
];

export const INCIDENT_STATUSES: readonly IncidentStatus[] = [
  'Reported',
  'Triaged',
  'InProgress',
  'Escalated',
  'Resolved',
  'Closed'
];
