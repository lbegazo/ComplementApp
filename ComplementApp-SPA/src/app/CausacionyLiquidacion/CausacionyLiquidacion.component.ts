import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';

import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { environment } from 'src/environments/environment';

import { Tercero } from 'src/app/_models/tercero';
import { PlanPagoService } from 'src/app/_services/planPago.service';
import { PlanPago } from 'src/app/_models/planPago';
import { AlertifyService } from 'src/app/_services/alertify.service';
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormArray,
  FormControl,
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EstadoPlanPago, ModalidadContrato, TipoPago } from '../_models/enum';
import { DetallePlanPago } from '../_models/detallePlanPago';
import { ListaService } from '../_services/lista.service';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';
import { Transaccion } from '../_models/transaccion';

@Component({
  selector: 'app-causacionyliquidacion',
  templateUrl: './CausacionyLiquidacion.component.html',
  styleUrls: ['./CausacionyLiquidacion.component.css'],
})
export class CausacionyLiquidacionComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;
  search: string;
  suggestions$: Observable<Tercero[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  listaEstadoId: string;
  arrayControls = new FormArray([]);
  estadoPlanPagoPorObligar = EstadoPlanPago.PorObligar.value;
  mostrarCabecera = true;
  modalidadContrato = 0;
  tipoPago = 0;

  listaPlanPago: PlanPago[] = [];
  planPagoIdSeleccionado = 0;
  detallePlanPago: DetallePlanPago;
  planPagoSeleccionado: PlanPago;
  tercero: Tercero = {
    terceroId: 0,
    nombre: '',
    numeroIdentificacion: '',
    tipoIdentificacion: '',
  };

  facturaHeaderForm = new FormGroup({});
  terceroId?: number = null;
  baseUrl = environment.apiUrl + 'lista/ObtenerListaTercero';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };
  formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private facturaService: PlanPagoService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.createForm();
    this.onBuscarFactura();

    this.suggestions$ = new Observable((observer: Observer<string>) => {
      observer.next(this.search);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Tercero[]>(this.baseUrl, {
              params: { numeroIdentificacion: query },
            })
            .pipe(
              map((data: Tercero[]) => data || []),
              tap(
                () => noop,
                (err) => {
                  // in case of http error
                  this.errorMessage =
                    (err && err.message) ||
                    'Algo salió mal, consulte a su administrador';
                }
              )
            );
        }
        return of([]);
      })
    );
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      terceroCtrl: ['', Validators.required],
      terceroDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  obtenerDatosDeResolver() {
    this.route.data.subscribe((data) => {
      this.listaPlanPago = data['planPagoResolver'].result;
      this.pagination = data['planPagoResolver'].pagination;
      this.crearControlesDeArray();
    });
  }

  crearControlesDeArray() {
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
        'No existen Facturas en estado por “Obligar” para el tercero registrado'
      );
    }
  }

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  onBuscarFactura() {
    this.listaEstadoId = this.estadoPlanPagoPorObligar.toString(); // Por obligar

    this.facturaService
      .ObtenerListaPlanPago(
        this.listaEstadoId,
        this.terceroId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<PlanPago[]>) => {
          this.listaPlanPago = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.facturaHeaderForm = this.fb.group({
            terceroCtrl: ['', Validators.required],
            terceroDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura() {
    this.listaPlanPago = [];
    this.planPagoIdSeleccionado = 0;
    this.tercero = null;
    this.search = '';
    this.terceroId = null;
    this.detallePlanPago = null;
    this.formatoCausacionyLiquidacionPago = null;

    this.onBuscarFactura();
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  onCheckChange(event) {
    /* Selected */
    this.planPagoIdSeleccionado = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.planPagoIdSeleccionado = +event.target.value;
    }
  }

  onLiquidar() {
    if (
      this.listaPlanPago &&
      this.listaPlanPago.length > 0 &&
      this.planPagoIdSeleccionado > 0
    ) {
      this.planPagoSeleccionado = this.listaPlanPago.filter(
        (x) => x.planPagoId === this.planPagoIdSeleccionado
      )[0];

      if (this.planPagoSeleccionado) {
        this.modalidadContrato = this.planPagoSeleccionado.modalidadContrato;
        this.tipoPago = this.planPagoSeleccionado.tipoPago;

        if (
          this.modalidadContrato ===
            ModalidadContrato.ProveedorConDescuento.value &&
          this.tipoPago === TipoPago.Variable.value
        ) {
          //#region ProveedorConDescuento

          let resultado: string;
          let valorIngresado = 0;

          resultado = window.prompt(
            'Debe ingresar el Valor Base Gravable',
            '0'
          );

          if (isNaN(+resultado)) {
            this.alertify.warning('Debe ingresar un valor númerico');
          } else if (+resultado === 0) {
            this.alertify.warning('El valor ingresado debe ser mayor a cero');
          } else {
            valorIngresado = +resultado;

            if (valorIngresado > this.planPagoSeleccionado.valorFacturado) {
              this.alertify.warning(
                'Debe ingresar un valor menor al valor total a cancelar ' +
                  this.planPagoSeleccionado.valorFacturado
              );
            } else {
              this.obtenerDetallePlanPago(valorIngresado);
            }
          }

          //#endregion ProveedorConDescuento
        } else {
          //#region ContratoPrestacionServicio

          this.obtenerDetallePlanPago(0);

          //#endregion ContratoPrestacionServicio
        }
      }
    }
  }

  obtenerDetallePlanPago(valorIngresado: number) {
    this.facturaService
      .ObtenerDetallePlanPago(this.planPagoIdSeleccionado)
      .subscribe(
        (response: DetallePlanPago) => {
          if (response) {
            this.detallePlanPago = response;
            this.terceroId = this.detallePlanPago.terceroId;
            this.mostrarCabecera = false;
          }
        },
        (error) => {
          this.alertify.error('Hubo un error al obtener el plan de pago.');
        },
        () => {
          if (!this.detallePlanPago) {
            this.alertify.error(
              'No se pudo obtener información del plan de pago.'
            );
            this.mostrarCabecera = true;
          } else {
            this.facturaService
              .ObtenerFormatoCausacionyLiquidacionPago(
                this.planPagoIdSeleccionado,
                valorIngresado
              )
              .subscribe(
                (response: FormatoCausacionyLiquidacionPago) => {
                  this.formatoCausacionyLiquidacionPago = response;
                  this.formatoCausacionyLiquidacionPago.cantidadPago = this.detallePlanPago.cantidadPago;
                },
                (error) => {
                  this.alertify.error(
                    'Ocurrió un error al realizar el proceso de liquidación: ' +
                      error.toString()
                  );
                  this.mostrarCabecera = true;
                  this.formatoCausacionyLiquidacionPago = null;
                }
              );
          }
        }
      );
  }

  HabilitarCabecera($event) {
    this.mostrarCabecera = true;
    this.onLimpiarFactura();
  }


}
