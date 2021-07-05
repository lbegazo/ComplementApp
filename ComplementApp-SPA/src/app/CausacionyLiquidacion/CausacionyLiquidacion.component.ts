import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  noop,
  Observable,
  Observer,
  of,
  Subscription,
  combineLatest,
} from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';

import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { environment } from 'src/environments/environment';

import { Tercero } from 'src/app/_models/tercero';
import { PlanPagoService } from 'src/app/_services/planPago.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EstadoPlanPago, ModalidadContrato, TipoPago } from '../_models/enum';
import { DetallePlanPago } from '../_models/detallePlanPago';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';
import { Transaccion } from '../_models/transaccion';
import { DetalleLiquidacionService } from '../_services/detalleLiquidacion.service';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PopupDatosAdicionalesComponent } from './popup-datos-adicionales/popup-datos-adicionales.component';
import { SolicitudPagoService } from '../_services/solicitudPago.service';
import { Cdp } from '../_models/cdp';

@Component({
  selector: 'app-causacionyliquidacion',
  templateUrl: './CausacionyLiquidacion.component.html',
  styleUrls: ['./CausacionyLiquidacion.component.css'],
})
export class CausacionyLiquidacionComponent implements OnInit {
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
  estadoPlanPagoPorObligar = EstadoPlanPago.PorObligar.value;
  mostrarCabecera = true;
  modalidadContrato = 0;
  tipoPago = 0;

  listaSolicitudPago: Cdp[] = [];
  solicitudPagoIdSeleccionado = 0;
  detallePlanPago: DetallePlanPago;
  solicitudPagoSeleccionado: Cdp;
  tercero: Tercero;

  listaActividadEconomica: ValorSeleccion[];
  mostrarActividadEconomica = false;
  mostrarValorIngresado = false;

  liquidacionForm = new FormGroup({});
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
  bsModalRef: BsModalRef;
  actividadEconomicaId = 0;

  listaSolicitudPagoSeleccionada: number[] = [];
  seleccionaTodas = false;
  liquidacionRegistrada = false;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private facturaService: PlanPagoService,
    private solicitudPagoService: SolicitudPagoService,
    private liquidacionService: DetalleLiquidacionService,
    private fb: FormBuilder,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef
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

    this.cargarBusquedaTerceroXCodigo();

