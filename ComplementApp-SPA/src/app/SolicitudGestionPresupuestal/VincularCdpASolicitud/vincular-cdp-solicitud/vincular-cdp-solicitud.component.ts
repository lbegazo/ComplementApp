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
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { SolicitudCDPDto } from 'src/app/_models/solicitudCDPDto';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SolicitudCdpService } from 'src/app/_services/solicitudCdp.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-vincular-cdp-solicitud',
  templateUrl: './vincular-cdp-solicitud.component.html',
  styleUrls: ['./vincular-cdp-solicitud.component.scss'],
})
export class VincularCdpSolicitudComponent implements OnInit {
  //#region Variables

  nombreTransaccion: string;
  transaccion: Transaccion;
  arrayControls = new FormArray([]);
  mostrarCabecera = true;
  esCreacion = true;
  nombreBoton = 'Vincular CDP';

  search: string;
  suggestions$: Observable<SolicitudCDPDto[]>;
  errorMessage: string;
  baseUrl = environment.apiUrl + 'lista/ObtenerListaSolicitudCDP';
  subscriptions: Subscription[] = [];

  solicitudCDPId = 0;
  solicitudCdpSeleccionado: SolicitudCDPDto;
  listaSolicitudCdp: SolicitudCDPDto[] = [];

  facturaHeaderForm = new FormGroup({});
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

  //#endregion Variables

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private cdpService: SolicitudCdpService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.createForm();

    this.cargarBusquedaTerceroXCodigo();

    this.onBuscarFactura();
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      cdpCtrl: [''],
      objetoBienCtrl: [''],
      rbtRadicarFacturaCtrl: ['1'],
      planPagoControles: this.arrayControls,
    });
  }

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.solicitudCdpSeleccionado = e.item as SolicitudCDPDto;
    if (this.solicitudCdpSeleccionado) {
      this.solicitudCDPId = this.solicitudCdpSeleccionado.solicitudCDPId;
    }
  }

  cargarBusquedaTerceroXCodigo() {
    this.suggestions$ = new Observable((observer: Observer<string>) => {
      observer.next(this.search);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<SolicitudCDPDto[]>(this.baseUrl, {
              params: { numeroSolicitud: query },
            })
            .pipe(
              map((data: SolicitudCDPDto[]) => data || []),
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

  onCreacion(event) {
    this.limpiarVariables();
    this.esCreacion = true;
    this.nombreBoton = 'Vincular CDP';
    this.onBuscarFactura();
  }

  onModificacion(event) {
    this.limpiarVariables();
    this.esCreacion = false;
    this.nombreBoton = 'Modificar CDP';
    this.onBuscarFactura();
  }

  onBuscarFactura() {
    this.cdpService
      .ObtenerListaSolicitudParaVincularCDP(
        this.esCreacion ? 1 : 2,
        this.solicitudCDPId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<SolicitudCDPDto[]>) => {
          this.listaSolicitudCdp = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          const tipo = this.esCreacion ? '1' : '2';
          this.facturaHeaderForm = this.fb.group({
            cdpCtrl: [''],
            objetoBienCtrl: [''],
            rbtRadicarFacturaCtrl: [tipo],
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaSolicitudCdp && this.listaSolicitudCdp.length > 0) {
      for (const detalle of this.listaSolicitudCdp) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl('', [Validators.required]),
          })
        );
      }
    } else {
      this.alertify.warning('No existen solicitudes para vincular CDP');
    }
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  limpiarVariables() {
    this.listaSolicitudCdp = [];
    this.nombreBoton = 'Vincular CDP';
    this.esCreacion = true;
    this.solicitudCDPId = 0;
    this.solicitudCdpSeleccionado = null;

    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };
  }

  onLimpiarFactura() {
    this.limpiarVariables();
    this.onBuscarFactura();
  }

  onCheckChange(event) {
    /* Selected */
    this.solicitudCDPId = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.solicitudCDPId = +event.target.value;
    }
  }

  onRegistrarSolicitud() {
    if (this.listaSolicitudCdp && this.listaSolicitudCdp.length > 0) {
      this.solicitudCdpSeleccionado = this.listaSolicitudCdp.filter(
        (x) => x.solicitudCDPId === this.solicitudCDPId
      )[0];
      if (this.solicitudCdpSeleccionado) {
        this.mostrarCabecera = false;
      }
    }
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

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }
}
