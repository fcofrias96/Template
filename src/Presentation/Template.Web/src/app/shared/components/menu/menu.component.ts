import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Menu, MenuResponse } from '@models/menu';
import { AuthService } from 'app/features/auth/shared/services/auth.service';
import { MenuService } from 'app/shared/services/menu.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss',
})
export class MenuComponent implements OnInit, OnDestroy {
  public menu: Menu[] = [];
  @Input() public subMenu: Menu[] = [];
  @Input() public isRootMenu = true;
  @Input() public parentRoute = '';
  public isAuthenticated = false;
  private readonly unsubscribe$ = new Subject<void>();

  constructor(
    private menuService: MenuService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {

    this.menuService.getList$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.getMenu();
      });


    if (this.subMenu.length) {
      this.menu = this.subMenu;
    } else {
      this.getMenu();
    }
  }

  getMenu(): void {
    this.menuService.get<MenuResponse>().subscribe((response: MenuResponse) => {
      this.menu = response.menus;
      if (!response.isAuthenticated && this.authService.isAuthenticated) {
        this.authService.logout(false);
      }
    });
  }


  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
