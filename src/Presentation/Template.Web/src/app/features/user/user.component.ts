import { Component } from '@angular/core';
import { UserListComponent } from './components/user-list/user-list.component';
import { UserFormComponent } from './components/user-form/user-form.component';
import { ModalService } from '@shared/services/modal.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [UserListComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserComponent {
  constructor(private modalService: ModalService) {}

  addUser() {
    this.modalService.openModal(UserFormComponent, 'Add User', null, {
      backdrop: 'static',
    });
  }
}
