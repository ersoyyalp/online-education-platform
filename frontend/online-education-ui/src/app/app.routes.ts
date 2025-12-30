import { Routes } from '@angular/router';
import { instructorGuard } from './core/instructor.guard';
import { participantGuard } from './core/participant.guard';

export const routes: Routes = [

  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login.component')
        .then(m => m.LoginComponent)
  },

  {
    path: 'instructor',
    loadComponent: () =>
      import('./layout/layout.component')
        .then(m => m.LayoutComponent),
    canActivate: [instructorGuard],
    children: [
      {
        path: 'schedule',
        loadComponent: () =>
          import('./pages/instructor-schedule/instructor-schedule')
            .then(m => m.InstructorScheduleComponent)
      }
    ]
  },

  {
    path: 'participant',
    loadComponent: () =>
      import('./layout/layout.component')
        .then(m => m.LayoutComponent),
    canActivate: [participantGuard],
    children: [
      {
        path: 'schedule',
        loadComponent: () =>
          import('./pages/participant-schedule/participant-schedule')
            .then(m => m.ParticipantScheduleComponent)
      }
    ]
  },

  { path: '**', redirectTo: 'login' }
];
