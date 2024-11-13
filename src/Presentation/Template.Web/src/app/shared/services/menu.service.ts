import { Menu } from "@models/menu";
import { MenuRepository } from "../repositories/menu.repository";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root'
})

export class MenuService {


  constructor(private menuRepository: MenuRepository) { }


  public getMenu(): Observable<Menu[]> {
    return this.menuRepository.getMenu();
  }
}
