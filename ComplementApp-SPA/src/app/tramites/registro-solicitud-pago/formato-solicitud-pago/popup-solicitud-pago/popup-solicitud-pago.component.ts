import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';

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
  }

  createEmptyForm() {
    this.popupForm = this.fb.group({
      numeroFacturaCtrl: ['', Validators.required],
      valorFacturaCtrl: ['', Validators.required],
      actividadEconomicaCtrl: [null, Validators.required],
      fechaInicioCtrl: ['', Validators.required],
      fechaFinCtrl: ['', Validators.required],
      observacionesCtrl: ['', Validators.required],
      numeroPlanillaCtrl: ['', Validators.required],
      mesCtrl: ['', Validators.required],
      baseCotizacionCtrl: ['', Validators.required],
    });
    this.popupForm.reset();
  }

  createFullForm() {
    let numeroFacturaC = '';
    let valorFacturaC = 0;
    let observacionesC = '';
    let numeroPlanillaC = '';
    let baseCotizacionC = 0;
    let fechaInicio = null;
    let fechaFinal = null;

    numeroFacturaC = this.formatoSolicitudPagoEdit.numeroFactura;
    valorFacturaC = this.formatoSolicitudPagoEdit.valorFacturado;
    observacionesC = this.formatoSolicitudPagoEdit.observaciones;
    numeroPlanillaC = this.formatoSolicitudPagoEdit.numeroPlanilla;
    baseCotizacionC = this.formatoSolicitudPagoEdit.baseCotizacion;

    this.idMesSelecionado = this.formatoSolicitudPagoEdit.mesId;
    this.idActividadEconomicaSelecionada = this.formatoSolicitudPagoEdit.actividadEconomicaId;

    fechaInicio = this.formatoSolicitudPagoEdit.fechaInicio;
    fechaFinal = this.formatoSolicitudPagoEdit.fechaFin;

    this.popupForm = this.fb.group({
      numeroFacturaCtrl: [numeroFacturaC, Validators.required],
      valorFacturaCtrl: [valorFacturaC, Validators.required],
      actividadEconomicaCtrl: [
        this.idActividadEconomicaSelecionada,
        Validators.required,
      ],
      fechaInicioCtrl: [
        formatDate(fechaInicio, 'dd-MM-yyyy', 'en'),
        Validators.required,
      ],
      fechaFinCtrl: [
        formatDate(fechaFinal, 'dd-MM-yyyy', 'en'),
        Validators.required,
      ],
      observacionesCtrl: [observacionesC, Validators.required],
      numeroPlanillaCtrl: [numeroPlanillaC, Validators.required],
      mesCtrl: [this.idMesSelecionado, Validators.required],
      baseCotizacionCtrl: [baseCotizacionC, Validators.required],
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
      const valueFechaFin = formValues.fechaFinCtrl;

      if (this.isValidDate(valueFechaInicio)) {
        dateFechaInicio = valueFechaInicio;
      } else {
        if (valueFechaInicio && valueFechaInicio.indexOf('-') > -1) {
          dateFechaInicio = this.dateString2Date(valueFechaInicio);
        }
      }

      if (this.isValidDate(valueFechaFin)) {
        dateFechaFin = valueFechaFin;
      } else {
        if (valueFechaFin && valueFechaFin.indexOf('-') > -1) {
          dateFechaFin = this.dateString2Date(valueFechaFin);
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
        valorFacturado: +formValues.valorFacturaCtrl,
        actividadEconomicaId: this.idActividadEconomicaSelecionada,
        actividadEconomicaDescripcion: this.actividadEconomicaSeleccionada
          .codigo,
        fechaInicio: dateFechaInicio,
        fechaFin: dateFechaFin,
        observaciones: formValues.observacionesCtrl,
        numeroPlanilla: formValues.numeroPlanillaCtrl,
        mesId: this.idMesSelecionado,
        mes: this.mesSeleccionado.nombre.toUpperCase(),
        baseCotizacion: +formValues.baseCotizacionCtrl,
      };

      this.bsModalRef.hide();
    }
  }

  dateString2Date(dateString: string) {
    const day = +dateString.substr(0, 2);
    const month = +dateString.substr(3, 2) - 1;
    const year = +dateString.substr(6, 4);
    const dateFechaProveedor = new Date(year, month, day);
    return dateFechaProveedor;
  }

  isValidDate(d) {
    return d instanceof Date;
  }

  get actividadEconomicaCtrl() {
    return this.popupForm.get('actividadEconomicaCtrl');
  }

  get mesControl() {
    return this.popupForm.get('mesCtrl');
  }
}
