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
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-base-modal',
  templateUrl: './base-modal.component.html',
  styleUrls: ['./base-modal.component.scss'],
})
export class BaseModalComponent implements OnInit, OnDestroy {
  @Input() title!: string;
  @Input() component!: Type<DynamicModalComponent>;
  @Input() data!: Record<string, unknown>;

  @ViewChild('dynamicComponent', { read: ViewContainerRef, static: true })
  dynamicComponent!: ViewContainerRef;

  private componentRef!: ComponentRef<DynamicModalComponent>;
  private unsubscribe$ = new Subject<void>();

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.loadComponent();
  }

  loadComponent(): void {
    this.dynamicComponent.clear();
    this.componentRef = this.dynamicComponent.createComponent(this.component);

    if (this.data) {
      Object.entries(this.data).forEach(([key, value]) => {
        this.componentRef.setInput(key, value);
      });
    }

    this.componentRef.instance.closeModal
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => this.close());
  }

  close(): void {
    this.bsModalRef.hide();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();

    if (this.componentRef) {
      this.componentRef.destroy();
    }
  }
}
