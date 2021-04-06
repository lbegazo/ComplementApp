import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { combineLatest } from 'rxjs';
import { Subscription } from 'rxjs';
import { ActividadGeneralPrincipalDto } from 'src/app/_dto/actividadGeneralPrincipalDto';
import { ActividadEspecifica } from 'src/app/_models/actividadEspecifica';
import { ActividadGeneral } from 'src/app/_models/actividadGeneral';
import { EstadoModificacion } from 'src/app/_models/enum';
import { Transaccion } from 'src/app/_models/transaccion';
import { ActividadGeneralService } from 'src/app/_services/actividadGeneral.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';
import { PopupRubroDecretoComponent } from './popup-rubro-decreto/popup-rubro-decreto.component';

@Component({
  selector: 'app-ejecucion-presupuestal',
  templateUrl: './ejecucion-presupuestal.component.html',
  styleUrls: ['./ejecucion-presupuestal.component.scss'],
})
export class EjecucionPresupuestalComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  subscriptions: Subscription[] = [];
  arrayControls = new FormArray([]);
  mostrarCabecera = true;

  listaActividad: ActividadEspecifica[] = [];
  actividadGeneralSeleccionado: ActividadGeneral;
  actividadSeleccionado: ActividadEspecifica;
  actividadEspecificaId = 0;
  inicioActividadEspecificaId = 5000;
  valor = 0;

  nombreBoton = 'Agregar';
  accion = true; // True: Agregar, False: Modificar
  habilitarBotonPopup = true;
  bsModalRef: BsModalRef;

  facturaHeaderForm = new FormGroup({});

  constructor(
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private actividadService: ActividadGeneralService,
    private router: Router,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.createEmptyForm();

    this.onBuscarActividadesEspecificas();
  }

  createEmptyForm() {
    this.facturaHeaderForm = this.fb.group({
      nombreCtrl: ['', Validators.required],
      valorCtrl: [0, Validators.required],
      saldoCtrl: [0],
      planPagoControles: this.arrayControls,
    });
  }

  crearControlesDeArray() {
    if (this.listaActividad && this.listaActividad.length > 0) {
      for (const detalle of this.listaActividad) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning(
        'No existen criterios de seguimiento a la ejecución presupuestal'
      );
    }
  }

  onAgregar() {
    if (this.facturaHeaderForm.valid) {
      if (this.validarValor()) {
        if (this.accion) {
          //#region Agregar

          this.inicioActividadEspecificaId =
            this.inicioActividadEspecificaId + 1;
          const actividadEspecificaNuevo = new ActividadEspecifica();
          actividadEspecificaNuevo.actividadEspecificaId = this.inicioActividadEspecificaId;
          actividadEspecificaNuevo.nombre = this.nombreCtrl.value;
          actividadEspecificaNuevo.valorApropiacionVigente = GeneralService.obtenerValorAbsoluto(
            this.valorCtrl.value
          );
          actividadEspecificaNuevo.saldoPorProgramar = GeneralService.obtenerValorAbsoluto(
            this.valorCtrl.value
          );
          actividadEspecificaNuevo.actividadGeneral = this.actividadGeneralSeleccionado;
          actividadEspecificaNuevo.rubroPresupuestal = this.actividadGeneralSeleccionado.rubroPresupuestal;
          actividadEspecificaNuevo.estadoModificacion =
            EstadoModificacion.Insertado;
          this.listaActividad.push(actividadEspecificaNuevo);

          this.arrayControls.push(
            new FormGroup({
              rubroControl: new FormControl(''),
            })
          );

          //#endregion Agregar
        } else {
          //#region Modificar

          this.actividadSeleccionado.nombre = this.nombreCtrl.value;
          this.actividadSeleccionado.estadoModificacion =
            EstadoModificacion.Modificado;
          this.actividadSeleccionado.valorApropiacionVigente = GeneralService.obtenerValorAbsoluto(
            this.valorCtrl.value
          );
          this.actividadSeleccionado.saldoPorProgramar = GeneralService.obtenerValorAbsoluto(
            this.valorCtrl.value
          );

          //#endregion Modificar
        }

        this.onLimpiar();
      }
    }
  }

  onLimpiar() {
    this.limpiarVariables();
    this.limpiarControles();
  }

  abrirPopup() {
    //#region Abrir Popup

    const initialState = {
      title: 'Rubro Presupuestal Nivel Decreto',
    };

    this.bsModalRef = this.modalService.show(
      PopupRubroDecretoComponent,
      Object.assign({ initialState }, { class: 'gray modal-lg' })
    );

    //#endregion Abrir Popup

    //#region Cargar información del popup (OnHidden event)

    const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (this.bsModalRef.content !== null) {
          this.actividadGeneralSeleccionado = this.bsModalRef
            .content as ActividadGeneral;
        }
        this.unsubscribe();
      })
    );

    this.subscriptions.push(combine);

    //#endregion Cargar información del popup (OnHidden event)
  }

  onBuscarActividadesEspecificas() {
    this.actividadService.ObtenerActividadesEspecificas().subscribe(
      (documentos: ActividadEspecifica[]) => {
        this.listaActividad = documentos;

        this.crearControlesDeArray();
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.facturaHeaderForm = this.fb.group({
          nombreCtrl: ['', Validators.required],
          valorCtrl: [0, Validators.required],
          saldoCtrl: [0],
          planPagoControles: this.arrayControls,
        });
      }
    );
  }

  limpiarVariables() {
    this.nombreBoton = 'Agregar';
    this.accion = true;
    this.habilitarBotonPopup = true;
    this.actividadSeleccionado = null;
    this.actividadGeneralSeleccionado = null;
    this.actividadEspecificaId = 0;
    this.valor = 0;
  }

  limpiarControles() {
    this.facturaHeaderForm.reset();
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  onCheckChange(event) {
    /* Selected */
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.actividadEspecificaId = +event.target.value;
      this.cargarActividadEspecifica();
      this.nombreBoton = 'Modificar';
      this.accion = false;

      if (this.actividadEspecificaId < 5000) {
        this.habilitarBotonPopup = false;
      } else {
        this.habilitarBotonPopup = true;
      }
    }
  }

  cargarActividadEspecifica() {
    if (this.actividadEspecificaId > 0) {
      this.actividadSeleccionado = this.listaActividad.filter(
        (x) => x.actividadEspecificaId === this.actividadEspecificaId
      )[0];

      if (this.actividadSeleccionado !== null) {
        this.valor = this.actividadSeleccionado.valorApropiacionVigente;
        this.facturaHeaderForm.patchValue({
          nombreCtrl: this.actividadSeleccionado.nombre,
          valorCtrl: GeneralService.obtenerFormatoLongMoney(
            this.actividadSeleccionado.valorApropiacionVigente
          ),
          saldoCtrl: GeneralService.obtenerFormatoLongMoney(
            this.actividadSeleccionado.saldoPorProgramar
          ),
        });
        this.actividadGeneralSeleccionado = this.actividadSeleccionado.actividadGeneral;
      }
    }
  }

  onRegistrar() {
    const actividadGeneralPrincipal: ActividadGeneralPrincipalDto = new ActividadGeneralPrincipalDto();

    let respuesta = 0;

    if (this.listaActividad && this.listaActividad.length > 0) {
      actividadGeneralPrincipal.listaActividadEspecifica = this.listaActividad;

      this.actividadService
        .RegistrarActividadesEspecificas(actividadGeneralPrincipal)
        .subscribe(
          (response: any) => {
            if (!isNaN(response)) {
              respuesta = +response;
              this.alertify.success('Criterios de indicadores registrados');
            } else {
              this.alertify.error(
                'No se pudo registrar los valores de las apropiaciones asignadas a la entidad'
              );
            }
          },

          (error) => {
            this.alertify.error(
              'Hubó un error al registrar los valores de las apropiaciones asignadas a la entidad ' +
                error
            );
          },
          () => {}
        );
    }
  }

  onCancelar() {
    this.limpiarVariables();
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  validarValor() {
    let respuesta = false;

    if (
      this.actividadGeneralSeleccionado !== null &&
      this.actividadGeneralSeleccionado.actividadGeneralId > 0
    ) {
      const valorDisponible = +this.actividadGeneralSeleccionado
        .apropiacionDisponible;
      const valorIngresado = +GeneralService.obtenerValorAbsoluto(
        this.valorCtrl.value
      );

      if (valorDisponible < valorIngresado) {
        this.alertify.error(
          'El valor debe ser menor o igual al saldo del rubro presupuestal a nivel de decreto'
        );
        return;
      } else {
        this.saldoCtrl.patchValue(
          GeneralService.obtenerFormatoMoney(valorIngresado)
        );
        respuesta = true;
      }
    } else {
      this.alertify.warning(
        'Debe elegir un rubro presupuestal a nivel decreto'
      );
    }

    return respuesta;
  }

  get nombreCtrl() {
    return this.facturaHeaderForm.get('nombreCtrl');
  }

  get valorCtrl() {
    return this.facturaHeaderForm.get('valorCtrl');
  }

  get saldoCtrl() {
    return this.facturaHeaderForm.get('saldoCtrl');
  }
}
