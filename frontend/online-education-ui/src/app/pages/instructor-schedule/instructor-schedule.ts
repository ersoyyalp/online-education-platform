import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import trLocale from '@fullcalendar/core/locales/tr';
import { NgZone } from '@angular/core';
import { InstructorScheduleService } from '../../services/instructor-schedule.service';
import { InstructorWeeklyAgendaResponse } from '../../models/instructor-weekly-agenda.model';
import { ViewChild } from '@angular/core';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-instructor-schedule',
  imports: [CommonModule, FullCalendarModule],
  templateUrl: './instructor-schedule.html',
  styleUrls: ['./instructor-schedule.scss'],
})
export class InstructorScheduleComponent implements OnInit {
  @ViewChild('calendar')
  calendarComponent!: FullCalendarComponent;

  offset = 0; // 👈 SADECE BU KALACAK

  agenda: InstructorWeeklyAgendaResponse | null = null;
  calendarEvents: any[] = [];
  selectedEvent: any = null;

  calendarOptions!: any;

  constructor(
    private scheduleService: InstructorScheduleService,
    private zone: NgZone,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initCalendar();
    this.loadSchedule();
  }

  private initCalendar() {
    this.calendarOptions = {
      plugins: [timeGridPlugin, interactionPlugin],
      initialView: 'timeGridWeek',
      locale: trLocale,
      firstDay: 1,
      slotMinTime: '08:00:00',
      slotMaxTime: '25:00:00',
      allDaySlot: false,
      headerToolbar: false,

      nowIndicator: true,
      height: 'auto',
      expandRows: true,

      eventClick: (arg: any) => {
        arg.jsEvent.preventDefault();
        arg.jsEvent.stopPropagation();

        this.zone.run(() => {
          this.selectedEvent = arg.event;
          this.cdr.detectChanges();
        });
      },

      events: []
    };
  }

  loadSchedule(): void {
    this.scheduleService
      .getWeeklySchedule(this.offset) // ✅ SADECE OFFSET
      .subscribe(res => {
        this.agenda = res;

        this.calendarEvents = res.items.map(item => ({
          id: `${item.lessonId}-${item.date}-${item.startTime}`,
          title: item.lessonTitle,
          start: `${item.date}T${item.startTime}`,
          end: `${item.date}T${item.endTime}`,
          classNames: ['lesson-event'],
          extendedProps: {
            participants: item.participants,
            participantCount: item.participantCount
          }
        }));

        const calendarApi = this.calendarComponent.getApi();
        calendarApi.removeAllEvents();
        calendarApi.addEventSource(this.calendarEvents);
        calendarApi.gotoDate(res.weekStart);
      });
  }

  nextWeek() {
    this.selectedEvent = null;
    this.offset++;
    this.loadSchedule();
  }

  previousWeek() {
    this.selectedEvent = null;
    this.offset--;
    this.loadSchedule();
  }
}
