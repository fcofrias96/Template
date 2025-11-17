import { Component } from '@angular/core';
import { CategoryListComponent } from './components/category-list/category-list.component';
import { CommonModule } from '@angular/common';
import { ModalService } from '@shared/services/modal.service';
import { CategoryFormComponent } from './components/category-form/category-form.component';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CategoryListComponent, CommonModule],
  templateUrl: './category.component.html',
  styleUrl: './category.component.scss',
})
export class CategoryComponent {
  constructor(private modalService: ModalService) { }

  addCategory() {
    const modal = this.modalService.openModal(
      CategoryFormComponent,
      'Add Product',
      null,
      {
        backdrop: 'static',
      }
    );
  }
}
