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
import { ResourceDetailComponent } from '../resource-detail/resource-detail.component';
import { ResourceFormComponent } from '../resource-form/resource-form.component';
import { Resource } from '@features/resource/types/resource.interface';
import { ResourceService } from '@features/resource/services/resource.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-resource-list',
  standalone: true,
  imports: [GenericTableComponent, CommonModule],
  templateUrl: './resource-list.component.html',
  styleUrl: './resource-list.component.scss',
})
export class ResourceListComponent implements OnInit, OnDestroy {
  resources$: Observable<PagedCollection<Resource>>;
  columns: TableColumn[];
  itemsPerPage = 15;
  unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private resourceService: ResourceService,
    private modalService: ModalService,
    private exportService: ExportService
  ) {
    this.resources$ = this.getResources();
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
    this.resourceService.refreshResources$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.resources$ = this.getResources();
      });
  }

  private getResources(
    searchParams:URLSearchParams | null = null
  ): Observable<PagedCollection<Resource>> {
    if(!searchParams) {
      searchParams = new URLSearchParams(`limit=${this.itemsPerPage}`);
    }
    return this.resourceService.getPagedList<Resource>(searchParams);
  }

  handleFilter(searchParams: URLSearchParams) {
    this.resources$ = this.getResources(searchParams);
  }

  private view(resource: Resource): void {
    this.modalService.openModal(
      ResourceDetailComponent,
      'Resource Detail',
      { resourceId: resource.id },
      {
        class: 'modal-lg',
      }
    );
  }

  private edit(resource: Resource) {
    this.modalService.openModal(
      ResourceFormComponent,
      'Edit Resource',
      { resourceId: resource.id, edit: true },
      {
        class: 'modal-lg',
      }
    );
  }

  private delete(resource: Resource) {
    this.modalService
      .openConfirmModal('Are you sure you want to delete this resource?')
      .then((result) => {
        if (result) {
          this.resourceService.delete(resource.id).subscribe(() => {
            this.resourceService.refreshResources$.next();
          });
        }
      });
  }

  actions(): Partial<Record<ActionType, (resource: Resource) => void>> {
    return {
      view: (resource: Resource) => this.view(resource),
      edit: (resource: Resource) => this.edit(resource),
      delete: (resource: Resource) => this.delete(resource),
    };
  }

  handleExporting($event: TableExporting) {
    this.exportService.handleExporting({ ...$event, entityName: 'resource' });
  }
}
