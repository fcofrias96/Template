import { Component, Input, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { CreateProduct } from '../../types/create-product.interface';
import { CommonModule } from '@angular/common';
import { FormFieldComponent } from 'app/shared/components/form-field/form-field.component';
import { validateNumberOrNegative } from 'app/core/utils/validate-number-negative';
import { CategoryService } from '@features/category/services/category.service';
import { FilterSelectComponent } from '@shared/components/filter-select/filter-select.component';
import { UpdateProduct } from '@features/product/types/update-product.interface';
import { ProductDetail } from '@features/product/types/product-detail.interface';
import { CategoryDetail } from '@features/category/types/category-detail.interface';
import { map, Observable, Subject } from 'rxjs';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { Catalog } from '@models/catalog';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    FormFieldComponent,
    ReactiveFormsModule,
    FilterSelectComponent,
  ],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent implements OnInit, DynamicModalComponent {
  public productForm: FormGroup;
  public categories$: Observable<Catalog[]>;
  @Input() productId!: number;
  @Input() edit = false;
  closeModal = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService
  ) {
    this.productForm = this.initForm();
    this.categories$ = this.categoryService.getCatalogList();
  }

  ngOnInit(): void {
    this.getProductData();
  }

  private getProductData() {
    if (this.productId && this.edit) {
      this.productService
        .getById<ProductDetail>(this.productId)
        .subscribe((product) => {
          this.productForm.patchValue(product);
        });
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      id: [null],
      name: [null, Validators.required],
      categoryId: [null, [Validators.required]],
      price: [null, [Validators.required, validateNumberOrNegative()]],
      description: [null, Validators.required],
    });
  }

  onSubmit(): void {
    if (this.edit) {
      this.update();
    } else {
      this.create();
    }
  }

  private create() {
    const newProduct: CreateProduct = this.productForm.value;
    this.productService.create(newProduct).subscribe(() => {
      this.productService.refreshProducts$.next();
      this.closeModal.next();
    });
  }

  private update() {
    const updatedProduct: UpdateProduct = {
      ...this.productForm.value,
    };

    this.productService.update(this.productId, updatedProduct).subscribe(() => {
      this.productService.refreshProducts$.next();
      this.closeModal.next();
    });
  }
}
