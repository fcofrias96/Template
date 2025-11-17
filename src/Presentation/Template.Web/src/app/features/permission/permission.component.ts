import { Component } from '@angular/core';
import { PermissionListComponent } from './components/permission-list/permission-list.component';
import { PermissionFormComponent } from './components/permission-form/permission-form.component';
import { ModalService } from '@shared/services/modal.service';

@Component({
  selector: 'app-permission',
  standalone: true,
  imports: [PermissionListComponent],
  templateUrl: './permission.component.html',
  styleUrl: './permission.component.scss',
})
export class PermissionComponent {
  constructor(private modalService: ModalService) { }

  addPermission() {
    this.modalService.openModal(PermissionFormComponent, 'Add Permission', null, {
      backdrop: 'static',
    });
  }
}
