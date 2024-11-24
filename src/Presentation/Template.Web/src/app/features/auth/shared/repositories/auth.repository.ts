import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environment';
import { LoginRequest } from '../types/login/login-request.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthRepository {

  constructor(private httpClient: HttpClient) { }


}
