import { isDataSource } from '@angular/cdk/collections';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/Operators';
import { EstadoPlanPago, TipoArchivoObligacion } from 'src/app/_models/enum';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-obligacion-presupuestal',
  templateUrl: './obligacion-presupuestal.component.html',
  styleUrls: ['./obligacion-presupuestal.component.scss'],
})
export class ObligacionPresupuestalComponent implements OnInit {
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
  tercero: Tercero;

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
  liquidacionesSeleccionadas: number[] = [];
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
      terceroCtrl: [''],
      terceroDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  onBuscarFactura() {
    this.listaEstadoId =
      EstadoPlanPago.ConLiquidacionDeducciones.value.toString();

    this.liquidacionService
      .ObtenerLiquidacionesParaArchivoObligacion(
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
            terceroCtrl: [''],
            terceroDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaPlanPago && this.listaPlanPago.length > 0) {
      if (this.seleccionaTodas) {
        this.listaPlanPago.forEach((item) => {
          item.esSeleccionada = this.seleccionaTodas;
        });

        this.listaPlanPago.forEach((val: FormatoCausacionyLiquidacionPago) => {
          if (
            this.liquidacionesSeleccionadas?.indexOf(
              val.detalleLiquidacionId
            ) === -1
          ) {
            this.liquidacionesSeleccionadas.push(val.detalleLiquidacionId);
          }
        });
      } else {
        if (
          this.liquidacionesSeleccionadas &&
          this.liquidacionesSeleccionadas.length > 0
        ) {
          this.listaPlanPago.forEach(
            (val: FormatoCausacionyLiquidacionPago) => {
              if (
                this.liquidacionesSeleccionadas?.indexOf(
                  val.detalleLiquidacionId
                ) > -1
              ) {
                val.esSeleccionada = true;
              }
            }
          );
        }
      }

      for (const detalle of this.listaPlanPago) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning(
        'No existen Facturas en estado por “ConLiquidacionDeducciones”'
      );
    }
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
    this.seleccionaTodas = false;
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

      if (this.liquidacionesSeleccionadas?.indexOf(valor) === -1) {
        this.liquidacionesSeleccionadas?.push(+valor);
      }
    } else {
      /* unselected */
      valor = +event.target.value;
      let index = 0;
      let i = 0;
      this.liquidacionesSeleccionadas.forEach((val: number) => {
        if (val === valor) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.liquidacionesSeleccionadas.splice(index, 1);
      }
    }

    if (this.pagination) {
      if (
        this.pagination.totalItems === this.liquidacionesSeleccionadas.length
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
      this.liquidacionesSeleccionadas = [];

      this.listaPlanPago.forEach((val: FormatoCausacionyLiquidacionPago) => {
        this.liquidacionesSeleccionadas.push(val.detalleLiquidacionId);
      });
    } else {
      this.seleccionaTodas = false;
      this.listaPlanPago.forEach((item) => (item.esSeleccionada = checked));
      this.liquidacionesSeleccionadas = [];
    }
  }

  public DescargarArchivoDetalleLiquidacion() {
    let fileName = '';
    let listaIds: number[] = [];

    const esSeleccionarTodas = this.seleccionaTodas ? 1 : 0;
    let listaCadenaIds = '';
    this.listaEstadoId =
      EstadoPlanPago.ConLiquidacionDeducciones.value.toString();
    let tipoArchivoObligacion = 0;
    let conUsoPresupuestal = 0;

    if (this.facturaHeaderForm.valid) {
      //#region Obtener lista de ids: listaCadenaIds

      if (!this.seleccionaTodas) {
        if (
          this.liquidacionesSeleccionadas &&
          this.liquidacionesSeleccionadas.length > 0
        ) {
          listaIds = this.liquidacionesSeleccionadas.filter(
            (v, i) => this.liquidacionesSeleccionadas.indexOf(v) === i
          );
          listaCadenaIds = listaIds.join();
        }
      }

      //#endregion Obtener lista de ids

      //#region Descargar archivo CABECERA CON USO PRESUPUESTAL

      tipoArchivoObligacion = TipoArchivoObligacion.Cabecera.value;
      conUsoPresupuestal = 1;

      this.liquidacionService
        .DescargarArchivoLiquidacionObligacion(
          listaCadenaIds,
          this.listaEstadoId,
          esSeleccionarTodas,
          this.terceroId,
          tipoArchivoObligacion,
          conUsoPresupuestal
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
            //#region Descargar archivo ITEM

            tipoArchivoObligacion = TipoArchivoObligacion.Item.value;

            this.liquidacionService
              .DescargarArchivoLiquidacionObligacion(
                listaCadenaIds,
                this.listaEstadoId,
                esSeleccionarTodas,
                this.terceroId,
                tipoArchivoObligacion,
                conUsoPresupuestal
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
                        fileName = 'SIGPAA_Items.txt';
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
                  //#region Descargar archivo DEDUCCIONES

                  tipoArchivoObligacion =
                    TipoArchivoObligacion.Deducciones.value;

                  this.liquidacionService
                    .DescargarArchivoLiquidacionObligacion(
                      listaCadenaIds,
                      this.listaEstadoId,
                      esSeleccionarTodas,
                      this.terceroId,
                      tipoArchivoObligacion,
                      conUsoPresupuestal
                    )
                    .subscribe(
                      (response) => {
                        switch (response.type) {
                          case HttpEventType.Response:
                            const downloadedFile = new Blob([response.body], {
                              type: response.body.type,
                            });

                            const nombreArchivo =
                              response.headers.get('filename');

                            if (
                              nombreArchivo != null &&
                              nombreArchivo.length > 0
                            ) {
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
                        //#region Descargar archivo USOS

                        tipoArchivoObligacion =
                          TipoArchivoObligacion.Usos.value;

                        this.liquidacionService
                          .DescargarArchivoLiquidacionObligacion(
                            listaCadenaIds,
                            this.listaEstadoId,
                            esSeleccionarTodas,
                            this.terceroId,
                            tipoArchivoObligacion,
                            conUsoPresupuestal
                          )
                          .subscribe(
                            (response) => {
                              switch (response.type) {
                                case HttpEventType.Response:
                                  const downloadedFile = new Blob(
                                    [response.body],
                                    {
                                      type: response.body.type,
                                    }
                                  );

                                  const nombreArchivo =
                                    response.headers.get('filename');

                                  if (
                                    nombreArchivo != null &&
                                    nombreArchivo.length > 0
                                  ) {
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
                              //#region Descargar archivo FACTURA

                              tipoArchivoObligacion =
                                TipoArchivoObligacion.Factura.value;

                              this.liquidacionService
                                .DescargarArchivoLiquidacionObligacion(
                                  listaCadenaIds,
                                  this.listaEstadoId,
                                  esSeleccionarTodas,
                                  this.terceroId,
                                  tipoArchivoObligacion,
                                  conUsoPresupuestal
                                )
                                .subscribe(
                                  (response) => {
                                    switch (response.type) {
                                      case HttpEventType.Response:
                                        if (response.body !== null) {
                                          const downloadedFile = new Blob(
                                            [response.body],
                                            {
                                              type: response.body.type,
                                            }
                                          );

                                          const nombreArchivo =
                                            response.headers.get('filename');

                                          if (
                                            nombreArchivo != null &&
                                            nombreArchivo.length > 0
                                          ) {
                                            fileName = nombreArchivo + '.txt';
                                          } else {
                                            fileName = 'SIGPAA_Maestro.txt';
                                          }

                                          const a = document.createElement('a');
                                          a.setAttribute(
                                            'style',
                                            'display:none;'
                                          );
                                          document.body.appendChild(a);
                                          a.download = fileName;
                                          a.href =
                                            URL.createObjectURL(downloadedFile);
                                          a.target = '_blank';
                                          a.click();
                                          document.body.removeChild(a);
                                        }
                                        break;
                                    }
                                  },
                                  (error) => {
                                    this.alertify.warning(error);
                                  },
                                  () => {
                                    //#region Descargar archivo CABECERA Y NO USO PRESUPUESTAL

                                    tipoArchivoObligacion =
                                      TipoArchivoObligacion.Cabecera.value;
                                    conUsoPresupuestal = 0;

                                    this.liquidacionService
                                      .DescargarArchivoLiquidacionObligacion(
                                        listaCadenaIds,
                                        this.listaEstadoId,
                                        esSeleccionarTodas,
                                        this.terceroId,
                                        tipoArchivoObligacion,
                                        conUsoPresupuestal
                                      )
                                      .subscribe(
                                        (response) => {
                                          switch (response.type) {
                                            case HttpEventType.Response:
                                              if (response.body !== null) {
                                                const downloadedFile = new Blob(
                                                  [response.body],
                                                  {
                                                    type: response.body.type,
                                                  }
                                                );

                                                const nombreArchivo =
                                                  response.headers.get(
                                                    'filename'
                                                  );

                                                if (
                                                  nombreArchivo != null &&
                                                  nombreArchivo.length > 0
                                                ) {
                                                  fileName =
                                                    nombreArchivo + '.txt';
                                                } else {
                                                  fileName =
                                                    'SIGPAA_Maestro.txt';
                                                }

                                                const a =
                                                  document.createElement('a');
                                                a.setAttribute(
                                                  'style',
                                                  'display:none;'
                                                );
                                                document.body.appendChild(a);
                                                a.download = fileName;
                                                a.href =
                                                  URL.createObjectURL(
                                                    downloadedFile
                                                  );
                                                a.target = '_blank';
                                                a.click();
                                                document.body.removeChild(a);
                                              }
                                              break;
                                          }
                                        },
                                        (error) => {
                                          this.alertify.warning(error);
                                        },
                                        () => {
                                          //#region Descargar archivo ITEM

                                          tipoArchivoObligacion =
                                            TipoArchivoObligacion.Item.value;

                                          this.liquidacionService
                                            .DescargarArchivoLiquidacionObligacion(
                                              listaCadenaIds,
                                              this.listaEstadoId,
                                              esSeleccionarTodas,
                                              this.terceroId,
                                              tipoArchivoObligacion,
                                              conUsoPresupuestal
                                            )
                                            .subscribe(
                                              (response) => {
                                                switch (response.type) {
                                                  case HttpEventType.Response:
                                                    if (
                                                      response.body !== null
                                                    ) {
                                                      const downloadedFile =
                                                        new Blob(
                                                          [response.body],
                                                          {
                                                            type: response.body
                                                              .type,
                                                          }
                                                        );

                                                      const nombreArchivo =
                                                        response.headers.get(
                                                          'filename'
                                                        );

                                                      if (
                                                        nombreArchivo != null &&
                                                        nombreArchivo.length > 0
                                                      ) {
                                                        fileName =
                                                          nombreArchivo +
                                                          '.txt';
                                                      } else {
                                                        fileName =
                                                          'SIGPAA_Items.txt';
                                                      }

                                                      const a =
                                                        document.createElement(
                                                          'a'
                                                        );
                                                      a.setAttribute(
                                                        'style',
                                                        'display:none;'
                                                      );
                                                      document.body.appendChild(
                                                        a
                                                      );
                                                      a.download = fileName;
                                                      a.href =
                                                        URL.createObjectURL(
                                                          downloadedFile
                                                        );
                                                      a.target = '_blank';
                                                      a.click();
                                                      document.body.removeChild(
                                                        a
                                                      );
                                                    }
                                                    break;
                                                }
                                              },
                                              (error) => {
                                                this.alertify.warning(error);
                                              },
                                              () => {
                                                //#region Descargar archivo DEDUCCIONES

                                                tipoArchivoObligacion =
                                                  TipoArchivoObligacion
                                                    .Deducciones.value;

                                                this.liquidacionService
                                                  .DescargarArchivoLiquidacionObligacion(
                                                    listaCadenaIds,
                                                    this.listaEstadoId,
                                                    esSeleccionarTodas,
                                                    this.terceroId,
                                                    tipoArchivoObligacion,
                                                    conUsoPresupuestal
                                                  )
                                                  .subscribe(
                                                    (response) => {
                                                      switch (response.type) {
                                                        case HttpEventType.Response:
                                                          if (
                                                            response.body !==
                                                            null
                                                          ) {
                                                            const downloadedFile =
                                                              new Blob(
                                                                [response.body],
                                                                {
                                                                  type: response
                                                                    .body.type,
                                                                }
                                                              );

                                                            const nombreArchivo =
                                                              response.headers.get(
                                                                'filename'
                                                              );

                                                            if (
                                                              nombreArchivo !=
                                                                null &&
                                                              nombreArchivo.length >
                                                                0
                                                            ) {
                                                              fileName =
                                                                nombreArchivo +
                                                                '.txt';
                                                            } else {
                                                              fileName =
                                                                'SIGPAA_Maestro.txt';
                                                            }

                                                            const a =
                                                              document.createElement(
                                                                'a'
                                                              );
                                                            a.setAttribute(
                                                              'style',
                                                              'display:none;'
                                                            );
                                                            document.body.appendChild(
                                                              a
                                                            );
                                                            a.download =
                                                              fileName;
                                                            a.href =
                                                              URL.createObjectURL(
                                                                downloadedFile
                                                              );
                                                            a.target = '_blank';
                                                            a.click();
                                                            document.body.removeChild(
                                                              a
                                                            );
                                                          }
                                                          break;
                                                      }
                                                    },
                                                    (error) => {
                                                      this.alertify.warning(
                                                        error
                                                      );
                                                    },
                                                    () => {
                                                      //#region Descargar archivo FACTURA

                                                      tipoArchivoObligacion =
                                                        TipoArchivoObligacion
                                                          .Factura.value;

                                                      this.liquidacionService
                                                        .DescargarArchivoLiquidacionObligacion(
                                                          listaCadenaIds,
                                                          this.listaEstadoId,
                                                          esSeleccionarTodas,
                                                          this.terceroId,
                                                          tipoArchivoObligacion,
                                                          conUsoPresupuestal
                                                        )
                                                        .subscribe(
                                                          (response) => {
                                                            switch (
                                                              response.type
                                                            ) {
                                                              case HttpEventType.Response:
                                                                if (
                                                                  response.body !==
                                                                  null
                                                                ) {
                                                                  const downloadedFile =
                                                                    new Blob(
                                                                      [
                                                                        response.body,
                                                                      ],
                                                                      {
                                                                        type: response
                                                                          .body
                                                                          .type,
                                                                      }
                                                                    );

                                                                  const nombreArchivo =
                                                                    response.headers.get(
                                                                      'filename'
                                                                    );

                                                                  if (
                                                                    nombreArchivo !=
                                                                      null &&
                                                                    nombreArchivo.length >
                                                                      0
                                                                  ) {
                                                                    fileName =
                                                                      nombreArchivo +
                                                                      '.txt';
                                                                  } else {
                                                                    fileName =
                                                                      'SIGPAA_Maestro.txt';
                                                                  }

                                                                  const a =
                                                                    document.createElement(
                                                                      'a'
                                                                    );
                                                                  a.setAttribute(
                                                                    'style',
                                                                    'display:none;'
                                                                  );
                                                                  document.body.appendChild(
                                                                    a
                                                                  );
                                                                  a.download =
                                                                    fileName;
                                                                  a.href =
                                                                    URL.createObjectURL(
                                                                      downloadedFile
                                                                    );
                                                                  a.target =
                                                                    '_blank';
                                                                  a.click();
                                                                  document.body.removeChild(
                                                                    a
                                                                  );
                                                                }
                                                                break;
                                                            }
                                                          },
                                                          (error) => {
                                                            this.alertify.warning(
                                                              error
                                                            );
                                                          },
                                                          () => {
                                                            this.onLimpiarFactura();
                                                            this.router.navigate(
                                                              [
                                                                '/GENERADOR_OBLIGACIONES',
                                                              ]
                                                            );
                                                          }
                                                        );

                                                      //#endregion Descargar archivo FACTURA
                                                    }
                                                  );

                                                //#endregion Descargar archivo DEDUCCIONES
                                              }
                                            );

                                          //#endregion Descargar archivo ITEM
                                        }
                                      );

                                    //#endregion Descargar archivo CABECERA
                                  }
                                );

                              //#endregion  Descargar archivo FACTURA
                            }
                          );

                        //#endregion  Descargar archivo USOS
                      }
                    );

                  //#endregion Descargar archivo DEDUCCIONES
                }
              );

            //#endregion Descargar archivo ITEM
          }
        );

      //#endregion Descargar archivo CABECERA
    }
  }
}
