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
  selector: 'app-popup-factura',
  templateUrl: './popup-factura.component.html',
  styleUrls: ['./popup-factura.component.scss'],
})
export class PopupFacturaComponent implements OnInit {
  terId = 0;
  title: string;
  crp: number;
  closeBtnName: string;
  list: any[] = [];
  listaPlanPago: PlanPago[];
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };
  arrayControls = new FormArray([]);
  popupForm = new FormGroup({});
  idfacturaSeleccionada = 0;
  facturaSeleccionada: PlanPago = null;
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

    this.bsModalRef.content = null;
  }

  cargarPlanesPago() {
    this.listaEstadoId =
      this.estadoPlanPagoPorPagar.toString(); // Por pagar

    this.facturaService
      .ObtenerListaPlanPagoXCompromiso(this.crp, this.listaEstadoId)
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
              'No existen Facturas en estado por “Obligar” para el compromiso ' +
                this.crp
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
      .ObtenerListaPlanPagoXCompromiso(
        this.crp,
        this.listaEstadoId,
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
              'No existen Facturas en estado por “Obligar” para el compromiso ' +
                this.crp
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
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.idfacturaSeleccionada = +event.target.value;
    }
  }

  onAceptar() {
    this.facturaSeleccionada = this.listaPlanPago.filter(
      (x) => x.planPagoId === this.idfacturaSeleccionada
    )[0];

    this.bsModalRef.hide();
    // this.bsModalRef.content = this.facturaSeleccionada;
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }
}
