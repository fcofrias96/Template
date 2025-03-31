import { Observable } from 'rxjs';
import { ApiRepository } from '../repositories/api.repository';
import { PagedCollection } from '@models/paged-collection.interface';
import { Cacheable } from 'ts-cacheable';

export abstract class ApiService {
  constructor(protected apiRepository: ApiRepository, protected url: string) {}

  getById<T>(id: string | number, showLoading = true): Observable<T> {
    return this.apiRepository.get<T>(`${this.url}/${id}`, showLoading);
  }

  @Cacheable()
  getList<T>(showLoading = true): Observable<T[]> {
    return this.apiRepository.get<T[]>(this.url, showLoading);
  }

  @Cacheable()
  getPagedList<T>(
    searchParams: string | null,
    showLoading = true
  ): Observable<PagedCollection<T>> {
    return this.apiRepository.get<PagedCollection<T>>(
      `${this.url}${searchParams ? '?' + searchParams : ''}`,
      showLoading
    );
  }

  create<T>(body: unknown, showLoading = true): Observable<T> {
    return this.apiRepository.post<T>(this.url, body, showLoading);
  }

  update(body: unknown, showLoading = true): Observable<void> {
    return this.apiRepository.put(this.url, body, showLoading);
  }

  delete(showLoading = true): Observable<void> {
    return this.apiRepository.delete(this.url, showLoading);
  }
}
