import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private messageSubject = new BehaviorSubject<{
    type: 'success' | 'error' | 'warning';
    text: string;
  } | null>(null);
  message$ = this.messageSubject.asObservable();

  showSuccess(message: string): void {
    this.messageSubject.next({ type: 'success', text: message });
    this.clearMessage();
  }

  showError(message: string): void {
    this.messageSubject.next({ type: 'error', text: message });
    this.clearMessage();
  }

  showWarning(message: string): void {
    this.messageSubject.next({ type: 'warning', text: message });
    this.clearMessage();
  }

  private clearMessage(): void {
    setTimeout(() => this.messageSubject.next(null), 5000);
  }
}
