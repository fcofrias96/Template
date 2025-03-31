import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Menu } from '@models/menu';
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
    this.authService.isAuthenticated$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((isAuthenticated) => {
        this.isAuthenticated = isAuthenticated;
        this.getMenu();
      });

    if (this.subMenu.length) {
      this.menu = this.subMenu;
    } else {
      this.getMenu();
    }
  }

  getMenu(): void {
    this.menuService.getList<Menu>().subscribe((menu: Menu[]) => {
      this.menu = menu;
    });
  }

  logout(): void {
    this.authService.logout();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
