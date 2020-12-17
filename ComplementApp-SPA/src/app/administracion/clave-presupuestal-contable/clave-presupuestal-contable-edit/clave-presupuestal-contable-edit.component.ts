import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { combineLatest, Subscription } from 'rxjs';
import { ClavePresupuestalContableDto } from 'src/app/_dto/clavePresupuestalContableDto';
import { RelacionContableDto } from 'src/app/_dto/relacionContableDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { Cdp } from 'src/app/_models/cdp';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClavePresupuestalContableService } from 'src/app/_services/clavePresupuestalContable.service';
import { PopupClavePresupuestalContableComponent } from './popup-clave-presupuestal-contable/popup-clave-presupuestal-contable.component';

@Component({
  selector: 'app-clave-presupuestal-contable-edit',
  templateUrl: './clave-presupuestal-contable-edit.component.html',
  styleUrls: ['./clave-presupuestal-contable-edit.component.scss'],
})
export class ClavePresupuestalContableEditComponent implements OnInit {
  @Input() listaClavePresupuestalContable: ClavePresupuestalContableDto[];
  @Input() cdpSeleccionado: Cdp;
  @Output() esCancelado = new EventEmitter<boolean>();

  cdpId: number;
  solicitudRegistrada = false;

  editForm = new FormGroup({});
  arrayControls = new FormArray([]);
  subscriptions: Subscription[] = [];
  bsModalRef: BsModalRef;

  listaUsoPresupuestal: ValorSeleccion[] = [];

  constructor(
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef,
    private clavePresupuestalService: ClavePresupuestalContableService
  ) {}

  ngOnInit() {
    this.cdpId = this.cdpSeleccionado.cdpId;
    this.cargarClavesPresupuestales();
  }

  cargarClavesPresupuestales() {
    if (
      this.listaClavePresupuestalContable &&
      this.listaClavePresupuestalContable.length > 0
    ) {
      for (const detalle of this.listaClavePresupuestalContable) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning(
        'No se pudo obtener información de los rubros presupuestales'
      );
    }

    this.editForm = this.fb.group({
      planPagoControles: this.arrayControls,
    });
  }

  cargarListaUsosPresupuestales() {}

  abrirPopup(index: number) {
    if (this.listaClavePresupuestalContable.length > 0) {
      const clavePresupuestal = this.listaClavePresupuestalContable[index];

      //#region Abrir Popup

      const initialState = {
        title: 'USO PRESUPUESTAL Y RELACION CONTABLE',
        clavePresupuestalContable: clavePresupuestal,
      };

      this.bsModalRef = this.modalService.show(
        PopupClavePresupuestalContableComponent,
        Object.assign({ initialState }, { class: 'gray modal-lg' })
      );

      //#endregion Abrir Popup

      //#region Cargar información del popup (OnHidden event)

      const combine = combineLatest(this.modalService.onHidden).subscribe(() =>
        this.changeDetection.markForCheck()
      );

      this.subscriptions.push(
        this.modalService.onHidden.subscribe((reason: string) => {
          if (
            this.bsModalRef.content != null &&
            this.bsModalRef.content.usoPresupuestalSeleccionado !== null &&
            this.bsModalRef.content.relacionContableSeleccionado !== null
          ) {
            const relacionContable = this.bsModalRef.content
              .relacionContableSeleccionado as RelacionContableDto;
            const valorRelacionContable: ValorSeleccion = new ValorSeleccion();
            (valorRelacionContable.id = relacionContable.relacionContableId),
              (valorRelacionContable.codigo =
                relacionContable.cuentaContable.codigo);

            clavePresupuestal.usoPresupuestal = this.bsModalRef.content
              .usoPresupuestalSeleccionado as ValorSeleccion;

            clavePresupuestal.relacionContable = valorRelacionContable;
          }
          this.unsubscribe();
        })
      );

      this.subscriptions.push(combine);

      //#endregion Cargar información del popup (OnHidden event)
    }
  }

  onGuardar() {
    if (this.validarListaClavePresupuetalContable) {
      this.clavePresupuestalService
        .RegistrarClavePresupuestalContable(this.listaClavePresupuestalContable)
        .subscribe(
          (response: any) => {
            if (!isNaN(response)) {
              this.alertify.success(
                'Se registraron los datos contables exitosamente'
              );
              this.solicitudRegistrada = true;
            } else {
              this.alertify.error('No se pudo registrar los datos contables');
            }
          },
          (error) => {
            this.alertify.error(
              'Hubó un error al registrar los datos contables ' + error
            );
          }
        );
    }
    else
    {
      this.alertify.warning('Debe definir los datos contables a los rubros presupuestales');
    }
  }

  onCancelar() {
    this.esCancelado.emit(true);
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  get validarListaClavePresupuetalContable() {
    const resultado = this.listaClavePresupuestalContable.filter(
      (x) => x.usoPresupuestal === null || x.relacionContable === null
    )[0];
    if (resultado === null || resultado === undefined) {
      return true;
    }
    return false;
  }
}
