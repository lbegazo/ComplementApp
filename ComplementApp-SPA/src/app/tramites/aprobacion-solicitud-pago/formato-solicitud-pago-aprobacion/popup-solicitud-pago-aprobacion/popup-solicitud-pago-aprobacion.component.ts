import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';

@Component({
  selector: 'app-popup-solicitud-pago-aprobacion',
  templateUrl: './popup-solicitud-pago-aprobacion.component.html',
  styleUrls: ['./popup-solicitud-pago-aprobacion.component.css'],
})
export class PopupSolicitudPagoAprobacionComponent implements OnInit {
  title: string;
  observaciones: string;

  popupForm = new FormGroup({});

  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder) {}

  ngOnInit() {
    this.createEmptyForm();
  }

  createEmptyForm() {
    this.popupForm = this.fb.group({
      observacionesCtrl: ['', Validators.required],
    });
    this.popupForm.reset();
  }

  onAceptar() {
    if (this.popupForm.valid) {
      const formValues = Object.assign({}, this.popupForm.value);
      this.observaciones = formValues.observacionesCtrl;
      this.bsModalRef.hide();
    }
  }
}
