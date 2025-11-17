import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { CategoryDetailComponent } from '@features/category/components/category-detail/category-detail.component';
import { CategoryFormComponent } from '@features/category/components/category-form/category-form.component';
import { CategoryService } from '@features/category/services/category.service';
import { Category } from '@features/category/types/category.interface';
import { PagedCollection } from '@models/paged-collection.interface';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import {
  TableColumn,
  TableExporting,
  ActionType,
} from '@shared/components/generic-table/types/table-column';
import { ModalService } from '@shared/services/modal.service';
import { ExportService } from 'app/core/services/export.service';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.scss',
})
export class CategoryListComponent implements OnInit, OnDestroy {
  categories$: Observable<PagedCollection<Category>>;
  columns: TableColumn[];
  itemsPerPage = 15;
  unsubscribe$: Subject<void> = new Subject<void>();

  constructor(
    private categoryService: CategoryService,
    private modalService: ModalService,
    private exportService: ExportService
  ) {
    this.categories$ = this.getCategories();
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
    this.categoryService.refreshCategories$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.categories$ = this.getCategories();
      });
  }

  private getCategories(
    searchParams: URLSearchParams | null = null
  ): Observable<PagedCollection<Category>> {
    if (!searchParams) {
      searchParams = new URLSearchParams(`limit=${this.itemsPerPage}`);
    }
    return this.categoryService.getPagedList<Category>(searchParams);
  }

  handleFilter(searchParams: URLSearchParams) {
    this.categories$ = this.getCategories(searchParams);
  }

  selectCategory(categoryId: string): void {
    this.modalService.openModal(
      CategoryDetailComponent,
      'Category Detail',
      { categoryId },
      {
        class: 'modal-lg',
      }
    );
  }

  private edit(category: Category) {
    this.modalService.openModal(
      CategoryFormComponent,
      'Edit Category',
      { categoryId: category.id, edit: true },
      {
        class: 'modal-lg',
      }
    );
  }

  private delete(category: Category) {
    this.modalService
      .openConfirmModal('Are you sure you want to delete this category?')
      .then((result) => {
        if (result) {
          this.categoryService.delete(category.id).subscribe(() => {
            this.categoryService.refreshCategories$.next();
          });
        }
      });
  }

  private view(category: Category): void {
    this.modalService.openModal(
      CategoryDetailComponent,
      'Category Detail',
      { categoryId: category.id },
      {
        class: 'modal-lg',
      }
    );
  }

  actions(): Partial<Record<ActionType, (category: Category) => void>> {
    return {
      view: (category: Category) => this.view(category),
      edit: (category: Category) => this.edit(category),
      delete: (category: Category) => this.delete(category),
    };
  }

  handleExporting($event: TableExporting) {
    this.exportService.handleExporting({ ...$event, entityName: 'category' });
  }
}
