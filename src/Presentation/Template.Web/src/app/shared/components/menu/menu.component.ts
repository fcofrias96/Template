import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { Menu } from '@models/menu';
import { MenuService } from 'app/shared/services/menu.service';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss',
})
export class MenuComponent implements OnInit {
  public menu: Menu[] = [];
  @Input() public subMenu: Menu[] = [];
  @Input() public isRootMenu: boolean = true;
  @Input() public parentRoute: string = '';


  constructor(private menuService: MenuService) {}

  ngOnInit(): void {
    if (this.subMenu.length) {
      this.menu = this.subMenu;
    }
    else{
      this.menuService.getMenu().subscribe((menu: Menu[]) => {
        this.menu = menu;
      });
    }
  }
}
