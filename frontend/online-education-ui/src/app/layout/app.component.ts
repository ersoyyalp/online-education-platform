// import { Component } from '@angular/core';
// import { RouterOutlet } from '@angular/router';
// import { NavbarComponent } from './navbar/navbar.component';
// import { SidebarComponent } from './sidebar/sidebar.component';
// import { CommonModule } from '@angular/common';

// @Component({
//   standalone: true,
//   selector: 'app-root',
//   imports: [CommonModule, RouterOutlet, NavbarComponent, SidebarComponent],
//   template: `
//     <app-navbar></app-navbar>

//     <div class="layout">
//       <app-sidebar></app-sidebar>

//       <main class="content">
//         <router-outlet></router-outlet>
//       </main>
//     </div>
//   `,
//   styleUrls: ['./app.component.scss']
// })
// export class AppComponent {}

import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet],
  template: `<router-outlet></router-outlet>`
})
export class AppComponent {}

