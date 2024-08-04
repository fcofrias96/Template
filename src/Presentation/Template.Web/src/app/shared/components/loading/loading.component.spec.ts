import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoadingComponent } from './loading.component';
import { LoadingService } from '@services/loading.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { of } from 'rxjs';

describe('LoadingComponent', () => {
  let component: LoadingComponent;
  let fixture: ComponentFixture<LoadingComponent>;
  let mockLoadingService: Partial<LoadingService>;

  beforeEach(async () => {
    // Crear un mock del LoadingService
    mockLoadingService = {
      loading$: of(false) // Supongamos que inicialmente no está cargando
    };

    await TestBed.configureTestingModule({
      imports: [MatProgressSpinnerModule, CommonModule,LoadingComponent],
      providers: [
        { provide: LoadingService, useValue: mockLoadingService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoadingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should react to loading changes', () => {
    // Simular un cambio en el estado de carga
    mockLoadingService.loading$ = of(true);
    component.loading.subscribe(isLoading => {
      expect(isLoading).toBe(true);
    });

    // Simular un cambio a no cargando
    mockLoadingService.loading$ = of(false);
    component.loading.subscribe(isLoading => {
      expect(isLoading).toBe(false);
    });
  });
});
