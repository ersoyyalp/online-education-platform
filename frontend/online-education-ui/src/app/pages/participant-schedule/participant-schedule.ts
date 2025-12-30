import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import trLocale from '@fullcalendar/core/locales/tr';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { ParticipantScheduleService } from '../../services/participant-schedule.service';

@Component({
  standalone: true,
  selector: 'app-participant-schedule',
  imports: [CommonModule, FullCalendarModule],
  templateUrl: './participant-schedule.html',
  styleUrls: ['./participant-schedule.scss'],
})
export class ParticipantScheduleComponent implements OnInit {
  @ViewChild('calendar')
  calendarComponent!: FullCalendarComponent;

  offset = 0;
  calendarOptions: any;

  constructor(private scheduleService: ParticipantScheduleService) {}

  ngOnInit(): void {
    this.calendarOptions = {
      plugins: [timeGridPlugin, interactionPlugin],
      initialView: 'timeGridWeek',
      locale: trLocale,
      firstDay: 1,
      slotMinTime: '08:00:00',
      slotMaxTime: '25:00:00',
      allDaySlot: false,
      headerToolbar: false,
      events: []
    };

    this.loadSchedule();
  }

  loadSchedule(): void {
    this.scheduleService.getWeeklySchedule(this.offset).subscribe(res => {
      const events = res.items.map(item => ({
        title: item.lessonTitle,
        start: `${item.date}T${item.startTime}`,
        end: `${item.date}T${item.endTime}`
      }));

      const api = this.calendarComponent.getApi();
      api.removeAllEvents();
      api.addEventSource(events);
      api.gotoDate(res.weekStart);
    });
  }

  nextWeek() {
    this.offset++;
    this.loadSchedule();
  }

  previousWeek() {
    this.offset--;
    this.loadSchedule();
  }
}
