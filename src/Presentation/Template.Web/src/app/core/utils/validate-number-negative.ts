import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function validateNumberOrNegative(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    if (!value) {
      return null;
    }


    const isValid = /^-?\d+(\.\d+)?$/.test(value);

    return isValid ? null : { invalidNumber: true };
  };
};
