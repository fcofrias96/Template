import { Injectable } from '@angular/core';
import { environment } from '@environment';
import { ApiRepository } from 'app/core/repositories/api.repository';
import { BehaviorSubject } from 'rxjs';
import { LoginRequest } from '../types/login/login-request.interface';
import { LoginResponse } from '../types/login/login-response.interface';
import { Router } from '@angular/router';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public isAuthenticated$ = new BehaviorSubject<boolean>(this.hasToken());
  private redirectUrl: string | null = null;

  constructor(private apiRepository: ApiRepository, private router: Router) {
    console.log(this.hasToken());
  }

  public login(data: LoginRequest): void {
    this.apiRepository
      .post<LoginResponse>(`${apiUrl}/auth/login`, data)
      .subscribe((response) => {
        localStorage.setItem('accessToken', response.accessToken);
        this.isAuthenticated$.next(true);
        this.router.navigate([this.redirectUrl || '/dashboard']);
        this.redirectUrl = null;
      });
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    this.isAuthenticated$.next(false);
    this.router.navigate(['/login']);
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
