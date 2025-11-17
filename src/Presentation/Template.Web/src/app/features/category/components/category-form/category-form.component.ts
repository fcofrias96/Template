import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CategoryService } from '@features/category/services/category.service';
import { CategoryDetail } from '@features/category/types/category-detail.interface';
import { CreateCategory } from '@features/category/types/create-category.interface';
import { UpdateCategory } from '@features/category/types/update-category.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, FormFieldComponent, ReactiveFormsModule],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss',
})
export class CategoryFormComponent implements OnInit, DynamicModalComponent {
  public categoryForm: FormGroup;
  @Input() categoryId!: number;
  @Input() edit = false;
  closeModal = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService
  ) {
    this.categoryForm = this.initForm();
  }

  ngOnInit(): void {
    this.getCategoryData();
  }

  private getCategoryData() {
    if (this.categoryId && this.edit) {
      this.categoryService
        .getById<CategoryDetail>(this.categoryId)
        .subscribe((category) => {
          this.categoryForm.patchValue(category);
        });
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      id: [null],
      name: [null, Validators.required],
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
    const newCategory: CreateCategory = this.categoryForm.value;
    this.categoryService.create(newCategory).subscribe(() => {
      this.categoryService.refreshCategories$.next();
      this.closeModal.next();
    });
  }

  private update() {
    const updatedCategory: UpdateCategory = {
      ...this.categoryForm.value,
    };
    this.categoryService
      .update(this.categoryId, updatedCategory)
      .subscribe(() => {
        this.categoryService.refreshCategories$.next();
        this.closeModal.next();
      });
  }
}
