import {
  Component,
  Input,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { Observable, Subject } from 'rxjs';
import { ProductDetail } from '../../types/product-detail.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss',
})
export class ProductDetailComponent
  implements OnChanges, DynamicModalComponent {
  product$!: Observable<ProductDetail>;
  @Input() productId!: string;

  constructor(private productService: ProductService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['productId'] && this.productId) {
      this.product$ = this.productService.getById(this.productId);
    }
  }
  closeModal = new Subject<void>();
}
