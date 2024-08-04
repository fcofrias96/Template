import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent, LoadingComponent, NavbarComponent } from './shared/components';
import { MatToolbarModule } from '@angular/material/toolbar';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,LoadingComponent,NavbarComponent,FooterComponent, MatToolbarModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Template.Web';
}
