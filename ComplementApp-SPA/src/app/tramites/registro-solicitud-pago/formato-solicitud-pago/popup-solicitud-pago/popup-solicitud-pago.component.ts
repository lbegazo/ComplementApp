import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { ModalidadContrato, TipoIva, TipoPago } from 'src/app/_models/enum';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { GeneralService } from 'src/app/_services/general.service';

@Component({
  selector: 'app-popup-solicitud-pago',
  templateUrl: './popup-solicitud-pago.component.html',
  styleUrls: ['./popup-solicitud-pago.component.scss'],
})
export class PopupSolicitudPagoComponent implements OnInit {
  listaActividadEconomica: ValorSeleccion[];
  listaMeses: ValorSeleccion[];
  formatoSolicitudPagoEdit: FormatoSolicitudPago;
  title: string;
  parametroLiquidacionTercero: ParametroLiquidacionTercero;

  formatoSolicitudPago: FormatoSolicitudPago = null;
  dateObj = new Date();

  idMesSelecionado: number;
  mesSeleccionado: ValorSeleccion;

  actividadEconomicaSeleccionada: ValorSeleccion;
  idActividadEconomicaSelecionada: number;

  popupForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;

  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder) {}

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
    }
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

      // Se inserta la actividad económica como primer elemento
      this.formatoSolicitudPago = {
        formatoSolicitudPagoId: 0,
        terceroId: 0,
        planPagoId: 0,
        crp: 0,
        numeroFactura: formValues.numeroFacturaCtrl,
        valorFacturado: +GeneralService.obtenerValorAbsoluto(
          formValues.valorFacturaCtrl
        ),
        actividadEconomicaId: this.idActividadEconomicaSelecionada,
        actividadEconomicaDescripcion: this.actividadEconomicaSeleccionada
          .codigo,
        fechaInicio: dateFechaInicio,
        fechaFinal: dateFechaFin,
        observaciones: formValues.observacionesCtrl,
        valorBaseGravableRenta: +GeneralService.obtenerValorAbsoluto(
          formValues.valorBaseGravableRentaCtrl
        ),
        valorIva: +GeneralService.obtenerValorAbsoluto(formValues.valorIvaCtrl),
        numeroPlanilla: formValues.numeroPlanillaCtrl,
        mesId: this.idMesSelecionado,
        mes: this.mesSeleccionado.nombre.toUpperCase(),
        baseCotizacion: +GeneralService.obtenerValorAbsoluto(
          formValues.baseCotizacionCtrl
        ),
        supervisorId: 0,
      };

      this.bsModalRef.hide();
    }
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
}
