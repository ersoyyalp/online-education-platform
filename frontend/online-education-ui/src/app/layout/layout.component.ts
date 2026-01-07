import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { AuthService } from '../core/auth.service';
import { AddLessonModalComponent } from "../shared/lesson-modal/add-lesson-modal.component";
import { LessonRequestModalComponent } from "../shared/lesson-request-modal/lesson-request-modal.component";

@Component({
  standalone: true,
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
  imports: [
    CommonModule,
    RouterOutlet,
    NavbarComponent,
    SidebarComponent,
    AddLessonModalComponent,
    LessonRequestModalComponent
]
})
export class LayoutComponent {

  isSidebarCollapsed = false;

  isAddLessonOpen = false;
  isLessonRequestOpen = false;

  toggleSidebar() {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  // Instructor
  openAddLesson() {
    this.isAddLessonOpen = true;
  }

  closeAddLesson() {
    this.isAddLessonOpen = false;
  }

  // Participant
  openLessonRequest() {
    this.isLessonRequestOpen = true;
  }

  closeLessonRequest() {
    this.isLessonRequestOpen = false;
  }
}
