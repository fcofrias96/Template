import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { LoadingService } from 'app/core/services/loading.service';
import { Observable } from 'rxjs/internal/Observable';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loading-spinner.component.html',
  styleUrl: './loading-spinner.component.scss',
})
export class LoadingSpinnerComponent {
  loading$: Observable<boolean>;

  constructor(private loadingService: LoadingService) {
    this.loading$ = this.loadingService.loading$;
  }
}
