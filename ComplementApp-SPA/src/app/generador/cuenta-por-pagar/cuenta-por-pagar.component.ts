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
import { listStyleType } from 'html2canvas/dist/types/css/property-descriptors/list-style-type';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/Operators';
import { EstadoPlanPago } from 'src/app/_models/enum';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-cuenta-por-pagar',
  templateUrl: './cuenta-por-pagar.component.html',
  styleUrls: ['./cuenta-por-pagar.component.css'],
})
export class CuentaPorPagarComponent implements OnInit {
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

  listaPlanPago: FormatoCausacionyLiquidacionPago[] = [];
  tercero: Tercero = {
    terceroId: 0,
    nombre: '',
    numeroIdentificacion: '',
    tipoIdentificacion: '',
  };

  facturaHeaderForm = new FormGroup({});
  terceroId?: number = null;
  baseUrl = environment.apiUrl + 'lista/ObtenerListaTercero';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };
  liquidacionesSeleccionadas: number[] = [];
  totalEnPagina = 0;
  seleccionaTodas = false;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private liquidacionService: DetalleLiquidacionService,
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
      this.totalEnPagina = this.listaPlanPago.length;
      for (const detalle of this.listaPlanPago) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl('', [Validators.required]),
          })
        );
      }
    } else {
      this.alertify.warning(
        'No existen Facturas en estado por “ConLiquidacionDeducciones”'
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
    this.listaEstadoId = EstadoPlanPago.ConLiquidacionDeducciones.value.toString();

    this.liquidacionService
      .ObtenerListaDetalleLiquidacion(
        this.listaEstadoId,
        this.terceroId,
        0,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<FormatoCausacionyLiquidacionPago[]>) => {
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
    this.tercero = null;
    this.search = '';
    this.terceroId = null;
    this.liquidacionesSeleccionadas = [];
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
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.liquidacionesSeleccionadas?.push(+event.target.value);
    } else {
      /* unselected */
      let index = 0;
      let i = 0;
      this.liquidacionesSeleccionadas.forEach((val: number) => {
        if (val === event.target.value) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.liquidacionesSeleccionadas.splice(index, 1);
      }
    }

    if (this.listaPlanPago.length === this.liquidacionesSeleccionadas.length) {
      this.seleccionaTodas = true;
    } else {
      this.seleccionaTodas = false;
    }
  }

  onCheckAllChange(event) {
    const checked = event.target.checked;
    if (checked) {
      this.seleccionaTodas = true;
      this.listaPlanPago.forEach((item) => (item.esSeleccionada = checked));
      this.liquidacionesSeleccionadas = [];

      this.listaPlanPago.forEach((val: FormatoCausacionyLiquidacionPago) => {
        this.liquidacionesSeleccionadas.push(val.detalleLiquidacionId);
      });
    } else {
      this.listaPlanPago.forEach((item) => (item.esSeleccionada = checked));
      this.liquidacionesSeleccionadas = [];
    }
  }

  public DescargarArchivoDetalleLiquidacion() {
    let fileName = '';

    if (this.seleccionaTodas) {
      //#region Seleccionar todas
      this.listaEstadoId = EstadoPlanPago.ConLiquidacionDeducciones.value.toString();
      this.liquidacionService
        .ObtenerListaDetalleLiquidacionTotal(
          this.listaEstadoId,
          this.terceroId,
          0
        )
        .subscribe(
          (lista: number[]) => {
            if (lista && lista.length > 0) {
              this.liquidacionesSeleccionadas = [];
              lista.forEach((element: number) => {
                this.liquidacionesSeleccionadas.push(element);
              });
            }
          },
          (error) => {
            this.alertify.error(error);
          },
          () => {
            if (
              this.liquidacionesSeleccionadas &&
              this.liquidacionesSeleccionadas.length > 0
            ) {
              this.liquidacionService
                .DescargarMaestroDetalleLiquidacionParaArchivo(
                  this.liquidacionesSeleccionadas.join()
                )
                .subscribe(
                  (response) => {
                    switch (response.type) {
                      case HttpEventType.Response:
                        const downloadedFile = new Blob([response.body], {
                          type: response.body.type,
                        });

                        const nombreArchivo = response.headers.get('filename');

                        if (nombreArchivo != null && nombreArchivo.length > 0) {
                          fileName = nombreArchivo + '.txt';
                        } else {
                          fileName = 'SIGPAA_Maestro.txt';
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
                    this.liquidacionService
                      .DescargarDetalleLiquidacionParaArchivo(
                        this.liquidacionesSeleccionadas.join()
                      )
                      .subscribe(
                        (response) => {
                          switch (response.type) {
                            case HttpEventType.Response:
                              const downloadedFile = new Blob([response.body], {
                                type: response.body.type,
                              });
                              const nombreArchivo = response.headers.get(
                                'filename'
                              );

                              if (
                                nombreArchivo != null &&
                                nombreArchivo.length > 0
                              ) {
                                fileName = nombreArchivo + '.txt';
                              } else {
                                fileName = 'SIGPAA_Detalle.txt';
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
                          this.onLimpiarFactura();
                          this.router.navigate(['/GENERADOR_CUENTAPORPAGAR']);
                        }
                      );
                  }
                );
            }
          }
        );

      //#endregion Seleccionar todas
    } else {
      //#region Seleccionar items
      if (
        this.liquidacionesSeleccionadas &&
        this.liquidacionesSeleccionadas.length > 0
      ) {
        this.liquidacionService
          .DescargarMaestroDetalleLiquidacionParaArchivo(
            this.liquidacionesSeleccionadas.join()
          )
          .subscribe(
            (response) => {
              switch (response.type) {
                case HttpEventType.Response:
                  const downloadedFile = new Blob([response.body], {
                    type: response.body.type,
                  });

                  const nombreArchivo = response.headers.get('filename');

                  if (nombreArchivo != null && nombreArchivo.length > 0) {
                    fileName = nombreArchivo + '.txt';
                  } else {
                    fileName = 'SIGPAA_Maestro.txt';
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
              this.liquidacionService
                .DescargarDetalleLiquidacionParaArchivo(
                  this.liquidacionesSeleccionadas.join()
                )
                .subscribe(
                  (response) => {
                    switch (response.type) {
                      case HttpEventType.Response:
                        const downloadedFile = new Blob([response.body], {
                          type: response.body.type,
                        });

                        const nombreArchivo = response.headers.get('filename');

                        if (nombreArchivo != null && nombreArchivo.length > 0) {
                          fileName = nombreArchivo + '.txt';
                        } else {
                          fileName = 'SIGPAA_Detalle.txt';
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
                    this.onLimpiarFactura();
                    this.router.navigate(['/GENERADOR_CUENTAPORPAGAR']);
                  }
                );
            }
          );
      }
    }
    //#endregion Seleccionar items
  }  
}
