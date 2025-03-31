import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {  RouterModule } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { CustomValidators } from 'app/core/utils/custom-validator';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    FormFieldComponent,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  public loginForm: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.loginForm = this.fb.group({
      email: [null, [Validators.required, CustomValidators.strictEmail]],
      password: [null, [Validators.required, Validators.minLength(6)]],
    });
  }

  onLogin(): void {
    const data = this.loginForm.value;
    this.authService.login(data);
  }
}
