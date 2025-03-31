import { Injectable } from '@angular/core';
import { ApiService } from 'app/core/services/api.service';
import { environment } from '@environment';

import { ApiRepository } from 'app/core/repositories/api.repository';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class CategoryService extends ApiService {
  constructor(apiRepository: ApiRepository) {
    const baseUrl = `${apiUrl}/categories`;
    super(apiRepository, baseUrl);
  }
}
