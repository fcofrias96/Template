import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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
import { Observable } from 'rxjs';
import { CategoryService } from '@features/category/services/category.service';
import { KeyValueOption } from '@models/key-value-option';
import { FilterSelectComponent } from '@shared/components/filter-select/filter-select.component';
import { Product } from '@features/product/types/product.interface';
import { UpdateProduct } from '@features/product/types/update-product.interface';
import { ProductDetail } from '@features/product/types/product-detail.interface';

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
export class ProductFormComponent implements OnInit {
  productForm: FormGroup;
  product: ProductDetail | null = null;
  categories: KeyValueOption[] = [];
  @Input() productId: number | null = null;
  @Input() edit: boolean = false;
  @Output() closeModal = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService
  ) {
    this.productForm = this.initForm();
  }

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe((categories) => {
      this.categories = categories;
      if (this.productId && this.edit) {
        this.productService.getProduct(this.productId).subscribe((product) => {
          this.product = product;
          this.productForm.patchValue(product);
        });
      }
    });
  }

  initForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      categoryId: [null, [Validators.required]],
      price: [null, [Validators.required, validateNumberOrNegative()]],
      description: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      if (this.edit) {
        const updatedProduct: UpdateProduct = {
          ...this.product,
          ...this.productForm.value,
        };
        this.productService.updateProduct(updatedProduct).subscribe({
          next: () => {
            this.productService.refreshProducts$.next();
            this.closeModal.emit();
          },
          error: (err) => {
            console.error(err);
          },
        });
      } else {
        const newProduct: CreateProduct = this.productForm.value;
        this.productService.createProduct(newProduct).subscribe(() => {
          this.productService.refreshProducts$.next();
          this.closeModal.emit();
        });
      }
    }
  }
}
