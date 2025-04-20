export interface TableColumn {
  key: string;
  title: string;
  sortable?: boolean;
  filterable?: boolean;
  type?: 'text' | 'date' | 'number' | 'boolean' ;
  currency?: 'USD' | 'EUR' | 'DOP';
  dateFormat?: 'MM/dd/yyyy' | 'dd/MM/yyyy' | 'yyyy/MM/dd';
}



export interface TableExporting {
  type: TableExportType;
  searchParams: string;
  entityName: string;
}

export enum TableExportType {
  CSV = 'csv',
  PDF = 'pdf',
  DOC = 'doc',
}


export type ActionType = 'view' | 'edit' | 'delete';
