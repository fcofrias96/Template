import { Component, OnDestroy, OnInit } from '@angular/core';
import { PagedCollection } from '@models/paged-collection.interface';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import {
  TableColumn,
  ActionType,
  TableExporting,
} from '@shared/components/generic-table/types/table-column';
import { ModalService } from '@shared/services/modal.service';
import { ExportService } from 'app/core/services/export.service';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PermissionDetailComponent } from '../permission-detail/permission-detail.component';
import { PermissionFormComponent } from '../permission-form/permission-form.component';
import { Permission } from '@features/permission/types/permission.interface';
import { PermissionService } from '@features/permission/services/permission.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-permission-list',
  standalone: true,
  imports: [GenericTableComponent, CommonModule],
  templateUrl: './permission-list.component.html',
  styleUrl: './permission-list.component.scss',
})
export class PermissionListComponent implements OnInit, OnDestroy {
  permissions$: Observable<PagedCollection<Permission>>;
  columns: TableColumn[];
  itemsPerPage = 15;
  unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private permissionService: PermissionService,
    private modalService: ModalService,
    private exportService: ExportService
  ) {
    this.permissions$ = this.getPermissions();
    this.columns = [
      {
        key: 'name',
        title: 'Name',
        sortable: true,
        filterable: true,
      },
      {
        key: 'description',
        title: 'Description',
        sortable: true,
        filterable: true,
      },
    ];
  }
  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit(): void {
    this.permissionService.refreshPermissions$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.permissions$ = this.getPermissions();
      });
  }

  private getPermissions(
    searchParams: URLSearchParams | null = null
  ): Observable<PagedCollection<Permission>> {
    if (!searchParams) {
      searchParams = new URLSearchParams(`limit=${this.itemsPerPage}`);
    }
    return this.permissionService.getPagedList<Permission>(searchParams);
  }

  handleFilter(searchParams: URLSearchParams): void {
    this.permissions$ = this.getPermissions(searchParams);
  }

  private view(permission: Permission): void {
    this.modalService.openModal(
      PermissionDetailComponent,
      'Permission Detail',
      { permissionId: permission.id },
      {
        class: 'modal-lg',
      }
    );
  }

  private edit(permission: Permission) {
    this.modalService.openModal(
      PermissionFormComponent,
      'Edit Permission',
      { permissionId: permission.id, edit: true },
      {
        class: 'modal-lg',
      }
    );
  }

  private delete(permission: Permission) {
    this.modalService
      .openConfirmModal('Are you sure you want to delete this permission?')
      .then((result) => {
        if (result) {
          this.permissionService.delete(permission.id).subscribe(() => {
            this.permissionService.refreshPermissions$.next();
          });
        }
      });
  }

  actions(): Partial<Record<ActionType, (permission: Permission) => void>> {
    return {
      view: (permission: Permission) => this.view(permission),
      edit: (permission: Permission) => this.edit(permission),
      delete: (permission: Permission) => this.delete(permission),
    };
  }

  handleExporting($event: TableExporting) {
    this.exportService.handleExporting({ ...$event, entityName: 'permission' });
  }
}
