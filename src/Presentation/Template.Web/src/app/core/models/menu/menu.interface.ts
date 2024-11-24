export interface Menu {
  id: number;
  title: string;
  url: string;
  icon: string;
  children: Menu[];
}
