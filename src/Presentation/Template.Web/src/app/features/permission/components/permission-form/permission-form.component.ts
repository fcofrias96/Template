import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { PermissionService } from '@features/permission/services/permission.service';
import { CreatePermission } from '@features/permission/types/create-permission.interface';
import { PermissionDetail } from '@features/permission/types/permission-detail.interface';
import { UpdatePermission } from '@features/permission/types/update-permission.interface';
import { ResourceService } from '@features/resource/services/resource.service';
import { Resource } from '@features/resource/types/resource.interface';
import { Action } from '@models/action';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { FilterSelectComponent } from '@shared/components/filter-select/filter-select.component';
import { ActionService } from 'app/core/services/action.service';
import { Observable, Subject } from 'rxjs';
import { FormFieldComponent } from '../../../../shared/components/form-field/form-field.component';
import { SelectComponent } from '@shared/components/select/select.component';
import { Dropdown } from '@models/dropdown';

@Component({
  selector: 'app-permission-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FilterSelectComponent,
    FormFieldComponent,
    SelectComponent,
  ],
  templateUrl: './permission-form.component.html',
  styleUrl: './permission-form.component.scss',
})
export class PermissionFormComponent implements OnInit, DynamicModalComponent {
  public permissionForm: FormGroup;
  @Input() permissionId!: number;
  @Input() edit = false;
  closeModal = new Subject<void>();
  actions$!: Observable<Dropdown[]>;
  resources$!: Observable<Resource[]>;

  constructor(
    private fb: FormBuilder,
    private permissionService: PermissionService,
    private readonly resourceService: ResourceService,
    private readonly actionService: ActionService
  ) {
    this.permissionForm = this.initForm();
  }

  ngOnInit(): void {
    this.getPermissionData();
  }

  private getPermissionData() {
    if (this.permissionId && this.edit) {
      this.permissionService
        .getById<PermissionDetail>(this.permissionId)
        .subscribe((permission) => {
          this.permissionForm.patchValue(permission);
        });
    }

    this.resources$ = this.resourceService.getDropdownList();
    this.actions$ = this.actionService.getDropdownList();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      name: [null, Validators.required],
      description: [null],
      actionId: [null, Validators.required],
      resourceId: [null, Validators.required],
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
    const newPermission: CreatePermission = this.permissionForm.value;
    this.permissionService.create(newPermission).subscribe(() => {
      this.permissionService.refreshPermissions$.next();
      this.closeModal.next();
    });
  }

  private update() {
    const updatedPermission: UpdatePermission = {
      ...this.permissionForm.value,
    };

    this.permissionService
      .update(this.permissionId, updatedPermission)
      .subscribe(() => {
        this.permissionService.refreshPermissions$.next();
        this.closeModal.next();
      });
  }
}
