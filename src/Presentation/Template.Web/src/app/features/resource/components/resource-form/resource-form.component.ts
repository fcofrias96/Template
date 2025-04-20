import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ResourceService } from '@features/resource/services/resource.service';
import { CreateResource } from '@features/resource/types/create-resource.interface';
import { ResourceDetail } from '@features/resource/types/resource-detail.interface';
import { UpdateResource } from '@features/resource/types/update-resource.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-resource-form',
  standalone: true,
  imports: [CommonModule, FormFieldComponent, ReactiveFormsModule],
  templateUrl: './resource-form.component.html',
  styleUrl: './resource-form.component.scss',
})
export class ResourceFormComponent implements OnInit, DynamicModalComponent {
  public resourceForm: FormGroup;
  @Input() resourceId!: number;
  @Input() edit = false;
  closeModal = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private resourceService: ResourceService,
  ) {
    this.resourceForm = this.initForm();
  }

  ngOnInit(): void {
    this.getResourceData();
  }

  private getResourceData() {
    if (this.resourceId && this.edit) {
      this.resourceService
        .getById<ResourceDetail>(this.resourceId)
        .subscribe((resource) => {
          this.resourceForm.patchValue(resource);
        });
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      id: [null],
      name: [null, Validators.required],
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
    const newResource: CreateResource = this.resourceForm.value;
    this.resourceService.create(newResource).subscribe(() => {
      this.resourceService.refreshResources$.next();
      this.closeModal.next();
    });
  }

  private update() {
    const updatedResource: UpdateResource = {
      ...this.resourceForm.value,
    };

    this.resourceService
      .update(this.resourceId, updatedResource)
      .subscribe(() => {
        this.resourceService.refreshResources$.next();
        this.closeModal.next();
      });
  }
}
