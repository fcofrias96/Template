import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { Product } from '../../types/product.interface';
import { CommonModule } from '@angular/common';
import { ModalService } from 'app/shared/services/modal.service';
import { ProductDetailComponent } from '../product-detail/product-detail.component';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
})
export class ProductListComponent implements OnInit, OnDestroy {
  products$: Observable<Product[]>;
  subscription!: Subscription;

  constructor(
    private productService: ProductService,
    private modalService: ModalService
  ) {
    this.products$ = this.productService.getProducts();
  }

  ngOnInit(): void {
    this.subscription = this.productService.refreshProducts$.subscribe(() => {
      this.products$ = this.productService.getProducts();
    });
  }

  selectProduct(productId: string): void {
    this.modalService.openModal(ProductDetailComponent, 'Product Detail', {productId},  {
      class: 'modal-lg',
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
