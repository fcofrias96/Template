import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { Dropdown } from '@models/dropdown';

@Component({
  selector: 'app-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './select.component.html',
  styleUrl: './select.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectComponent),
      multi: true,
    },
  ],
})
export class SelectComponent implements ControlValueAccessor {
  @Input() label!: string;
  @Input() placeholder?: string;
  @Input() isRequired = false;
  @Input() options: Dropdown[] = [];
  @Input() validationMessage?: string;
  @Input() validationClass?: string;

  // Valor interno del componente
  value: any;

  // Callbacks que se asignarán desde el formulario reactivo
  onChange: any = (_: any) => { };
  onTouched: any = () => { };

  // Método que se llama cuando el formulario escribe un valor en el componente
  writeValue(value: any): void {
    this.value = value;
  }

  // Se registra la función que notifica cuando cambia el valor
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  // Se registra la función que notifica cuando el componente es tocado
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  // Opcional: manejar el estado disabled
  setDisabledState?(isDisabled: boolean): void {
    // Puedes implementar la lógica para deshabilitar el select si es necesario
  }

  // Se llama cuando cambia la selección en el elemento select
  onSelectChange(event: Event): void {
    const newValue = (event.target as HTMLSelectElement).value;
    this.value = newValue;
    this.onChange(newValue);
    this.onTouched();
  }
}
