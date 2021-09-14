import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
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
import { Contrato } from 'src/app/_models/contrato';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ContratoService } from 'src/app/_services/contrato.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-contrato',
  templateUrl: './contrato.component.html',
  styleUrls: ['./contrato.component.scss'],
})
export class ContratoComponent implements OnInit {
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
  tipoPago = 0;

  listaCdp: Cdp[] = [];
  crp = 0;
  cdpSeleccionado: Cdp;
  contratoSeleccionado: Contrato;
  tercero: Tercero;

  nombreBoton = 'Registrar Contrato';
  esCreacion = true;

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

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private contratoService: ContratoService,
    private router: Router
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

    this.cargarBusquedaTerceroXNombre();

    this.onBuscarFactura();
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      rbtRadicarFacturaCtrl: ['1'],
      terceroCtrl: ['', Validators.required],
      terceroDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  crearControlesDeArray() {
    if (this.listaCdp && this.listaCdp.length > 0) {
      for (const detalle of this.listaCdp) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl('', [Validators.required]),
          })
        );
      }
    } else {
      this.alertify.warning('No existen compromisos sin contrato registrado');
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

  onCreacion(event) {
    this.limpiarVariables();
    this.esCreacion = true;
    this.nombreBoton = 'Registrar Contrato';
    this.onBuscarFactura();
  }

  onModificacion(event) {
    this.limpiarVariables();
    this.esCreacion = false;
    this.nombreBoton = 'Modificar Contrato';
    this.onBuscarFactura();
  }

  onLimpiarFactura() {
    this.limpiarVariables();
    this.onBuscarFactura();
  }

  onBuscarFactura() {
    this.contratoService
      .ObtenerCompromisosParaContrato(
        this.esCreacion ? 1 : 2,
        this.terceroId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Cdp[]>) => {
          this.listaCdp = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          const tipo = this.esCreacion ? '1' : '2';
          this.facturaHeaderForm = this.fb.group({
            rbtRadicarFacturaCtrl: [tipo],
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

  limpiarVariables() {
    this.nombreBoton = 'Registrar Contrato';
    this.esCreacion = true;
    this.listaCdp = [];
    this.crp = 0;
    this.tercero = null;
    this.search = '';
    this.terceroId = null;
    this.cdpSeleccionado = null;
    this.contratoSeleccionado = null;

    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  onCheckChange(event) {
    /* Selected */
    this.crp = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.crp = +event.target.value;
    }
  }

  onRegistrarContrato() {
    if (this.listaCdp && this.listaCdp.length > 0 && this.crp > 0) {
      this.cdpSeleccionado = this.listaCdp.filter((x) => x.crp === this.crp)[0];
      if (this.cdpSeleccionado) {
        this.mostrarCabecera = false;

        if (!this.esCreacion) {
          this.contratoService
            .ObtenerContrato(this.cdpSeleccionado.contratoId)
            .subscribe(
              (documento: Contrato) => {
                if (documento) {
                  this.contratoSeleccionado = documento;
                  this.mostrarCabecera = false;
                } else {
                  this.alertify.error(
                    'No se pudo obtener información del contrato seleccionado'
                  );
                  this.mostrarCabecera = true;
                }
              },
              (error) => {
                this.alertify.error(error);
              }
            );
        }
      } else {
        this.alertify.error('Hubo un error al obtener el compromiso.');
      }
    }
  }

  exportarExcel() {
    let fileName = '';

    this.contratoService
      .DescargarListaContratoTotal()
      .subscribe(
        (response) => {
          switch (response.type) {
            case HttpEventType.Response:
              const downloadedFile = new Blob([response.body], {
                type: response.body.type,
              });

              const nombreArchivo = response.headers.get('filename');

              if (nombreArchivo != null && nombreArchivo.length > 0) {
                fileName = nombreArchivo;
              } else {
                fileName = 'Contratos.xlsx';
              }

              const a = document.createElement('a');
              a.setAttribute('style', 'display:none;');
              document.body.appendChild(a);
              a.download = fileName;
              a.href = URL.createObjectURL(downloadedFile);
              a.target = '_blank';
              a.click();
              document.body.removeChild(a);
              break;
          }
        },
        (error) => {
          this.alertify.warning(error);
        },
        () => {
          this.router.navigate(['/ADMINISTRACION_CONTRATO']);
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

    this.onLimpiarFactura();
    this.mostrarCabecera = true;
  }
}
