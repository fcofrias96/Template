import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { UserService } from '@features/user/services/user.service';
import { CreateUser } from '@features/user/types/create-user.interface';
import { UserDetail } from '@features/user/types/user-detail.interface';
import { UpdateUser } from '@features/user/types/update-user.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { Observable, Subject } from 'rxjs';
import { RoleService } from '@features/role/services/role.service';
import { Dropdown } from '@models/dropdown';
import { CheckboxListComponent } from '@shared/components/checkbox-list/checkbox-list.component';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    CommonModule,
    FormFieldComponent,
    ReactiveFormsModule,
    CheckboxListComponent,
  ],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
})
export class UserFormComponent implements OnInit, DynamicModalComponent {
  public userForm: FormGroup;
  @Input() userId!: number;
  @Input() edit = false;
  closeModal = new Subject<void>();
  public roles$: Observable<Dropdown[]>;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private roleService: RoleService
  ) {
    this.userForm = this.initForm();
    this.roles$ = this.roleService.getDropdownList();
  }

  ngOnInit(): void {
    this.getUserData();
  }

  private getUserData() {
    if (this.userId && this.edit) {
      this.userService.getById<UserDetail>(this.userId).subscribe((user) => {
        this.userForm.patchValue(user);
      });
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      id: [null],
      firstName: [null, Validators.required],
      lastName: [null, Validators.required],
      userName: [null, Validators.required],
      email: [null, Validators.required],
      roles: [this.fb.array([])],
    });
  }

  onSubmit(): void {
    if (this.edit) {
      this.update();
    } else {
      this.create();
    }
  }

  private create() {
    const newUser: CreateUser = this.userForm.value;
    this.userService.create(newUser).subscribe(() => {
      this.userService.refreshUsers$.next();
      this.closeModal.next();
    });
  }

  private update() {
    const updatedUser: UpdateUser = {
      ...this.userForm.value,
    };

    this.userService.update(this.userId, updatedUser).subscribe(() => {
      this.userService.refreshUsers$.next();
      this.closeModal.next();
    });
  }
}
