import { Observable } from 'rxjs';
import { ApiRepository } from '../repositories/api.repository';
import { PagedCollection } from '@models/paged-collection.interface';
import { Dropdown } from '@models/dropdown';

export abstract class ApiService {
  constructor(protected apiRepository: ApiRepository, protected url: string) {}

  getById<T>(id: string | number, showLoading = true): Observable<T> {
    return this.apiRepository.get<T>(`${this.url}/${id}`, { showLoading });
  }

  get<T>(showLoading = true): Observable<T> {
    return this.apiRepository.get<T>(`${this.url}`, { showLoading });
  }

  //@Cacheable()
  getDropdownList(showLoading = true): Observable<Dropdown[]> {
    return this.apiRepository.get<Dropdown[]>(`${this.url}/dropdown`, { showLoading });
  }

  getPagedList<T>(
    searchParams: URLSearchParams | null = null,
    showLoading = true
  ): Observable<PagedCollection<T>> {
    return this.apiRepository.get<PagedCollection<T>>(
      `${this.url}${searchParams ? '?' + searchParams : ''}`,
      { showLoading }
    );
  }

  create<T>(body: unknown, showLoading = true): Observable<T> {
    return this.apiRepository.post<T>(this.url, body, { showLoading });
  }

  update(
    id: string | number,
    body: unknown,
    showLoading = true
  ): Observable<void> {
    return this.apiRepository.put(this.url, id, body, showLoading);
  }

  delete(id: string | number, showLoading = true): Observable<void> {
    return this.apiRepository.delete(`${this.url}/${id}`, showLoading);
  }
}
