import { Injectable } from '@angular/core';
import {
  CanDeactivate
} from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable()
export class PreventUnsavedChanges
  implements CanDeactivate<MemberEditComponent> {
  canDeactivate(component: MemberEditComponent): boolean {
    if (component.editForm.dirty) {
      return confirm(
        '¿Estas seguro de que quieres continuar? Los cambios en el formulario se perderán'
      );
    }
    return true;
  }
}

