import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiRepository {
  constructor(private http: HttpClient) {}

  get<T>(
    url: string,
    options?: {
      showLoading?: boolean;
      additionalHeaders?: Record<string, string>;
      withCredentials?: boolean;
      responseType?: 'json' | 'blob';
    }
  ): Observable<T> {
    const { showLoading = true, additionalHeaders, responseType, withCredentials } = options || {};
    const headers = this.getHeaders(showLoading ?? true, additionalHeaders);
    return this.http.get<T>(url, {
      headers,
      responseType: responseType as 'json',
      withCredentials,
    });
  }

  post<T>(
    url: string,
    body: unknown,
    options: {
      additionalHeaders?: Record<string, string>;
      withCredentials?: boolean;
      showLoading?: boolean;
    } = {}
  ): Observable<T> {
    const { additionalHeaders, withCredentials, showLoading = true } = options;
    const headers = this.getHeaders(showLoading, additionalHeaders);
    return this.http.post<T>(url, body, { headers, withCredentials });
  }

  put(
    url: string,
    id: string | number,
    body: unknown,
    showLoading = true,
    additionalHeaders: Record<string, string> = {}
  ): Observable<void> {
    const headers = this.getHeaders(showLoading, additionalHeaders);
    return this.http.put<void>(`${url}/${id}`, body, { headers });
  }

  delete(
    url: string,
    showLoading = true,
    additionalHeaders: Record<string, string> = {}
  ): Observable<void> {
    const headers = this.getHeaders(showLoading, additionalHeaders);
    return this.http.delete<void>(url, { headers });
  }

  private getHeaders(
    showLoading: boolean,
    additionalHeaders: Record<string, string> = {}
  ): HttpHeaders {
    let headers = new HttpHeaders();

    if (!showLoading) {
      headers = headers.set('X-No-Loading', 'true');
    }

    for (const [key, value] of Object.entries(additionalHeaders)) {
      headers = headers.set(key, value);
    }

    return headers;
  }
}
