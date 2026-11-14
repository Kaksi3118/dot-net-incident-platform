import { Priority } from './incident.model';

export interface SlaPolicy {
  id: string;
  name: string;
  responseTimeHours: number;
  resolutionTimeHours: number;
  priority: Priority;
}
