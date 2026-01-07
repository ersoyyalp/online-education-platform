import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  standalone: true,
  selector: 'app-lesson-request-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './lesson-request-modal.component.html',
  styleUrls: ['./lesson-request-modal.component.scss'],
})
export class LessonRequestModalComponent {

  @Output() close = new EventEmitter<void>();
  @Output() requestCreated = new EventEmitter<void>();

  lessonDate = '';
  startTime = '';
  endTime = '';
  lessonTitle = '';
  lessonDescription = '';

  errors: string[] = [];
  loading = false;

  constructor(
    private http: HttpClient,
    private toastr: ToastrService
  ) {}

submit() {
  this.errors = [];

  if (!this.lessonDate) {
    this.errors.push('Tarih zorunludur.');
  }

  if (!this.startTime) {
    this.errors.push('Başlangıç saati zorunludur.');
  }

  if (!this.endTime) {
    this.errors.push('Bitiş saati zorunludur.');
  }

  if (!this.lessonTitle.trim()) {
    this.errors.push('Ders adı zorunludur.');
  }

  if (this.startTime && this.endTime && this.startTime >= this.endTime) {
    this.errors.push('Bitiş saati, başlangıç saatinden sonra olmalıdır.');
  }

  if (this.errors.length > 0) return;

  const startDateTime = `${this.lessonDate}T${this.startTime}:00`;
  const endDateTime   = `${this.lessonDate}T${this.endTime}:00`;

  const payload = {
    title: this.lessonTitle.trim(),
    description: this.lessonDescription?.trim() || null,
    startTime: startDateTime,
    endTime: endDateTime
  };

  this.http
    .post(
      'https://localhost:7050/api/participant/lesson-requests/request',
      payload
    )
    .subscribe({
      next: () => {
        this.toastr.success(
          'Ders talebin eğitmene iletildi',
          'Başarılı'
        );
        this.requestCreated.emit();
        this.close.emit();
      },
      error: (err) => {
        const msg =
          err?.error?.message || 'Ders talebi oluşturulamadı';
        this.toastr.error(msg, 'Hata');
      },
    });
}

}
