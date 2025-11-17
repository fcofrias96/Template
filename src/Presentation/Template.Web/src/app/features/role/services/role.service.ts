import { Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiService } from 'app/core/services/api.service';
import { environment } from '@environment';

import { ApiRepository } from 'app/core/repositories/api.repository';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class RoleService extends ApiService {
  constructor(apiRepository: ApiRepository) {
    const baseUrl = `${apiUrl}/roles`;
    super(apiRepository, baseUrl);
  }

  public refreshRoles$ = new Subject<void>();
}
