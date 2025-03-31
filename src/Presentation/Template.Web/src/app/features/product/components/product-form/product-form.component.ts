import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { CreateProduct } from '../../types/create-product.interface';
import { CommonModule, KeyValue } from '@angular/common';
import { FormFieldComponent } from 'app/shared/components/form-field/form-field.component';
import { validateNumberOrNegative } from 'app/core/utils/validate-number-negative';
import { CategoryService } from '@features/category/services/category.service';
import { FilterSelectComponent } from '@shared/components/filter-select/filter-select.component';import { UpdateProduct } from '@features/product/types/update-product.interface';
import { ProductDetail } from '@features/product/types/product-detail.interface';
import { Category } from '@features/category/types/product.interface';
import { Product } from '@features/product/types/product.interface';


@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    FormFieldComponent,
    ReactiveFormsModule,
    FilterSelectComponent
  ],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent implements OnInit {
  public productForm: FormGroup;
  public product!: ProductDetail;
  public categories: KeyValue<string, string> [] = [];
  @Input() productId: number | null = null;
  @Input() edit = false;
  @Output() closeModal = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService
  ) {
    this.productForm = this.initForm();
  }

  ngOnInit(): void {
    this.categoryService.getList<KeyValue<string, string>>().subscribe((categories) => {
      this.categories = categories;
      if (this.productId && this.edit) {
        this.productService.getById<ProductDetail>(this.productId).subscribe((product) => {
          this.product = product;
          this.productForm.patchValue(product);
        });
      }
    });

    this.productForm
      .get('description')
      ?.valueChanges.subscribe((value) => console.log(value));
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
    if (this.edit) {
      const updatedProduct: UpdateProduct = {
        ...this.product,
        ...this.productForm.value,
      };
      this.productService.update(updatedProduct).subscribe({
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
      this.productService.create(newProduct).subscribe(() => {
        this.productService.refreshProducts$.next();
        this.closeModal.emit();
      });
    }
  }
}

