import {
  Component,
  OnInit,
  ChangeDetectorRef,
  ViewChild,
  ElementRef,
  Renderer2,
} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  noop,
  Observable,
  Observer,
  of,
  Subscription,
  combineLatest,
} from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { environment } from 'src/environments/environment';
import { Tercero } from 'src/app/_models/tercero';
import { PopupBuscarFacturaComponent } from '../popup-buscar-factura/popup-buscar-factura.component';
import { PlanPago } from 'src/app/_models/planPago';
import {
  FormGroup,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Transaccion } from 'src/app/_models/transaccion';

@Component({
  selector: 'app-factura-main',
  templateUrl: './factura-main.component.html',
  styleUrls: ['./factura-main.component.css'],
})
export class FacturaMainComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  search: string;
  suggestions$: Observable<Tercero[]>;
  errorMessage: string;
  bsModalRef: BsModalRef;
  subscriptions: Subscription[] = [];
  planesPagoSeleccionado: number[];
  esRadicarFactura = true;

  listaPlanPago: PlanPago[] = [];
  planPagoIdSeleccionado = 0;
  tercero: Tercero; 
  // = {
  //   terceroId: 0,
  //   nombre: '',
  //   numeroIdentificacion: '',
  //   tipoDocumentoIdentidad: '',
  //   tipoDocumentoIdentidadId: 0,
  // };

  facturaHeaderForm = new FormGroup({});

  @ViewChild('myRbtRadicarFactura', { static: true })
  myRbtRadicarFactura: ElementRef;

  terceroId = 0;
  baseUrl = environment.apiUrl + 'lista/ObtenerListaTercero';

  constructor(
    private http: HttpClient,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef,
    private renderer: Renderer2,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.createForm();

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
      rbtRadicarFacturaCtrl: ['1', Validators.required],
      terceroCtrl: ['', Validators.required],
      terceroDescripcionCtrl: [''],
    });
  }

  onRadicarFactura(event) {
    this.onLimpiarFactura();
    this.esRadicarFactura = true;
  }

  onModificarFactura(event) {
    this.onLimpiarFactura();
    this.esRadicarFactura = false;
  }

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  onBuscarFactura() {
    //#region Abrir Popup de rubros presupuestales
    const initialState = {
      title: 'SELECCIONE LA FACTURA',
      terId: this.terceroId,
      radicarFactura: this.esRadicarFactura,
    };

    this.bsModalRef = this.modalService.show(
      PopupBuscarFacturaComponent,
      Object.assign({ initialState }, { class: 'gray modal-lg' })
    );

    this.bsModalRef.content.closeBtnName = 'Aceptar';

    //#endregion Abrir Popup de rubros presupuestales

    //#region Cargar información del popup (OnHidden event)

    const combine = combineLatest(this.modalService.onHidden).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (this.bsModalRef.content != null) {
          this.planesPagoSeleccionado = this.bsModalRef.content;
        }

        if (this.planesPagoSeleccionado) {
          this.planPagoIdSeleccionado = +this.planesPagoSeleccionado[0];

          //   this.deshabilitarControles();
          //   this.editForm.reset();
        }

        this.unsubscribe();
      })
    );

    this.subscriptions.push(combine);

    //#endregion Cargar información del popup (OnHidden event)
  }

  onLimpiarFactura() {
    this.esRadicarFactura = true;
    this.planesPagoSeleccionado = [];
    this.listaPlanPago = [];
    this.planPagoIdSeleccionado = 0;
    this.tercero = null;
    this.search = '';
  }

  receiveMessage($event) {
    this.onLimpiarFactura();
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }
}
