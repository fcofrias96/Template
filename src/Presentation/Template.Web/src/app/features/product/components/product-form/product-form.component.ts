import { Component, EventEmitter, OnInit, Output } from '@angular/core';
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

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, FormFieldComponent, ReactiveFormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent implements OnInit {
  productForm!: FormGroup;
  @Output() closeModal = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      categoryId: [null, [Validators.required]],
      price: [null, [Validators.required, validateNumberOrNegative()]],
      description: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      const newProduct: CreateProduct = this.productForm.value;
      this.productService.createProduct(newProduct).subscribe(() => {
        this.productService.refreshProducts$.next();
        this.closeModal.emit();
      });
    }
  }
}
