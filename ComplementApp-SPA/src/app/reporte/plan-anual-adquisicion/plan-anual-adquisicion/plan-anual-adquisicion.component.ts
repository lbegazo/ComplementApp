import { HttpClient, HttpEventType } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import {
  combineLatest,
  noop,
  Observable,
  Observer,
  of,
  Subscription,
} from 'rxjs';
import { map, switchMap, tap } from 'rxjs/Operators';
import { Cdp } from 'src/app/_models/cdp';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { RubroPresupuestal } from 'src/app/_models/rubroPresupuestal';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { PlanAdquisicionService } from 'src/app/_services/planAdquisicion.service';
import { environment } from 'src/environments/environment';
import { PopupDetallePlanAdquisicionComponent } from './popup-detalle-plan-adquisicion/popup-detalle-plan-adquisicion.component';

@Component({
  selector: 'app-plan-anual-adquisicion',
  templateUrl: './plan-anual-adquisicion.component.html',
  styleUrls: ['./plan-anual-adquisicion.component.scss'],
})
export class PlanAnualAdquisicionComponent implements OnInit {
  //#region Variables

  nombreTransaccion: string;
  transaccion: Transaccion;
  search: string;
  searchNombre: string;
  suggestions$: Observable<RubroPresupuestal[]>;
  suggestionsXNombre$: Observable<RubroPresupuestal[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  arrayControls = new FormArray([]);
  mostrarCabecera = true;
  nombreBoton = 'Ver Detalle';
  bsModalRef: BsModalRef;

  listaPlanAdquisicion: DetalleCDP[] = [];
  rubroPresupuestal: RubroPresupuestal;
  rubroPresupuestalId = 0;
  cdp: number;
  instancia: number;

  facturaHeaderForm = new FormGroup({});
  baseUrl = environment.apiUrl + 'lista/ObtenerListaRubroPresupuestal';
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
    private planAdquisicionService: PlanAdquisicionService,
    private router: Router,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef
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
      rubroPresupuestalCtrl: [''],
      rubroPresupuestalDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  cargarBusquedaTerceroXCodigo() {
    this.suggestions$ = new Observable((observer: Observer<string>) => {
      observer.next(this.search);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<RubroPresupuestal[]>(this.baseUrl, {
              params: { identificacion: query },
            })
            .pipe(
              map((data: RubroPresupuestal[]) => data || []),
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
            .get<RubroPresupuestal[]>(this.baseUrl, {
              params: { nombre: query },
            })
            .pipe(
              map((data: RubroPresupuestal[]) => data || []),
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
    this.rubroPresupuestal = e.item as RubroPresupuestal;
    if (this.rubroPresupuestal) {
      this.rubroPresupuestalId = this.rubroPresupuestal.rubroPresupuestalId;
    }
  }

  typeaheadOnSelectXNombre(e: TypeaheadMatch): void {
    this.rubroPresupuestal = e.item as RubroPresupuestal;
    if (this.rubroPresupuestal) {
      this.rubroPresupuestalId = this.rubroPresupuestal.rubroPresupuestalId;
    }
  }

  onBuscarFactura() {
    this.planAdquisicionService
      .ObtenerListaPlanAdquisicionReporte(
        this.rubroPresupuestalId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<DetalleCDP[]>) => {
          this.listaPlanAdquisicion = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.facturaHeaderForm = this.fb.group({
            rubroPresupuestalCtrl: [''],
            rubroPresupuestalDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaPlanAdquisicion && this.listaPlanAdquisicion.length > 0) {
      for (const detalle of this.listaPlanAdquisicion) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning('No existen planes de adquisición');
    }
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura() {
    this.listaPlanAdquisicion = [];
    this.rubroPresupuestalId = 0;
    this.rubroPresupuestal = null;
    this.search = '';
    this.searchNombre = '';
    this.nombreBoton = 'Registrar';

    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };

    this.onBuscarFactura();
  }

  exportarExcel() {
    let fileName = '';

    this.planAdquisicionService
      .DescargarListaPlanAnualAdquisicion(this.rubroPresupuestalId)
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
        () => {}
      );
  }

  abrirPopupGeneral(cdp: number, instancia: number) {
    this.cdp = cdp;
    this.instancia = instancia;
    this.abrirPopupDetallePlanAdquisicion();
  }

  abrirPopupDetallePlanAdquisicion() {
    //#region Abrir Popup

    const initialState = {
      title: 'Detalle Plan Anual de Adquisiciones',
      cdp: this.cdp,
      instancia: this.instancia,
    };

    this.bsModalRef = this.modalService.show(
      PopupDetallePlanAdquisicionComponent,
      Object.assign({ initialState }, { class: 'gray modal-xl' })
    );

    //#endregion Abrir Popup

    //#region Cargar información del popup (OnHidden event)

    const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        this.unsubscribe();
      })
    );

    this.subscriptions.push(combine);

    //#endregion Cargar información del popup (OnHidden event)
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
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
}
