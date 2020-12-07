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
import { ClavePresupuestalContable } from 'src/app/_dto/clavePresupuestalContable';
import { Cdp } from 'src/app/_models/cdp';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ObligacionService } from 'src/app/_services/obligacion.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-clave-presupuestal-contable',
  templateUrl: './clave-presupuestal-contable.component.html',
  styleUrls: ['./clave-presupuestal-contable.component.scss'],
})
export class ClavePresupuestalContableComponent implements OnInit {
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

  listaPlanPago: Cdp[] = [];
  detalleLiquidacionIdSeleccionado = 0;
  planPagoSeleccionado: Cdp;
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
    maxSize: 10,
  };
  listaClavePresupuestalContable: ClavePresupuestalContable[];

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private obligacionService: ObligacionService
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
    this.obligacionService
      .ObtenerCompromisosParaClavePresupuestalContable(
        this.terceroId,
        null,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Cdp[]>) => {
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
    this.listaClavePresupuestalContable = [];

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
    // if (
    //   this.listaPlanPago &&
    //   this.listaPlanPago.length > 0 &&
    //   this.detalleLiquidacionIdSeleccionado > 0
    // ) {
    //   this.planPagoSeleccionado = this.listaPlanPago.filter(
    //     (x) => x.detalleLiquidacionId === this.detalleLiquidacionIdSeleccionado
    //   )[0];
    //   if (this.planPagoSeleccionado) {
    //     this.ObtenerDetalleFormatoCausacionyLiquidacionPago();
    //   }
    // }
  }

  ObtenerRubrosParaClavePresupuestalContable() {
    this.obligacionService
      .ObtenerRubrosParaClavePresupuestalContable(
        this.detalleLiquidacionIdSeleccionado
      )
      .subscribe(
        (response: ClavePresupuestalContable[]) => {
          if (response) {
            this.listaClavePresupuestalContable = response;
            if (
              this.listaClavePresupuestalContable &&
              this.listaClavePresupuestalContable.length > 0
            ) {
              this.terceroId = this.listaClavePresupuestalContable[0].tercero.id;
              this.mostrarCabecera = false;
            }
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
    this.mostrarCabecera = true;
    this.onLimpiarFactura();
  }
}
