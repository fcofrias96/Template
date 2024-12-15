import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.scss'],
})
export class ConfirmModalComponent {
  @Input() title!: string;
  public result: boolean = false;

  constructor(public bsModalRef: BsModalRef) {}

  public onYesButton() {
    this.result = true;
    this.close();
  }

  public onNoButton() {
    this.result = false;
    this.close();
  }

  public close() {
    this.bsModalRef.hide();
  }
}
