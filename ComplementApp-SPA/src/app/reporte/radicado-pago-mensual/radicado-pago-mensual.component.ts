import { HttpClient, HttpEventType } from '@angular/common/http';
import { OnDestroy } from '@angular/core';
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
import { RadicadoDto } from 'src/app/_dto/radicadoDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { EstadoPlanPago } from 'src/app/_models/enum';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ListaService } from 'src/app/_services/lista.service';
import { PlanPagoService } from 'src/app/_services/planPago.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-radicado-pago-mensual',
  templateUrl: './radicado-pago-mensual.component.html',
  styleUrls: ['./radicado-pago-mensual.component.scss'],
})
export class RadicadoPagoMensualComponent implements OnInit, OnDestroy {
  nombreTransaccion: string;
  transaccion: Transaccion;
  search: string;
  suggestions$: Observable<Tercero[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  listaEstadoId: string;
  dateObj = new Date();

  listaPlanPago: RadicadoDto[] = [];
  tercero: Tercero = {
    terceroId: 0,
    nombre: '',
    numeroIdentificacion: '',
    tipoDocumentoIdentidad: '',
    tipoDocumentoIdentidadId: 0,
  };

  facturaHeaderForm = new FormGroup({});
  terceroId = 0;
  baseUrl = environment.apiUrl + 'lista/ObtenerListaTercero';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

  listaMeses: ValorSeleccion[];
  idMesSelecionado: number;
  mesSeleccionado: ValorSeleccion;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private facturaService: PlanPagoService,
    private fb: FormBuilder,
    private listaService: ListaService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.idMesSelecionado = this.dateObj.getUTCMonth() + 1;

    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.cargarEstados();
    this.cargarMeses();
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
      mesCtrl: ['', Validators.required],
      terceroCtrl: [''],
      terceroDescripcionCtrl: [''],
    });
  }

  cargarMeses() {
    this.listaService.ObtenerListaMeses().subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaMeses = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.mesSeleccionado = this.listaMeses.filter(
          (x) => x.id === this.idMesSelecionado
        )[0];
      }
    );
  }

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  onBuscarFactura() {
    this.facturaService
      .ObtenerListaRadicadoPaginado(
        this.idMesSelecionado,
        this.listaEstadoId,
        this.terceroId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<RadicadoDto[]>) => {
          this.listaPlanPago = documentos.result;
          this.pagination = documentos.pagination;

          if (this.listaPlanPago && this.listaPlanPago.length === 0) {
            this.alertify.warning('No existen Radicados');
          }
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {}
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura(form: FormGroup) {
    this.listaPlanPago = [];
    this.tercero = null;
    this.search = '';
    this.terceroId = 0;
    this.idMesSelecionado = 0;
    this.mesSeleccionado = null;

    form.reset();
  }

  exportarExcel() {
    let fileName = '';

    this.facturaService
      .DescargarListaRadicado(
        this.idMesSelecionado,
        this.listaEstadoId,
        this.terceroId === null ? 0 : this.terceroId
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
                fileName = nombreArchivo;
              } else {
                fileName = 'Radicados.xlsx';
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
          this.router.navigate(['/CONSULTAS_RADICADOPAGOMENSUAL']);
        }
      );
  }

  onSelectMes() {
    this.mesSeleccionado = this.mesControl.value as ValorSeleccion;
    this.idMesSelecionado = this.mesSeleccionado.id;
  }

  ngOnDestroy() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  cargarEstados() {
    this.listaEstadoId =
      EstadoPlanPago.PorObligar.value.toString() +
      ',' +
      EstadoPlanPago.ConLiquidacionDeducciones.value.toString() +
      ',' +
      EstadoPlanPago.Obligado.value.toString() +
      ',' +
      EstadoPlanPago.Pagado.value.toString() +
      ',' +
      EstadoPlanPago.Rechazada.value.toString() +
      ',' +
      EstadoPlanPago.ConOrdenPago.value.toString();
  }

  get mesControl() {
    return this.facturaHeaderForm.get('mesCtrl');
  }
}
