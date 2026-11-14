import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'incidents',
    pathMatch: 'full'
  },
  {
    path: 'incidents',
    loadComponent: () =>
      import('./features/incidents/incident-list/incident-list.component').then(
        module => module.IncidentListComponent
      )
  },
  {
    path: 'incidents/:id',
    loadComponent: () =>
      import('./features/incidents/incident-detail/incident-detail.component').then(
        module => module.IncidentDetailComponent
      )
  },
  {
    path: '**',
    redirectTo: 'incidents'
  }
];
