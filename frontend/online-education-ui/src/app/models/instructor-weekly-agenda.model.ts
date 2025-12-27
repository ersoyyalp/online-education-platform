import { InstructorWeeklySchedule } from './instructor-weekly-schedule.model';

export interface InstructorWeeklyAgendaResponse {
  weekStart: string;
  weekEnd: string;
  items: InstructorWeeklySchedule[];
}
