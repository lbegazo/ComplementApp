import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  NgForm,
  Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { noop, Observable, of, Subscription } from 'rxjs';
import { Observer } from 'rxjs/internal/types';
import { map, switchMap, tap } from 'rxjs/Operators';
import { PerfilUsuario } from 'src/app/_models/enum';
import { Estado } from 'src/app/_models/estado';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { SolicitudCDP } from 'src/app/_models/solicitudCDP';
import { SolicitudCDPDto } from 'src/app/_models/solicitudCDPDto';
import { TipoOperacion } from 'src/app/_models/tipoOperacion';
import { Transaccion } from 'src/app/_models/transaccion';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CdpService } from 'src/app/_services/cdp.service';
import { GeneralService } from 'src/app/_services/general.service';
import { ListaService } from 'src/app/_services/lista.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-solicitud-cdp',
  templateUrl: './solicitud-cdp.component.html',
  styleUrls: ['./solicitud-cdp.component.scss'],
})
export class SolicitudCdpComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;
  search: string;
  suggestions$: Observable<Usuario[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  arrayControls = new FormArray([]);
  mostrarCabecera = true;

  listaTO: TipoOperacion[];
  idTipoOperacionSelecionado: number;
  tipoOperacionSelecionado: TipoOperacion;

  listaEstado: Estado[];
  idEstadoSelecionado: number;
  EstadoSelecionado: Estado;

  bsConfig: Partial<BsDaterangepickerConfig>;

  listaPlanPago: SolicitudCDPDto[] = [];
  solicitudCDPIdSeleccionado = 0;
  planPagoSeleccionado: SolicitudCDPDto;

  usuarioLogueado: Usuario;
  usuarioSeleccionado: Usuario;
  usuarioIdSeleccionado?: number = null;
  esPerfilSupervisorContractual = false;

  facturaHeaderForm = new FormGroup({});

  baseUrl = environment.apiUrl + 'lista/ObtenerListaUsuarioxFiltro';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };
  solicitudCDPSeleccionado: SolicitudCDP;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private cdpService: CdpService,
    private listaService: ListaService,
    private generalService: GeneralService
  ) {}

  ngOnInit(): void {
    this.createForm();

    this.cargarTipoOperacion();
    this.cargarListaEstado();

    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }

      this.usuarioLogueado = data['usuarioLogueado'];
      if (this.usuarioLogueado) {
        this.setSupervisorContractual();
      }
    });

    this.onBuscarFactura();

    this.suggestions$ = new Observable((observer: Observer<string>) => {
      observer.next(this.search);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Usuario[]>(this.baseUrl, {
              params: { nombres: query },
            })
            .pipe(
              map((data: Usuario[]) => data || []),
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

    this.facturaHeaderForm.reset();
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      numeroCtrl: [''],
      tOperacionCtrl: [''],
      usuarioCtrl: [''],
      fechaCtrl: [''],
      estadoCtrl: [''],
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
      this.alertify.warning('No existen solicitudes de CDP');
    }
  }

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.usuarioSeleccionado = e.item as Usuario;
    if (this.usuarioSeleccionado) {
      this.usuarioIdSeleccionado = this.usuarioSeleccionado.usuarioId;
    }
  }

  cargarTipoOperacion() {
    this.listaService.ObtenerListaTipoOperacion().subscribe(
      (lista: TipoOperacion[]) => {
        this.listaTO = lista;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  cargarListaEstado() {
    this.listaService
      .ObtenerListaEstadoSolicitudCDP('EstadoSolicitudCDP')
      .subscribe(
        (lista: Estado[]) => {
          this.listaEstado = lista;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  onSelectTipoOperacion() {
    this.tipoOperacionSelecionado = this.tOperacionCtrl.value as TipoOperacion;
    this.idTipoOperacionSelecionado = +this.tipoOperacionSelecionado
      .tipoOperacionId;
  }

  onSelectEstado() {
    this.EstadoSelecionado = this.estadoControl.value as Estado;
    this.idEstadoSelecionado = +this.EstadoSelecionado.estadoId;
  }

  onBuscarFactura() {
    let fechaRegistro = null;
    let usuarioId = null;

    const numero = this.numeroControl.value;
    const valorFecha = this.fechaControl.value;

    if (this.esPerfilSupervisorContractual) {
      usuarioId = this.usuarioLogueado.usuarioId;
    } else {
      if (this.usuarioSeleccionado) {
        usuarioId = this.usuarioSeleccionado.usuarioId;
      }
    }

    if (this.generalService.isValidDate(valorFecha)) {
      fechaRegistro = this.generalService.convertirAFormatoFecha(valorFecha);
    } else {
      if (valorFecha && valorFecha.indexOf('-') > -1) {
        fechaRegistro = this.generalService.dateString2Date(valorFecha);
      }
    }

    this.cdpService
      .ObtenerListaSolicitudCDP(
        numero > 0 ? numero : null,
        this.idTipoOperacionSelecionado > 0
          ? this.idTipoOperacionSelecionado
          : null,
        usuarioId,
        fechaRegistro !== null ? fechaRegistro : null,
        this.idEstadoSelecionado > 0 ? this.idEstadoSelecionado : null,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<SolicitudCDPDto[]>) => {
          this.listaPlanPago = documentos.result;
          this.pagination = documentos.pagination;
          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          // this.facturaHeaderForm = this.fb.group({
          //   terceroCtrl: ['', Validators.required],
          //   terceroDescripcionCtrl: [''],
          //   planPagoControles: this.arrayControls,
          // });
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura(form: FormGroup) {
    this.listaPlanPago = [];
    this.solicitudCDPIdSeleccionado = 0;
    this.usuarioSeleccionado = null;
    this.search = '';
    this.usuarioSeleccionado = null;
    this.solicitudCDPSeleccionado = null;
    this.idTipoOperacionSelecionado = 0;
    this.idEstadoSelecionado = 0;
    this.EstadoSelecionado = null;
    this.tipoOperacionSelecionado = null;
    form.reset();

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
    this.solicitudCDPIdSeleccionado = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.solicitudCDPIdSeleccionado = +event.target.value;
    }
  }

  onLiquidar() {
    if (
      this.listaPlanPago &&
      this.listaPlanPago.length > 0 &&
      this.solicitudCDPIdSeleccionado > 0
    ) {
      this.planPagoSeleccionado = this.listaPlanPago.filter(
        (x) => x.solicitudCDPId === this.solicitudCDPIdSeleccionado
      )[0];

      if (this.planPagoSeleccionado) {
        this.ObtenerDetalleSolicitudCDP();
      }
    }
  }

  ObtenerDetalleSolicitudCDP() {
    this.cdpService
      .ObtenerSolicitudCDP(this.solicitudCDPIdSeleccionado)
      .subscribe(
        (response: SolicitudCDP) => {
          if (response) {
            this.solicitudCDPSeleccionado = response;
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
    this.mostrarCabecera = true;
    this.onLimpiarFactura(this.facturaHeaderForm);
  }

  private setSupervisorContractual() {
    const perfiles = this.usuarioLogueado.perfiles;

    if (perfiles && perfiles.length > 0) {
      const perfil = perfiles[0];
      if (
        perfil &&
        perfil.perfilId === PerfilUsuario.SupervisorContractual.value
      ) {
        this.esPerfilSupervisorContractual = true;
      }
    }
  }

  get numeroControl() {
    return this.facturaHeaderForm.get('numeroCtrl');
  }

  get fechaControl() {
    return this.facturaHeaderForm.get('fechaCtrl');
  }

  get tOperacionCtrl() {
    return this.facturaHeaderForm.get('tOperacionCtrl');
  }

  get estadoControl() {
    return this.facturaHeaderForm.get('estadoCtrl');
  }
}
