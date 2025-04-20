import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subject, takeUntil } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { Product } from '../../types/product.interface';
import { CommonModule } from '@angular/common';
import { ModalService } from 'app/shared/services/modal.service';
import { ProductDetailComponent } from '../product-detail/product-detail.component';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import {
  ActionType,
  TableColumn,
  TableExporting,
} from '@shared/components/generic-table/types/table-column';
import { ProductFormComponent } from '../product-form/product-form.component';
import { PagedCollection } from '@models/paged-collection.interface';
import { ExportService } from 'app/core/services/export.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
})
export class ProductListComponent implements OnInit, OnDestroy {
  products$: Observable<PagedCollection<Product>>;
  columns: TableColumn[];
  itemsPerPage = 15;
  unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private productService: ProductService,
    private modalService: ModalService,
    private exportService: ExportService
  ) {
    this.products$ = this.getProducts();
    this.columns = [
      {
        key: 'name',
        title: 'Name',
        sortable: true,
        filterable: true,
      },
      {
        key: 'price',
        title: 'Price',
        sortable: true,
        filterable: true,
        type: 'number',
      },
      {
        key: 'category',
        title: 'Category',
        sortable: true,
        filterable: true,
      },
      {
        key: 'created',
        title: 'Created',
        sortable: true,
        filterable: true,
        type: 'date',
        dateFormat: 'dd/MM/yyyy',
      },
      {
        key: 'isDeleted',
        title: 'Active',
        sortable: false,
        filterable: false,
        type: 'boolean',
      },
    ];
  }
  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit(): void {
    this.productService.refreshProducts$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.products$ = this.getProducts();
      });
  }

  private getProducts(
    searchParams: URLSearchParams| null = null
  ): Observable<PagedCollection<Product>> {
    if(!searchParams) {
      searchParams = new URLSearchParams(`limit=${this.itemsPerPage}`);
    }
    return this.productService.getPagedList<Product>(searchParams);
  }

  handleFilter(searchParams: URLSearchParams) {
    this.products$ = this.getProducts(searchParams);
  }

  private view(product: Product): void {
    this.modalService.openModal(
      ProductDetailComponent,
      'Product Detail',
      { productId: product.id },
      {
        class: 'modal-lg',
      }
    );
  }

  private edit(product: Product) {
    this.modalService.openModal(
      ProductFormComponent,
      'Edit Product',
      { productId: product.id, edit: true },
      {
        class: 'modal-lg',
      }
    );
  }

  private delete(product: Product) {
    this.modalService
      .openConfirmModal('Are you sure you want to delete this product?')
      .then((result) => {
        if (result) {
          this.productService.delete(product.id).subscribe(() => {
            this.productService.refreshProducts$.next();
          });
        }
      });
  }

  actions(): Partial<Record<ActionType, (product: Product) => void>> {
    return {
      view: (product: Product) => this.view(product),
      edit: (product: Product) => this.edit(product),
      delete: (product: Product) => this.delete(product),
    };
  }

  handleExporting($event: TableExporting) {
    this.exportService.handleExporting({ ...$event, entityName: 'items' });
  }
}
