import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  ControlValueAccessor,
  NG_VALUE_ACCESSOR
} from '@angular/forms';

@Component({
  selector: 'app-checkbox-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkbox-list.component.html',
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => CheckboxListComponent),
    multi: true
  }]
})
export class CheckboxListComponent implements ControlValueAccessor {
  @Input() items: any[] = [];
  @Input() labelKey = 'label';
  @Input() valueKey = 'id';
  @Input() label!: string;
  @Input() formControlName!: string;

  // Ahora siempre strings
  selectedItems: string[] = [];
  private onChange: (value: any[]) => void = () => {};
  private onTouched = () => {};
  isDisabled = false;

  writeValue(value: any[] | null): void {
    if (!Array.isArray(value)) {
      this.selectedItems = [];
      return;
    }
    // Mapea objetos y primitivos a string
    this.selectedItems = value.map(val => {
      if (val != null && typeof val === 'object') {
        return String(val[this.valueKey]);
      }
      return String(val);
    });
  }

  registerOnChange(fn: any): void { this.onChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }
  setDisabledState(disabled: boolean): void { this.isDisabled = disabled; }

  isSelected(item: any): boolean {
    // Compara como string–string
    return this.selectedItems.includes(String(item[this.valueKey]));
  }

  onCheckboxChange(item: any, checked: boolean): void {
    const id = String(item[this.valueKey]);
    if (checked) {
      this.selectedItems = [...this.selectedItems, id];
    } else {
      this.selectedItems = this.selectedItems.filter(sel => sel !== id);
    }
    // Devolver el array de IDs (strings)
    this.onChange(this.selectedItems);
    this.onTouched();
  }
}
