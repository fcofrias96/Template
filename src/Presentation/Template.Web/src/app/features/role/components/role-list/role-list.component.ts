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
import { RoleDetailComponent } from '../role-detail/role-detail.component';
import { RoleFormComponent } from '../role-form/role-form.component';
import { Role } from '@features/role/types/role.interface';
import { RoleService } from '@features/role/services/role.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [GenericTableComponent, CommonModule],
  templateUrl: './role-list.component.html',
  styleUrl: './role-list.component.scss',
})
export class RoleListComponent implements OnInit, OnDestroy {
  roles$: Observable<PagedCollection<ApplicationRole>>;
  columns: TableColumn[];
  itemsPerPage = 15;
  unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private roleService: RoleService,
    private modalService: ModalService,
    private exportService: ExportService
  ) {
    this.roles$ = this.getRoles();
    this.columns = [
      {
        key: 'name',
        title: 'Name',
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
    this.roleService.refreshRoles$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.roles$ = this.getRoles();
      });
  }

  private getRoles(
    searchParams: URLSearchParams | null = null
  ): Observable<PagedCollection<ApplicationRole>> {
    if (!searchParams) {
      searchParams = new URLSearchParams(`limit=${this.itemsPerPage}`);
    }
    return this.roleService.getPagedList<ApplicationRole>(searchParams);
  }

  handleFilter(searchParams: URLSearchParams) {
    this.roles$ = this.getRoles(searchParams);
  }

  private view(role: Role): void {
    this.modalService.openModal(
      RoleDetailComponent,
      'Role Detail',
      { roleId: role.id },
      {
        class: 'modal-lg',
      }
    );
  }

  private edit(role: Role) {
    this.modalService.openModal(
      RoleFormComponent,
      'Edit Role',
      { roleId: role.id, edit: true },
      {
        class: 'modal-lg',
      }
    );
  }

  private delete(role: Role) {
    this.modalService
      .openConfirmModal('Are you sure you want to delete this role?')
      .then((result) => {
        if (result) {
          this.roleService.delete(role.id).subscribe(() => {
            this.roleService.refreshRoles$.next();
          });
        }
      });
  }

  actions(): Partial<Record<ActionType, (role: Role) => void>> {
    return {
      view: (role: Role) => this.view(role),
      edit: (role: Role) => this.edit(role),
      delete: (role: Role) => this.delete(role),
    };
  }

  handleExporting($event: TableExporting) {
    this.exportService.handleExporting({ ...$event, entityName: 'role' });
  }
}
