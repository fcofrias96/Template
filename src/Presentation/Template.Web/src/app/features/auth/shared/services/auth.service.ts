import { Injectable } from '@angular/core';
import { environment } from '@environment';
import { ApiRepository } from 'app/core/repositories/api.repository';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { LoginRequest } from '../types/login/login-request.interface';
import {
  LoginResponse,
  RefreshTokenResponse,
} from '../types/login/login-response.interface';
import { Router } from '@angular/router';
import { MenuService } from '@shared/services/menu.service';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public isAuthenticated$ = new BehaviorSubject<boolean>(this.hasToken());
  private redirectUrl: string | null = null;
  private readonly baseUrl = `${apiUrl}/auth`;

  constructor(
    private apiRepository: ApiRepository,
    private router: Router,
    private readonly menuService: MenuService
  ) {}

  public login(data: LoginRequest): void {
    this.apiRepository
      .post<LoginResponse>(`${this.baseUrl}/login`, data, {
        withCredentials: true,
      })
      .subscribe((response) => {
        this.saveAccessToken(response.accessToken);
        this.isAuthenticated$.next(true);
        this.menuService.getList$.next();
        this.router.navigate([this.redirectUrl || '/dashboard']);
        this.redirectUrl = null;
      });
  }

  logout(goLogin = true): void {
    //this.apiRepository.post<string>(`${this.baseUrl}/logout`, {}).subscribe();
    localStorage.removeItem('accessToken');
    this.isAuthenticated$.next(false);
    if (goLogin) {
      this.router.navigate(['/login']);
    }
  }

  refreshToken(): Observable<string> {
    return this.apiRepository
      .post<RefreshTokenResponse>(
        `${this.baseUrl}/refresh-token`,
        {},
        { withCredentials: true }
      )
      .pipe(
        map((res) => {
          this.saveAccessToken(res.accessToken);
          return res.accessToken;
        })
      );
  }

  saveAccessToken(accessToken: string) {
    localStorage.setItem('accessToken', accessToken);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  get isAuthenticated(): boolean {
    return this.isAuthenticated$.value;
  }

  setRedirectUrl(url: string): void {
    this.redirectUrl = url;
  }
}
