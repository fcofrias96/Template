import { Component } from '@angular/core';
import { RoleListComponent } from './components/role-list/role-list.component';
import { RoleFormComponent } from './components/role-form/role-form.component';
import { ModalService } from '@shared/services/modal.service';

@Component({
  selector: 'app-role',
  standalone: true,
  imports: [RoleListComponent],
  templateUrl: './role.component.html',
  styleUrl: './role.component.scss',
})
export class RoleComponent {
  constructor(private modalService: ModalService) {}

  addRole() {
    this.modalService.openModal(RoleFormComponent, 'Add Role', null, {
      backdrop: 'static',
    });
  }
}
