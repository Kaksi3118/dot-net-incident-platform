import { IncidentStatus, Priority } from './incident.model';

export interface IncidentQueryParams {
  pageNumber?: number;
  pageSize?: number;
  status?: IncidentStatus | null;
}

export interface CreateIncidentRequest {
  title: string;
  description: string;
  priority: Priority;
  assignedTeamId: string;
}

export interface UpdateStatusRequest {
  newStatus: IncidentStatus;
  reason: string;
}
