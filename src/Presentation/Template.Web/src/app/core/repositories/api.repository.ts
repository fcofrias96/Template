import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.interface';
import { NotificationService } from '../services/notification.service';

@Injectable({
  providedIn: 'root',
})
export class ApiRepository {
  constructor(
    private http: HttpClient,
    private notificationService: NotificationService
  ) {}

  get<T>(url: string, showLoading: boolean = true): Observable<ApiResponse<T>> {
    const headers = this.getHeaders(showLoading);
    return this.http
      .get<ApiResponse<T>>(url, { headers })
      .pipe(catchError(error => this.handleError(error)));
  }

  post<T>(
    url: string,
    body: any,
    showLoading: boolean = true
  ): Observable<ApiResponse<T>> {
    const headers = this.getHeaders(showLoading);
    return this.http
      .post<ApiResponse<T>>(url, body, { headers })
      .pipe(catchError(error => this.handleError(error)));
  }

  put<T>(
    url: string,
    body: any,
    showLoading: boolean = true
  ): Observable<ApiResponse<T>> {
    const headers = this.getHeaders(showLoading);
    return this.http
      .put<ApiResponse<T>>(url, body, { headers })
      .pipe(catchError(error => this.handleError(error)));
  }

  delete<T>(
    url: string,
    showLoading: boolean = true
  ): Observable<ApiResponse<T>> {
    const headers = this.getHeaders(showLoading);
    return this.http
      .delete<ApiResponse<T>>(url, { headers })
      .pipe(catchError(error => this.handleError(error)));
  }

  private getHeaders(showLoading: boolean): HttpHeaders {
    let headers = new HttpHeaders();
    if (!showLoading) {
      headers = headers.set('X-No-Loading', 'true');
    }
    return headers;
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client error: ${error.error.message}`;
      this.notificationService.showWarning(errorMessage);
    } else {
      if (error.status >= 400 && error.status < 500) {
        errorMessage = `Warning: ${
          error.error?.message || 'Request could not be processed.'
        }`;
        this.notificationService.showWarning(errorMessage);
      } else {
        errorMessage = `Server error: ${error.message}`;
        this.notificationService.showError(errorMessage);
      }
    }

    console.error('An error occurred:', error);
    return throwError(() => new Error(errorMessage));
  }
}