    this.cargarBusquedaTerceroXNombre();
  }

  createForm() {
    this.liquidacionForm = this.fb.group({
      terceroCtrl: [''],
      terceroDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  crearControlesDeArray() {
    if (this.listaSolicitudPago && this.listaSolicitudPago.length > 0) {
      if (this.seleccionaTodas) {
        this.listaSolicitudPago.forEach((item) => {
          item.esSeleccionada = this.seleccionaTodas;
        });

        this.listaSolicitudPago.forEach((val: Cdp) => {
          if (
            this.listaSolicitudPagoSeleccionada?.indexOf(
              val.formatoSolicitudPagoId
            ) === -1
          ) {
            this.listaSolicitudPagoSeleccionada.push(
              val.formatoSolicitudPagoId
            );
          }
        });
      } else {
        if (
          this.listaSolicitudPagoSeleccionada &&
          this.listaSolicitudPagoSeleccionada.length > 0
        ) {
          this.listaSolicitudPago.forEach((val: Cdp) => {
            if (
              this.listaSolicitudPagoSeleccionada?.indexOf(
                val.formatoSolicitudPagoId
              ) > -1
            ) {
              val.esSeleccionada = true;
            }
          });
        }
      }

      for (const detalle of this.listaSolicitudPago) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
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
    this.listaEstadoId = this.estadoPlanPagoPorObligar.toString(); // Por obligar

    this.solicitudPagoService
      .ObtenerListaSolicitudPagoAprobada(
        this.terceroId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Cdp[]>) => {
          this.listaSolicitudPago = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.liquidacionForm = this.fb.group({
            terceroCtrl: [''],
            terceroDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });

          if (
            !this.listaSolicitudPago ||
            this.listaSolicitudPago.length === 0
          ) {
            this.alertify.warning('No existen Solicitudes de Pago Aprobadas');
          }
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura() {
    this.listaSolicitudPago = [];
    this.listaSolicitudPagoSeleccionada = [];
    this.solicitudPagoIdSeleccionado = 0;
    this.tercero = null;
    this.search = '';
    this.terceroId = null;
    this.detallePlanPago = null;
    this.formatoCausacionyLiquidacionPago = null;
    this.seleccionaTodas = false;
    this.liquidacionRegistrada = false;

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
    let valor = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      valor = +event.target.value;

      if (this.listaSolicitudPagoSeleccionada?.indexOf(valor) === -1) {
        this.listaSolicitudPagoSeleccionada?.push(+valor);
      }
    } else {
      /* unselected */
      valor = +event.target.value;
      let index = 0;
      let i = 0;
      this.listaSolicitudPagoSeleccionada.forEach((val: number) => {
        if (val === valor) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.listaSolicitudPagoSeleccionada.splice(index, 1);
      }
    }

    if (this.pagination) {
      if (
        this.pagination.totalItems ===
        this.listaSolicitudPagoSeleccionada.length
      ) {
        this.seleccionaTodas = true;
      } else {
        this.seleccionaTodas = false;
      }
    }
  }

  onCheckAllChange(event) {
    const checked = event.target.checked;
    if (checked) {
      this.seleccionaTodas = true;
      this.listaSolicitudPago.forEach(
        (item) => (item.esSeleccionada = checked)
      );
      this.listaSolicitudPagoSeleccionada = [];

      this.listaSolicitudPago.forEach((val: Cdp) => {
        this.listaSolicitudPagoSeleccionada.push(val.formatoSolicitudPagoId);
      });
    } else {
      this.seleccionaTodas = false;
      this.listaSolicitudPago.forEach(
        (item) => (item.esSeleccionada = checked)
      );
      this.listaSolicitudPagoSeleccionada = [];
    }
  }

  onLiquidar() {
    if (this.liquidacionForm.valid) {
      let listaSolicitudPagoId: number[] = [];
      const esSeleccionarTodas = this.seleccionaTodas ? 1 : 0;
      let listaSolicitudPagoCadenaId = '';

      if (!this.seleccionaTodas) {
        if (
          this.listaSolicitudPagoSeleccionada &&
          this.listaSolicitudPagoSeleccionada.length > 0
        ) {
          listaSolicitudPagoId = this.listaSolicitudPagoSeleccionada.filter(
            (v, i) => this.listaSolicitudPagoSeleccionada.indexOf(v) === i
          );
          listaSolicitudPagoCadenaId = listaSolicitudPagoId.join();
        }
      }

      this.alertify.confirm2(
        'Formato de Causación y Liquidación',
        '¿Esta seguro que desea liquidar los planes de pago seleccionados?',
        () => {
          this.liquidacionService
            .RegistrarListaDetalleLiquidacion(
              listaSolicitudPagoCadenaId,
              this.listaEstadoId,
              esSeleccionarTodas,
              this.terceroId
            )
            .subscribe(
              (response: any) => {
                if (!isNaN(response)) {
                  this.alertify.success(
                    'Se registraron los formatos de causación y liquidación seleccionados'
                  );
                  this.liquidacionRegistrada = true;
                  this.onLimpiarFactura();
                } else {
                  this.alertify.error(
                    'No se pudo registrar los formatos de causación y liquidación'
                  );
                }
              },

              (error) => {
                this.alertify.error(
                  'Hubó un error al registrar los formatos de liquidación ' +
                    error
                );
              },
              () => {}
            );
        }
      );
    }
  }

  verLiquidacion() {
    this.obtenerSolicitudPago();

    if (this.solicitudPagoSeleccionado) {
      this.obtenerDetallePlanPago(0);
    }
  }

  rechazarLiquidacion() {
    let mensaje = '';
    let planPagoId = 0;
    this.alertify.confirm2(
      'Formato de Causación y Liquidación',
      '¿Esta seguro que desea rechazar el plan de pago?',
      () => {
        mensaje = window.prompt('Motivo de rechazo: ', '');
        if (mensaje.length === 0) {
          this.alertify.warning('Debe ingresar el motivo de rechazo');
        } else {
          this.obtenerSolicitudPago();

          if (this.solicitudPagoSeleccionado) {
            this.liquidacionService
              .RechazarDetalleLiquidacion(
                this.solicitudPagoSeleccionado.planPagoId,
                this.solicitudPagoSeleccionado.formatoSolicitudPagoId,
                mensaje
              )
              .subscribe(
                (response: any) => {
                  if (!isNaN(response)) {
                    planPagoId = +response;
                    this.alertify.success(
                      'El formato de causación y liquidación se rechazó correctamente'
                    );
                  } else {
                    this.alertify.error(
                      'No se pudo rechazar el formato de causación y liquidación '
                    );
                  }
                },

                (error) => {
                  this.alertify.error(
                    'Hubó un error al rechazar la liquidación ' + error
                  );
                },
                () => {
                  this.onLimpiarFactura();
                }
              );
          }
        }
      }
    );
  }

  cargarListaActividadEconomicaXTercero() {
    this.liquidacionService
      .ObtenerListaActividadesEconomicaXTercero(
        this.solicitudPagoSeleccionado.terceroId
      )
      .subscribe(
        (lista: ValorSeleccion[]) => {
          this.listaActividadEconomica = lista;
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {}
      );
  }

  obtenerSolicitudPago() {
    if (
      this.listaSolicitudPago &&
      this.listaSolicitudPago.length > 0 &&
      this.listaSolicitudPagoSeleccionada.length === 1
    ) {
      this.solicitudPagoIdSeleccionado = this.listaSolicitudPagoSeleccionada[0];
      this.solicitudPagoSeleccionado = this.listaSolicitudPago.filter(
        (x) => x.formatoSolicitudPagoId === this.solicitudPagoIdSeleccionado
      )[0];
    }
  }

  abrirPopup() {
    let valor = 0;
    const initialState = {
      title: 'Datos adicionales',
      terId: this.terceroId,
      mostrarActividad: this.mostrarActividadEconomica,
      mostrarValor: this.mostrarValorIngresado,
      valorFacturado: this.solicitudPagoSeleccionado.valorFacturado,
      listaActividades: this.listaActividadEconomica,
    };

    this.bsModalRef = this.modalService.show(
      PopupDatosAdicionalesComponent,
      Object.assign(
        {
          animated: true,
          keyboard: true,
          backdrop: true,
          ignoreBackdropClick: false,
        },
        { initialState },
        { class: 'gray modal-md' }
      )
    );

    const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (this.bsModalRef.content != null) {
          const resultado = this.bsModalRef.content;

          if (this.mostrarActividadEconomica) {
            this.actividadEconomicaId = +resultado[0];
          }

          if (this.mostrarValorIngresado) {
            valor = +resultado[1];
            if (isNaN(valor) || valor <= 0) {
              this.alertify.warning(
                'Debe ingresar un valor para la liquidación'
              );
              return;
            }
          }

          this.obtenerDetallePlanPago(valor);
        }

        this.unsubscribe();
      })
    );
    this.subscriptions.push(combine);
  }

  obtenerDetallePlanPago(valorIngresado: number) {
    this.facturaService
      .ObtenerDetallePlanPago(this.solicitudPagoSeleccionado.planPagoId)
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
            this.liquidacionService
              .ObtenerFormatoCausacionyLiquidacionPago(
                this.solicitudPagoIdSeleccionado,
                this.solicitudPagoSeleccionado.planPagoId,
                valorIngresado,
                this.actividadEconomicaId
              )
              .subscribe(
                (response: FormatoCausacionyLiquidacionPago) => {
                  if (response !== null && this.detallePlanPago) {
                    this.formatoCausacionyLiquidacionPago = response;
                    this.formatoCausacionyLiquidacionPago.cantidadPago =
                      this.detallePlanPago.cantidadPago;
                  } else {
                    this.alertify.error(
                      'El tercero no tiene parametros de liquidación definidos'
                    );
                    this.mostrarCabecera = true;
                    this.formatoCausacionyLiquidacionPago = null;
                  }
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
