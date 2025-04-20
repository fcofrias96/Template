import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import {
  ControlContainer,
  FormGroupDirective,
  FormControl,
  ReactiveFormsModule,
  Validators,
  NG_VALUE_ACCESSOR,
  ControlValueAccessor,
  ValidationErrors,
} from '@angular/forms';
import { NgxMaskDirective } from 'ngx-mask';

@Component({
  selector: 'app-form-field',
  templateUrl: './form-field.component.html',
  styleUrls: ['./form-field.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgxMaskDirective],
  viewProviders: [
    { provide: ControlContainer, useExisting: FormGroupDirective },
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormFieldComponent),
      multi: true,
    },
  ],
})
export class FormFieldComponent implements ControlValueAccessor {
  @Input() formControlName!: string;
  @Input() label!: string;
  @Input() type: 'text' | 'number' | 'textarea' | 'password' = 'text';
  @Input() placeholder = '';
  @Input() mask!: string;
  @Input() dropSpecialCharacters!: boolean | string[];

  passwordVisible = false;

  // Allowed keys for number inputs
  private allowedKeys = [
    'Backspace',
    'Tab',
    'ArrowLeft',
    'ArrowRight',
    'Delete',
  ];

  constructor(private controlContainer: ControlContainer) {}
  value: any;

  onChange: any = () => {};
  onTouched: any = () => {};

  writeValue(value: any): void {
    this.value = value;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {}

  /** Returns the corresponding FormControl */
  get control(): FormControl {
    return this.controlContainer.control?.get(
      this.formControlName
    ) as FormControl;
  }

  /** Computes the validation class to apply */
  get validationClass(): string {
    if (!this.control) return '';

    const { touched, dirty, valid, invalid } = this.control;
    const interacted = touched || dirty;

    if (interacted) {
      return invalid ? 'invalid' : valid ? 'valid' : '';
    }
    return '';
  }

  /** Checks if the field is required */
  get isRequired(): boolean {
    return this.control.hasValidator(Validators.required);
  }

  /** Generates the validation message based on control errors */
  get validationMessage(): string | null {
    const control = this.control;
    if (!control || !control.errors) return null;

    const errorMessages: Record<string, (error: ValidationErrors) => string> = {
      required: () => `${this.label} is required.`,
      invalidNumber: () => `${this.label} must be a valid number.`,
      minlength: (error) =>
        `${this.label} must be at least ${error['requiredLength']} characters long.`,
      maxlength: (error) =>
        `${this.label} cannot be longer than ${error['requiredLength']} characters.`,
      pattern: () => `The format of ${this.label} is incorrect.`,
      email: () => `Please enter a valid email address.`,
      passwordMismatch: () => `Passwords do not match. Please try again.`,
    };

    const errorKey = Object.keys(control.errors)[0];
    return errorMessages[errorKey]
      ? errorMessages[errorKey](control.errors[errorKey])
      : null;
  }



  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }
}
