import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import trLocale from '@fullcalendar/core/locales/tr';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { ParticipantScheduleService } from '../../services/participant-schedule.service';
import { ScheduleParticipant } from '../../models/instructor-weekly-schedule.model';
import { InstructorWeeklyAgendaResponse } from '../../models/instructor-weekly-agenda.model';

interface CalendarEvent {
  title: string;
  date: string;
  start: string;
  end: string;
  participants: ScheduleParticipant[];
  participantCount: number;
  color: string;
}

interface CalendarDay {
  label: string;
  events: CalendarEvent[];
}

@Component({
  standalone: true,
  selector: 'app-participant-schedule',
  imports: [CommonModule],
  templateUrl: './participant-schedule.html',
  styleUrls: ['./participant-schedule.scss'],
})
export class ParticipantScheduleComponent implements OnInit {
  offset = 0;
  agenda: InstructorWeeklyAgendaResponse | null = null;
  days: CalendarDay[] = [];
  selectedEvent: CalendarEvent | null = null;


  constructor(private scheduleService: ParticipantScheduleService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadSchedule();
  }

  loadSchedule(): void {
      this.days = [];
      this.agenda = null;
  
      this.scheduleService.getWeeklySchedule(this.offset).subscribe((res) => {
        this.agenda = res;
  
        // 🔥 Kritik nokta
        this.days = this.buildDays(res);
  
        // 🔥 Angular’a “tamam bu tur bitti” de
        this.cdr.detectChanges();
      });
    }
  
    buildDays(res: InstructorWeeklyAgendaResponse): CalendarDay[] {
      const map: Record<string, CalendarDay> = {};
  
      const start = new Date(res.weekStart);
  
      for (let i = 0; i < 7; i++) {
        const d = new Date(start);
        d.setDate(start.getDate() + i);
  
        // 🔥 UTC YOK, ISO YOK
        const key = d.toLocaleDateString('en-CA'); // YYYY-MM-DD
  
        map[key] = {
          label: d.toLocaleDateString('tr-TR', {
            weekday: 'short',
            day: 'numeric',
          }),
          events: [],
        };
      }
  
      res.items.forEach((item, index) => {
        if (!map[item.date]) return;
  
        map[item.date].events.push({
          title: item.lessonTitle,
          date: item.date,
          start: item.startTime,
          end: item.endTime,
          participants: item.participants,
          participantCount: item.participantCount,
          color: this.getColorByIndex(index),
        });
      });
  
      return Object.values(map);
    }
  
    getColorByIndex(index: number): string {
      const colors = ['#6366f1', '#22c55e', '#0ea5e9', '#f97316', '#ec4899', '#14b8a6'];
      return colors[index % colors.length];
    }
  
    getInitial(name: string): string {
      return name ? name.charAt(0).toUpperCase() : '';
    }
  
    onEventClick(event: MouseEvent, ev: CalendarEvent): void {
      event.preventDefault();
      event.stopPropagation();
      this.selectedEvent = ev;
      document.body.classList.add('modal-open');
    }
  
    closeModal(): void {
      this.selectedEvent = null;
      document.body.classList.remove('modal-open');
    }
  
    nextWeek(): void {
      this.offset++;
      this.loadSchedule();
    }
  
    previousWeek(): void {
      this.offset--;
      this.loadSchedule();
    }
  
    get displayWeekEnd(): Date | null {
      if (!this.agenda) return null;
  
      const end = new Date(this.agenda.weekEnd);
      end.setDate(end.getDate() - 1); // 🔥 1 gün geri
      return end;
    }
}
