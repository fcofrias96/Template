import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  public loading$ = new BehaviorSubject<boolean>(false);
  private activeRequests = 0;

  show() {
    if (this.activeRequests === 0) {
      this.loading$.next(true);
    }
    this.activeRequests++;
  }

  hide() {
    this.activeRequests--;
    if (this.activeRequests <= 0) {
      this.loading$.next(false);
      this.activeRequests = 0;
    }
  }
}
