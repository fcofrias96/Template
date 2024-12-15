import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { Product } from '../../types/product.interface';
import { CommonModule } from '@angular/common';
import { ModalService } from 'app/shared/services/modal.service';
import { ProductDetailComponent } from '../product-detail/product-detail.component';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import { TableAction, TableColumn } from '@shared/components/generic-table/types/table-column';
import { ProductFormComponent } from '../product-form/product-form.component';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
})
export class ProductListComponent implements OnInit, OnDestroy {
  products$: Observable<Product[]>;
  subscription!: Subscription;
  columns: TableColumn[];
  itemsPerPage: number = 15;

  constructor(
    private productService: ProductService,
    private modalService: ModalService
  ) {
    this.products$ = this.productService.getProducts();
    this.columns = [
      { key: 'name', title: 'Name', sortable: true, filterable: true },
      { key: 'price', title: 'Price', sortable: true, filterable: true },
      { key: 'category', title: 'Category', sortable: true, filterable: true },
    ];
  }

  ngOnInit(): void {
    this.subscription = this.productService.refreshProducts$.subscribe(() => {
      this.products$ = this.productService.getProducts();
    });
  }

  selectProduct(productId: string): void {
    this.modalService.openModal(
      ProductDetailComponent,
      'Product Detail',
      { productId },
      {
        class: 'modal-lg',
      }
    );
  }

  handleAction(event: TableAction) {
    if (event.action === 'edit') {

      this.modalService.openModal(
        ProductFormComponent,
        'Edit Product',
        { productId: event.item.id,
          edit: true
         },
        {
          class: 'modal-lg',
        }
      );

    } else if (event.action === 'delete') {

      this.modalService.openConfirmModal('Are you sure you want to delete this product?').then((result) => {

        if (result) {
          this.productService.deleteProduct(event.item.id).subscribe({

            next: () => {
            this.productService.refreshProducts$.next();}
            ,error: (err) => {
              console.error(err);
            }
          });
        }
      });

    }
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
