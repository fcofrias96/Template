import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { PagedCollection } from '@models/wrappers';
import { Observable } from 'rxjs';
import { CategoryCreateDto } from '../models/category-create.dto';
import { CategoryUpdateDto } from '../models/category-update.dto';
import { CategoryDto } from '../models/category.dto';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${environment.baseUrl}/Categories`;

  constructor(private http: HttpClient) { }

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(this.apiUrl);
  }

  getCategory(id: number): Observable<CategoryDto> {
    return this.http.get<CategoryDto>(`${this.apiUrl}/${id}`);
  }

  createCategory(Category: CategoryCreateDto): Observable<CategoryDto> {
    return this.http.post<CategoryDto>(this.apiUrl, Category);
  }

  updateCategory(id: number, Category: CategoryUpdateDto): Observable<CategoryDto> {
    return this.http.put<CategoryDto>(`${this.apiUrl}/${id}`, Category);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
