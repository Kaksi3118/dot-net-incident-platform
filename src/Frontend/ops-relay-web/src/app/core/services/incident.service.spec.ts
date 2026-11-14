import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { Incident } from '../models/incident.model';
import { IncidentService } from './incident.service';

describe('IncidentService', () => {
  let service: IncidentService;
  let httpMock: HttpTestingController;

  const incident: Incident = {
    id: '11111111-1111-1111-1111-111111111111',
    title: 'Database outage',
    description: 'Primary database unavailable',
    priority: 'Critical',
    status: 'Reported',
    reportedAt: '2026-11-12T09:00:00Z',
    resolvedAt: null,
    slaDeadline: '2026-11-12T13:00:00Z',
    assignedTeamId: 'SRE-Ops'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(IncidentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('sends pagination and status filters when listing incidents', () => {
    service
      .getIncidents({ pageNumber: 2, pageSize: 5, status: 'Reported' })
      .subscribe(result => {
        expect(result.items).toEqual([incident]);
        expect(result.totalCount).toBe(1);
      });

    const request = httpMock.expectOne(
      candidate =>
        candidate.url === '/api/v1/incidents' &&
        candidate.params.get('pageNumber') === '2' &&
        candidate.params.get('pageSize') === '5' &&
        candidate.params.get('status') === 'Reported'
    );

    expect(request.request.method).toBe('GET');
    request.flush({
      items: [incident],
      pageNumber: 2,
      pageSize: 5,
      totalCount: 1,
      totalPages: 1,
      hasNextPage: false,
      hasPreviousPage: true
    });
  });

  it('loads a single incident by id', () => {
    service.getIncidentById(incident.id).subscribe(result => {
      expect(result).toEqual(incident);
    });

    const request = httpMock.expectOne(`/api/v1/incidents/${incident.id}`);
    expect(request.request.method).toBe('GET');
    request.flush(incident);
  });

  it('creates incidents with a POST request', () => {
    service
      .createIncident({
        title: 'Database outage',
        description: 'Primary database unavailable',
        priority: 'Critical',
        assignedTeamId: 'SRE-Ops'
      })
      .subscribe();

    const request = httpMock.expectOne('/api/v1/incidents');
    expect(request.request.method).toBe('POST');
    request.flush(incident, { status: 201, statusText: 'Created' });
  });

  it('patches incident status', () => {
    service
      .updateStatus(incident.id, {
        newStatus: 'Triaged',
        reason: 'Triaged by on-call'
      })
      .subscribe();

    const request = httpMock.expectOne(
      `/api/v1/incidents/${incident.id}/status`
    );

    expect(request.request.method).toBe('PATCH');
    expect(request.request.body).toEqual({
      newStatus: 'Triaged',
      reason: 'Triaged by on-call'
    });

    request.flush(null, { status: 204, statusText: 'No Content' });
  });
});
