import { Injectable } from '@angular/core';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { BaseModalComponent } from '../components/base-modal/base-modal.component';
import { Type } from '@angular/core';
import { ConfirmModalComponent } from '@shared/components/confirm-modal/confirm-modal.component';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';

@Injectable({
  providedIn: 'root',
})
export class ModalService {
  constructor(private modalService: BsModalService) { }

  openModal(
    component: Type<DynamicModalComponent>,
    title: string,
    data?: unknown,
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

    return this.modalService.show(BaseModalComponent, modalConfig);
  }

  openConfirmModal(title: string, config?: ModalOptions): Promise<boolean> {
    return new Promise((resolve) => {
      const initialState = {
        title: title,
      };

      const modalConfig: ModalOptions = {
        initialState,
        ...config,
      };

      const bsModalRef = this.modalService.show(
        ConfirmModalComponent,
        modalConfig
      );

      bsModalRef.onHidden?.subscribe(() => {
        const result = bsModalRef.content?.result ?? false;
        resolve(result);
      });
    });
  }
}
