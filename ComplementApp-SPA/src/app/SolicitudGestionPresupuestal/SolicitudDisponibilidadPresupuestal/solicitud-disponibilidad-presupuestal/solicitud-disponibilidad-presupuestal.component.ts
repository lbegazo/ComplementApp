import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/Operators';
import { Cdp } from 'src/app/_models/cdp';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { TipoOperacionSolicitudCdp } from 'src/app/_models/enum';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { RubroPresupuestal } from 'src/app/_models/rubroPresupuestal';
import { TipoOperacion } from 'src/app/_models/tipoOperacion';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ListaService } from 'src/app/_services/lista.service';
import { PlanAdquisicionService } from 'src/app/_services/planAdquisicion.service';
import { SolicitudCdpService } from 'src/app/_services/solicitudCdp.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-solicitud-disponibilidad-presupuestal',
  templateUrl: './solicitud-disponibilidad-presupuestal.component.html',
  styleUrls: ['./solicitud-disponibilidad-presupuestal.component.css'],
})
export class SolicitudDisponibilidadPresupuestalComponent implements OnInit {
  //#region Variables

  nombreTransaccion: string;
  transaccion: Transaccion;
  search: string;
  searchNombre: string;
  suggestions$: Observable<RubroPresupuestal[]>;
  suggestionsXNombre$: Observable<RubroPresupuestal[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  arrayControls = new FormArray([]);
  mostrarCabecera = true;
  esCreacion = true;
  nombreBoton = 'Registrar';
  cdp: Cdp;
  listaTipoOperacionTotal: TipoOperacion[];
  listaTipoOperacionModificacion: TipoOperacion[];
  idTipoOperacionSelecionado = 0;
  tipoOperacionSelecionado: TipoOperacion = null;
  tipoOperacionSolicitudInicial: TipoOperacion = null;

  listaPlanAdquisicion: DetalleCDP[] = [];
  listaPlanAdquisicionTotal: DetalleCDP[] = [];
  rubroPresupuestal: RubroPresupuestal;
  rubroPresupuestalId = 0;

  facturaHeaderForm = new FormGroup({});
  baseUrl = environment.apiUrl + 'lista/ObtenerListaRubroPresupuestal';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };
  listaPlanAdquisicionSeleccionada: number[] = [];
  seleccionaTodas = false;

  //#endregion Variables

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private planAdquisicionService: PlanAdquisicionService,
    private router: Router,
    private listaService: ListaService,
    private cdpService: SolicitudCdpService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.cargarTipoOperacion();

    this.createForm();

    this.cargarBusquedaTerceroXCodigo();

    this.cargarBusquedaTerceroXNombre();

    this.deshabilitarControles();

