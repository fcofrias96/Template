import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environment';
import { Menu } from '@models/menu';
import { Observable } from 'rxjs';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root'
})

export class MenuRepository {

  constructor(private http: HttpClient) { }

  public getMenu(): Observable<Menu[]> {
    return this.http.get<Menu[]>(`${apiUrl}/menus`);
  }
}
