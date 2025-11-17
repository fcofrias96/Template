import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CategoryService } from '@features/category/services/category.service';
import { CategoryDetail } from '@features/category/types/category-detail.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-category-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category-detail.component.html',
  styleUrl: './category-detail.component.scss',
})
export class CategoryDetailComponent
  implements OnChanges, DynamicModalComponent {
  category$!: Observable<CategoryDetail>;
  @Input() categoryId!: string;

  constructor(private categoryService: CategoryService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryId'] && this.categoryId) {
      this.category$ = this.categoryService.getById(this.categoryId);
    }
  }
  closeModal = new Subject<void>();
}
