/* eslint-disable @typescript-eslint/no-explicit-any */
import { CommonModule, KeyValue } from '@angular/common';
import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output,
  QueryList,
  SimpleChanges,
  ViewChildren,
} from '@angular/core';
import {
  ActionType,
  TableColumn,
  TableExporting,
  TableExportType,
} from './types/table-column';
import { SelectComponent } from '../select/select.component';
import { debounceTime, Subject, takeUntil } from 'rxjs';
import { PagedCollection } from '@models/paged-collection.interface';
import { NotificationService } from 'app/core/services/notification.service';
import { Dropdown } from '@models/dropdown';

@Component({
  selector: 'app-generic-table',
  standalone: true,
  imports: [CommonModule, SelectComponent],
  templateUrl: './generic-table.component.html',
  styleUrl: './generic-table.component.scss',
})
export class GenericTableComponent implements OnChanges, OnDestroy {
  @Input() data: PagedCollection<any> | undefined;
  @Input() columns: TableColumn[] = [];
  @Input() itemsPerPage = 10;
  @Output() filterChanged = new EventEmitter<URLSearchParams>();
  @Output() exportTo = new EventEmitter<TableExporting>();
  @ViewChildren('filterInput') filterInputs!: QueryList<
    ElementRef<HTMLInputElement>
  >;
  @Input() actions!: Partial<Record<ActionType, (element: any) => void>>;

  public readonly itemsPerPageOptions: Dropdown[] = [];
  private filterSubject = new Subject<{ column: string; value: string }>();
  private unsubscribe$: Subject<void> = new Subject<void>();
  searchParams: URLSearchParams = new URLSearchParams();
  totalPages = 1;
  showFilter = false;

  sortDirection: 'asc' | 'desc' = 'asc';
  sortColumn = '';
  currentPage = 1;

  constructor(private notificationService: NotificationService) {
    this.filterSubject
      .pipe(debounceTime(500), takeUntil(this.unsubscribe$))
      .subscribe(({ column, value }) => {
        if (value) {
          this.searchParams.set(column, value);
        } else {
          this.searchParams.delete(column);
        }
        this.searchParams.set('offset', '0');
        this.currentPage = 1;
        this.emitFilter();
      });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['data']) {
      const data = changes['data'].currentValue;
      const total = data?.total || 0;
      this.totalPages = Math.ceil(total / this.itemsPerPage);
    }

    if (changes['itemsPerPage']) {
      this.itemsPerPage = changes['itemsPerPage'].currentValue;
      this.itemsPerPageOptions.length = 0;
      for (let i = 1; i < 5; i++) {
        const value = (i * this.itemsPerPage).toString();
        this.itemsPerPageOptions.push({ id: value, name: value });
      }
    }
  }

  onFilterChange(column: string, event: any) {
    const value = event.target.value;
    this.filterSubject.next({ column, value });
  }

  onInputFilterChange(
    column: string,
    event: any,
    position: 'start' | 'end' = 'start'
  ): void {
    const inputValue = event.target.value;
    const [startValue = '', endValue = ''] = this.searchParams
      .get(column)
      ?.split('..') ?? ['', ''];

    const isStart = position === 'start';
    const start = isStart ? inputValue : startValue;
    const end = isStart ? endValue : inputValue;

    if (start && end && Number(start) > Number(end)) {
      this.notificationService.showWarning(
        'Min price cannot be greater than max price'
      );
      return;
    }

    let newValue = [start, end].join('..');

    if (newValue === '..') {
      newValue = '';
    }

    this.filterSubject.next({ column, value: newValue });
  }

  sortBy(columnKey: string): void {
    const isSameColumn = this.sortColumn === columnKey;

    this.sortColumn = columnKey;
    this.sortDirection =
      isSameColumn && this.sortDirection === 'asc' ? 'desc' : 'asc';

    this.searchParams.set('sortBy', columnKey);
    this.searchParams.set(
      'isDescending',
      (this.sortDirection === 'desc').toString()
    );

    this.emitFilter();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.searchParams.set(
      'offset',
      ((page - 1) * this.itemsPerPage).toString()
    );
    this.emitFilter();
  }

  goNext() {
    this.searchParams.set(
      'offset',
      (this.currentPage * this.itemsPerPage).toString()
    );
    this.emitFilter();
    this.currentPage++;
  }

  goPrevious() {
    this.searchParams.set(
      'offset',
      ((this.currentPage - 2) * this.itemsPerPage).toString()
    );
    this.emitFilter();
    this.currentPage--;
  }

  public getPages(): number[] {
    const pages: number[] = [];
    const startPage = Math.max(2, this.currentPage - 2);
    const endPage = Math.min(this.totalPages - 1, this.currentPage + 2);
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  onEditClick(item: any) {
    this.actions['edit']?.(item);
  }

  onDeleteClick(item: any) {
    this.actions['delete']?.(item);
  }

  onViewClick(item: any) {
    this.actions['view']?.(item);
  }

  onItemsPerPageChange(event: Event) {
    const selectedValue = (event.target as HTMLSelectElement).value;
    this.itemsPerPage = Number(selectedValue);
    this.searchParams.set('offset', '0');
    this.currentPage = 1;
    this.emitFilter();
  }

  emitFilter() {
    this.searchParams.set('limit', this.itemsPerPage.toString());
    this.filterChanged.emit(this.searchParams);
  }

  exportToCSV(): void {
    this.exportTo.emit({
      type: TableExportType.CSV,
      searchParams: this.searchParams.toString(),
    } as TableExporting);
  }
  exportToPDF(): void {
    this.exportTo.emit({
      type: TableExportType.PDF,
      searchParams: this.searchParams.toString(),
    } as TableExporting);
  }
  exportToDOC(): void {
    this.exportTo.emit({
      type: TableExportType.DOC,
      searchParams: this.searchParams.toString(),
    } as TableExporting);
  }

  toggle() {
    this.showFilter = !this.showFilter;

    if (!this.showFilter) {
      this.initializeFilterInputs();
      this.resetSearchParams();
      this.emitFilter();
    }
  }

  private resetSearchParams() {
    this.searchParams = new URLSearchParams();
    this.currentPage = 1;
  }

  private initializeFilterInputs() {
    this.filterInputs.forEach((input) => (input.nativeElement.value = ''));
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
