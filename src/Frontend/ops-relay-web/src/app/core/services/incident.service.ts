import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateIncidentRequest,
  IncidentQueryParams,
  UpdateStatusRequest
} from '../models/incident-requests.model';
import { Incident } from '../models/incident.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class IncidentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/incidents`;

  getIncidents(params: IncidentQueryParams): Observable<PagedResult<Incident>> {
    let httpParams = new HttpParams();

    if (params.pageNumber !== undefined) {
      httpParams = httpParams.set('pageNumber', params.pageNumber);
    }

    if (params.pageSize !== undefined) {
      httpParams = httpParams.set('pageSize', params.pageSize);
    }

    if (params.status) {
      httpParams = httpParams.set('status', params.status);
    }

    return this.http.get<PagedResult<Incident>>(this.baseUrl, {
      params: httpParams
    });
  }

  getIncidentById(id: string): Observable<Incident> {
    return this.http.get<Incident>(`${this.baseUrl}/${id}`);
  }

  createIncident(request: CreateIncidentRequest): Observable<Incident> {
    return this.http.post<Incident>(this.baseUrl, request);
  }

  updateStatus(id: string, request: UpdateStatusRequest): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/status`, request);
  }
}
