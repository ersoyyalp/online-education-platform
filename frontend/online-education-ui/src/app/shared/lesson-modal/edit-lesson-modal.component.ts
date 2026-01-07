import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { ScheduleParticipant } from '../../models/instructor-weekly-schedule.model';

@Component({
  standalone: true,
  selector: 'app-edit-lesson-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-lesson-modal.component.html',
  styleUrls: ['./edit-lesson-modal.component.scss'],
})
export class EditLessonModalComponent {
  @Input() lesson!: any;
  @Output() close = new EventEmitter<void>();
  @Output() lessonUpdated = new EventEmitter<void>();

  title = '';
  description = '';
  lessonDate = '';
  startTime = '';
  endTime = '';
  participants: ScheduleParticipant[] = [];

  constructor(private http: HttpClient, private toastr: ToastrService) {}

  ngOnInit() {
    debugger;
    this.title = this.lesson.title;
    this.description = this.lesson.description || '';
    this.lessonDate = this.lesson.date;
    this.startTime = this.lesson.start;
    this.endTime = this.lesson.end;
    this.participants = this.lesson.participants || [];
  }
getInitial(name: string): string {
  return name ? name.charAt(0).toUpperCase() : '';
}

  submit() {
    const payload = {
      title: this.title.trim(),
      description: this.description?.trim() || null,
      startTime: `${this.lessonDate}T${this.startTime}:00`,
      endTime: `${this.lessonDate}T${this.endTime}:00`,
    };

    this.http
      .put(`https://localhost:7050/api/instructor/schedule/${this.lesson.lessonId}`, payload)
      .subscribe({
        next: () => {
          this.toastr.info('Ders başarıyla güncellendi', 'Başarılı');
          this.lessonUpdated.emit();
          this.close.emit();
        },
        error: (err) => {
          const message = err?.error?.message || 'Ders güncellenemedi.';

          this.toastr.error(message, 'İşlem Başarısız');
        },
      });
  }
}
