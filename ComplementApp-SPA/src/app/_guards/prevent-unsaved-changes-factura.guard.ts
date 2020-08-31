import { Injectable, ViewChild } from '@angular/core';
import {
  CanDeactivate
} from '@angular/router';
import { NgForm } from '@angular/forms';
import { UsuarioEditComponent } from '../Usuario/usuario-edit/usuario-edit.component';
import { FacturaEditComponent } from '../facturaCompromiso/factura-edit/factura-edit.component';


@Injectable()
export class PreventUnsavedChangesFactura
  implements CanDeactivate<FacturaEditComponent> {
  canDeactivate(component: FacturaEditComponent): boolean {

    // if (component.registerForm.dirty) {
    //   return confirm(
    //     '¿Estas seguro de que quieres continuar? Los cambios en el formulario se perderán'
    //   );
    // }
    return true;
  }
}
