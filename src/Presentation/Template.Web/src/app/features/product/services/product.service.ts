import { Product } from '../types/product.interface';
import { map, Observable, Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiService } from 'app/core/services/api.service';
import { environment } from '@environment';
import { CreateProduct } from '../types/create-product.interface';
import { PagedCollection } from '@models/paged-collection.interface';
import { ProductDetail } from '../types/product-detail.interface';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class ProductService extends ApiService {
  private baseUrl = `${apiUrl}/items`;
  public refreshProducts$ = new Subject<void>();

  public getProducts(): Observable<Product[]> {
    return this.getPagedList<Product>(this.baseUrl).pipe(map(data => data.elements));
  }

  public createProduct(product: CreateProduct): Observable<Product> {
    return this.postData(this.baseUrl, product);
  }

  public getProduct(id: string): Observable<ProductDetail> {
    return this.getData(`${this.baseUrl}/${id}`);
  }
}
