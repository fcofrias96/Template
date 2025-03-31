/* eslint-disable @typescript-eslint/no-explicit-any */
import { CommonModule, KeyValue } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output,
  SimpleChanges,
} from '@angular/core';
import { TableAction, TableColumn } from './types/table-column';
import { SelectComponent } from '../select/select.component';
import { debounceTime, Subject, takeUntil } from 'rxjs';
import { PagedCollection } from '@models/paged-collection.interface';

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
  @Output() actionClicked = new EventEmitter<TableAction>();
  @Output() filterChanged = new EventEmitter<string>();
  public readonly itemsPerPageOptions: KeyValue<string, string>[] = [];
  private filterSubject = new Subject<{ column: string; value: string }>();
  private unsubscribe$: Subject<void> = new Subject<void>();
  searchParams: URLSearchParams = new URLSearchParams();
  totalPages = 1;

  sortDirection: 'asc' | 'desc' = 'asc';
  sortColumn = '';
  currentPage: any;

  constructor() {
    this.filterSubject
      .pipe(debounceTime(500), takeUntil(this.unsubscribe$))
      .subscribe(({ column, value }) => {
        if (value) {
          this.searchParams.set(column, value);
        } else {
          this.searchParams.delete(column);
        }
        this.emitFilter();
      });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['itemsPerPage']) {
      this.itemsPerPage = changes['itemsPerPage'].currentValue;
      this.itemsPerPageOptions.length = 0;
      for (let i = 1; i < 5; i++) {
        const value = (i * this.itemsPerPage).toString();
        this.itemsPerPageOptions.push({ key: value, value: value });
      }
    }
  }

  onFilterChange(column: string, event: any) {
    const value = event.target.value;
    this.filterSubject.next({ column, value });
  }

  onNumberFilterChange(
    column: string,
    event: any,
    position: 'start' | 'end' = 'start'
  ): void {
    const value = event.target.value;
    let newValue;
    const existingValue = this.searchParams.get(column);
    const [startValue, endValue] = existingValue?.split('..') ?? ['', ''];

    if (position === 'start') {
      newValue = value ? `${value}..${endValue}` : `..${endValue}`;
    } else {
      newValue = value ? `${startValue}..${value}` : `${startValue}..`;
    }

    if (newValue === '..') {
      newValue = '';
    }
    this.filterSubject.next({ column, value: newValue });
  }

  sortBy(columnKey: string) {
    if (this.sortColumn === columnKey) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = columnKey;
      this.sortDirection = 'asc';
    }

    const isDescending = this.sortDirection === 'desc' ? 'true' : 'false';
    this.searchParams.set('sortBy', columnKey);
    this.searchParams.set('isDescending', isDescending);
    this.emitFilter();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
  }

  goNext() {
    const nextUrl = this.data?.next;
    if (nextUrl) {
      const url = new URL(nextUrl);
      this.searchParams = new URLSearchParams(url.search);
      this.emitFilter();
    }
  }
  GoPrevious() {
    const prevUrl = this.data?.prev;
    if (prevUrl) {
      const url = new URL(prevUrl);
      this.searchParams = new URLSearchParams(url.search);
      this.emitFilter();
    }
  }

  onEditClick(item: any) {
    this.actionClicked.emit({ action: 'edit', item });
  }

  onDeleteClick(item: any) {
    this.actionClicked.emit({ action: 'delete', item });
  }

  onViewClick(item: any) {
    this.actionClicked.emit({ action: 'view', item });
  }

  onItemsPerPageChange(event: Event) {
    const selectedValue = (event.target as HTMLSelectElement).value;
    this.searchParams.set('limit', selectedValue);
    this.searchParams.set('offset', '0');
    this.emitFilter();
  }

  emitFilter() {
    this.filterChanged.emit(this.searchParams.toString());
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
