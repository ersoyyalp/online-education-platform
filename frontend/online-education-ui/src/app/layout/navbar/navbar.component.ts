import {
  Component,
  EventEmitter,
  Output,
  OnInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({
  standalone: true,
  selector: 'app-navbar',
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  @Output() toggleSidebar = new EventEmitter<void>();

  // Instructor → mevcut ders ekleme modalı
  @Output() addLesson = new EventEmitter<void>();

  // Participant → ders talebi modalı
  @Output() addLessonRequest = new EventEmitter<void>();

  role: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();
  }

  onAddLessonClick(): void {
    if (this.role === 'Instructor') {
      this.addLesson.emit();
      return;
    }

    if (this.role === 'Participant') {
      this.addLessonRequest.emit();
      return;
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
