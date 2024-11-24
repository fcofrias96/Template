import { Component } from '@angular/core';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { CategoryComponent } from '../category/category.component';
import { BrowserModule } from '@angular/platform-browser';
import { ModalService } from 'app/shared/services/modal.service';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { ProductListComponent } from './components/product-list/product-list.component';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [ProductListComponent],
  templateUrl: './product.component.html',
  styleUrl: './product.component.scss',
})
export class ProductComponent {
  constructor(private modalService: ModalService) {}

  addProduct() {

    const modal =this.modalService.openModal(ProductFormComponent, 'Add Product', null, {
      backdrop: 'static',

    });

  }
}
