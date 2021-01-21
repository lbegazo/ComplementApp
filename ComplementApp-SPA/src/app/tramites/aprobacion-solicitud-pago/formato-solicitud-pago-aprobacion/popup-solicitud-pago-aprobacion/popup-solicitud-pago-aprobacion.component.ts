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
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';

@Component({
  selector: 'app-popup-solicitud-pago-aprobacion',
  templateUrl: './popup-solicitud-pago-aprobacion.component.html',
  styleUrls: ['./popup-solicitud-pago-aprobacion.component.css'],
})
export class PopupSolicitudPagoAprobacionComponent implements OnInit {
  //Datos de ingreso
  title: string;
  rubrosPresupuestales: DetalleCDP[];

  //Datos de salida
  observaciones: string;
  numeroContratista: string;
  numeroSupervisor: string;
  fechaContratista: Date;
  fechaSupervisor: Date;

  popupForm = new FormGroup({});
  arrayControls = new FormArray([]);
  bsConfig: Partial<BsDaterangepickerConfig>;

  constructor(
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.crearControlesRubros();

    this.createEmptyForm();
  }

  crearControlesRubros() {
    for (const detalle of this.rubrosPresupuestales) {
      this.arrayControls.push(
        new FormGroup({
          rubroControl: new FormControl('', [Validators.required]),
        })
      );
    }
  }

  createEmptyForm() {
    this.popupForm = this.fb.group({
      numeroContratistaCtrl: ['', Validators.required],
      fechaContratistaCtrl: [null, Validators.required],
      numeroSupervisorCtrl: ['', Validators.required],
      fechaSupervisorCtrl: [null, Validators.required],
      observacionesCtrl: ['', Validators.required],
      rubrosControles: this.arrayControls,
    });
    this.popupForm.reset();
  }

  EliminarRubroPresupuestal(index: number) {
    if (this.rubrosPresupuestales.length > 1) {
      this.rubrosPresupuestales.splice(index, 1);
      this.arrayControls.removeAt(index);
    } else {
      this.alertify.warning('Debe existir por lo menos un rubro presupuestal');
    }
  }

  onAceptar() {
    if (this.popupForm.valid) {
      const formValues = Object.assign({}, this.popupForm.value);

      //#region Read dates

      //#region Rubros Presupuestales

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

      //#endregion

      let dateFechaProveedor = null;
      let dateFechaSupervisor = null;
      const valueFechaContratista = formValues.fechaContratistaCtrl;
      const valueFechaSupervisor = formValues.fechaSupervisorCtrl;

      if (GeneralService.isValidDate(valueFechaContratista)) {
        dateFechaProveedor = valueFechaContratista;
      } else {
        if (valueFechaContratista && valueFechaContratista.indexOf('-') > -1) {
          dateFechaProveedor = GeneralService.dateString2Date(
            valueFechaContratista
          );
        }
      }

      if (GeneralService.isValidDate(valueFechaSupervisor)) {
        dateFechaSupervisor = valueFechaSupervisor;
      } else {
        if (valueFechaSupervisor && valueFechaSupervisor.indexOf('-') > -1) {
          dateFechaSupervisor = GeneralService.dateString2Date(
            valueFechaSupervisor
          );
        }
      }

      //#endregion Read dates

      this.observaciones = formValues.observacionesCtrl;
      this.numeroContratista = formValues.numeroContratistaCtrl;
      this.fechaContratista = dateFechaProveedor;
      this.numeroSupervisor = formValues.numeroSupervisorCtrl;
      this.fechaSupervisor = dateFechaSupervisor;

      this.bsModalRef.hide();
    }
  }

  get rubrosControles() {
    return this.popupForm.get('rubrosControles') as FormArray;
  }

  get numeroContratistaCtrl() {
    return this.popupForm.get('numeroContratistaCtrl');
  }
  get fechaContratistaCtrl() {
    return this.popupForm.get('fechaContratistaCtrl');
  }
  get numeroSupervisorCtrl() {
    return this.popupForm.get('numeroSupervisorCtrl');
  }
  get fechaSupervisorCtrl() {
    return this.popupForm.get('fechaSupervisorCtrl');
  }
  get observacionesCtrl() {
    return this.popupForm.get('observacionesCtrl');
  }
}
