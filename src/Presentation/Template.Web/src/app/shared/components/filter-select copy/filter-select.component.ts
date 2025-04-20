import { CommonModule } from '@angular/common';
import {
  Component,
  forwardRef,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  NG_VALIDATORS,
  Validator,
  AbstractControl,
  ValidationErrors,
  FormControl,
  FormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-filter-select',
  templateUrl: './filter-select.component.html',
  styleUrls: ['./filter-select.component.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FilterSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => FilterSelectComponent),
      multi: true,
    },
  ],
})
export class FilterSelectComponent
  implements ControlValueAccessor, Validator, OnInit, OnChanges
{
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['options'] && changes['options'].currentValue) {
      this.setFilterText();
    }
  }
  @Input() options: any[] = [];
  @Input() placeholder = 'Select an option';
  @Input() required = false;
  @Input() label = '';
  @Input() bind!: { key: string; value: string };

  filteredOptions: any[] = [];
  selectedKey: string | null = null;
  filterText = '';
  selectedFilterText = '';
  isOpen = false;
  isTouched = false;

  onChange = (value: any) => {};
  onTouched = () => {};

  internalControl: FormControl = new FormControl();

  ngOnInit() {
    this.filteredOptions = [...this.options];
    if (this.required) {
      this.internalControl.setValidators(this.validate.bind(this));
    }
  }

  writeValue(value: any): void {
    this.selectedKey = value;
    this.setFilterText();
    this.internalControl.setValue(value);
    this.updateValidation();

    const filterText = this.options.find(
      (opt) => opt[this.bind.key] === value
    )?.[this.bind.value];
    if (filterText) {
      this.filterText = this.selectedFilterText = filterText;
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
    this.internalControl.valueChanges.subscribe(fn);
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  validate(control: AbstractControl): ValidationErrors | null {
    if (this.required && !this.selectedKey) {
      return { required: true };
    }
    return null;
  }

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.filteredOptions = [...this.options];
      this.filterText = '';
    }
  }

  selectOption(option: any): void {
    this.selectedKey = option[this.bind.key];
    this.filterText = option[this.bind.value];
    this.selectedFilterText = option[this.bind.value];
    this.isOpen = false;
    this.onChange(this.selectedKey);
    this.internalControl.setValue(this.selectedKey);
    this.markAsTouched();
    this.updateValidation();
  }

  clearSelection(): void {
    this.selectedKey = null;
    this.filterText = '';
    this.selectedFilterText = '';
    this.onChange(null);
    this.internalControl.setValue(null);
    this.markAsTouched();
    this.updateValidation();
  }

  onFilterChange(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.filterText = inputElement.value;
    this.filteredOptions = this.options.filter((option) =>
      option[this.bind.value]
        .toLowerCase()
        .includes(this.filterText.toLowerCase())
    );
  }

  markAsTouched(): void {
    if (!this.isTouched) {
      this.onTouched();
      this.isTouched = true;
    }
    this.internalControl.markAsTouched();
  }

  private setFilterText(): void {
    if (this.selectedKey) {
      const selectedOption = this.options.find(
        (option) => option[this.bind.key] === this.selectedKey
      );
      if (selectedOption) {
        this.filterText = selectedOption[this.bind.value];
      }
    } else {
      this.filterText = '';
    }
  }

  private updateValidation(): void {
    this.internalControl.updateValueAndValidity();
  }
}
