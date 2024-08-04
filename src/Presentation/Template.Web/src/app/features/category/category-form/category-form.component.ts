import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CategoryDto } from '../models/category.dto';
import { CategoryService } from '../service/category.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatButtonModule],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.css',
})
export class CategoryFormComponent implements OnInit {
  categoryForm: FormGroup;
  isEditMode = false;
  categoryId: number | undefined;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.isEditMode = true;
        this.categoryId = Number(id);
        this.categoryService.getCategory(this.categoryId).subscribe((data) => {
          this.categoryForm.patchValue(data);
        });
      }
    });
  }

  onSubmit(): void {
    if (this.categoryForm.valid) {
      const category: CategoryDto = this.categoryForm.value;
      if (this.isEditMode && this.categoryId !== undefined) {
        this.categoryService
          .updateCategory(this.categoryId, category)
          .subscribe(() => {
            this.router.navigate(['/categories']);
          });
      } else {
        this.categoryService.createCategory(category).subscribe(() => {
          this.router.navigate(['/categories']);
        });
      }
    }
  }
}
