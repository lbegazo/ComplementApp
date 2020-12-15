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
import { PlanPagoService } from 'src/app/_services/planPago.service';
import { PlanPago } from 'src/app/_models/planPago';
import { EstadoPlanPago } from 'src/app/_models/enum';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';

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
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10
  };
  arrayControls = new FormArray([]);
  popupForm = new FormGroup({});
  arrayRubro: number[] = [];
  listaEstadoId = '';

  estadoPlanPagoPorPagar = EstadoPlanPago.PorPagar.value;
  estadoPlanPagoPorObligar = EstadoPlanPago.PorObligar.value;
  estadoPlanPagoRechazada = EstadoPlanPago.Rechazada.value;

  constructor(
    public bsModalRef: BsModalRef,
    private facturaService: PlanPagoService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.cargarPlanesPago();
  }

  cargarPlanesPago() {
    if (this.radicarFactura) {
      //#region Radicar factura

      this.listaEstadoId = this.estadoPlanPagoPorPagar.toString(); // Por pagar

      //#endregion Radicar factura
    } else {
      //#region Modificar factura
      this.listaEstadoId =
        this.estadoPlanPagoPorObligar.toString() +
        ',' +
        this.estadoPlanPagoRechazada.toString(); // ESTADO: Por Obligar y rechazada

      //#endregion Modificar factura
    }

    this.facturaService
      .ObtenerListaPlanPago(this.listaEstadoId, this.terId)
      .subscribe(
        (documentos: PaginatedResult<PlanPago[]>) => {
          this.listaPlanPago = documentos.result;
          this.pagination = documentos.pagination;

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

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.cargarListaPlanPago();
  }

  cargarListaPlanPago() {
    this.facturaService
      .ObtenerListaPlanPago(
        this.listaEstadoId,
        this.terId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<PlanPago[]>) => {
          this.listaPlanPago = documentos.result;
          this.pagination = documentos.pagination;

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
    this.bsModalRef.hide();
    this.bsModalRef.content = this.arrayRubro;
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }
}
