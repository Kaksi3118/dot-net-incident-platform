import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, retry, throwError, timer } from 'rxjs';

import { ProblemDetails } from '../models/problem-details.model';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const notificationService = inject(NotificationService);

  return next(request).pipe(
    retry({
      count: 1,
      delay: (error: unknown) => {
        if (error instanceof HttpErrorResponse && error.status === 503) {
          return timer(1000);
        }

        return throwError(() => error);
      }
    }),
    catchError((error: HttpErrorResponse) => {
      notificationService.showError(extractErrorMessage(error));
      return throwError(() => error);
    })
  );
};

function extractErrorMessage(error: HttpErrorResponse): string {
  const payload = error.error;

  if (payload && typeof payload === 'object') {
    const problem = payload as ProblemDetails;

    if (problem.errors) {
      const messages = Object.values(problem.errors)
        .flat()
        .filter((message): message is string => Boolean(message));

      if (messages.length > 0) {
        return messages.join(', ');
      }
    }

    if (problem.detail) {
      return problem.detail;
    }

    if (problem.title) {
      return problem.title;
    }
  }

  return error.message || 'An unexpected error occurred.';
}
