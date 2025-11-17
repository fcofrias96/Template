import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { PermissionService } from '@features/permission/services/permission.service';
import { PermissionDetail } from '@features/permission/types/permission-detail.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-permission-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './permission-detail.component.html',
  styleUrl: './permission-detail.component.scss'
})
export class PermissionDetailComponent implements OnChanges, DynamicModalComponent {
  permission$!: Observable<PermissionDetail>;
  @Input() permissionId!: string;

  constructor(private permissionService: PermissionService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['permissionId'] && this.permissionId) {
      this.permission$ = this.permissionService.getById(this.permissionId);
    }
  }
  closeModal = new Subject<void>();
}
