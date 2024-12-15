export interface TableColumn {
  key: string;
  title: string;
  sortable?: boolean;
  filterable?: boolean;
}

export interface TableAction {
  action: 'view' | 'edit' | 'delete';
  item: any;
}
