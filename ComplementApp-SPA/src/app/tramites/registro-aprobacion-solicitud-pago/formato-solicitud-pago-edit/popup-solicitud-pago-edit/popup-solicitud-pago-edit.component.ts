import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { DetalleFormatoSolicitudPagoDto } from 'src/app/_dto/detalleFormatoSolicitudPagoDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { ModalidadContrato, TipoIva, TipoPago } from 'src/app/_models/enum';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { PlanPago } from 'src/app/_models/planPago';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';

@Component({
  selector: 'app-popup-solicitud-pago-edit',
  templateUrl: './popup-solicitud-pago-edit.component.html',
  styleUrls: ['./popup-solicitud-pago-edit.component.css'],
})
export class PopupSolicitudPagoEditComponent implements OnInit {
  // Datos de ingreso
  listaActividadEconomica: ValorSeleccion[];
  listaMeses: ValorSeleccion[];
  formatoSolicitudPagoEdit: FormatoSolicitudPago;
  title: string;
  parametroLiquidacionTercero: ParametroLiquidacionTercero;
  planPagoSeleccionada: PlanPago;
  rubrosPresupuestales: DetalleCDP[];

  formatoSolicitudPago: FormatoSolicitudPago = null;
  dateObj = new Date();

  idMesSelecionado: number;
  mesSeleccionado: ValorSeleccion;

  actividadEconomicaSeleccionada: ValorSeleccion;
  idActividadEconomicaSelecionada: number;

  popupForm = new FormGroup({});
  arrayControls = new FormArray([]);
  bsConfig: Partial<BsDaterangepickerConfig>;

  constructor(
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.idMesSelecionado = this.dateObj.getUTCMonth() + 1;
    this.mesSeleccionado = this.listaMeses.filter(
      (x) => x.id === this.idMesSelecionado
    )[0];

    if (
      this.listaActividadEconomica &&
      this.listaActividadEconomica.length === 1
    ) {
      this.actividadEconomicaSeleccionada = this.listaActividadEconomica[0];
      this.idActividadEconomicaSelecionada = this.actividadEconomicaSeleccionada.id;
    }

    this.createEmptyForm();

    if (
      this.formatoSolicitudPagoEdit &&
      this.formatoSolicitudPagoEdit.numeroFactura.length > 0
    ) {
      this.createFullForm();
    } else {
      this.createEmptyForm();
    }

    if (this.parametroLiquidacionTercero) {
      if (
        this.parametroLiquidacionTercero.modalidadContrato ===
        ModalidadContrato.ContratoPrestacionServicio.value
      ) {
        this.valorBaseGravableRentaCtrl.disable();
        this.valorIvaCtrl.disable();
        this.numeroPlanillaCtrl.enable();
        this.mesCtrl.enable();
        this.baseCotizacionCtrl.enable();
      } else {
        if (
          this.parametroLiquidacionTercero.tipoPago === TipoPago.Variable.value
        ) {
          this.valorBaseGravableRentaCtrl.enable();
        } else {
          this.valorBaseGravableRentaCtrl.disable();
        }

        if (this.parametroLiquidacionTercero.tipoIva === TipoIva.Variable) {
          this.valorIvaCtrl.enable();
        } else {
          this.valorIvaCtrl.disable();
        }

        this.numeroPlanillaCtrl.disable();
        this.mesCtrl.disable();
        this.baseCotizacionCtrl.disable();
      }

      if (this.parametroLiquidacionTercero.facturaElectronicaId === 0) {
        this.numeroFacturaCtrl.disable();
      } else {
        this.numeroFacturaCtrl.enable();
      }
    }

    this.crearControlesRubros(0);

    this.popupForm.setControl('rubrosControles', this.arrayControls);
  }

  createEmptyForm() {
    this.popupForm = this.fb.group({
      numeroFacturaCtrl: ['', Validators.required],
      valorFacturaCtrl: ['', Validators.required],
      actividadEconomicaCtrl: [null, Validators.required],
      fechaInicioCtrl: ['', Validators.required],
      fechaFinalCtrl: ['', Validators.required],
      valorBaseGravableRentaCtrl: ['', Validators.required],
      valorIvaCtrl: ['', Validators.required],
      observacionesCtrl: ['', Validators.required],
      numeroPlanillaCtrl: ['', Validators.required],
      mesCtrl: ['', Validators.required],
      baseCotizacionCtrl: ['', Validators.required],
      rubrosControles: this.arrayControls,
    });
    this.popupForm.reset();

    if (this.actividadEconomicaSeleccionada) {
      this.popupForm.patchValue({
        actividadEconomicaCtrl: this.actividadEconomicaSeleccionada,
      });
    }
  }

