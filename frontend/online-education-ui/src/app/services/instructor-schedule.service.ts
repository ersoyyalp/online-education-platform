import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InstructorWeeklyAgendaResponse } from '../models/instructor-weekly-agenda.model';

@Injectable({ providedIn: 'root' })
export class InstructorScheduleService {
  private baseUrl = 'https://localhost:7050/api/instructor/schedule';

  constructor(private http: HttpClient) {}

  getWeeklySchedule(offset: number) {
    return this.http.get<InstructorWeeklyAgendaResponse>(
      `${this.baseUrl}/weekly?offset=${offset}`
    );
  }
}
