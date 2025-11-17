import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ResourceService } from '@features/resource/services/resource.service';
import { ResourceDetail } from '@features/resource/types/resource-detail.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-resource-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './resource-detail.component.html',
  styleUrl: './resource-detail.component.scss'
})
export class ResourceDetailComponent implements OnChanges, DynamicModalComponent {
  resource$!: Observable<ResourceDetail>;
  @Input() resourceId!: string;

  constructor(private resourceService: ResourceService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['resourceId'] && this.resourceId) {
      this.resource$ = this.resourceService.getById(this.resourceId);
    }
  }
  closeModal = new Subject<void>();
}
