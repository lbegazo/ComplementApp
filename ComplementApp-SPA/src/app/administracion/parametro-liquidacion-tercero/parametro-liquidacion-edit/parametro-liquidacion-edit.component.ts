import { formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { ModalidadContrato, TipoLista } from 'src/app/_models/enum';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { Tercero } from 'src/app/_models/tercero';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ListaService } from 'src/app/_services/lista.service';
import { TerceroService } from 'src/app/_services/tercero.service';
import { ParametroLiquidacionTerceroComponent } from '../parametro-liquidacion-tercero.component';

@Component({
  selector: 'app-parametro-liquidacion-edit',
  templateUrl: './parametro-liquidacion-edit.component.html',
  styleUrls: ['./parametro-liquidacion-edit.component.scss'],
})
export class ParametroLiquidacionEditComponent implements OnInit {
  @Input() esCreacion: boolean;
  @Input() tercero: Tercero;
  @Output() esCancelado = new EventEmitter<boolean>();

  parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  listaModalidadContrato: ValorSeleccion[] = [];
  listaTipoPago: ValorSeleccion[] = [];
  listaTipoIva: ValorSeleccion[] = [];
  listaTipoCuentaXPagar: ValorSeleccion[] = [];
  listaTipoDocumentoSoporte: ValorSeleccion[] = [];

  editForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  nombreBoton = 'Registrar';

  idModalidadContratoSelecionado?: number;
  modalidadContratoSeleccionado: ValorSeleccion = null;

  idTipoPagoSelecionado?: number;
  tipoPagoSeleccionado: ValorSeleccion = null;

  idTipoIvaSelecionado?: number;
  tipoIvaSeleccionado: ValorSeleccion = null;

  idTipoCuentaXPagarSelecionado?: number;
  tipoCuentaXPagarSeleccionado: ValorSeleccion = null;

  idTipoDocumentoSoporteSelecionado?: number;
  tipoDocumentoSoporteSeleccionado: ValorSeleccion = null;

  constructor(
    private listaService: ListaService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private terceroService: TerceroService
  ) {}

  ngOnInit() {
    this.cargarParametrosGenerales();

    this.cargarListas();

    this.createEmptyForm();

    if (!this.esCreacion) {
      this.buscarDetalleParametrizacion();
      this.nombreBoton = 'Guardar';
    } else {
      this.createEmptyForm();
      this.editForm.reset();
      this.nombreBoton = 'Registrar';
    }
  }

