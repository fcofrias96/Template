import { Subject } from "rxjs";

export interface DynamicModalComponent {
  closeModal: Subject<void>;
}
