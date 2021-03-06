import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/Operators';
import {
  EstadoPlanPago,
} from 'src/app/_models/enum';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-liquidacion-pago',
  templateUrl: './liquidacion-pago.component.html',
  styleUrls: ['./liquidacion-pago.component.scss'],
})
export class LiquidacionPagoComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;
  search: string;
  suggestions$: Observable<Tercero[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  listaEstadoId: string;
  arrayControls = new FormArray([]);
  mostrarCabecera = true;
  modalidadContrato = 0;
  tipoPago = 0;

  listaPlanPago: FormatoCausacionyLiquidacionPago[] = [];
  detalleLiquidacionIdSeleccionado = 0;
  planPagoSeleccionado: FormatoCausacionyLiquidacionPago;
  tercero: Tercero;
  //  = {
  //   terceroId: 0,
  //   nombre: '',
  //   numeroIdentificacion: '',
  //   tipoDocumentoIdentidad: '',
  //   tipoDocumentoIdentidadId: 0,
  // };

  facturaHeaderForm = new FormGroup({});
  terceroId?: number = null;
  baseUrl = environment.apiUrl + 'lista/ObtenerListaTercero';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };
  formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private liquidacionService: DetalleLiquidacionService
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
        'No existen Facturas en estado por “ConLiquidacionDeducciones” para el tercero registrado'
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
    this.listaEstadoId =
      EstadoPlanPago.ConLiquidacionDeducciones.value.toString() +
      ',' +
      EstadoPlanPago.Obligado.value.toString() +
      ',' +
      EstadoPlanPago.Pagado.value.toString() +
      ',' +
      EstadoPlanPago.ConOrdenPago.value.toString();

    this.liquidacionService
      .ObtenerLiquidacionesParaCuentaPorPagarArchivo(
        this.listaEstadoId,
        this.terceroId,
        null,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<FormatoCausacionyLiquidacionPago[]>) => {
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
    this.detalleLiquidacionIdSeleccionado = 0;
    this.tercero = null;
    this.search = '';
    this.terceroId = null;
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
    this.detalleLiquidacionIdSeleccionado = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.detalleLiquidacionIdSeleccionado = +event.target.value;
    }
  }

  onLiquidar() {
    if (
      this.listaPlanPago &&
      this.listaPlanPago.length > 0 &&
      this.detalleLiquidacionIdSeleccionado > 0
    ) {
      this.planPagoSeleccionado = this.listaPlanPago.filter(
        (x) => x.detalleLiquidacionId === this.detalleLiquidacionIdSeleccionado
      )[0];

      if (this.planPagoSeleccionado) {
        this.ObtenerDetalleFormatoCausacionyLiquidacionPago();
      }
    }
  }

  ObtenerDetalleFormatoCausacionyLiquidacionPago() {
    this.liquidacionService
      .ObtenerDetalleFormatoCausacionyLiquidacionPago(
        this.detalleLiquidacionIdSeleccionado
      )
      .subscribe(
        (response: FormatoCausacionyLiquidacionPago) => {
          if (response) {
            this.formatoCausacionyLiquidacionPago = response;
            this.terceroId = this.formatoCausacionyLiquidacionPago.terceroId;
            this.mostrarCabecera = false;
          }
        },
        (error) => {
          this.alertify.error(
            'Hubo un error al obtener el formato de liquidación.'
          );
        }
      );
  }

  HabilitarCabecera($event) {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };

    this.mostrarCabecera = true;
    this.onLimpiarFactura();
  }
}
