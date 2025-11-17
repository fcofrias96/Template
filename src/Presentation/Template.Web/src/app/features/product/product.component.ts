import { Component } from '@angular/core';
import { ModalService } from 'app/shared/services/modal.service';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { ProductListComponent } from './components/product-list/product-list.component';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [ProductListComponent],
  templateUrl: './product.component.html',
  styleUrl: './product.component.scss',
})
export class ProductComponent {
  constructor(private modalService: ModalService) { }

  addProduct() {
    this.modalService.openModal(
      ProductFormComponent,
      'Add Product',
      null,
      {
        backdrop: 'static',
      }
    );
  }
}
