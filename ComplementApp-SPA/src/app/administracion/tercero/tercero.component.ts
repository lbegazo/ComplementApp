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
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TerceroService } from 'src/app/_services/tercero.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-tercero',
  templateUrl: './tercero.component.html',
  styleUrls: ['./tercero.component.scss'],
})
export class TerceroComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  search: string;
  searchNombre: string;
  suggestions$: Observable<Tercero[]>;
  suggestionsXNombre$: Observable<Tercero[]>;

  errorMessage: string;
  subscriptions: Subscription[] = [];
  esCreacion = false;
  mostrarCabecera = true;

  listaTercero: Tercero[] = [];
  tercero: Tercero;
  terceroSeleccionado: Tercero;

  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

  facturaHeaderForm = new FormGroup({});

  terceroId = 0;
  arrayControls = new FormArray([]);
  baseUrl = environment.apiUrl + 'lista/ObtenerListaTercero';

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private terceroService: TerceroService,
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

    this.onBuscarTercero();
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      rbtRadicarFacturaCtrl: ['1'],
      terceroCtrl: [''],
      terceroDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  onCreacion(event) {
    this.limpiarVariables();
    this.esCreacion = true;
    this.mostrarCabecera = false;
  }

  onModificacion(event) {
    this.limpiarVariables();
    this.esCreacion = false;
    this.onBuscarTercero();
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

  onCheckChange(event) {
    /* Selected */
    this.terceroId = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.terceroId = +event.target.value;

      this.terceroSeleccionado = this.listaTercero.filter(
        (x) => x.terceroId === this.terceroId
      )[0];
    }
  }

  onBuscarTercero() {
    this.terceroService
      .ObtenerTerceros(
        this.esCreacion ? 2 : 1,
        this.terceroId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Tercero[]>) => {
          this.listaTercero = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          if (this.listaTercero && this.listaTercero.length === 0) {
            this.alertify.warning('No existen terceros registrados');
          }
          const tipo = this.esCreacion ? '2' : '1';
          this.facturaHeaderForm = this.fb.group({
            rbtRadicarFacturaCtrl: [tipo],
            terceroCtrl: [''],
            terceroDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaTercero && this.listaTercero.length > 0) {
      for (const detalle of this.listaTercero) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl('', [Validators.required]),
          })
        );
      }
    }
  }

  onModificar() {
    if (this.terceroId > 0) {
      if (!this.esCreacion) {
        this.terceroService.ObtenerTercero(this.terceroId).subscribe(
          (documento: Tercero) => {
            if (documento) {
              this.terceroSeleccionado = documento;
              this.mostrarCabecera = false;
            } else {
              this.alertify.error(
                'No se pudo obtener información para el tercero'
              );
              this.mostrarCabecera = true;
            }
          },
          (error) => {
            this.alertify.error(error);
          }
        );
      } else {
        this.mostrarCabecera = false;
      }
    }
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

  onLimpiarFactura() {
    this.limpiarVariables();
    this.onBuscarTercero();
  }

  limpiarVariables() {
    this.esCreacion = false;
    this.listaTercero = [];
    this.tercero = null;
    this.terceroId = 0;
    this.search = '';
    this.terceroSeleccionado = null;
  }

  exportarExcel() {
    let fileName = '';

    this.terceroService
      .DescargarListaTercero()
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
                fileName = 'Terceros.xlsx';
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
          this.router.navigate(['/ADMINISTRACION_TERCERO']);
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarTercero();
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }
}
