import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { InstructorScheduleComponent } from '../../pages/instructor-schedule/instructor-schedule';
import { ToastrService } from 'ngx-toastr';

interface Participant {
  participantId: number;
  fullName: string;
}

@Component({
  standalone: true,
  selector: 'app-add-lesson-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-lesson-modal.component.html',
  styleUrls: ['./add-lesson-modal.component.scss'],
})
export class AddLessonModalComponent implements OnInit {
  @Output() close = new EventEmitter<void>();
  @Output() lessonCreated = new EventEmitter<void>();

  participants: Participant[] = [];
  selectedIds: number[] = [];
  search = '';
  loading = false;

  errors: string[] = [];

  // 🔥 DATE & TIME
  lessonDate: string = '';
  startTime: string = '';
  endTime: string = '';

  lessonTitle: string = '';
  lessonDescription: string = '';

  constructor(private http: HttpClient, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.loadParticipants();
  }

  loadParticipants() {
    this.loading = true;

    this.http.get<Participant[]>('https://localhost:7050/api/instructor/participants').subscribe({
      next: (res) => {
        this.participants = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        alert('Öğrenciler yüklenemedi');
      },
    });
  }

  toggle(id: number) {
    this.selectedIds.includes(id)
      ? (this.selectedIds = this.selectedIds.filter((x) => x !== id))
      : this.selectedIds.push(id);
  }

  isSelected(id: number): boolean {
    return this.selectedIds.includes(id);
  }

  filteredParticipants(): Participant[] {
    return this.participants.filter((p) =>
      p.fullName.toLowerCase().includes(this.search.toLowerCase())
    );
  }

submit() {
  this.errors = [];

  if (this.selectedIds.length === 0) {
    this.errors.push('En az bir öğrenci seçmelisiniz.');
  }

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

  if (this.errors.length > 0) {
    return;
  }

  const startDateTime = `${this.lessonDate}T${this.startTime}:00`;
  const endDateTime   = `${this.lessonDate}T${this.endTime}:00`;

  const payload = {
    title: this.lessonTitle.trim(),
    description: this.lessonDescription?.trim() || null,
    meetingProvider: null,
    meetingUrl: null,
    startTime: startDateTime,
    endTime: endDateTime,
    participantIds: this.selectedIds,
  };
  debugger;
  this.http
    .post<{ lessonId: number }>(
      'https://localhost:7050/api/instructor/schedule/lessons',
      payload
    )
    .subscribe({
      next: (res) => {
        debugger;
        window.location.reload();
        this.toastr.success('Ders başarıyla eklendi', 'Başarılı');
        this.lessonCreated.emit(); 
        this.close.emit();  
      },
      error: (err) => {
        console.error(err);
          const message = err?.error?.message || 'Ders eklenemedi.';

          this.toastr.error(message, 'İşlem Başarısız');      },
    });
}

}
