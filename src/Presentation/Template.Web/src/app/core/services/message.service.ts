import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private snackBar: MatSnackBar) {}

  private show(message: string, action: string, config: MatSnackBarConfig): void {
    this.snackBar.open(message, action, config);
  }

  showError(message: string, action: string = 'Close', duration: number = 3000): void {
    const config: MatSnackBarConfig = {
      duration: duration,
      panelClass: ['snackbar-error']
    };
    this.show(message, action, config);
  }

  showSuccess(message: string, action: string = 'Close', duration: number = 3000): void {
    const config: MatSnackBarConfig = {
      duration: duration,
      panelClass: ['snackbar-success']
    };
    this.show(message, action, config);
  }

  showWarning(message: string, action: string = 'Close', duration: number = 3000): void {
    const config: MatSnackBarConfig = {
      duration: duration,
      panelClass: ['snackbar-warning']
    };
    this.show(message, action, config);
  }
}
