import { Routes } from '@angular/router';
import { InstructorScheduleComponent } from './pages/instructor-schedule/instructor-schedule';

export const routes: Routes = [
  {
    path: 'instructor/schedule',
    component: InstructorScheduleComponent
  },
  {
    path: '',
    redirectTo: 'instructor/schedule',
    pathMatch: 'full'
  }
];
