import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { UserService } from '@features/user/services/user.service';
import { UserDetail } from '@features/user/types/user-detail.interface';
import { DynamicModalComponent } from '@models/dynamic-modal.interface';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.scss'
})
export class UserDetailComponent implements OnChanges, DynamicModalComponent {
  user$!: Observable<UserDetail>;
  @Input() userId!: string;

  constructor(private userService: UserService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['userId'] && this.userId) {
      this.user$ = this.userService.getById(this.userId);
    }
  }
  closeModal = new Subject<void>();
}
