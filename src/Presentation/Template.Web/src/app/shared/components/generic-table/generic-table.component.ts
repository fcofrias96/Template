import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { TableAction, TableColumn } from './types/table-column';

@Component({
  selector: 'app-generic-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './generic-table.component.html',
  styleUrl: './generic-table.component.scss'
})
export class GenericTableComponent implements OnInit, OnChanges {
  @Input() data: any[]  = [];
  @Input() columns: TableColumn[] = [];
  @Input() itemsPerPage: number = 10;
  @Output() actionClicked = new EventEmitter<TableAction>();

  displayedData: any[] = [];
  currentPage: number = 1;
  totalPages: number = 1;
  filters: { [key: string]: string } = {};
  sortColumn: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  ngOnInit() {
    this.applyFilters();
  }


  ngOnChanges(changes: SimpleChanges) {
    if (changes['data']) {
      this.currentPage = 1;
      this.applyFilters();
    }
  }
  applyFilters() {
    let filteredData = [...this.data];


    for (const key in this.filters) {
      if (this.filters[key]) {
        filteredData = filteredData.filter(item =>
          item[key].toString().toLowerCase().includes(this.filters[key].toLowerCase())
        );
      }
    }


    if (this.sortColumn) {
      filteredData.sort((a, b) => {
        const aValue = a[this.sortColumn];
        const bValue = b[this.sortColumn];
        if (aValue < bValue) return this.sortDirection === 'asc' ? -1 : 1;
        if (aValue > bValue) return this.sortDirection === 'asc' ? 1 : -1;
        return 0;
      });
    }


    this.totalPages = Math.ceil(filteredData.length / this.itemsPerPage);
    this.displayedData = filteredData.slice(
      (this.currentPage - 1) * this.itemsPerPage,
      this.currentPage * this.itemsPerPage
    );
  }

  onFilterChange(columnKey: string, event: any) {
    this.filters[columnKey] = event.target.value;
    this.currentPage = 1;
    this.applyFilters();
  }

  sortBy(columnKey: string) {
    if (this.sortColumn === columnKey) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = columnKey;
      this.sortDirection = 'asc';
    }
    this.applyFilters();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.applyFilters();
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
}
