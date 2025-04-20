import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { PermissionService } from '@features/permission/services/permission.service';
import { ResourceService } from '@features/resource/services/resource.service';
import { RoleService } from '@features/role/services/role.service';
import { CreateRole } from '@features/role/types/create-role.interface';
import { RoleDetail } from '@features/role/types/role-detail.interface';
import { UpdateRole } from '@features/role/types/update-role.interface';
import { Permission } from '@features/user/types/user-detail.interface';
import { Dropdown } from '@models/dropdown';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { CheckboxListComponent } from '@shared/components/checkbox-list/checkbox-list.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-role-form',
  standalone: true,
  imports: [CommonModule, FormFieldComponent, ReactiveFormsModule, CheckboxListComponent],
  templateUrl: './role-form.component.html',
  styleUrl: './role-form.component.scss',
})
export class RoleFormComponent implements OnInit, DynamicModalComponent {
  public roleForm: FormGroup;
  @Input() roleId!: number;
  @Input() edit = false;
  closeModal = new Subject<void>();
  permissions$: Observable<Dropdown[]>;

  constructor(
    private fb: FormBuilder,
    private roleService: RoleService,
    private permissionService: PermissionService,
  ) {
    this.roleForm = this.initForm();
    this.permissions$ = this.permissionService.getDropdownList();
  }

  ngOnInit(): void {
    this.getRoleData();
  }

  private getRoleData() {
    if (this.roleId && this.edit) {
      this.roleService
        .getById<RoleDetail>(this.roleId)
        .subscribe((role) => {
          this.roleForm.patchValue(role);
        });
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      id: [null],
      name: [null, Validators.required],
      permissions: [this.fb.array([])],
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
    const newRole: CreateRole = this.roleForm.value;
    this.roleService.create(newRole).subscribe(() => {
      this.roleService.refreshRoles$.next();
      this.closeModal.next();
    });
  }

  private update() {
    const updatedRole: UpdateRole = {
      ...this.roleForm.value,
    };

    this.roleService
      .update(this.roleId, updatedRole)
      .subscribe(() => {
        this.roleService.refreshRoles$.next();
        this.closeModal.next();
      });
  }
}
