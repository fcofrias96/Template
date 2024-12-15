import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ControlContainer, FormGroupDirective, FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-form-field',
  templateUrl: './form-field.component.html',
  styleUrls: ['./form-field.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],
})
export class FormFieldComponent {
  @Input() controlName!: string;
  @Input() label!: string;
  @Input() type: 'text' | 'textarea' = 'text';
  @Input() id!: string;
  @Input() placeholder: string = '';
  @Input() onlyNumbers: boolean = false;

  constructor(private controlContainer: ControlContainer) {}

  /** Getter for the FormControl based on controlName */
  get control(): FormControl {
    return this.controlContainer.control?.get(this.controlName) as FormControl;
  }

  /** Determines the validation class to apply */
  getValidationClass(): string {
    const control = this.control;
    if (!control) {
      return '';
    }
    if ((control.touched || control.dirty) && control.invalid) {
      return 'is-invalid';
    }
    if ((control.touched || control.dirty) && control.valid) {
      return 'is-valid';
    }
    return '';
  }

  /** Retrieves the validation message based on control errors */
  getValidationMessage(): string | null {
    const control = this.control;
    if (!control || !control.errors) {
      return null;
    }

    if (control.errors['required']) {
      return `${this.label} is required.`;
    }
    if (control.errors['invalidNumber']) {
      return `${this.label} must be a valid number.`;
    }
    if (control.errors['minlength']) {
      return `${this.label} must be at least ${control.errors['minlength'].requiredLength} characters long.`;
    }
    if (control.errors['maxlength']) {
      return `${this.label} cannot exceed ${control.errors['maxlength'].requiredLength} characters.`;
    }
    if (control.errors['pattern']) {
      return `${this.label} format is invalid.`;
    }

    return null;
  }

  /** Handles keydown events to allow only numbers if specified */
  onKeydown(event: KeyboardEvent): void {
    if (this.onlyNumbers) {
      const allowedKeys = ['Backspace', 'Tab', 'ArrowLeft', 'ArrowRight', 'Delete'];
      const isNumberKey = event.key >= '0' && event.key <= '9';
      const isAllowedKey = allowedKeys.includes(event.key);

      const inputElement = event.target as HTMLInputElement;
      const isDecimalKey = event.key === '.' && !inputElement.value.includes('.');
      const isNegativeKey =
        event.key === '-' &&
        inputElement.selectionStart === 0 &&
        !inputElement.value.includes('-');

      if (!isNumberKey && !isAllowedKey && !isDecimalKey && !isNegativeKey) {
        event.preventDefault();
      }
    }
  }
}
