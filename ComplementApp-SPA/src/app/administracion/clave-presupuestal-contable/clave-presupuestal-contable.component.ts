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
import { ClavePresupuestalContableDto } from 'src/app/_dto/clavePresupuestalContableDto';
import { Cdp } from 'src/app/_models/cdp';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Tercero } from 'src/app/_models/tercero';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClavePresupuestalContableService } from 'src/app/_services/clavePresupuestalContable.service';
import { SolicitudPagoService } from 'src/app/_services/solicitudPago.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-clave-presupuestal-contable',
  templateUrl: './clave-presupuestal-contable.component.html',
  styleUrls: ['./clave-presupuestal-contable.component.scss'],
})
export class ClavePresupuestalContableComponent implements OnInit {
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

  listaCdp: Cdp[] = [];
  crp = 0;
  cdpSeleccionado: Cdp;
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
  listaClavePresupuestalContable: ClavePresupuestalContableDto[];

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private clavePresupuestalContableService: ClavePresupuestalContableService
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
    if (this.listaCdp && this.listaCdp.length > 0) {
      for (const detalle of this.listaCdp) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl('', [Validators.required]),
          })
        );
      }
    } else {
      this.alertify.warning(
        'El tercero ya tiene registrados clave contable'
      );
    }
  }

  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  onBuscarFactura() {
    this.clavePresupuestalContableService
      .ObtenerCompromisosParaClavePresupuestalContable(
        this.terceroId,
        null,
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
    this.listaCdp = [];
    this.crp = 0;
    this.tercero = null;
    this.search = '';
    this.terceroId = null;
    this.listaClavePresupuestalContable = [];

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
    this.crp = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.crp = +event.target.value;
    }
  }

  onRegistrarClavePresupuestal() {
    if (this.listaCdp && this.listaCdp.length > 0 && this.crp > 0) {
      this.cdpSeleccionado = this.listaCdp.filter(
        (x) => x.crp === this.crp
      )[0];
      if (this.cdpSeleccionado) {
        this.ObtenerRubrosParaClavePresupuestalContable();
      }
    }
  }

  ObtenerRubrosParaClavePresupuestalContable() {
    this.clavePresupuestalContableService
      .ObtenerRubrosPresupuestalesXCompromiso(this.crp)
      .subscribe(
        (response: ClavePresupuestalContableDto[]) => {
          if (response) {
            this.listaClavePresupuestalContable = response;
            if (
              this.listaClavePresupuestalContable &&
              this.listaClavePresupuestalContable.length > 0
            ) {
              this.terceroId = this.listaClavePresupuestalContable[0].tercero.id;
              this.mostrarCabecera = false;
            }
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
    this.onLimpiarFactura();
    this.mostrarCabecera = true;
  }
}
