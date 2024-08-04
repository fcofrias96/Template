import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { Observable, map, tap } from 'rxjs';
import { UserCreateDto } from '@models/user';
import { LoginRequest, LoginResponse } from '../models/login';
import { refreshToken } from '../models/refresh-token';
import { ApiResponse } from '@models/wrappers';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.baseUrl}/auth`;

  constructor(private http: HttpClient) {}

  confirmEmail(userId: string, token: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/confirm-email`, {
      userId,
      token,
    });
  }

  refreshToken(): Observable<refreshToken> {
    return this.http.post<refreshToken>(
      `${this.apiUrl}/refresh-token`,
      {},
      { withCredentials: true }
    );
  }

  forgotPassword(email: string): Observable<any>{
    return this.http.post<refreshToken>(`${this.apiUrl}/forgot-password`, {
      email,
    });
  }

  login(LoginRequest: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, LoginRequest, {
      withCredentials: true,
    });
  }

  register(user: UserCreateDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, user);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  getUserId(): string | null {
    return localStorage.getItem('userId');
  }

  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/logout`, {}).pipe(
      map((response) => {
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        localStorage.removeItem('userId');
        return response;
      })
    );
  }
}