  createFullForm() {
    let numeroFacturaC = '';
    let valorFacturaC = '';
    let observacionesC = '';
    let numeroPlanillaC = '';
    let baseCotizacionC = '';
    let fechaInicio = null;
    let fechaFinal = null;
    let valorBaseGravableC = '';
    let valorIvaC = '';

    numeroFacturaC = this.formatoSolicitudPagoEdit.numeroFactura;
    valorFacturaC = GeneralService.obtenerFormatoMoney(
      this.formatoSolicitudPagoEdit.valorFacturado
    );
    observacionesC = this.formatoSolicitudPagoEdit.observaciones;
    numeroPlanillaC = this.formatoSolicitudPagoEdit.numeroPlanilla;
    baseCotizacionC = GeneralService.obtenerFormatoMoney(
      this.formatoSolicitudPagoEdit.baseCotizacion
    );
    valorBaseGravableC = GeneralService.obtenerFormatoMoney(
      this.formatoSolicitudPagoEdit.valorBaseGravableRenta
    );
    valorIvaC = GeneralService.obtenerFormatoMoney(
      this.formatoSolicitudPagoEdit.valorIva
    );

    this.idMesSelecionado = this.formatoSolicitudPagoEdit.mesId;
    this.idActividadEconomicaSelecionada = this.formatoSolicitudPagoEdit.actividadEconomicaId;

    fechaInicio = this.formatoSolicitudPagoEdit.fechaInicio;
    fechaFinal = this.formatoSolicitudPagoEdit.fechaFinal;

    this.popupForm.patchValue({
      numeroFacturaCtrl: numeroFacturaC,
      valorFacturaCtrl: valorFacturaC,
      actividadEconomicaCtrl: this.actividadEconomicaSeleccionada,
      fechaInicioCtrl: formatDate(fechaInicio, 'dd-MM-yyyy', 'en'),
      fechaFinalCtrl: formatDate(fechaFinal, 'dd-MM-yyyy', 'en'),
      valorBaseGravableRentaCtrl: valorBaseGravableC,
      valorIvaCtrl: valorIvaC,
      observacionesCtrl: observacionesC,
      numeroPlanillaCtrl: numeroPlanillaC,
      mesCtrl: this.mesSeleccionado,
      baseCotizacionCtrl: baseCotizacionC,
    });
  }

