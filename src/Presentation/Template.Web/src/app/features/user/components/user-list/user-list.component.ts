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
import { UserDetailComponent } from '../user-detail/user-detail.component';
import { UserFormComponent } from '../user-form/user-form.component';
import { User } from '@features/user/types/user.interface';
import { UserService } from '@features/user/services/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [GenericTableComponent, CommonModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
})
export class UserListComponent implements OnInit, OnDestroy {
  users$: Observable<PagedCollection<ApplicationUser>>;
  columns: TableColumn[];
  itemsPerPage = 15;
  unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private userService: UserService,
    private modalService: ModalService,
    private exportService: ExportService
  ) {
    this.users$ = this.getUsers();
    this.columns = [
      {
        key: 'firstName',
        title: 'Fist Name',
        sortable: true,
        filterable: true,
      },
      {
        key: 'lastName',
        title: 'Last Name',
        sortable: true,
        filterable: true,
      },
      {
        key: 'email',
        title: 'Email',
        sortable: true,
        filterable: true,
      },
      {
        key: 'userName',
        title: 'User Name',
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
    this.userService.refreshUsers$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.users$ = this.getUsers();
      });
  }

  private getUsers(
    searchParams: URLSearchParams | null = null
  ): Observable<PagedCollection<ApplicationUser>> {
    if (!searchParams) {
      searchParams = new URLSearchParams(`limit=${this.itemsPerPage}`);
    }
    return this.userService.getPagedList<ApplicationUser>(searchParams);
  }

  handleFilter(searchParams: URLSearchParams) {
    this.users$ = this.getUsers(searchParams);
  }

  private view(user: User): void {
    this.modalService.openModal(
      UserDetailComponent,
      'User Detail',
      { userId: user.id },
      {
        class: 'modal-lg',
      }
    );
  }

  private edit(user: User) {
    this.modalService.openModal(
      UserFormComponent,
      'Edit User',
      { userId: user.id, edit: true },
      {
        class: 'modal-lg',
      }
    );
  }

  private delete(user: User) {
    this.modalService
      .openConfirmModal('Are you sure you want to delete this user?')
      .then((result) => {
        if (result) {
          this.userService.delete(user.id).subscribe(() => {
            this.userService.refreshUsers$.next();
          });
        }
      });
  }

  actions(): Partial<Record<ActionType, (user: User) => void>> {
    return {
      view: (user: User) => this.view(user),
      edit: (user: User) => this.edit(user),
      delete: (user: User) => this.delete(user),
    };
  }

  handleExporting($event: TableExporting) {
    this.exportService.handleExporting({ ...$event, entityName: 'user' });
  }
}
