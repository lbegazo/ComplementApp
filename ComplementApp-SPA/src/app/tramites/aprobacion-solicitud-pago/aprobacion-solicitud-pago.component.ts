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
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { Cdp } from 'src/app/_models/cdp';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ListaService } from 'src/app/_services/lista.service';
import { SolicitudPagoService } from 'src/app/_services/solicitudPago.service';
import { TerceroService } from 'src/app/_services/tercero.service';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-aprobacion-solicitud-pago',
  templateUrl: './aprobacion-solicitud-pago.component.html',
  styleUrls: ['./aprobacion-solicitud-pago.component.css'],
})
export class AprobacionSolicitudPagoComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  search: string;
  searchNombre: string;
  suggestions$: Observable<Tercero[]>;
  suggestionsXNombre$: Observable<Tercero[]>;

  errorMessage: string;
  subscriptions: Subscription[] = [];
  listaEstadoId: string;
  arrayControls = new FormArray([]);
  mostrarCabecera = true;
  modalidadContrato = 0;
  tipoPago = 0;

  listaPlanPago: Cdp[] = [];
  formatoSolicitudPagoId = 0;
  planPagoSeleccionado: Cdp;
  formatoSolicitudPago: FormatoSolicitudPagoDto;
  tercero: Tercero;
  formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago = null;

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

  parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  listaNotasLegales: ValorSeleccion[] = [];

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private solicitudPagoService: SolicitudPagoService,
    private terceroService: TerceroService,
    private listaService: ListaService
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

    this.cargarBusquedaTerceroXCodigo();

    this.cargarBusquedaTerceroXNombre();
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
      this.alertify.warning('No existen solicitudes de Pago para el usuario');
    }
  }

  cargarBusquedaTerceroXCodigo() {
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

  cargarBusquedaTerceroXNombre() {
    this.suggestionsXNombre$ = new Observable((observer: Observer<string>) => {
      observer.next(this.searchNombre);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Tercero[]>(this.baseUrl, {
              params: { nombre: query },
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

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  typeaheadOnSelectXNombre(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  onBuscarFactura() {
    this.solicitudPagoService
      .ObtenerSolicitudesPagoParaAprobar(
        this.usuarioLogueado.usuarioId,
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
    this.formatoSolicitudPagoId = 0;
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
    this.formatoSolicitudPagoId = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.formatoSolicitudPagoId = +event.target.value;
    }
  }

  onVerSolicitud() {
    if (
      this.listaPlanPago &&
      this.listaPlanPago.length > 0 &&
      this.formatoSolicitudPagoId > 0
    ) {
      this.planPagoSeleccionado = this.listaPlanPago.filter(
        (x) => x.formatoSolicitudPagoId === this.formatoSolicitudPagoId
      )[0];
      if (this.planPagoSeleccionado) {
        this.ObtenerFormatoSolicitudPagoXId();
      }
    }
  }

  ObtenerFormatoSolicitudPagoXId() {
    this.solicitudPagoService
      .ObtenerFormatoSolicitudPagoXId(this.formatoSolicitudPagoId)
      .subscribe(
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
        },
        () => {
          if (this.terceroId > 0) {
            this.obtenerParametrizacionTercero(this.terceroId);
          }
        }
      );
  }

  obtenerParametrizacionTercero(terceroId: number) {
    if (terceroId > 0) {
      this.terceroService
        .ObtenerParametrizacionLiquidacionXTercero(terceroId)
        .subscribe(
          (documento: ParametroLiquidacionTercero) => {
            if (documento) {
              this.parametroLiquidacionSeleccionado = documento;
              this.mostrarCabecera = false;
            } else {
              this.alertify.error(
                'No se pudo obtener información de la parametrización del tercero'
              );
            }
          },
          () => {},
          () => {
            this.cargarNotasLegales();
          }
        );
    }
  }

  cargarNotasLegales() {
    this.listaService.ObtenerParametrosGeneralesXTipo('NotaLegal').subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaNotasLegales = lista;
      },
      (error) => {
        this.alertify.error(error);
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
