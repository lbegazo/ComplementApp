import { Component, OnInit, Input } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import {
  FormBuilder,
  FormArray,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { FacturaService } from 'src/app/_services/factura.service';
import { PlanPago } from 'src/app/_models/planPago';
import { Tercero } from 'src/app/_models/tercero';
import { FiltroFactura } from 'src/app/_models/filtroFactura';

@Component({
  selector: 'app-popup-buscar-factura',
  templateUrl: './popup-buscar-factura.component.html',
  styleUrls: ['./popup-buscar-factura.component.css'],
})
export class PopupBuscarFacturaComponent implements OnInit {
  terId = 0;
  title: string;
  radicarFactura: boolean;
  closeBtnName: string;
  list: any[] = [];
  listaPlanPago: PlanPago[];
  arrayControls = new FormArray([]);
  popupForm = new FormGroup({});
  arrayRubro: number[] = [];
  listaEstadoId = '';

  estadoPlanPagoPorPagar = 4;

  constructor(
    public bsModalRef: BsModalRef,
    private facturaService: FacturaService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.cargarPlanesPago();
  }

  cargarPlanesPago() {

    if (this.radicarFactura) {
      //#region Radicar factura

      this.listaEstadoId = '4'; // Por pagar

      //#endregion Radicar factura
    } else {
      //#region Modificar factura
      this.listaEstadoId = '5,13'; // ESTADO: Por Obligar y rechazada

      //#endregion Modificar factura
    }

    this.facturaService
      .ObtenerListaPlanPago(this.terId, this.listaEstadoId)
      .subscribe(
        (documentos: PlanPago[]) => {
          this.listaPlanPago = documentos;

          if (this.listaPlanPago && this.listaPlanPago.length > 0) {
            for (const detalle of this.listaPlanPago) {
              this.arrayControls.push(
                new FormGroup({
                  rubroControl: new FormControl('', [Validators.required]),
                })
              );
            }
          } else {
            this.alertify.warning(
              'No existen Radicados de Facturas en estado por “Obligar” para el tercero registrado'
            );
          }
        },
        (error) => {
          this.alertify.error(error);
        }
      );

    this.popupForm = this.fb.group({
      planPagoControles: this.arrayControls,
    });
  }

  onCheckChange(event) {
    /* Selected */
    this.arrayRubro = [];
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.arrayRubro?.push(+event.target.value);
    } else {
      /* unselected */
      let index = 0;
      let i = 0;
      this.arrayRubro.forEach((val: number) => {
        if (val === event.target.value) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.arrayRubro.splice(index, 1);
      }
    }
  }

  onAceptar() {
    // console.log('Popup ' + this.arrayRubro);
    this.bsModalRef.hide();
    this.bsModalRef.content = this.arrayRubro;
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }
}