    this.onBuscarFactura();
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      tipoOperacionCtrl: [null, Validators.required],
      cdpCtrl: [null, Validators.required],
      rbtRadicarFacturaCtrl: ['1'],
      rubroPresupuestalCtrl: [''],
      rubroPresupuestalDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  cargarBusquedaTerceroXCodigo() {
    this.suggestions$ = new Observable((observer: Observer<string>) => {
      observer.next(this.search);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<RubroPresupuestal[]>(this.baseUrl, {
              params: { identificacion: query },
            })
            .pipe(
              map((data: RubroPresupuestal[]) => data || []),
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
            .get<RubroPresupuestal[]>(this.baseUrl, {
              params: { nombre: query },
            })
            .pipe(
              map((data: RubroPresupuestal[]) => data || []),
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
    this.rubroPresupuestal = e.item as RubroPresupuestal;
    if (this.rubroPresupuestal) {
      this.rubroPresupuestalId = this.rubroPresupuestal.rubroPresupuestalId;
    }
  }

  typeaheadOnSelectXNombre(e: TypeaheadMatch): void {
    this.rubroPresupuestal = e.item as RubroPresupuestal;
    if (this.rubroPresupuestal) {
      this.rubroPresupuestalId = this.rubroPresupuestal.rubroPresupuestalId;
    }
  }

  onCreacion(event) {
    this.onLimpiarFactura();
    this.esCreacion = true;
    this.nombreBoton = 'Registrar';
    this.deshabilitarControles();
  }

  onModificacion(event) {
    this.onLimpiarFactura();
    this.esCreacion = false;
    this.nombreBoton = 'Modificar';
    this.deshabilitarControles();
  }

  deshabilitarControles() {
    if (this.esCreacion) {
      this.cdpCtrl.disable();
      this.tipoOperacionCtrl.disable();
    } else {
      this.cdpCtrl.enable();
      this.tipoOperacionCtrl.enable();
    }
  }

  cargarTipoOperacion() {
    this.listaService.ObtenerListaTipoOperacion().subscribe(
      (lista: TipoOperacion[]) => {
        this.listaTipoOperacionTotal = lista;

        this.listaTipoOperacionModificacion =
          this.listaTipoOperacionTotal.filter(
            (element) =>
              element.tipoOperacionId !==
              TipoOperacionSolicitudCdp.SolicitudInicial.valueOf()
          );

        this.tipoOperacionSolicitudInicial =
          this.listaTipoOperacionTotal.filter(
            (element) =>
              element.tipoOperacionId ===
              TipoOperacionSolicitudCdp.SolicitudInicial.valueOf()
          )[0];
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  onSeleccionarTipoOperacion() {
    this.tipoOperacionSelecionado = this.tipoOperacionCtrl
      .value as TipoOperacion;
    this.idTipoOperacionSelecionado =
      +this.tipoOperacionSelecionado.tipoOperacionId;
  }

  get cdpSeleccionado(): boolean {
    if (this.cdp != null) {
      return true;
    }
    return false;
  }

  onBuscarFactura() {
    this.planAdquisicionService
      .ObtenerListaPlanAnualAdquisicionPaginada(
        this.esCreacion ? 1 : 0,
        this.rubroPresupuestalId,
        0,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<DetalleCDP[]>) => {
          this.listaPlanAdquisicion = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          const tipo = this.esCreacion ? '1' : '2';
          this.facturaHeaderForm = this.fb.group({
            tipoOperacionCtrl: [null, Validators.required],
            cdpCtrl: ['', Validators.required],
            rbtRadicarFacturaCtrl: [tipo],
            rubroPresupuestalCtrl: [''],
            rubroPresupuestalDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaPlanAdquisicion && this.listaPlanAdquisicion.length > 0) {
      if (this.seleccionaTodas) {
        this.listaPlanAdquisicion.forEach((item) => {
          item.esSeleccionada = this.seleccionaTodas;
        });

        this.listaPlanAdquisicion.forEach((val: DetalleCDP) => {
          if (
            this.listaPlanAdquisicionSeleccionada?.indexOf(val.detalleCdpId) ===
            -1
          ) {
            this.listaPlanAdquisicionSeleccionada.push(val.detalleCdpId);
          }
        });
      } else {
        if (
          this.listaPlanAdquisicionSeleccionada &&
          this.listaPlanAdquisicionSeleccionada.length > 0
        ) {
          this.listaPlanAdquisicion.forEach((val: DetalleCDP) => {
            if (
              this.listaPlanAdquisicionSeleccionada?.indexOf(val.detalleCdpId) >
              -1
            ) {
              val.esSeleccionada = true;
            }
          });
        }
      }

      for (const detalle of this.listaPlanAdquisicion) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning('No existen planes de adquisición');
    }
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura() {
    this.listaPlanAdquisicion = [];
    this.listaPlanAdquisicionTotal = [];
    this.rubroPresupuestalId = 0;
    this.rubroPresupuestal = null;
    this.search = '';
    this.searchNombre = '';
    this.listaPlanAdquisicionSeleccionada = [];
    this.seleccionaTodas = false;
    this.nombreBoton = 'Registrar';
    this.esCreacion = true;
    this.idTipoOperacionSelecionado = 0;
    this.cdp = null;

    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };

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

      if (this.listaPlanAdquisicionSeleccionada?.indexOf(valor) === -1) {
        this.listaPlanAdquisicionSeleccionada?.push(+valor);
      }
    } else {
      /* unselected */
      valor = +event.target.value;
      let index = 0;
      let i = 0;
      this.listaPlanAdquisicionSeleccionada.forEach((val: number) => {
        if (val === valor) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.listaPlanAdquisicionSeleccionada.splice(index, 1);
      }
    }

    if (this.pagination) {
      if (
        this.pagination.totalItems ===
        this.listaPlanAdquisicionSeleccionada.length
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
      this.listaPlanAdquisicion.forEach(
        (item) => (item.esSeleccionada = checked)
      );
      this.listaPlanAdquisicionSeleccionada = [];

      this.listaPlanAdquisicion.forEach((val: DetalleCDP) => {
        this.listaPlanAdquisicionSeleccionada.push(val.detalleCdpId);
      });
    } else {
      this.seleccionaTodas = false;
      this.listaPlanAdquisicion.forEach(
        (item) => (item.esSeleccionada = checked)
      );
      this.listaPlanAdquisicionSeleccionada = [];
    }
  }

  seleccionarCdp() {
    const numCdp = this.cdpCtrl.value;
    const numeroCdp = numCdp === '' || numCdp === null ? 0 : numCdp;

    if (numeroCdp !== 0) {
      this.cdpService.ObtenerCDP(numeroCdp).subscribe(
        (documento: Cdp) => {
          if (documento) {
            this.cdp = documento;
            this.mostrarCabecera = false;
          } else {
            this.mostrarCabecera = true;
          }
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          if (this.cdp == null) {
            this.alertify.warning('No se encontró el documento');
          }
        }
      );
    } else {
      this.alertify.warning('Debe ingresar el número de CDP');
    }
  }

  obtenerListaPlanAdquisicionSinCdp() {
    let listaIds: number[] = [];
    let listaCadenaIds = '';

    const esSeleccionarTodas = this.seleccionaTodas ? 1 : 0;

    if (
      this.listaPlanAdquisicionSeleccionada &&
      this.listaPlanAdquisicionSeleccionada.length > 0
    ) {
      if (!this.seleccionaTodas) {
        listaIds = this.listaPlanAdquisicionSeleccionada.filter(
          (v, i) => this.listaPlanAdquisicionSeleccionada.indexOf(v) === i
        );
        listaCadenaIds = listaIds.join();
      }

      this.planAdquisicionService
        .ObtenerListaPlanAdquisicionSinCDPXIds(
          listaCadenaIds,
          esSeleccionarTodas,
          this.rubroPresupuestalId
        )
        .subscribe(
          (documentos: DetalleCDP[]) => {
            if (documentos) {
              this.listaPlanAdquisicionTotal = documentos;
              this.mostrarCabecera = false;
            } else {
              this.mostrarCabecera = true;
            }
          },
          (error) => {
            this.alertify.error(error);
          },
          () => {}
        );
    } else {
      this.alertify.warning(
        'Debe seleccionar por lo menos un plan de adquisición'
      );
    }
  }

  get tipoOperacionCtrl() {
    return this.facturaHeaderForm.get('tipoOperacionCtrl');
  }

  get cdpCtrl() {
    return this.facturaHeaderForm.get('cdpCtrl');
  }

  habilitarCabecera($event) {
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

  public noWhitespaceValidator(control: AbstractControl) {
    const isWhitespace = (control.value || '').trim().length === 0;
    const isValid = !isWhitespace;
    return isValid;
  }
}
