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
import { FormatoSolicitudPagoDto } from 'src/app/_dto/formatoSolicitudPagoDto';
import { Cdp } from 'src/app/_models/cdp';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ObligacionService } from 'src/app/_services/obligacion.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-registro-solicitud-pago',
  templateUrl: './registro-solicitud-pago.component.html',
  styleUrls: ['./registro-solicitud-pago.component.scss'],
})
export class RegistroSolicitudPagoComponent implements OnInit {
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
  cdpId = 0;
  planPagoSeleccionado: Cdp;
  formatoSolicitudPago: FormatoSolicitudPagoDto;
  tercero: Tercero;

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

  usuarioLogueado: Usuario;
  perfilId: number;

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
      this.usuarioLogueado = data['usuarioLogueado'];
    });

    if (this.usuarioLogueado && this.usuarioLogueado.perfiles) {
      this.perfilId = this.usuarioLogueado.perfiles[0].perfilId;
    }

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
      .ObtenerCompromisosParaSolicitudRegistroPago(
        this.usuarioLogueado.usuarioId,
        this.perfilId,
        this.terceroId,
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
    this.cdpId = 0;
    this.tercero = null;
    this.search = '';
    this.terceroId = null;
    this.formatoSolicitudPago = null;

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
    this.cdpId = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.cdpId = +event.target.value;
    }
  }

  onRegistrarSolicitud() {
    if (this.listaPlanPago && this.listaPlanPago.length > 0 && this.cdpId > 0) {
      this.planPagoSeleccionado = this.listaPlanPago.filter(
        (x) => x.cdpId === this.cdpId
      )[0];
      if (this.planPagoSeleccionado) {
        this.ObtenerFormatoSolicitudPago();
      }
    }
  }

  ObtenerFormatoSolicitudPago() {
    this.obligacionService.ObtenerFormatoSolicitudPago(this.cdpId).subscribe(
      (response: FormatoSolicitudPagoDto) => {
        if (response !== null) {
          this.formatoSolicitudPago = response;
          if (this.formatoSolicitudPago) {
            this.terceroId = this.formatoSolicitudPago.tercero.terceroId;
            this.mostrarCabecera = false;
          } else {
            this.alertify.error(
              'No se puede obtener información para el formato de solicitud de pago'
            );
          }
        } else {
          this.alertify.error(
            'No se encuentra contrato para el tercero seleccionado'
          );
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
