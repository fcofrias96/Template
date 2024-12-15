import { Category } from '../types/product.interface';
import { map, Observable, Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiService } from 'app/core/services/api.service';
import { environment } from '@environment';
import { CreateCategory } from '../types/create-category.interface';

import { CategoryDetail } from '../types/category-detail.interface';
import { KeyValueOption } from '@models/key-value-option';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class CategoryService extends ApiService {
  private baseUrl = `${apiUrl}/categories`;

  public getCategories(): Observable<KeyValueOption[]> {
    return this.getList<Category>(this.baseUrl).pipe(
      map((data) =>
        data.map(
          (category): KeyValueOption => ({
            key: category.id,
            value: category.name,
          })
        )
      )
    );
  }

  public createCategory(product: CreateCategory): Observable<Category> {
    return this.postData(this.baseUrl, product);
  }

  public getCategory(id: string): Observable<CategoryDetail> {
    return this.getData(`${this.baseUrl}/${id}`);
  }
}
