import { Injectable } from '@angular/core';
import { environment } from '@environment';
import { ApiRepository } from 'app/core/repositories/api.repository';
import { BehaviorSubject, firstValueFrom, Observable } from 'rxjs';
import { LoginRequest } from '../types/login/login-request.interface';
import { LoginResponse } from '../types/login/login-response.interface';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
const { apiUrl } = environment;

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(
    this.hasToken()
  );
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  private redirectUrl: string | null = null;

  constructor(
    private apiRepository: ApiRepository,
    private router: Router
  ) {}

  public login(data: LoginRequest): void {
    const response = this.apiRepository
      .post<LoginResponse>(`${apiUrl}/auth/login`, data)
      .pipe(map((response) => response.data));

    firstValueFrom(response).then((response) => {
      localStorage.setItem('accessToken', response.accessToken);
      this.isAuthenticatedSubject.next(true);
      this.router.navigate([this.redirectUrl || '/dashboard']);
      this.redirectUrl = null;
    });
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/login']);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  setRedirectUrl(url: string): void {
    this.redirectUrl = url;
  }
}
