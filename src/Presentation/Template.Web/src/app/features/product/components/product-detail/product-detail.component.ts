import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { Observable } from 'rxjs';
import { ProductDetail } from '../../types/product-detail.interface';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss',
})
export class ProductDetailComponent {
  product$: Observable<ProductDetail>;
  @Input() productId!: string;

  constructor(private productService: ProductService) {
    this.product$ = this.productService.getById(this.productId);
  }
}
