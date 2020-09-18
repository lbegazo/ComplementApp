import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

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
import { EstadoPlanPago } from '../_models/enum';
import { DetallePlanPago } from '../_models/detallePlanPago';
import { Transaccion } from '../_models/transaccion';
import { ListaService } from '../_services/lista.service';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { ParametroGeneral } from '../_models/parametroGeneral';
import { ParametroLiquidacionTercero } from '../_models/parametroLiquidacionTercero';
import { FormatoCausacionLiquidacionComponent } from './formato-causacion-liquidacion/formato-causacion-liquidacion.component';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';

@Component({
  selector: 'app-causacionyliquidacion',
  templateUrl: './CausacionyLiquidacion.component.html',
  styleUrls: ['./CausacionyLiquidacion.component.css'],
})
export class CausacionyLiquidacionComponent implements OnInit {
  readonly codigoTransaccion = 'CAUSACION';
  nombreTransaccion: string;
  search: string;
  suggestions$: Observable<Tercero[]>;
  errorMessage: string;
  bsModalRef: BsModalRef;
  subscriptions: Subscription[] = [];
  listaEstadoId: string;
  arrayControls = new FormArray([]);
  estadoPlanPagoPorObligar = EstadoPlanPago.PorObligar.value;
  mostrarCabecera = true;

  listaPlanPago: PlanPago[] = [];
  planPagoIdSeleccionado = 0;
  detallePlanPago: DetallePlanPago;
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
    private listaService: ListaService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.createForm();
    this.obtenerNombreTransaccion();
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

  onSeleccionar() {
    this.obtenerPlanPago();
  }

  obtenerPlanPago() {
    if (this.planPagoIdSeleccionado > 0) {
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
                  this.planPagoIdSeleccionado
                )
                .subscribe((response: FormatoCausacionyLiquidacionPago) => {
                  this.formatoCausacionyLiquidacionPago = response;
                  console.log(this.formatoCausacionyLiquidacionPago);
                });
            }
          }
        );
    }
  }

  HabilitarCabecera($event) {
    this.mostrarCabecera = true;

    this.onLimpiarFactura();
  }

  private obtenerNombreTransaccion() {
    this.nombreTransaccion = this.listaService.obtenerNombreTransaccionPorCodigo(
      this.codigoTransaccion
    );
  }
}
