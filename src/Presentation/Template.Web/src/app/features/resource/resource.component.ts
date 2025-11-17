import { Component } from '@angular/core';
import { ResourceListComponent } from './components/resource-list/resource-list.component';
import { ResourceFormComponent } from './components/resource-form/resource-form.component';
import { ModalService } from '@shared/services/modal.service';

@Component({
  selector: 'app-resource',
  standalone: true,
  imports: [ResourceListComponent],
  templateUrl: './resource.component.html',
  styleUrl: './resource.component.scss',
})
export class ResourceComponent {
  constructor(private modalService: ModalService) { }

  addResource() {
    this.modalService.openModal(ResourceFormComponent, 'Add Resource', null, {
      backdrop: 'static',
    });
  }
}
