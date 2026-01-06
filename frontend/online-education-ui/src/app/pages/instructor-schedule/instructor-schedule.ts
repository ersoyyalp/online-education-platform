import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InstructorScheduleService } from '../../services/instructor-schedule.service';
import { InstructorWeeklyAgendaResponse } from '../../models/instructor-weekly-agenda.model';
import { ScheduleParticipant } from '../../models/instructor-weekly-schedule.model';
import { AddLessonModalComponent } from '../../shared/lesson-modal/add-lesson-modal.component';
import { EditLessonModalComponent } from '../../shared/lesson-modal/edit-lesson-modal.component';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';

interface CalendarEvent {
  lessonId: number;
  title: string;
  description?: string;
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
  selector: 'app-instructor-schedule',
  imports: [CommonModule, AddLessonModalComponent, EditLessonModalComponent],
  templateUrl: './instructor-schedule.html',
  styleUrls: ['./instructor-schedule.scss'],
})
export class InstructorScheduleComponent implements OnInit {
  offset = 0;
  agenda: InstructorWeeklyAgendaResponse | null = null;
  days: CalendarDay[] = [];

  selectedEvent: CalendarEvent | null = null;

  showAddLessonModal = false;
  showEditLessonModal = false;

  // 🗑️ DELETE
  lessonToDelete: CalendarEvent | null = null;
  showDeleteConfirm = false;

  constructor(
    private scheduleService: InstructorScheduleService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef,
    private http: HttpClient, 
  ) {}

  ngOnInit(): void {
    this.loadSchedule();
  }

  loadSchedule(): void {
    this.days = [];
    this.agenda = null;

    this.scheduleService.getWeeklySchedule(this.offset).subscribe(res => {
      this.agenda = res;
      this.days = this.buildDays(res);
      this.cdr.detectChanges();
    });
  }

  buildDays(res: InstructorWeeklyAgendaResponse): CalendarDay[] {
    const map: Record<string, CalendarDay> = {};
    const start = new Date(res.weekStart);

    for (let i = 0; i < 7; i++) {
      const d = new Date(start);
      d.setDate(start.getDate() + i);

      const key = d.toLocaleDateString('en-CA');
      map[key] = {
        label: d.toLocaleDateString('tr-TR', { weekday: 'short', day: 'numeric' }),
        events: [],
      };
    }

    res.items.forEach((item, index) => {
      if (!map[item.date]) return;

      map[item.date].events.push({
        lessonId: item.lessonId,
        title: item.lessonTitle,
        description: item.description,
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
    this.showEditLessonModal = true;
  }

  onDeleteClick(event: MouseEvent, ev: CalendarEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.lessonToDelete = ev;
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void {
    this.lessonToDelete = null;
    this.showDeleteConfirm = false;
  }

  

  confirmDelete(): void {
    if (!this.lessonToDelete) return;

     this.http
      .delete(`https://localhost:7050/api/instructor/schedule/${this.lessonToDelete.lessonId}`)
      .subscribe({
        next: () => {
          this.toastr.info('Ders başarıyla silindi', 'Başarılı');
        this.showDeleteConfirm = false;
        this.lessonToDelete = null;
        this.loadSchedule();
        },
        error: (err) => {
          const message = err?.error?.message || 'Ders silinemedi.';

          this.toastr.error(message, 'İşlem Başarısız');  this.toastr.error(
          err?.error?.message || 'Ders silinemedi',
          'Hata'
        );
        },
      });
    // this.scheduleService.deleteLesson(this.lessonToDelete.lessonId).subscribe({
    //   next: () => {
    //     this.toastr.success('Ders silindi', 'Başarılı');
    //     this.showDeleteConfirm = false;
    //     this.lessonToDelete = null;
    //     this.loadSchedule();
    //   },
    //   error: (err) => {
    //     this.toastr.error(
    //       err?.error?.message || 'Ders silinemedi',
    //       'Hata'
    //     );
    //   },
    // });
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
    end.setDate(end.getDate() - 1);
    return end;
  }

  onLessonCreated(): void {
    this.showAddLessonModal = false;
    this.loadSchedule();
  }

  onLessonUpdated(): void {
    this.showEditLessonModal = false;
    this.loadSchedule();
  }
}
