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
import { PlanPago } from 'src/app/_models/planPago';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EstadoPlanPago, ModalidadContrato, TipoPago } from '../_models/enum';
import { DetallePlanPago } from '../_models/detallePlanPago';
import { ListaService } from '../_services/lista.service';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';
import { Transaccion } from '../_models/transaccion';
import { DetalleLiquidacionService } from '../_services/detalleLiquidacion.service';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PopupDatosAdicionalesComponent } from './popup-datos-adicionales/popup-datos-adicionales.component';

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

  listaPlanPagoSeleccionada: number[] = [];
  seleccionaTodas = false;
  liquidacionRegistrada = false;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private facturaService: PlanPagoService,
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
    this.liquidacionForm = this.fb.group({
      terceroCtrl: [''],
      terceroDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  // obtenerDatosDeResolver() {
  //   this.route.data.subscribe((data) => {
  //     this.listaPlanPago = data['planPagoResolver'].result;
  //     this.pagination = data['planPagoResolver'].pagination;
  //     this.crearControlesDeArray();
  //   });
  // }

  crearControlesDeArray() {
    if (this.listaPlanPago && this.listaPlanPago.length > 0) {
      if (this.seleccionaTodas) {
        this.listaPlanPago.forEach((item) => {
          item.esSeleccionada = this.seleccionaTodas;
        });

        this.listaPlanPago.forEach((val: PlanPago) => {
          if (this.listaPlanPagoSeleccionada?.indexOf(val.planPagoId) === -1) {
            this.listaPlanPagoSeleccionada.push(val.planPagoId);
          }
        });
      } else {
        if (
          this.listaPlanPagoSeleccionada &&
          this.listaPlanPagoSeleccionada.length > 0
        ) {
          this.listaPlanPago.forEach((val: PlanPago) => {
            if (this.listaPlanPagoSeleccionada?.indexOf(val.planPagoId) > -1) {
              val.esSeleccionada = true;
            }
          });
        }
      }

      for (const detalle of this.listaPlanPago) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
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
          this.liquidacionForm = this.fb.group({
            terceroCtrl: [''],
            terceroDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });

          if (!this.listaPlanPago || this.listaPlanPago.length === 0) {
            this.alertify.warning(
              'No existen Facturas en estado por “ConLiquidacionDeducciones”'
            );
          }
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura() {
    this.listaPlanPago = [];
    this.listaPlanPagoSeleccionada = [];
    this.planPagoIdSeleccionado = 0;
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

      if (this.listaPlanPagoSeleccionada?.indexOf(valor) === -1) {
        this.listaPlanPagoSeleccionada?.push(+valor);
      }
    } else {
      /* unselected */
      valor = +event.target.value;
      let index = 0;
      let i = 0;
      this.listaPlanPagoSeleccionada.forEach((val: number) => {
        if (val === valor) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.listaPlanPagoSeleccionada.splice(index, 1);
      }
    }

    if (this.pagination) {
      if (
        this.pagination.totalItems === this.listaPlanPagoSeleccionada.length
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
      this.listaPlanPago.forEach((item) => (item.esSeleccionada = checked));
      this.listaPlanPagoSeleccionada = [];

      this.listaPlanPago.forEach((val: PlanPago) => {
        this.listaPlanPagoSeleccionada.push(val.planPagoId);
      });
    } else {
      this.seleccionaTodas = false;
      this.listaPlanPago.forEach((item) => (item.esSeleccionada = checked));
      this.listaPlanPagoSeleccionada = [];
    }
  }

  onLiquidar() {
    if (this.liquidacionForm.valid) {
      let listaPlanPagoId: number[] = [];
      const esSeleccionarTodas = this.seleccionaTodas ? 1 : 0;
      let listaPlanPagoCadenaId = '';

      if (!this.seleccionaTodas) {
        if (
          this.listaPlanPagoSeleccionada &&
          this.listaPlanPagoSeleccionada.length > 0
        ) {
          listaPlanPagoId = this.listaPlanPagoSeleccionada.filter(
            (v, i) => this.listaPlanPagoSeleccionada.indexOf(v) === i
          );
          listaPlanPagoCadenaId = listaPlanPagoId.join();
        }
      }

      this.alertify.confirm2(
        'Formato de Causación y Liquidación',
        '¿Esta seguro que desea liquidar los planes de pago seleccionados?',
        () => {
          this.liquidacionService
            .RegistrarListaDetalleLiquidacion(
              listaPlanPagoCadenaId,
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
    if (
      this.listaPlanPago &&
      this.listaPlanPago.length > 0 &&
      this.listaPlanPagoSeleccionada.length === 1
    ) {
      this.planPagoIdSeleccionado = this.listaPlanPagoSeleccionada[0];
      this.planPagoSeleccionado = this.listaPlanPago.filter(
        (x) => x.planPagoId === this.planPagoIdSeleccionado
      )[0];

      if (this.planPagoSeleccionado) {
        this.cargarListaActividadEconomicaXTercero();
      }
    }
  }

  cargarListaActividadEconomicaXTercero() {
    this.liquidacionService
      .ObtenerListaActividadesEconomicaXTercero(
        this.planPagoSeleccionado.terceroId
      )
      .subscribe(
        (lista: ValorSeleccion[]) => {
          this.listaActividadEconomica = lista;
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          // Si la lista de actividades económicas es mayor a 1
          // entonces no se obliga a escoger actividad económica
          // o
          // Modalidad de contrato: ProveedorConDescuento o TipoPago: Variable
          if (this.esAbrirPopup) {
            this.abrirPopup();
          } else {
            this.obtenerDetallePlanPago(0);
          }
        }
      );
  }

  get esAbrirPopup() {
    const resultado = false;
    this.mostrarValorIngresado = false;
    this.mostrarActividadEconomica = false;

    if (this.planPagoSeleccionado) {
      this.modalidadContrato = this.planPagoSeleccionado.modalidadContrato;
      this.tipoPago = this.planPagoSeleccionado.tipoPago;
      this.terceroId = this.planPagoSeleccionado.terceroId;

      if (
        this.modalidadContrato ===
          ModalidadContrato.ProveedorConDescuento.value &&
        this.tipoPago === TipoPago.Variable.value
      ) {
        this.mostrarValorIngresado = true;
      }

      if (
        this.listaActividadEconomica &&
        this.listaActividadEconomica.length > 1
      ) {
        this.mostrarActividadEconomica = true;
      }

      if (this.mostrarValorIngresado || this.mostrarActividadEconomica) {
        return true;
      }

      return resultado;
    }
  }

  abrirPopup() {
    let valor = 0;
    const initialState = {
      title: 'Datos adicionales',
      terId: this.terceroId,
      mostrarActividad: this.mostrarActividadEconomica,
      mostrarValor: this.mostrarValorIngresado,
      valorFacturado: this.planPagoSeleccionado.valorFacturado,
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
            this.liquidacionService
              .ObtenerFormatoCausacionyLiquidacionPago(
                this.planPagoIdSeleccionado,
                valorIngresado,
                this.actividadEconomicaId
              )
              .subscribe(
                (response: FormatoCausacionyLiquidacionPago) => {
                  if (response !== null && this.detallePlanPago) {
                    this.formatoCausacionyLiquidacionPago = response;
                    this.formatoCausacionyLiquidacionPago.cantidadPago = this.detallePlanPago.cantidadPago;
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
    this.mostrarCabecera = true;
    this.onLimpiarFactura();
  }
}