  buscarDetalleParametrizacion() {
    if (this.tercero.terceroId > 0) {
      if (!this.esCreacion) {
        this.terceroService
          .ObtenerParametrizacionLiquidacionXTercero(this.tercero.terceroId)
          .subscribe(
            (documento: ParametroLiquidacionTercero) => {
              if (documento) {
                this.parametroLiquidacionSeleccionado = documento;
              } else {
                this.alertify.error(
                  'No se pudo obtener información para la parametrización del tercero'
                );
              }
            },
            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.createFullForm();
            }
          );
      }
    }
  }

  createEmptyForm() {
    this.editForm = this.fb.group({
      modalidadContratoCtrl: [0, Validators.required],
      tipoPagoCtrl: [0, Validators.required],
      honorarioSinIvaCtrl: ['', Validators.required],
      tarifaIvaCtrl: ['', Validators.required],
      tipoIvaCtrl: [0, Validators.required],
      tipoCuentaXPagarCtrl: [0, Validators.required],
      tipoDocumentoSoporteCtrl: [0, Validators.required],

      baseAporteSaludCtrl: ['', Validators.required],
      aporteSaludCtrl: ['', Validators.required],
      aportePensionCtrl: ['', Validators.required],
      riesgoLaboralCtrl: ['', Validators.required],
      fondoSolidaridadCtrl: ['', Validators.required],

      pensionVoluntariaCtrl: ['', Validators.required],
      dependienteCtrl: ['', Validators.required],
      afcCtrl: ['', Validators.required],
      medicinaPrepagadaCtrl: ['', Validators.required],
      interesesViviendaCtrl: ['', Validators.required],
      fechaInicioCtrl: [null, Validators.required],
      fechaFinalCtrl: [null, Validators.required],
    });
  }

  createFullForm() {
    let honorarioSinIvaC = 0;
    let tarifaIvaC = 0;

    let baseAporteSaludC = 0;
    let aporteSaludC = 0;
    let aportePensionC = 0;
    let riesgoLaboralC = 0;
    let fondoSolidaridadC = 0;

    let pensionVoluntariaC = 0;
    let dependienteC = 0;
    let afcC = 0;
    let medicinaPrepagadaC = 0;
    let interesesViviendaC = 0;

    let fechaInicio = null;
    let fechaFinal = null;

    this.idModalidadContratoSelecionado =
      this.parametroLiquidacionSeleccionado.modalidadContrato > 0
        ? this.parametroLiquidacionSeleccionado.modalidadContrato
        : null;
    this.modalidadContratoSeleccionado = this.listaModalidadContrato.filter(
      (x) => x.id === this.idModalidadContratoSelecionado
    )[0];

    this.idTipoPagoSelecionado =
      this.parametroLiquidacionSeleccionado.tipoPago > 0
        ? this.parametroLiquidacionSeleccionado.tipoPago
        : null;
    if (this.idTipoPagoSelecionado !== null) {
      this.tipoPagoSeleccionado = this.listaTipoPago.filter(
        (x) => x.id === this.idTipoPagoSelecionado
      )[0];
    }

    this.idTipoIvaSelecionado =
      this.parametroLiquidacionSeleccionado.tipoIva > 0
        ? this.parametroLiquidacionSeleccionado.tipoIva
        : null;
    if (this.idTipoIvaSelecionado !== null) {
      this.tipoIvaSeleccionado = this.listaTipoIva.filter(
        (x) => x.id === this.idTipoIvaSelecionado
      )[0];
    }

    this.idTipoCuentaXPagarSelecionado =
      this.parametroLiquidacionSeleccionado.tipoCuentaPorPagar > 0
        ? this.parametroLiquidacionSeleccionado.tipoCuentaPorPagar
        : null;
    if (this.idTipoCuentaXPagarSelecionado !== null) {
      this.tipoCuentaXPagarSeleccionado = this.listaTipoCuentaXPagar.filter(
        (x) => x.id === this.idTipoCuentaXPagarSelecionado
      )[0];
    }

    this.idTipoDocumentoSoporteSelecionado =
      this.parametroLiquidacionSeleccionado.tipoDocumentoSoporte > 0
        ? this.parametroLiquidacionSeleccionado.tipoDocumentoSoporte
        : null;
    if (this.idTipoDocumentoSoporteSelecionado !== null) {
      this.tipoDocumentoSoporteSeleccionado = this.listaTipoDocumentoSoporte.filter(
        (x) => x.id === this.idTipoDocumentoSoporteSelecionado
      )[0];
    }

    honorarioSinIvaC = this.parametroLiquidacionSeleccionado?.honorarioSinIva;
    tarifaIvaC = this.parametroLiquidacionSeleccionado?.tarifaIva;

    baseAporteSaludC = this.parametroLiquidacionSeleccionado?.baseAporteSalud;
    aporteSaludC = this.parametroLiquidacionSeleccionado?.aporteSalud;
    aportePensionC = this.parametroLiquidacionSeleccionado?.aportePension;
    riesgoLaboralC = this.parametroLiquidacionSeleccionado?.riesgoLaboral;
    fondoSolidaridadC = this.parametroLiquidacionSeleccionado?.fondoSolidaridad;

    pensionVoluntariaC = this.parametroLiquidacionSeleccionado
      ?.pensionVoluntaria;
    dependienteC = this.parametroLiquidacionSeleccionado?.dependiente;
    afcC = this.parametroLiquidacionSeleccionado?.afc;
    medicinaPrepagadaC = this.parametroLiquidacionSeleccionado
      ?.medicinaPrepagada;
    interesesViviendaC = this.parametroLiquidacionSeleccionado?.interesVivienda;

    fechaInicio = this.parametroLiquidacionSeleccionado
      .fechaInicioDescuentoInteresVivienda;
    fechaFinal = this.parametroLiquidacionSeleccionado
      .fechaFinalDescuentoInteresVivienda;

    this.editForm = this.fb.group({
      modalidadContratoCtrl: [
        this.idModalidadContratoSelecionado,
        Validators.required,
      ],
      tipoPagoCtrl: [this.idTipoPagoSelecionado, Validators.required],
      honorarioSinIvaCtrl: [honorarioSinIvaC, Validators.required],
      tarifaIvaCtrl: [tarifaIvaC, Validators.required],
      tipoIvaCtrl: [this.idTipoIvaSelecionado, Validators.required],
      tipoCuentaXPagarCtrl: [
        this.idTipoCuentaXPagarSelecionado,
        Validators.required,
      ],
      tipoDocumentoSoporteCtrl: [
        this.idTipoDocumentoSoporteSelecionado,
        Validators.required,
      ],

      baseAporteSaludCtrl: [baseAporteSaludC, Validators.required],
      aporteSaludCtrl: [aporteSaludC, Validators.required],
      aportePensionCtrl: [aportePensionC, Validators.required],
      riesgoLaboralCtrl: [riesgoLaboralC, Validators.required],
      fondoSolidaridadCtrl: [fondoSolidaridadC, Validators.required],

      pensionVoluntariaCtrl: [pensionVoluntariaC, Validators.required],
      dependienteCtrl: [dependienteC, Validators.required],
      afcCtrl: [afcC, Validators.required],
      medicinaPrepagadaCtrl: [medicinaPrepagadaC, Validators.required],
      interesesViviendaCtrl: [interesesViviendaC, Validators.required],

      fechaInicioCtrl: [
        formatDate(fechaInicio, 'dd-MM-yyyy', 'en'),
        Validators.required,
      ],
      fechaFinalCtrl: [
        formatDate(fechaFinal, 'dd-MM-yyyy', 'en'),
        Validators.required,
      ],
    });

    this.ocultarControlesFormulario();
  }

  onModalidadContrato() {
    this.modalidadContratoSeleccionado = this.modalidadContratoCtrl
      .value as ValorSeleccion;
    this.idModalidadContratoSelecionado = +this.modalidadContratoSeleccionado
      .id;

    this.ocultarControlesFormulario();
  }

  ocultarControlesFormulario() {
    if (
      this.idModalidadContratoSelecionado ===
      ModalidadContrato.ContratoPrestacionServicio.value
    ) {
      this.baseAporteSaludCtrl.enable();
      this.aporteSaludCtrl.enable();
      this.aportePensionCtrl.enable();
      this.riesgoLaboralCtrl.enable();
      this.fondoSolidaridadCtrl.enable();

      this.pensionVoluntariaCtrl.enable();
      this.dependienteCtrl.enable();
      this.afcCtrl.enable();
      this.medicinaPrepagadaCtrl.enable();
      this.interesesViviendaCtrl.enable();
      this.fechaInicioCtrl.enable();
      this.fechaFinalCtrl.enable();
    }

    if (
      this.idModalidadContratoSelecionado ===
        ModalidadContrato.ProveedorConDescuento.value ||
      this.idModalidadContratoSelecionado ===
        ModalidadContrato.ProveedorSinDescuento.value
    ) {
      this.baseAporteSaludCtrl.disable();
      this.aporteSaludCtrl.disable();
      this.aportePensionCtrl.disable();
      this.riesgoLaboralCtrl.disable();
      this.fondoSolidaridadCtrl.disable();

      this.pensionVoluntariaCtrl.disable();
      this.dependienteCtrl.disable();
      this.afcCtrl.disable();
      this.medicinaPrepagadaCtrl.disable();
      this.interesesViviendaCtrl.disable();
      this.fechaInicioCtrl.disable();
      this.fechaFinalCtrl.disable();
    }
  }

  onTipoPago() {
    this.tipoPagoSeleccionado = this.tipoPagoCtrl.value as ValorSeleccion;
    this.idTipoPagoSelecionado = +this.tipoPagoSeleccionado.id;
  }

  onTipoCuentaXPagar() {
    this.tipoCuentaXPagarSeleccionado = this.tipoCuentaXPagarCtrl
      .value as ValorSeleccion;
    this.idTipoCuentaXPagarSelecionado = +this.tipoCuentaXPagarSeleccionado.id;
  }

  onTipoIva() {
    this.tipoIvaSeleccionado = this.tipoIvaCtrl.value as ValorSeleccion;
    this.idTipoIvaSelecionado = +this.tipoIvaSeleccionado.id;
  }

  onTipoDocumentoSoporte() {
    this.tipoDocumentoSoporteSeleccionado = this.tipoDocumentoSoporteCtrl
      .value as ValorSeleccion;
    this.idTipoDocumentoSoporteSelecionado = +this
      .tipoDocumentoSoporteSeleccionado.id;
  }

  onLimpiarForm() {
    this.idModalidadContratoSelecionado = 0;
    this.modalidadContratoSeleccionado = null;
    this.idTipoCuentaXPagarSelecionado = 0;
    this.tipoCuentaXPagarSeleccionado = null;
    this.idTipoDocumentoSoporteSelecionado = 0;
    this.tipoCuentaXPagarSeleccionado = null;
    this.idTipoIvaSelecionado = 0;
    this.tipoIvaSeleccionado = null;
    this.idTipoPagoSelecionado = 0;
    this.tipoPagoSeleccionado = null;

    this.editForm.reset();
  }

  onGuardar() {
    if (this.editForm.valid) {
      const formValues = Object.assign({}, this.editForm.value);

      //#region Read dates

      let dateFechaInicio = null;
      let dateFechaFinal = null;
      const valueFechaInicio = this.editForm.get('fechaInicioCtrl').value;
      const valueFechaFinal = this.editForm.get('fechaFinalCtrl').value;

      if (this.isValidDate(valueFechaInicio)) {
        dateFechaInicio = valueFechaInicio;
      } else {
        if (valueFechaInicio && valueFechaInicio.indexOf('-') > -1) {
          dateFechaInicio = this.dateString2Date(valueFechaInicio);
        }
      }

      if (this.isValidDate(valueFechaFinal)) {
        dateFechaFinal = valueFechaFinal;
      } else {
        if (valueFechaFinal && valueFechaFinal.indexOf('-') > -1) {
          dateFechaFinal = this.dateString2Date(valueFechaFinal);
        }
      }

      //#endregion Read dates

      if (this.esCreacion) {
        const parametroNuevo: ParametroLiquidacionTercero = {
          parametroLiquidacionTerceroId: 0,
          modalidadContrato: this.idModalidadContratoSelecionado,
          tipoCuentaPorPagar: this.idTipoCuentaXPagarSelecionado,
          tipoIva: this.idTipoIvaSelecionado,
          tipoDocumentoSoporte: this.idTipoDocumentoSoporteSelecionado,
          tipoPago: this.idTipoPagoSelecionado,
          honorarioSinIva: +formValues.honorarioSinIvaCtrl,
          tarifaIva: +formValues.tarifaIvaCtrl,

          baseAporteSalud:
            formValues.baseAporteSaludCtrl === undefined
              ? 0
              : +formValues.baseAporteSaludCtrl,
          aporteSalud:
            formValues.aporteSaludCtrl === undefined
              ? 0
              : +formValues.aporteSaludCtrl,
          aportePension:
            formValues.aportePensionCtrl === undefined
              ? 0
              : +formValues.aportePensionCtrl,
          riesgoLaboral:
            formValues.riesgoLaboralCtrl === undefined
              ? 0
              : +formValues.riesgoLaboralCtrl,
          fondoSolidaridad:
            formValues.fondoSolidaridadCtrl === undefined
              ? 0
              : +formValues.fondoSolidaridadCtrl,

          pensionVoluntaria:
            formValues.pensionVoluntariaCtrl === undefined
              ? 0
              : +formValues.pensionVoluntariaCtrl,
          dependiente:
            formValues.dependienteCtrl === undefined
              ? 0
              : +formValues.dependienteCtrl,
          afc: formValues.afcCtrl,
          medicinaPrepagada:
            formValues.medicinaPrepagadaCtrl === undefined
              ? 0
              : +formValues.medicinaPrepagadaCtrl,
          interesVivienda:
            formValues.interesesViviendaCtrl === undefined
              ? 0
              : +formValues.interesesViviendaCtrl,
          fechaInicioDescuentoInteresVivienda: dateFechaInicio,
          fechaFinalDescuentoInteresVivienda: dateFechaFinal,

          debito: '',
          credito: '',
          numeroCuenta: '',
          tipoCuenta: '',
          convenioFontic: 0,
          terceroId: this.tercero.terceroId,
        };
        this.terceroService
          .RegistrarParametroLiquidacionTercero(parametroNuevo)
          .subscribe(
            () => {
              this.editForm.reset(parametroNuevo);
              this.alertify.success(
                'La parametrización de liquidación se registró correctamente'
              );
            },

            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.esCancelado.emit(true);
            }
          );
      } else {
        this.parametroLiquidacionSeleccionado.modalidadContrato = this.idModalidadContratoSelecionado;
        this.parametroLiquidacionSeleccionado.tipoCuentaPorPagar = this.idTipoCuentaXPagarSelecionado;
        this.parametroLiquidacionSeleccionado.tipoIva = this.idTipoIvaSelecionado;
        this.parametroLiquidacionSeleccionado.tipoPago = this.idTipoPagoSelecionado;
        this.parametroLiquidacionSeleccionado.tipoDocumentoSoporte = this.idTipoDocumentoSoporteSelecionado;
        this.parametroLiquidacionSeleccionado.honorarioSinIva = +formValues.honorarioSinIvaCtrl;
        this.parametroLiquidacionSeleccionado.tarifaIva = +formValues.tarifaIvaCtrl;

        this.parametroLiquidacionSeleccionado.baseAporteSalud =
          formValues.baseAporteSaludCtrl === undefined
            ? 0
            : +formValues.baseAporteSaludCtrl;
        this.parametroLiquidacionSeleccionado.aporteSalud =
          formValues.aporteSaludCtrl === undefined
            ? 0
            : +formValues.aporteSaludCtrl;
        this.parametroLiquidacionSeleccionado.aportePension =
          formValues.aportePensionCtrl === undefined
            ? 0
            : +formValues.aportePensionCtrl;
        this.parametroLiquidacionSeleccionado.riesgoLaboral =
          formValues.riesgoLaboralCtrl === undefined
            ? 0
            : +formValues.riesgoLaboralCtrl;
        this.parametroLiquidacionSeleccionado.fondoSolidaridad =
          formValues.fondoSolidaridadCtrl === undefined
            ? 0
            : +formValues.fondoSolidaridadCtrl;

        this.parametroLiquidacionSeleccionado.pensionVoluntaria =
          formValues.pensionVoluntariaCtrl === undefined
            ? 0
            : +formValues.pensionVoluntariaCtrl;
        this.parametroLiquidacionSeleccionado.dependiente =
          formValues.dependienteCtrl === undefined
            ? 0
            : +formValues.dependienteCtrl;
        this.parametroLiquidacionSeleccionado.afc =
          formValues.afcCtrl === undefined ? 0 : +formValues.afcCtrl;
        this.parametroLiquidacionSeleccionado.medicinaPrepagada =
          formValues.medicinaPrepagadaCtrl === undefined
            ? 0
            : +formValues.medicinaPrepagadaCtrl;
        this.parametroLiquidacionSeleccionado.interesVivienda =
          formValues.interesesViviendaCtrl === undefined
            ? 0
            : +formValues.interesesViviendaCtrl;
        this.parametroLiquidacionSeleccionado.fechaInicioDescuentoInteresVivienda = dateFechaInicio;
        this.parametroLiquidacionSeleccionado.fechaFinalDescuentoInteresVivienda = dateFechaFinal;

        this.terceroService
          .ActualizarParametroLiquidacionTercero(
            this.parametroLiquidacionSeleccionado
          )
          .subscribe(
            () => {
              this.editForm.reset(this.parametroLiquidacionSeleccionado);
              this.alertify.success(
                'La parametrización de liquidación se modificó correctamente'
              );
            },

            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.esCancelado.emit(true);
            }
          );
      }
    }
  }

  onCancelar() {
    this.parametroLiquidacionSeleccionado = null;
    this.esCancelado.emit(true);
  }

  get modalidadContratoCtrl() {
    return this.editForm.get('modalidadContratoCtrl');
  }
  get tipoPagoCtrl() {
    return this.editForm.get('tipoPagoCtrl');
  }
  get tipoIvaCtrl() {
    return this.editForm.get('tipoIvaCtrl');
  }
  get tipoCuentaXPagarCtrl() {
    return this.editForm.get('tipoCuentaXPagarCtrl');
  }
  get tipoDocumentoSoporteCtrl() {
    return this.editForm.get('tipoDocumentoSoporteCtrl');
  }

  get baseAporteSaludCtrl() {
    return this.editForm.get('baseAporteSaludCtrl');
  }
  get aporteSaludCtrl() {
    return this.editForm.get('aporteSaludCtrl');
  }
  get aportePensionCtrl() {
    return this.editForm.get('aportePensionCtrl');
  }
  get riesgoLaboralCtrl() {
    return this.editForm.get('riesgoLaboralCtrl');
  }
  get fondoSolidaridadCtrl() {
    return this.editForm.get('fondoSolidaridadCtrl');
  }

  get pensionVoluntariaCtrl() {
    return this.editForm.get('pensionVoluntariaCtrl');
  }
  get dependienteCtrl() {
    return this.editForm.get('dependienteCtrl');
  }
  get afcCtrl() {
    return this.editForm.get('afcCtrl');
  }
  get medicinaPrepagadaCtrl() {
    return this.editForm.get('medicinaPrepagadaCtrl');
  }
  get interesesViviendaCtrl() {
    return this.editForm.get('interesesViviendaCtrl');
  }
  get fechaInicioCtrl() {
    return this.editForm.get('fechaInicioCtrl');
  }
  get fechaFinalCtrl() {
    return this.editForm.get('fechaFinalCtrl');
  }

  dateString2Date(dateString: string) {
    const day = +dateString.substr(0, 2);
    const month = +dateString.substr(3, 2) - 1;
    const year = +dateString.substr(6, 4);
    const dateFecha = new Date(year, month, day);
    return dateFecha;
  }

  isValidDate(d) {
    return d instanceof Date;
  }

  //#region Listas

  cargarParametrosGenerales() {}

  cargarListas() {
    this.cargarListaModalidadContrato(TipoLista.ModalidadContrato.value);
    this.cargarlistaTipoCuentaXPagar(TipoLista.TipoCuentaXPagar.value);
    this.cargarlistaTipoDocumentoSoporte(TipoLista.TipoDocumentoSoporte.value);
    this.cargarlistaTipoIva(TipoLista.TipoIva.value);
    this.cargarlistaTipoPago(TipoLista.TipoPago.value);
  }

  cargarListaModalidadContrato(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaModalidadContrato = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaTipoPago(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoPago = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaTipoIva(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoIva = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaTipoCuentaXPagar(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoCuentaXPagar = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaTipoDocumentoSoporte(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoDocumentoSoporte = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  //#endregion Listas
}
