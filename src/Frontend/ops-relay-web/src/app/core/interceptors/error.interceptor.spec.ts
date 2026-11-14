import {
  HttpClient,
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { fakeAsync, TestBed, tick } from '@angular/core/testing';

import { NotificationService } from '../services/notification.service';
import { errorInterceptor } from './error.interceptor';

describe('errorInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let notificationService: jasmine.SpyObj<NotificationService>;

  beforeEach(() => {
    notificationService = jasmine.createSpyObj<NotificationService>(
      'NotificationService',
      ['showError', 'showSuccess']
    );

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        { provide: NotificationService, useValue: notificationService }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('shows RFC 7807 validation messages', fakeAsync(() => {
    http.get('/api/test').subscribe({ error: () => undefined });

    const request = httpMock.expectOne('/api/test');
    request.flush(
      {
        title: 'One or more validation errors occurred.',
        status: 400,
        errors: {
          Title: ['Title must be at least 5 characters.'],
          Priority: ['Priority must be valid.']
        }
      },
      { status: 400, statusText: 'Bad Request' }
    );

    tick();

    expect(notificationService.showError).toHaveBeenCalledWith(
      'Title must be at least 5 characters., Priority must be valid.'
    );
  }));

  it('retries a 503 response exactly once after one second', fakeAsync(() => {
    http.get('/api/test').subscribe({ error: () => undefined });

    const firstRequest = httpMock.expectOne('/api/test');
    firstRequest.flush('', {
      status: 503,
      statusText: 'Service Unavailable'
    });

    tick(999);
    httpMock.expectNone('/api/test');

    tick(1);
    const secondRequest = httpMock.expectOne('/api/test');
    secondRequest.flush('', {
      status: 503,
      statusText: 'Service Unavailable'
    });

    tick();

    expect(notificationService.showError).toHaveBeenCalledTimes(1);
  }));

  it('shows a fallback message for unhandled server errors', fakeAsync(() => {
    http.get('/api/test').subscribe({ error: () => undefined });

    const request = httpMock.expectOne('/api/test');
    request.flush({}, { status: 500, statusText: 'Server Error' });

    tick();

    expect(notificationService.showError).toHaveBeenCalledWith(
      jasmine.stringMatching(/500/)
    );
  }));
});
