export interface TableColumn {
  key: string;
  title: string;
  sortable?: boolean;
  filterable?: boolean;
  filterType?: 'text' | 'date' | 'number';
}

export interface TableAction {
  action: 'view' | 'edit' | 'delete';
  item: any;
}
