import { Injectable } from '@angular/core';
import { ApiService } from 'app/core/services/api.service';
import { environment } from '@environment';
import { ApiRepository } from 'app/core/repositories/api.repository';
import { Subject } from 'rxjs';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class MenuService extends ApiService {
  public getList$: Subject<void> = new Subject<void>();

  constructor(apiRepository: ApiRepository) {
    const baseUrl = `${apiUrl}/menu`;
    super(apiRepository, baseUrl);
  }
}
