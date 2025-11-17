export interface PagedCollection<T> {
  href: string;
  next?: string;
  prev?: string;
  total: number;
  limit: number;
  offset: number;
  elements: T[];
}
