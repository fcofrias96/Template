import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiRepository {
  constructor(private http: HttpClient) {}

  get<T>(url: string, showLoading = true): Observable<T> {
    const headers = this.getHeaders(showLoading);
    return this.http.get<T>(url, { headers });
  }


  post<T>(url: string, body: unknown, showLoading = true): Observable<T> {
    const headers = this.getHeaders(showLoading);
    return this.http.post<T>(url, body, { headers });
  }

  put(url: string, body: unknown, showLoading = true): Observable<void> {
    const headers = this.getHeaders(showLoading);
    return this.http.put<void>(url, body, { headers });
  }

  delete(url: string, showLoading = true): Observable<void> {
    const headers = this.getHeaders(showLoading);
    return this.http.delete<void>(url, { headers });
  }

  private getHeaders(showLoading: boolean): HttpHeaders {
    let headers = new HttpHeaders();
    if (!showLoading) {
      headers = headers.set('X-No-Loading', 'true');
    }
    return headers;
  }
}
