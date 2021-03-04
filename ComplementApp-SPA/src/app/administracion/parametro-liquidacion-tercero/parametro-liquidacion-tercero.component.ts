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
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ListaService } from 'src/app/_services/lista.service';
import { TerceroService } from 'src/app/_services/tercero.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-parametro-liquidacion-tercero',
  templateUrl: './parametro-liquidacion-tercero.component.html',
  styleUrls: ['./parametro-liquidacion-tercero.component.scss'],
})
export class ParametroLiquidacionTerceroComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  search: string;
  searchNombre: string;
  suggestions$: Observable<Tercero[]>;
  suggestionsXNombre$: Observable<Tercero[]>;

  errorMessage: string;
  subscriptions: Subscription[] = [];
  esCreacion = true;
  mostrarCabecera = true;
  parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  nombreBoton = 'Registrar';

  listaTercero: Tercero[] = [];
  tercero: Tercero;
  terceroSeleccionado: Tercero;
  listaNotasLegales: ValorSeleccion[] = [];

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
    private listaService: ListaService
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

    this.cargarNotasLegales();

    this.onBuscarFactura();
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
    this.nombreBoton = 'Registrar';
    this.onBuscarFactura();
  }

  onModificacion(event) {
    this.limpiarVariables();
    this.esCreacion = false;
    this.nombreBoton = 'Modificar';
    this.onBuscarFactura();
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

  onBuscarFactura() {
    this.terceroService
      .ObtenerTercerosParaParametrizacionLiquidacion(
        this.esCreacion ? 1 : 2,
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
            this.alertify.warning(
              'No existen terceros sin parametros de liquidación faltantes'
            );
          }
          const tipo = this.esCreacion ? '1' : '2';
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

  onParametrizar() {
    if (this.terceroId > 0) {
      if (!this.esCreacion) {
        this.terceroService
          .ObtenerParametrizacionLiquidacionXTercero(this.terceroId)
          .subscribe(
            (documento: ParametroLiquidacionTercero) => {
              if (documento) {
                this.parametroLiquidacionSeleccionado = documento;
                this.mostrarCabecera = false;
              } else {
                this.alertify.error(
                  'No se pudo obtener información para la parametrización del tercero'
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
    this.onBuscarFactura();
  }

  limpiarVariables() {
    this.nombreBoton = 'Registrar';
    this.esCreacion = true;
    this.listaTercero = [];
    this.tercero = null;
    this.terceroId = 0;
    this.search = '';
    this.parametroLiquidacionSeleccionado = null;

    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };

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

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }
}
