import { Injectable } from '@angular/core';
import {
  TableExporting,
  TableExportType,
} from '@shared/components/generic-table/types/table-column';
import { Observable } from 'rxjs';
import { ApiRepository } from '../repositories/api.repository';
import { environment } from '@environment';

const { apiUrl } = environment;
@Injectable({
  providedIn: 'root',
})
export class ExportService {
  constructor(protected apiRepository: ApiRepository) {}

  public handleExporting({
    searchParams,
    type,
    entityName,
  }: TableExporting): void {
    const exportFnMap: Record<
      TableExportType,
      (params: string | null) => Observable<Blob>
    > = {
      [TableExportType.CSV]: (params) => this.exportToCSV(entityName, params),
      [TableExportType.PDF]: (params) => this.exportToPDF(entityName, params),
      [TableExportType.DOC]: (params) => this.exportToWORD(entityName, params),
    };

    const extensionMap: Record<TableExportType, string> = {
      [TableExportType.CSV]: 'csv',
      [TableExportType.PDF]: 'pdf',
      [TableExportType.DOC]: 'docx',
    };

    const exportFn = exportFnMap[type];
    const extension = extensionMap[type];

    if (!exportFn || !extension) return;

    exportFn(searchParams).subscribe((blob) => {
      const fileName = `${entityName}.${extension}`;
      this.downloadFile(blob, fileName);
    });
  }

  private exportToCSV(
    entityName: string | null,
    searchParams: string | null
  ): Observable<Blob> {
    return this.apiRepository.get(
      `${apiUrl}/${entityName}/export/csv${
        searchParams ? '?' + searchParams : ''
      }`,
      { responseType: 'blob' }
    );
  }

  private exportToPDF(
    entityName: string,
    searchParams: string | null
  ): Observable<Blob> {
    return this.apiRepository.get<Blob>(
      `${apiUrl}/${entityName}/export/pdf/${
        searchParams ? '?' + searchParams : ''
      }`,
      { responseType: 'blob' }
    );
  }

  private exportToWORD(
    entityName: string | null,
    searchParams: string | null
  ): Observable<Blob> {
    return this.apiRepository.get(
      `${apiUrl}/${entityName}/export/docx/${
        searchParams ? '?' + searchParams : ''
      }`,
      { responseType: 'blob' }
    );
  }

  private downloadFile(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    window.URL.revokeObjectURL(url);
  }
}
