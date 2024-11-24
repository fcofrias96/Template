import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiRepository } from '../repositories/api.repository';
import { HttpHeaders } from '@angular/common/http';
import { PagedCollection } from '@models/paged-collection.interface';

@Injectable({
  providedIn: 'root'
})
export abstract class ApiService {
  constructor(private apiRepository: ApiRepository) {}

  getData<T>(url: string, showLoading: boolean = true): Observable<T> {
    return this.apiRepository.get<T>(url, showLoading).pipe(
      map(response => response.data)
    );
  }

  getList<T>(url: string, showLoading: boolean = true): Observable<T[]> {
    return this.apiRepository.get<T[]>(url, showLoading).pipe(
      map(response => response.data)
    );
  }

  getPagedList<T>(url: string, showLoading: boolean = true): Observable<PagedCollection<T>> {
    return this.apiRepository.get<PagedCollection<T>>(url, showLoading).pipe(
      map(response => response.data)
    );
  }

  postData<T>(url: string, body: any, showLoading: boolean = true): Observable<T> {
    return this.apiRepository.post<T>(url, body, showLoading).pipe(
      map(response => response.data)
    );
  }


  putData<T>(url: string, body: any, showLoading: boolean = true): Observable<T> {
    return this.apiRepository.put<T>(url, body, showLoading).pipe(
      map(response => response.data)
    );
  }

  deleteData<T>(url: string, showLoading: boolean = true): Observable<T> {
    return this.apiRepository.delete<T>(url, showLoading).pipe(
      map(response => response.data)
    );
  }

  private getHeaders(showLoading: boolean): HttpHeaders {
    let headers = new HttpHeaders();
    if (!showLoading) {
      headers = headers.set('X-No-Loading', 'true');
    }
    return headers;
  }


}
