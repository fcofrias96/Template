import { Injectable } from '@angular/core';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { BaseModalComponent } from '../components/base-modal/base-modal.component';
import { Type } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ModalService {
  bsModalRef: BsModalRef | null = null;

  constructor(private modalService: BsModalService) {}

  openModal(
    component: Type<any>,
    title: string,
    data?: any,
    config?: ModalOptions
  ): BsModalRef {
    const initialState = {
      title: title,
      component: component,
      data: data,
    };

    const modalConfig: ModalOptions = {
      initialState,
      ...config,
    };

    this.bsModalRef = this.modalService.show(BaseModalComponent, modalConfig);
    return this.bsModalRef;
  }

  closeModal() {
    if (this.bsModalRef) {
      this.bsModalRef.hide();
      this.bsModalRef = null;
    }
  }
}
