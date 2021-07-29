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
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { ActividadEspecifica } from 'src/app/_models/actividadEspecifica';
import { Cdp } from 'src/app/_models/cdp';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { EstadoModificacion } from 'src/app/_models/enum';
import { RubroPresupuestal } from 'src/app/_models/rubroPresupuestal';
import { Transaccion } from 'src/app/_models/transaccion';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';
import { PlanAdquisicionService } from 'src/app/_services/planAdquisicion.service';
import { PopupActividadEspecificaComponent } from './popup-actividad-especifica/popup-actividad-especifica.component';
import { PopupCompromisoComponent } from './popup-compromiso/popup-compromiso.component';
import { PopupRubroPresupuestalComponent } from './popup-rubro-presupuestal/popup-rubro-presupuestal.component';

@Component({
  selector: 'app-plan-adquisicion',
  templateUrl: './plan-adquisicion.component.html',
  styleUrls: ['./plan-adquisicion.component.css'],
})
export class PlanAdquisicionComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  subscriptions: Subscription[] = [];
  arrayControls = new FormArray([]);
  mostrarCabecera = true;

  listaPlanAdquisicion: DetalleCDP[] = [];
  actividadEspecificaSeleccionado: ActividadEspecifica;
  rubroPresupuestalSeleccionado: RubroPresupuestal;
  planAdquisicionSeleccionado: DetalleCDP;
  cdpSeleccionado: Cdp;
  planAdquisicionId = 0;
  inicioPlanAdquisicionId = 5000;
  valor = 0;

  listaResponsable: ValorSeleccion[] = [];
  listaDependencia: ValorSeleccion[] = [];

  idResponsableSeleccionado?: number;
  responsableSeleccionado: ValorSeleccion = null;

  idDependenciaSeleccionado?: number;
  dependenciaSeleccionado: ValorSeleccion = null;

  nombreBoton = 'Agregar';
  accion = true; // True: Agregar, False: Modificar
  habilitarBotonPopupActividadEspecifica = true;
  habilitarBotonPopupRubroPresupuestal = false;
  habilitarBotonPopupCompromiso = false;
  bsModalRef: BsModalRef;

  facturaHeaderForm = new FormGroup({});
  nombreValor = 'Valor Actividad';

  constructor(
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private planAdquisicionService: PlanAdquisicionService,
    private router: Router,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.createEmptyForm();

    this.cargarListasResolver();

    this.onBuscarActividadesEspecificas();
  }

  createEmptyForm() {
    this.facturaHeaderForm = this.fb.group({
      nombreCtrl: ['', Validators.required],
      valorCtrl: [0, Validators.required],
      responsableCtrl: [null, Validators.required],
      dependenciaCtrl: [null, Validators.required],
      aplicaContratoCtrl: [null],
      planPagoControles: this.arrayControls,
    });
  }

  cargarListasResolver() {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.route.data.subscribe((data) => {
      this.listaResponsable = data['responsable'];
    });
    this.route.data.subscribe((data) => {
      this.listaDependencia = data['dependencia'];
    });
  }

  crearControlesDeArray() {
    if (this.listaPlanAdquisicion && this.listaPlanAdquisicion.length > 0) {
      for (const detalle of this.listaPlanAdquisicion) {
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
      const respuesta = this.validarValorActividadEspecifica();
      if (respuesta) {
        if (this.accion) {
          //#region Agregar

          this.inicioPlanAdquisicionId = this.inicioPlanAdquisicionId + 1;
          const planAdquisicionNuevo = new DetalleCDP();
          planAdquisicionNuevo.planAdquisicionId = this.inicioPlanAdquisicionId;
          planAdquisicionNuevo.planDeCompras = this.nombreCtrl.value;
          planAdquisicionNuevo.rubroPresupuestal = new RubroPresupuestal();
          planAdquisicionNuevo.rubroPresupuestal.rubroPresupuestalId =
            this.rubroPresupuestalSeleccionado.rubroPresupuestalId;
          planAdquisicionNuevo.rubroPresupuestal.identificacion =
            this.rubroPresupuestalSeleccionado.identificacion;
          planAdquisicionNuevo.rubroPresupuestal.nombre =
            this.rubroPresupuestalSeleccionado.nombre;
          planAdquisicionNuevo.rubroPresupuestal.padreRubroId =
            this.rubroPresupuestalSeleccionado.padreRubroId;
          planAdquisicionNuevo.aplicaContrato =
            this.aplicaContratoCtrl.value === null ||
            this.aplicaContratoCtrl.value === false
              ? false
              : true;
          planAdquisicionNuevo.usuarioId = this.idResponsableSeleccionado;
          planAdquisicionNuevo.responsable = this.responsableSeleccionado;

          planAdquisicionNuevo.dependenciaId = this.idDependenciaSeleccionado;
          planAdquisicionNuevo.valorAct = GeneralService.obtenerValorAbsoluto(
            this.valorCtrl.value
          );
          planAdquisicionNuevo.actividadEspecifica =
            this.actividadEspecificaSeleccionado;
          planAdquisicionNuevo.estadoModificacion =
            EstadoModificacion.Insertado;
          this.listaPlanAdquisicion.push(planAdquisicionNuevo);

          this.onRegistrar(planAdquisicionNuevo);

          //#endregion Agregar
        } else {
          //#region Modificar
          (this.planAdquisicionSeleccionado.planDeCompras =
            this.nombreCtrl.value),
            (this.planAdquisicionSeleccionado.aplicaContrato =
              this.aplicaContratoCtrl.value === null ||
              this.aplicaContratoCtrl.value === false
                ? false
                : true),
            (this.planAdquisicionSeleccionado.usuarioId =
              this.idResponsableSeleccionado),
            (this.planAdquisicionSeleccionado.responsable =
              this.responsableSeleccionado),
            (this.planAdquisicionSeleccionado.dependenciaId =
              this.idDependenciaSeleccionado),
            (this.planAdquisicionSeleccionado.valorAct =
              GeneralService.obtenerValorAbsoluto(this.valorCtrl.value));
          this.planAdquisicionSeleccionado.crp = this.cdpSeleccionado.crp;
          (this.planAdquisicionSeleccionado.estadoModificacion =
            EstadoModificacion.Modificado),
            this.onRegistrar(this.planAdquisicionSeleccionado);

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

  abrirPopupActividadEspecifica() {
    //#region Abrir Popup

    const initialState = {
      title: 'Indicadores de Seguimiento Presupuestal',
    };

    this.bsModalRef = this.modalService.show(
      PopupActividadEspecificaComponent,
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
          this.actividadEspecificaSeleccionado = this.bsModalRef
            .content as ActividadEspecifica;
          this.habilitarBotonPopupRubroPresupuestal = true;
        }
        this.unsubscribe();
      })
    );

    this.subscriptions.push(combine);

    //#endregion Cargar información del popup (OnHidden event)
  }

  abrirPopupRubroPresupuestal() {
    //#region Abrir Popup

    const initialState = {
      title: 'Rubro Presupuestal',
      rubroPapaId:
        this.actividadEspecificaSeleccionado.rubroPresupuestal
          .rubroPresupuestalId,
    };

    this.bsModalRef = this.modalService.show(
      PopupRubroPresupuestalComponent,
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
          this.rubroPresupuestalSeleccionado = this.bsModalRef
            .content as RubroPresupuestal;
        }
        this.unsubscribe();
      })
    );

    this.subscriptions.push(combine);

    //#endregion Cargar información del popup (OnHidden event)
  }

  abrirPopupCompromiso() {
    //#region Abrir Popup

    const initialState = {
      title: 'Compromiso',
    };

    this.bsModalRef = this.modalService.show(
      PopupCompromisoComponent,
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
          this.cdpSeleccionado = this.bsModalRef.content as Cdp;
        }
        this.unsubscribe();
      })
    );

    this.subscriptions.push(combine);

    //#endregion Cargar información del popup (OnHidden event)
  }

  onBuscarActividadesEspecificas() {
    this.planAdquisicionService.ObtenerListaPlanAnualAdquisicion().subscribe(
      (documentos: DetalleCDP[]) => {
        this.listaPlanAdquisicion = documentos;

        this.crearControlesDeArray();
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.facturaHeaderForm = this.fb.group({
          nombreCtrl: ['', Validators.required],
          valorCtrl: [0, Validators.required],
          responsableCtrl: [null, Validators.required],
          dependenciaCtrl: [null, Validators.required],
          aplicaContratoCtrl: [null],
          planPagoControles: this.arrayControls,
        });
      }
    );
  }

  limpiarVariables() {
    this.nombreBoton = 'Agregar';
    this.accion = true;
    this.habilitarBotonPopupActividadEspecifica = true;
    this.habilitarBotonPopupRubroPresupuestal = false;
    this.habilitarBotonPopupCompromiso = false;
    this.planAdquisicionSeleccionado = null;
    this.actividadEspecificaSeleccionado = null;
    this.rubroPresupuestalSeleccionado = null;
    this.cdpSeleccionado = null;

    this.responsableSeleccionado = null;
    this.dependenciaSeleccionado = null;
    this.idResponsableSeleccionado = 0;
    this.idDependenciaSeleccionado = 0;
    this.planAdquisicionId = 0;
    this.valor = 0;
    this.nombreValor = 'Valor Actividad';
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

  onSeleccionarResponsable() {
    this.responsableSeleccionado = this.responsableCtrl.value as ValorSeleccion;
    this.idResponsableSeleccionado = +this.responsableSeleccionado.id;
  }

  onSeleccionarDependencia() {
    this.dependenciaSeleccionado = this.dependenciaCtrl.value as ValorSeleccion;
    this.idDependenciaSeleccionado = +this.dependenciaSeleccionado.id;
  }

  onCheckChange(event) {
    /* Selected */
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.planAdquisicionId = +event.target.value;
      this.cargarPlanAquisicion();
      this.nombreBoton = 'Modificar';
      this.nombreValor = 'Valor Modificación';
      this.accion = false;
      this.habilitarBotonPopupActividadEspecifica = false;
      this.habilitarBotonPopupRubroPresupuestal = false;
      this.habilitarBotonPopupCompromiso = true;
    }
  }

  cargarPlanAquisicion() {
    if (this.planAdquisicionId > 0) {
      this.planAdquisicionSeleccionado = this.listaPlanAdquisicion.filter(
        (x) => x.planAdquisicionId === this.planAdquisicionId
      )[0];

      if (this.planAdquisicionSeleccionado !== null) {
        this.responsableSeleccionado = this.listaResponsable.filter(
          (x) => x.id === this.planAdquisicionSeleccionado.usuarioId
        )[0];
        if (this.responsableSeleccionado !== null) {
          this.idResponsableSeleccionado = this.responsableSeleccionado.id;
        }
        this.dependenciaSeleccionado = this.listaDependencia.filter(
          (x) => x.id === this.planAdquisicionSeleccionado.dependenciaId
        )[0];
        if (this.dependenciaSeleccionado !== null) {
          this.idDependenciaSeleccionado = this.dependenciaSeleccionado.id;
        }
        const aplicaContrato = this.planAdquisicionSeleccionado.aplicaContrato;

        this.actividadEspecificaSeleccionado =
          this.planAdquisicionSeleccionado.actividadEspecifica;
        this.rubroPresupuestalSeleccionado =
          this.planAdquisicionSeleccionado.rubroPresupuestal;
        this.cdpSeleccionado = this.planAdquisicionSeleccionado.cdpDocumento;

        this.valor = this.planAdquisicionSeleccionado.saldoAct;

        this.facturaHeaderForm.patchValue({
          nombreCtrl: this.planAdquisicionSeleccionado.planDeCompras,
          valorCtrl: 0,
          responsableCtrl: this.responsableSeleccionado,
          dependenciaCtrl: this.dependenciaSeleccionado,
          aplicaContratoCtrl: aplicaContrato,
        });
      }
    }
  }

  onRegistrar(planAdquisicion: DetalleCDP) {
    let respuesta = 0;

    if (planAdquisicion) {
      this.planAdquisicionService
        .RegistrarPlanAdquisicion(planAdquisicion)
        .subscribe(
          (response: any) => {
            if (!isNaN(response)) {
              respuesta = +response;
              this.alertify.success('Criterio de indicador registrado');
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
          () => {
            this.onBuscarActividadesEspecificas();
          }
        );
    }
  }

  onCancelar() {
    this.limpiarVariables();
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  validarValorActividadEspecifica() {
    let respuesta = false;
    let esNegativo = false;

    const saldoActual = this.planAdquisicionSeleccionado.saldoAct;

    const valorIngresado = +GeneralService.obtenerValorAbsoluto(
      this.valorCtrl.value
    );
    esNegativo = valorIngresado > 0 ? false : true;

    if (esNegativo) {
      const valorDiferencia = saldoActual + valorIngresado;
      if (valorDiferencia < 0) {
        this.alertify.error(
          'El valor ingresado debe ser menor o igual al saldo'
        );
        return;
      }
    }

    if (
      this.actividadEspecificaSeleccionado !== null &&
      this.actividadEspecificaSeleccionado !== undefined &&
      this.actividadEspecificaSeleccionado.actividadEspecificaId > 0
    ) {
      const valorDisponible =
        +this.actividadEspecificaSeleccionado.saldoPorProgramar;

      // Modificacion
      if (!this.accion) {
        const valorExistente = this.planAdquisicionSeleccionado.valorAct;

        // No se hizo modificaciones al valor
        if (valorExistente === valorIngresado) {
          respuesta = true;
          return respuesta;
        }
      }

      if (valorDisponible < valorIngresado) {
        this.alertify.error(
          'El valor debe ser menor o igual al saldo de la actividad específica'
        );
        return;
      } else {
        respuesta = true;
        return respuesta;
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

  get aplicaContratoCtrl() {
    return this.facturaHeaderForm.get('aplicaContratoCtrl');
  }

  get responsableCtrl() {
    return this.facturaHeaderForm.get('responsableCtrl');
  }

  get dependenciaCtrl() {
    return this.facturaHeaderForm.get('dependenciaCtrl');
  }
}
