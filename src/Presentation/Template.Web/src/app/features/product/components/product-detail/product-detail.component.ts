import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../types/product.interface';
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
export class ProductDetailComponent implements OnInit {

  product$!: Observable<ProductDetail>;
  @Input() productId!: string;


  constructor(private productService: ProductService) {

  }


  ngOnInit(): void {
    this.product$ = this.productService.getProduct(this.productId);
  }
}
