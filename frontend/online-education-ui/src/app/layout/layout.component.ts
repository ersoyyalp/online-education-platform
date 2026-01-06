import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { AuthService } from '../core/auth.service';
import { AddLessonModalComponent } from "../shared/lesson-modal/add-lesson-modal.component";

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
    AddLessonModalComponent
]
})
export class LayoutComponent {
  isSidebarCollapsed = false;
  isAddLessonOpen = false;

  toggleSidebar() {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  openAddLesson() {
    this.isAddLessonOpen = true;
  }

  closeAddLesson() {
    this.isAddLessonOpen = false;
  }
}
