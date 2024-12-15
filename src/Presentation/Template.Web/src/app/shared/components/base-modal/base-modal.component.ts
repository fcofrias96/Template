import {
  Component,
  Input,
  OnInit,
  OnDestroy,
  ViewChild,
  ViewContainerRef,
  Type,
  ComponentRef,
} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-base-modal',
  templateUrl: './base-modal.component.html',
  styleUrls: ['./base-modal.component.scss'],
})
export class BaseModalComponent implements OnInit, OnDestroy {
  @Input() title!: string;
  @Input() component!: Type<any>;
  @Input() data: any;

  @ViewChild('dynamicComponent', { read: ViewContainerRef, static: true })
  dynamicComponent!: ViewContainerRef;

  private componentRef!: ComponentRef<any>;
  private subscriptions: Subscription = new Subscription();

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {
    this.loadComponent();
  }

  loadComponent() {
    this.dynamicComponent.clear();
    this.componentRef = this.dynamicComponent.createComponent(this.component);


    if (this.data) {
      Object.keys(this.data).forEach((key) => {
        this.componentRef.instance[key] = this.data[key];
      });
    }


    if (this.componentRef.instance.closeModal) {
      const sub = this.componentRef.instance.closeModal.subscribe(() => this.close());
      this.subscriptions.add(sub);
    }
  }

  close() {
    this.bsModalRef.hide();
  }

  ngOnDestroy(): void {

    this.subscriptions.unsubscribe();


    if (this.componentRef) {
      this.componentRef.destroy();
    }
  }
}
