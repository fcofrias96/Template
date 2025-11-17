import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { RoleService } from '@features/role/services/role.service';
import { RoleDetail } from '@features/role/types/role-detail.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-role-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './role-detail.component.html',
  styleUrl: './role-detail.component.scss'
})
export class RoleDetailComponent implements OnChanges, DynamicModalComponent {
  role$!: Observable<RoleDetail>;
  @Input() roleId!: string;

  constructor(private roleService: RoleService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['roleId'] && this.roleId) {
      this.role$ = this.roleService.getById(this.roleId);
    }
  }
  closeModal = new Subject<void>();
}
