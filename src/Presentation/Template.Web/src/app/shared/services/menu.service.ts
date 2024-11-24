import { Menu } from '@models/menu';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiService } from 'app/core/services/api.service';
import { environment } from '@environment';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class MenuService extends ApiService {
  private baseUrl = `${apiUrl}/menu`;

  public getMenu(): Observable<Menu[]> {
    return this.getList(this.baseUrl);
  }
}