  crearControlesRubros(valor: number) {
    if (this.rubrosPresupuestales) {
      for (const detalle of this.rubrosPresupuestales) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(
              GeneralService.obtenerFormatoLongMoney(valor),
              [Validators.required]
            ),
          })
        );
      }
    }
  }

  EliminarRubroPresupuestal(index: number) {
    if (this.rubrosPresupuestales.length > 1) {
      this.rubrosPresupuestales.splice(index, 1);
      this.arrayControls.removeAt(index);
    } else {
      this.alertify.warning('Debe existir por lo menos un rubro presupuestal');
    }
  }

  onSelectMes() {
    this.mesSeleccionado = this.mesControl.value as ValorSeleccion;
    this.idMesSelecionado = this.mesSeleccionado.id;
  }

  onSelectActividadEconomica() {
    this.actividadEconomicaSeleccionada = this.actividadEconomicaCtrl
      .value as ValorSeleccion;
    this.idActividadEconomicaSelecionada = this.actividadEconomicaSeleccionada.id;
  }

  onAceptar() {
    if (this.popupForm.valid) {
      const formValues = Object.assign({}, this.popupForm.value);

      const valorIngresado = +GeneralService.obtenerValorAbsoluto(
        formValues.valorFacturaCtrl
      );

      if (!this.validarValorFacturado(valorIngresado)) {
        this.alertify.warning(
          'El valor facturado no puede ser superior al Valor a Pagar'
        );
        return;
      }

      //#region Read dates

      let dateFechaInicio = null;
      let dateFechaFin = null;
      const valueFechaInicio = formValues.fechaInicioCtrl;
      const valueFechaFin = formValues.fechaFinalCtrl;

      if (GeneralService.isValidDate(valueFechaInicio)) {
        dateFechaInicio = valueFechaInicio;
      } else {
        if (valueFechaInicio && valueFechaInicio.indexOf('-') > -1) {
          dateFechaInicio = GeneralService.dateString2Date(valueFechaInicio);
        }
      }

      if (GeneralService.isValidDate(valueFechaFin)) {
        dateFechaFin = valueFechaFin;
      } else {
        if (valueFechaFin && valueFechaFin.indexOf('-') > -1) {
          dateFechaFin = GeneralService.dateString2Date(valueFechaFin);
        }
      }

      //#endregion Read dates

      //#region Read Rubros Presupuestales

      const arrayControles = this.rubrosControles as FormArray;
      if (arrayControles && arrayControles.length > 0) {
        for (let index = 0; index < arrayControles.length; index++) {
          const item = arrayControles.at(index);
          const itemDetalle = this.rubrosPresupuestales[index];
          itemDetalle.valorSolicitud = GeneralService.obtenerValorAbsoluto(
            item.value.rubroControl
          );
        }
      }

      if (!this.validarValorRubroPresupuestal(valorIngresado)) {
        this.alertify.warning(
          'Los valores registrados en los Rubros Presupuestales no es igual al valor facturado del Plan de Pago.'
        );
        return;
      }

      const listaDetalle = this.leerRubrosPresupuestales();

      //#endregion Read Rubros Presupuestales

      // Se inserta la actividad económica como primer elemento
      this.formatoSolicitudPago = {
        formatoSolicitudPagoId: 0,
        terceroId: 0,
        planPagoId: 0,
        crp: 0,
        numeroFactura:
          formValues.numeroFacturaCtrl !== undefined
            ? formValues.numeroFacturaCtrl.trim()
            : 0,
        valorFacturado: +GeneralService.obtenerValorAbsoluto(
          formValues.valorFacturaCtrl
        ),
        actividadEconomicaId: this.idActividadEconomicaSelecionada,
        actividadEconomicaDescripcion: this.actividadEconomicaSeleccionada
          .codigo,
        fechaInicio: dateFechaInicio,
        fechaFinal: dateFechaFin,
        observaciones:
          formValues.observacionesCtrl !== undefined
            ? formValues.observacionesCtrl.trim()
            : '',
        valorBaseGravableRenta: +GeneralService.obtenerValorAbsoluto(
          formValues.valorBaseGravableRentaCtrl
        ),
        valorIva: +GeneralService.obtenerValorAbsoluto(formValues.valorIvaCtrl),
        numeroPlanilla:
          formValues.numeroPlanillaCtrl !== undefined
            ? formValues.numeroPlanillaCtrl.trim()
            : '',
        mesId: this.idMesSelecionado,
        mes: this.mesSeleccionado.nombre.toUpperCase(),
        baseCotizacion: +GeneralService.obtenerValorAbsoluto(
          formValues.baseCotizacionCtrl
        ),
        supervisorId: 0,
        detallesFormatoSolicitudPago: listaDetalle,
      };

      this.bsModalRef.hide();
    }
  }

  replicarValorFacturado() {
    const formValues = Object.assign({}, this.popupForm.value);

    const valorFacturado = +GeneralService.obtenerValorAbsoluto(
      formValues.valorFacturaCtrl
    );

    if (this.rubrosPresupuestales && this.rubrosPresupuestales.length === 1) {
      const rubroPresupuestal = this.rubrosPresupuestales[0];
      rubroPresupuestal.valorSolicitud = valorFacturado;

      this.arrayControls.clear();
      this.crearControlesRubros(valorFacturado);

      this.popupForm.setControl('rubrosControles', this.arrayControls);
    }
  }

  leerRubrosPresupuestales() {
    const listaDetalle: DetalleFormatoSolicitudPagoDto[] = [];

    this.rubrosPresupuestales.forEach((element) => {
      if (element.valorSolicitud > 0) {
        const rubro = new ValorSeleccion();

        rubro.id = element.rubroPresupuestal.rubroPresupuestalId;
        rubro.codigo = element.rubroPresupuestal.identificacion;
        rubro.nombre = element.rubroPresupuestal.nombre;

        const item: DetalleFormatoSolicitudPagoDto = {
          detalleFormatoSolicitudPagoId: 0,
          formatoSolicitudPagoId: 0,
          valorAPagar: element.valorSolicitud,
          rubroPresupuestal: rubro,
        };

        listaDetalle.push(item);
      }
    });

    return listaDetalle;
  }

  validarValorFacturado(valor: number): boolean {
    const valorAPagar = this.planPagoSeleccionada.valorAPagar;
    if (valor > valorAPagar) {
      return false;
    }
    return true;
  }

  validarValorRubroPresupuestal(valor: number): boolean {
    let valorTotalRubro = 0;
    this.rubrosPresupuestales.forEach((element) => {
      if (element.valorSolicitud > 0) {
        valorTotalRubro = valorTotalRubro + +element.valorSolicitud;
      }
    });

    if (valor !== valorTotalRubro) {
      return false;
    }
    return true;
  }

  //#region Controles

  get rubrosControles() {
    return this.popupForm.get('rubrosControles') as FormArray;
  }

  get numeroFacturaCtrl() {
    return this.popupForm.get('numeroFacturaCtrl');
  }

  get actividadEconomicaCtrl() {
    return this.popupForm.get('actividadEconomicaCtrl');
  }

  get mesControl() {
    return this.popupForm.get('mesCtrl');
  }

  get valorBaseGravableRentaCtrl() {
    return this.popupForm.get('valorBaseGravableRentaCtrl');
  }

  get valorIvaCtrl() {
    return this.popupForm.get('valorIvaCtrl');
  }

  get numeroPlanillaCtrl() {
    return this.popupForm.get('numeroPlanillaCtrl');
  }

  get mesCtrl() {
    return this.popupForm.get('mesCtrl');
  }

  get baseCotizacionCtrl() {
    return this.popupForm.get('baseCotizacionCtrl');
  }

  //#endregion Controles
}
