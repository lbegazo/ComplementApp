import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-popup-solicitud-pago-rechazo',
  templateUrl: './popup-solicitud-pago-rechazo.component.html',
  styleUrls: ['./popup-solicitud-pago-rechazo.component.scss']
})
export class PopupSolicitudPagoRechazoComponent implements OnInit {
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
