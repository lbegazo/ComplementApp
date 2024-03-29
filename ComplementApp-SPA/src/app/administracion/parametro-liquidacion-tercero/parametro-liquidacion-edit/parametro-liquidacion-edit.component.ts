import { formatDate } from '@angular/common';
import { HttpClient, HttpEventType } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
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
import { DeduccionDto } from 'src/app/_dto/deduccionDto';
import { TerceroDeduccionDto } from 'src/app/_dto/terceroDeduccionDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { ActividadEconomica } from 'src/app/_models/actividadEconomica';
import { Deduccion } from 'src/app/_models/deduccion';
import {
  EstadoModificacion,
  ModalidadContrato,
  TipoLista,
} from 'src/app/_models/enum';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { Tercero } from 'src/app/_models/tercero';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';
import { ListaService } from 'src/app/_services/lista.service';
import { TerceroService } from 'src/app/_services/tercero.service';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { environment } from 'src/environments/environment';
import { PopupParametroLiquidacionTerceroComponent } from './popup-parametro-liquidacion-tercero/popup-parametro-liquidacion-tercero.component';

@Component({
  selector: 'app-parametro-liquidacion-edit',
  templateUrl: './parametro-liquidacion-edit.component.html',
  styleUrls: ['./parametro-liquidacion-edit.component.scss'],
})
export class ParametroLiquidacionEditComponent implements OnInit {
  //#region Inputs and outputs

  @Input() esCreacion: boolean;
  @Input() tercero: Tercero;
  @Input() parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  @Input() listaNotasLegales: ValorSeleccion[];
  @Output() esCancelado = new EventEmitter<boolean>();
  // @ViewChild('staticTabs', { static: false }) staticTabs: TabsetComponent;

  //#endregion Inputs and outputs

  //#region Variables

  searchCodigoDeduccion: string;
  searchNombreDeduccion: string;
  suggestionsCodigoDeduccion$: Observable<Deduccion[]>;
  suggestionsNombreDeduccion$: Observable<Deduccion[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  baseUrlDeduccion = environment.apiUrl + 'lista/ObtenerListaDeducciones';
  deduccion: Deduccion;
  deduccionId = 0;
  terceroDeduccionSeleccionadoId = 0;

  searchActividad: string;
  suggestionsActividad$: Observable<ActividadEconomica[]>;
  baseUrlActividad =
    environment.apiUrl + 'lista/ObtenerListaActividadesEconomicas';
  actividadEconomica: ActividadEconomica;
  actividadEconomicaId = 0;
  inicioId = 100000;

  arrayControls = new FormArray([]);

  listaModalidadContrato: ValorSeleccion[] = [];
  listaTipoPago: ValorSeleccion[] = [];
  listaTipoIva: ValorSeleccion[] = [];
  listaTipoCuentaXPagar: ValorSeleccion[] = [];
  listaTipoDocumentoSoporte: ValorSeleccion[] = [];
  listaFacturaElectronica: ValorSeleccion[] = [];
  listaSubcontrata: ValorSeleccion[] = [];
  listaAdminPila: ValorSeleccion[] = [];

  editForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  nombreBoton = 'Registrar';
  nombreBotonAgregar = 'Agregar';
  bsModalRef: BsModalRef;

  idModalidadContratoSelecionado?: number;
  modalidadContratoSeleccionado: ValorSeleccion = null;

  idTipoPagoSelecionado?: number;
  tipoPagoSeleccionado: ValorSeleccion = null;

  idTipoIvaSelecionado?: number;
  tipoIvaSeleccionado: ValorSeleccion = null;

  idTipoCuentaXPagarSelecionado?: number;
  tipoCuentaXPagarSeleccionado: ValorSeleccion = null;

  idTipoDocumentoSoporteSelecionado?: number;
  tipoDocumentoSoporteSeleccionado: ValorSeleccion = null;

  idFacturaElectronicaSeleccionado?: number;
  facturaElectronicaSeleccionado: ValorSeleccion = null;

  idSubcontrataSeleccionado?: number;
  subcontrataSeleccionado: ValorSeleccion = null;

  idTipoAdminPila?: number;
  tipoAdminPilaSeleccionado: ValorSeleccion = null;

  listaTerceroDeducciones: TerceroDeduccionDto[] = [];
  listaTerceroDeduccionesEliminadas: TerceroDeduccionDto[] = [];
  terceroDeduccionSeleccionado: TerceroDeduccionDto;
  listaParametrosGeneral: ValorSeleccion[] = [];

  habilitarBotonAgregar = false;
  accionAgregarDeduccion = true;

  notaLegal1Descripcion = '';
  notaLegal2Descripcion = '';
  notaLegal3Descripcion = '';
  notaLegal4Descripcion = '';
  notaLegal5Descripcion = '';
  notaLegal6Descripcion = '';

  //#endregion Variables

  constructor(
    private http: HttpClient,
    private listaService: ListaService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private terceroService: TerceroService,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    this.cargarListasResolver();

    this.cargarBusquedaDeducciones();

    this.cargarBusquedaDeduccionesXDescripcion();

    this.cargarBusquedaActividad();

    this.cargarInformacionNotasLegales();

    if (this.esCreacion) {
      this.cargarParametrosGenerales();
      this.nombreBoton = 'Registrar';
    } else {
      this.createFullForm();
      this.nombreBoton = 'Guardar';
    }
  }

  cargarInformacionNotasLegales() {
    if (this.listaNotasLegales && this.listaNotasLegales.length > 0) {
      this.notaLegal1Descripcion = this.listaNotasLegales[0].valor;
      this.notaLegal2Descripcion = this.listaNotasLegales[1].valor;
      this.notaLegal3Descripcion = this.listaNotasLegales[2].valor;
      this.notaLegal4Descripcion = this.listaNotasLegales[3].valor;
      this.notaLegal5Descripcion = this.listaNotasLegales[4].valor;
      this.notaLegal6Descripcion = this.listaNotasLegales[5].valor;
    }
  }

  cargarBusquedaDeducciones() {
    this.suggestionsCodigoDeduccion$ = new Observable(
      (observer: Observer<string>) => {
        observer.next(this.searchCodigoDeduccion);
      }
    ).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Deduccion[]>(this.baseUrlDeduccion, {
              params: { codigo: query },
            })
            .pipe(
              map((data: Deduccion[]) => data || []),
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

  cargarBusquedaDeduccionesXDescripcion() {
    this.suggestionsNombreDeduccion$ = new Observable(
      (observer: Observer<string>) => {
        observer.next(this.searchNombreDeduccion);
      }
    ).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Deduccion[]>(this.baseUrlDeduccion, {
              params: { nombre: query },
            })
            .pipe(
              map((data: Deduccion[]) => data || []),
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

  typeaheadOnSelectDeduccion(e: TypeaheadMatch): void {
    this.deduccion = e.item as Deduccion;
    if (this.deduccion) {
      this.deduccionId = this.deduccion.deduccionId;

      if (this.deduccionId > 0) {
        if (
          this.idModalidadContratoSelecionado !==
          ModalidadContrato.ProveedorSinDescuento.value
        ) {
          if (
            this.actividadEconomica &&
            this.actividadEconomica.actividadEconomicaId > 0
          ) {
            this.habilitarBotonAgregar = true;
            this.habilitarValorFijoControl(this.deduccion.esValorFijo);
          }
        }
      }
    }
  }

  typeaheadOnSelectDeduccionXNombre(e: TypeaheadMatch): void {
    this.deduccion = e.item as Deduccion;
    if (this.deduccion) {
      this.deduccionId = this.deduccion.deduccionId;

      if (this.deduccionId > 0) {
        if (
          this.idModalidadContratoSelecionado !==
          ModalidadContrato.ProveedorSinDescuento.value
        ) {
          if (
            this.actividadEconomica &&
            this.actividadEconomica.actividadEconomicaId > 0
          ) {
            this.habilitarBotonAgregar = true;
            this.habilitarValorFijoControl(this.deduccion.esValorFijo);
          }
        }
      }
    }
  }

  cargarBusquedaActividad() {
    this.suggestionsActividad$ = new Observable(
      (observer: Observer<string>) => {
        observer.next(this.searchActividad);
      }
    ).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<ActividadEconomica[]>(this.baseUrlActividad, {
              params: { codigo: query },
            })
            .pipe(
              map((data: ActividadEconomica[]) => data || []),
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

  typeaheadOnSelectActividad(e: TypeaheadMatch): void {
    this.actividadEconomica = e.item as ActividadEconomica;
    if (this.actividadEconomica) {
      this.actividadEconomicaId = this.actividadEconomica.actividadEconomicaId;

      if (this.actividadEconomicaId > 0) {
        this.habilitarBotonAgregar = true;
      }
    }
  }

  createEmptyForm() {
    this.editForm = this.fb.group({
      modalidadContratoCtrl: [null, Validators.required],
      tipoPagoCtrl: [null, Validators.required],
      honorarioSinIvaCtrl: ['', Validators.required],
      tarifaIvaCtrl: ['', Validators.required],
      tipoIvaCtrl: [null, Validators.required],
      facturaElectronicaCtrl: [null, Validators.required],
      subcontrataCtrl: [null, Validators.required],
      adminPilaCtrl: [null, Validators.required],
      tipoCuentaXPagarCtrl: [null, Validators.required],
      tipoDocumentoSoporteCtrl: [null, Validators.required],

      baseAporteSaludCtrl: ['', Validators.required],
      aporteSaludCtrl: ['', Validators.required],
      aportePensionCtrl: ['', Validators.required],
      riesgoLaboralCtrl: ['', Validators.required],
      fondoSolidaridadCtrl: ['', Validators.required],

      pensionVoluntariaCtrl: ['', Validators.required],
      dependienteCtrl: ['', Validators.required],
      afcCtrl: ['', Validators.required],
      medicinaPrepagadaCtrl: ['', Validators.required],

      interesesViviendaCtrl: ['', Validators.required],
      fechaInicioCtrl: [null, Validators.required],
      fechaFinalCtrl: [null, Validators.required],

      otrosDescuentosCtrl: [''],
      fecIniOtrosDescuentosCtrl: [null],
      fecFinOtrosDescuentosCtrl: [null],

      notaLegal1Ctrl: [''],
      notaLegal2Ctrl: [''],
      notaLegal3Ctrl: [''],
      notaLegal4Ctrl: [''],
      notaLegal5Ctrl: [''],
      notaLegal6Ctrl: [''],

      codigoDeduccionCtrl: [''],
      deduccionCtrl: [''],
      valorFijoCtrl: [''],
      codigoActividadCtrl: [''],
      actividadCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  createDefaultForm() {
    let baseAporteSaludC = 0;
    let aporteSaludC = 0;
    let aportePensionC = 0;
    let riesgoLaboralC = 0;
    let tarifaIvaC = 0;
    let dependientesC = 0;

    if (this.listaParametrosGeneral && this.listaParametrosGeneral.length > 0) {
      baseAporteSaludC = +this.listaParametrosGeneral[0].valor;
      aporteSaludC = +this.listaParametrosGeneral[1].valor;
      aportePensionC = +this.listaParametrosGeneral[2].valor;
      riesgoLaboralC = +this.listaParametrosGeneral[3].valor;
      tarifaIvaC = +this.listaParametrosGeneral[4].valor;
      dependientesC = +this.listaParametrosGeneral[5].valor;

      this.editForm.patchValue({
        tarifaIvaCtrl: GeneralService.obtenerFormatoLongMoney(tarifaIvaC),
        baseAporteSaludCtrl:
          GeneralService.obtenerFormatoLongMoney(baseAporteSaludC),
        aporteSaludCtrl: GeneralService.obtenerFormatoLongMoney(aporteSaludC),
        aportePensionCtrl:
          GeneralService.obtenerFormatoLongMoney(aportePensionC),
        riesgoLaboralCtrl:
          GeneralService.obtenerFormatoLongMoney(riesgoLaboralC),
        dependienteCtrl: GeneralService.obtenerFormatoLongMoney(dependientesC),
      });
    }
  }

  createFullForm() {
    let honorarioSinIvaC = 0;
    let tarifaIvaC = 0;

    let baseAporteSaludC = 0;
    let aporteSaludC = 0;
    let aportePensionC = 0;
    let riesgoLaboralC = 0;
    let fondoSolidaridadC = 0;

    let pensionVoluntariaC = 0;
    let dependienteC = 0;
    let afcC = 0;
    let medicinaPrepagadaC = 0;
    let interesesViviendaC = 0;

    let otrosDescuentosC = 0;
    let fechaInicioOtrosDescuentos = null;
    let fechaFinalOtrosDescuentos = null;

    let fechaInicio = null;
    let fechaFinal = null;

    let notaLegal1 = false;
    let notaLegal2 = false;
    let notaLegal3 = false;
    let notaLegal4 = false;
    let notaLegal5 = false;
    let notaLegal6 = false;

    this.idModalidadContratoSelecionado =
      this.parametroLiquidacionSeleccionado.modalidadContrato > 0
        ? this.parametroLiquidacionSeleccionado.modalidadContrato
        : null;
    this.modalidadContratoSeleccionado = this.listaModalidadContrato.filter(
      (x) => x.id === this.idModalidadContratoSelecionado
    )[0];

    this.idTipoPagoSelecionado =
      this.parametroLiquidacionSeleccionado.tipoPago > 0
        ? this.parametroLiquidacionSeleccionado.tipoPago
        : null;
    if (this.idTipoPagoSelecionado !== null) {
      this.tipoPagoSeleccionado = this.listaTipoPago.filter(
        (x) => x.id === this.idTipoPagoSelecionado
      )[0];
    }

    this.idTipoIvaSelecionado =
      this.parametroLiquidacionSeleccionado.tipoIva > 0
        ? this.parametroLiquidacionSeleccionado.tipoIva
        : null;
    if (this.idTipoIvaSelecionado !== null) {
      this.tipoIvaSeleccionado = this.listaTipoIva.filter(
        (x) => x.id === this.idTipoIvaSelecionado
      )[0];
    }

    this.idFacturaElectronicaSeleccionado =
      this.parametroLiquidacionSeleccionado.facturaElectronicaId;
    if (this.idFacturaElectronicaSeleccionado !== null) {
      this.facturaElectronicaSeleccionado = this.listaFacturaElectronica.filter(
        (x) => x.id === this.idFacturaElectronicaSeleccionado
      )[0];
    }

    this.idSubcontrataSeleccionado =
      this.parametroLiquidacionSeleccionado.subcontrataId;
    if (this.idSubcontrataSeleccionado !== null) {
      this.subcontrataSeleccionado = this.listaSubcontrata.filter(
        (x) => x.id === this.idSubcontrataSeleccionado
      )[0];
    }

    this.idTipoAdminPila =
      this.parametroLiquidacionSeleccionado.tipoAdminPilaId;
    if (this.idTipoAdminPila !== null) {
      this.tipoAdminPilaSeleccionado = this.listaAdminPila.filter(
        (x) => x.id === this.idTipoAdminPila
      )[0];
    }

    this.idTipoCuentaXPagarSelecionado =
      this.parametroLiquidacionSeleccionado.tipoCuentaXPagarId > 0
        ? this.parametroLiquidacionSeleccionado.tipoCuentaXPagarId
        : null;
    if (this.idTipoCuentaXPagarSelecionado !== null) {
      this.tipoCuentaXPagarSeleccionado = this.listaTipoCuentaXPagar.filter(
        (x) => x.id === this.idTipoCuentaXPagarSelecionado
      )[0];
    }

    this.idTipoDocumentoSoporteSelecionado =
      this.parametroLiquidacionSeleccionado.tipoDocumentoSoporteId > 0
        ? this.parametroLiquidacionSeleccionado.tipoDocumentoSoporteId
        : null;
    if (this.idTipoDocumentoSoporteSelecionado !== null) {
      this.tipoDocumentoSoporteSeleccionado =
        this.listaTipoDocumentoSoporte.filter(
          (x) => x.id === this.idTipoDocumentoSoporteSelecionado
        )[0];
    }

    honorarioSinIvaC = this.parametroLiquidacionSeleccionado?.honorarioSinIva;
    tarifaIvaC = this.parametroLiquidacionSeleccionado?.tarifaIva;

    baseAporteSaludC = this.parametroLiquidacionSeleccionado?.baseAporteSalud;
    aporteSaludC = this.parametroLiquidacionSeleccionado?.aporteSalud;
    aportePensionC = this.parametroLiquidacionSeleccionado?.aportePension;
    riesgoLaboralC = this.parametroLiquidacionSeleccionado?.riesgoLaboral;
    fondoSolidaridadC = this.parametroLiquidacionSeleccionado?.fondoSolidaridad;

    pensionVoluntariaC =
      this.parametroLiquidacionSeleccionado?.pensionVoluntaria;
    dependienteC = this.parametroLiquidacionSeleccionado?.dependiente;
    afcC = this.parametroLiquidacionSeleccionado?.afc;
    medicinaPrepagadaC =
      this.parametroLiquidacionSeleccionado?.medicinaPrepagada;

    otrosDescuentosC = this.parametroLiquidacionSeleccionado?.otrosDescuentos;
    fechaInicioOtrosDescuentos =
      this.parametroLiquidacionSeleccionado.fechaInicioOtrosDescuentos;
    fechaFinalOtrosDescuentos =
      this.parametroLiquidacionSeleccionado.fechaFinalOtrosDescuentos;

    interesesViviendaC = this.parametroLiquidacionSeleccionado?.interesVivienda;
    fechaInicio =
      this.parametroLiquidacionSeleccionado.fechaInicioDescuentoInteresVivienda;
    fechaFinal =
      this.parametroLiquidacionSeleccionado.fechaFinalDescuentoInteresVivienda;

    notaLegal1 = this.parametroLiquidacionSeleccionado.notaLegal1;
    notaLegal2 = this.parametroLiquidacionSeleccionado.notaLegal2;
    notaLegal3 = this.parametroLiquidacionSeleccionado.notaLegal3;
    notaLegal4 = this.parametroLiquidacionSeleccionado.notaLegal4;
    notaLegal5 = this.parametroLiquidacionSeleccionado.notaLegal5;
    notaLegal6 = this.parametroLiquidacionSeleccionado.notaLegal6;

    //#region Deducciones

    if (
      this.parametroLiquidacionSeleccionado.terceroDeducciones &&
      this.parametroLiquidacionSeleccionado.terceroDeducciones.length
    ) {
      this.listaTerceroDeducciones.push(
        ...this.parametroLiquidacionSeleccionado.terceroDeducciones
      );

      this.listaTerceroDeducciones.forEach((x) => {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      });
    }

    //#endregion Deducciones

    this.editForm.patchValue({
      modalidadContratoCtrl: this.modalidadContratoSeleccionado,
      tipoPagoCtrl: this.tipoPagoSeleccionado,
      honorarioSinIvaCtrl: GeneralService.obtenerFormatoMoney(honorarioSinIvaC),
      tarifaIvaCtrl: GeneralService.obtenerFormatoLongMoney(tarifaIvaC),
      tipoIvaCtrl: this.tipoIvaSeleccionado,
      tipoCuentaXPagarCtrl: this.tipoCuentaXPagarSeleccionado,
      tipoDocumentoSoporteCtrl: this.tipoDocumentoSoporteSeleccionado,
      facturaElectronicaCtrl: this.facturaElectronicaSeleccionado,
      subcontrataCtrl: this.subcontrataSeleccionado,
      adminPilaCtrl: this.tipoAdminPilaSeleccionado,

      baseAporteSaludCtrl:
        GeneralService.obtenerFormatoLongMoney(baseAporteSaludC),
      aporteSaludCtrl: GeneralService.obtenerFormatoLongMoney(aporteSaludC),
      aportePensionCtrl: GeneralService.obtenerFormatoLongMoney(aportePensionC),
      riesgoLaboralCtrl: GeneralService.obtenerFormatoLongMoney(riesgoLaboralC),
      fondoSolidaridadCtrl:
        GeneralService.obtenerFormatoMoney(fondoSolidaridadC),

      pensionVoluntariaCtrl:
        GeneralService.obtenerFormatoMoney(pensionVoluntariaC),
      dependienteCtrl: GeneralService.obtenerFormatoLongMoney(dependienteC),
      afcCtrl: GeneralService.obtenerFormatoMoney(afcC),
      medicinaPrepagadaCtrl:
        GeneralService.obtenerFormatoMoney(medicinaPrepagadaC),

      interesesViviendaCtrl:
        GeneralService.obtenerFormatoMoney(interesesViviendaC),
      fechaInicioCtrl: formatDate(fechaInicio, 'dd-MM-yyyy', 'en'),
      fechaFinalCtrl: formatDate(fechaFinal, 'dd-MM-yyyy', 'en'),

      otrosDescuentosCtrl: GeneralService.obtenerFormatoMoney(otrosDescuentosC),
      fecIniOtrosDescuentosCtrl:
        fechaInicioOtrosDescuentos !== null
          ? formatDate(fechaInicioOtrosDescuentos, 'dd-MM-yyyy', 'en')
          : '',
      fecFinOtrosDescuentosCtrl:
        fechaFinalOtrosDescuentos !== null
          ? formatDate(fechaFinalOtrosDescuentos, 'dd-MM-yyyy', 'en')
          : '',

      notaLegal1Ctrl: notaLegal1,
      notaLegal2Ctrl: notaLegal2,
      notaLegal3Ctrl: notaLegal3,
      notaLegal4Ctrl: notaLegal4,
      notaLegal5Ctrl: notaLegal5,
      notaLegal6Ctrl: notaLegal6,

      codigoDeduccionCtrl: '',
      deduccionCtrl: '',
      codigoActividadCtrl: '',
      actividadCtrl: '',
    });
    this.editForm.setControl('planPagoControles', this.arrayControls);

    this.ocultarControlesFormulario();
  }

  onModalidadContrato() {
    this.modalidadContratoSeleccionado = this.modalidadContratoCtrl
      .value as ValorSeleccion;
    this.idModalidadContratoSelecionado =
      +this.modalidadContratoSeleccionado.id;

    this.ocultarControlesFormulario();
  }

  ocultarControlesFormulario() {
    if (
      this.idModalidadContratoSelecionado ===
      ModalidadContrato.ContratoPrestacionServicio.value
    ) {
      this.honorarioSinIvaCtrl.enable();
      this.tarifaIvaCtrl.enable();
      this.baseAporteSaludCtrl.enable();
      this.aporteSaludCtrl.enable();
      this.aportePensionCtrl.enable();
      this.riesgoLaboralCtrl.enable();
      this.fondoSolidaridadCtrl.enable();
      this.subcontrataCtrl.enable();

      this.pensionVoluntariaCtrl.enable();
      this.dependienteCtrl.enable();
      this.afcCtrl.enable();
      this.medicinaPrepagadaCtrl.enable();
      this.interesesViviendaCtrl.enable();
      this.fechaInicioCtrl.enable();
      this.fechaFinalCtrl.enable();
      this.adminPilaCtrl.enable();

      this.tipoPagoCtrl.disable();
      this.tipoIvaCtrl.disable();
    }

    if (
      this.idModalidadContratoSelecionado ===
        ModalidadContrato.ProveedorConDescuento.value ||
      this.idModalidadContratoSelecionado ===
        ModalidadContrato.ProveedorSinDescuento.value
    ) {
      if (
        this.idModalidadContratoSelecionado ===
        ModalidadContrato.ProveedorConDescuento.value
      ) {
        this.honorarioSinIvaCtrl.disable();
      }

      this.baseAporteSaludCtrl.disable();
      this.aporteSaludCtrl.disable();
      this.aportePensionCtrl.disable();
      this.riesgoLaboralCtrl.disable();
      this.fondoSolidaridadCtrl.disable();
      this.subcontrataCtrl.disable();
      this.adminPilaCtrl.disable();

      this.pensionVoluntariaCtrl.disable();
      this.dependienteCtrl.disable();
      this.afcCtrl.disable();
      this.medicinaPrepagadaCtrl.disable();
      this.interesesViviendaCtrl.disable();
      this.fechaInicioCtrl.disable();
      this.fechaFinalCtrl.disable();
      this.tipoPagoCtrl.enable();
      this.tipoIvaCtrl.enable();
    }

    // if (
    //   this.idModalidadContratoSelecionado ===
    //   ModalidadContrato.ProveedorSinDescuento.value
    // ) {
    //   this.staticTabs.tabs[1].disabled = true;
    // } else {
    //   this.staticTabs.tabs[1].disabled = false;
    // }
  }

  onTipoPago() {
    this.tipoPagoSeleccionado = this.tipoPagoCtrl.value as ValorSeleccion;
    this.idTipoPagoSelecionado = +this.tipoPagoSeleccionado.id;
  }

  onTipoCuentaXPagar() {
    this.tipoCuentaXPagarSeleccionado = this.tipoCuentaXPagarCtrl
      .value as ValorSeleccion;
    this.idTipoCuentaXPagarSelecionado = +this.tipoCuentaXPagarSeleccionado.id;
  }

  onTipoIva() {
    this.tipoIvaSeleccionado = this.tipoIvaCtrl.value as ValorSeleccion;
    this.idTipoIvaSelecionado = +this.tipoIvaSeleccionado.id;
  }

  onTipoDocumentoSoporte() {
    this.tipoDocumentoSoporteSeleccionado = this.tipoDocumentoSoporteCtrl
      .value as ValorSeleccion;
    this.idTipoDocumentoSoporteSelecionado =
      +this.tipoDocumentoSoporteSeleccionado.id;
  }

  onFacturaElectronica() {
    this.facturaElectronicaSeleccionado = this.facturaElectronicaCtrl
      .value as ValorSeleccion;
    this.idFacturaElectronicaSeleccionado =
      +this.facturaElectronicaSeleccionado.id;
  }

  onSubcontrata() {
    this.subcontrataSeleccionado = this.subcontrataCtrl.value as ValorSeleccion;
    this.idSubcontrataSeleccionado = +this.subcontrataSeleccionado.id;
  }

  onAdminPila() {
    this.tipoAdminPilaSeleccionado = this.adminPilaCtrl.value as ValorSeleccion;
    this.idTipoAdminPila = +this.tipoAdminPilaSeleccionado.id;
  }

  onAgregarDeduccion() {
    if (this.accionAgregarDeduccion) {
      //#region Agregar Deducción Actividad Economica
      if (this.deduccion === undefined || this.deduccion === null) {
        //#region Solo Actividad Economica

        if (this.actividadEconomica && this.actividadEconomicaId > 0) {
          const filtro = this.listaTerceroDeducciones.filter(
            (x) =>
              x.actividadEconomica.id === this.actividadEconomicaId &&
              x.deduccion.deduccionId === 0
          )[0];

          if (filtro) {
            this.alertify.warning('La actividad económica ya fue agregada');
            return;
          }

          const actividadT = new ValorSeleccion();
          actividadT.id = this.actividadEconomicaId;
          actividadT.codigo = this.actividadEconomica.codigo;
          actividadT.nombre = this.actividadEconomica.nombre;

          const deduccionT = new DeduccionDto();
          deduccionT.deduccionId = 0;
          const terceroDeDeduccionT = new ValorSeleccion();
          const terceroT = new ValorSeleccion();
          terceroT.id = this.tercero.terceroId;
          this.inicioId = this.inicioId + 1;

          const terceroDeduccion: TerceroDeduccionDto = {
            terceroDeduccionId: this.inicioId,
            deduccion: deduccionT,
            actividadEconomica: actividadT,
            tercero: terceroT,
            terceroDeDeduccion: terceroDeDeduccionT,
            tipoIdentificacion: 0,
            identificacionTercero: '',
            codigo: '',
            estadoModificacion: EstadoModificacion.Insertado,
            valorFijo: 0,
          };

          this.listaTerceroDeducciones.push(terceroDeduccion);

          this.arrayControls.push(
            new FormGroup({
              rubroControl: new FormControl(''),
            })
          );
        }

        //#endregion Solo Actividad Economica
      } else {
        if (
          this.deduccion &&
          this.deduccion.deduccionId > 0 &&
          this.actividadEconomica &&
          this.actividadEconomicaId > 0
        ) {
          //#region Descuento y Actividad Economica

          const formValues = Object.assign({}, this.editForm.value);
          let valorFijoDeduccion = 0;

          const filtro = this.listaTerceroDeducciones.filter(
            (x) =>
              x.deduccion.deduccionId === this.deduccion.deduccionId &&
              x.actividadEconomica.id === this.actividadEconomicaId
          )[0];

          if (filtro) {
            this.alertify.warning(
              'Ya existe la combinación deducción-actividad económica'
            );
            return;
          }

          const actividadT = new ValorSeleccion();
          actividadT.id = this.actividadEconomicaId;
          actividadT.codigo = this.actividadEconomica.codigo;
          actividadT.nombre = this.actividadEconomica.nombre;

          const deduccionT = new DeduccionDto();
          deduccionT.deduccionId = this.deduccion.deduccionId;
          deduccionT.codigo = this.deduccion.codigo;
          deduccionT.nombre = this.deduccion.nombre;
          deduccionT.tarifa = this.deduccion.tarifa;
          deduccionT.esValorFijo = this.deduccion.esValorFijo;

          if (this.deduccion.esValorFijo) {
            const valorFijo =
              formValues.valorFijoCtrl === '' ? 0 : formValues.valorFijoCtrl;

            if (valorFijo === 0 || valorFijo === '0') {
              this.alertify.warning('Debe registrar el valor fijo');
              return;
            }

            valorFijoDeduccion = GeneralService.obtenerValorAbsoluto(valorFijo);
          }

          this.inicioId = this.inicioId + 1;

          const terceroDeDeduccionT = new ValorSeleccion();
          if (this.deduccion.tercero && this.deduccion.tercero.terceroId > 0) {
            terceroDeDeduccionT.id = this.deduccion.tercero.terceroId;
            terceroDeDeduccionT.codigo =
              this.deduccion.tercero.numeroIdentificacion;
            terceroDeDeduccionT.nombre = this.deduccion.tercero.nombre;
            terceroDeDeduccionT.valor = 'SI';
          } else {
            terceroDeDeduccionT.id = 0;
            terceroDeDeduccionT.codigo = '';
            terceroDeDeduccionT.nombre = '';
            terceroDeDeduccionT.valor = '';
          }

          const terceroT = new ValorSeleccion();
          terceroT.id = this.tercero.terceroId;

          const terceroDeduccion: TerceroDeduccionDto = {
            terceroDeduccionId: this.inicioId,
            deduccion: deduccionT,
            actividadEconomica: actividadT,
            tercero: terceroT,
            terceroDeDeduccion: terceroDeDeduccionT,
            tipoIdentificacion: 0,
            identificacionTercero: '',
            codigo: '',
            estadoModificacion: EstadoModificacion.Insertado,
            valorFijo: valorFijoDeduccion,
          };

          this.listaTerceroDeducciones.push(terceroDeduccion);

          this.arrayControls.push(
            new FormGroup({
              rubroControl: new FormControl(''),
            })
          );

          //#endregion Descuento y Actividad Economica
        }
      }

      //#endregion Agregar Deducción Actividad Economica
    } else {
      //#region Moficar Deducción Actividad Economica

      if (
        this.terceroDeduccionSeleccionado &&
        this.terceroDeduccionSeleccionadoId > 0
      ) {
        if (
          this.idModalidadContratoSelecionado ===
          ModalidadContrato.ProveedorSinDescuento.value
        ) {
          //#region ProveedorSinDescuento

          if (this.actividadEconomicaId > 0) {
            const filtro = this.listaTerceroDeducciones.filter(
              (x) =>
                x.actividadEconomica.id === this.actividadEconomicaId &&
                x.terceroDeduccionId !== this.terceroDeduccionSeleccionadoId
            )[0];

            if (filtro) {
              this.alertify.warning('La actividad económica ya fue agregada');
              return;
            }

            const actividadT = new ValorSeleccion();
            actividadT.id = this.actividadEconomicaId;
            actividadT.codigo = this.actividadEconomica.codigo;
            actividadT.nombre = this.actividadEconomica.nombre;

            const deduccionT = new DeduccionDto();
            const terceroDeDeduccionT = new ValorSeleccion();
            const terceroT = new ValorSeleccion();
            terceroT.id = this.tercero.terceroId;

            this.terceroDeduccionSeleccionado.actividadEconomica = actividadT;

            if (
              this.terceroDeduccionSeleccionado.terceroDeduccionId <
              this.inicioId
            ) {
              this.terceroDeduccionSeleccionado.estadoModificacion =
                EstadoModificacion.Modificado;
            }
          }

          //#endregion ProveedorSinDescuento
        } else {
          if (this.deduccionId > 0 && this.actividadEconomicaId > 0) {
            //#region Descuento y Actividad Economica

            const formValues = Object.assign({}, this.editForm.value);

            const filtro = this.listaTerceroDeducciones.filter(
              (x) =>
                x.deduccion.deduccionId === this.deduccionId &&
                x.actividadEconomica.id === this.actividadEconomicaId &&
                x.terceroDeduccionId !== this.terceroDeduccionSeleccionadoId
            )[0];

            if (filtro) {
              this.alertify.warning(
                'Ya existe la combinación deducción-actividad económica'
              );
              return;
            }

            if (this.actividadEconomica) {
              const actividadT = new ValorSeleccion();
              actividadT.id = this.actividadEconomicaId;
              actividadT.codigo = this.actividadEconomica.codigo;
              actividadT.nombre = this.actividadEconomica.nombre;

              this.terceroDeduccionSeleccionado.actividadEconomica = actividadT;
            }

            if (this.deduccion) {
              //#region Selecciono deducción
              const deduccionT = new DeduccionDto();
              deduccionT.deduccionId = this.deduccionId;
              deduccionT.codigo = this.deduccion.codigo;
              deduccionT.nombre = this.deduccion.nombre;
              deduccionT.tarifa = this.deduccion.tarifa;
              deduccionT.esValorFijo = this.deduccion.esValorFijo;

              this.terceroDeduccionSeleccionado.deduccion = deduccionT;

              const terceroDeDeduccionT = new ValorSeleccion();
              if (
                this.deduccion.tercero &&
                this.deduccion.tercero.terceroId > 0
              ) {
                terceroDeDeduccionT.id = this.deduccion.tercero.terceroId;
                terceroDeDeduccionT.codigo =
                  this.deduccion.tercero.numeroIdentificacion;
                terceroDeDeduccionT.nombre = this.deduccion.tercero.nombre;
                terceroDeDeduccionT.valor = 'SI';
              } else {
                terceroDeDeduccionT.id = 0;
                terceroDeDeduccionT.codigo = '';
                terceroDeDeduccionT.nombre = '';
                terceroDeDeduccionT.valor = '';
              }

              const terceroT = new ValorSeleccion();
              terceroT.id = this.tercero.terceroId;
              this.terceroDeduccionSeleccionado.terceroDeDeduccion =
                terceroDeDeduccionT;

              //#endregion Selecciono deducción
            }

            if (this.terceroDeduccionSeleccionado.deduccion.esValorFijo) {
              let valorFijo =
                formValues.valorFijoCtrl === '' ? 0 : formValues.valorFijoCtrl;

              if (valorFijo === 0 || valorFijo === '0') {
                this.alertify.warning('Debe registrar el valor fijo');
                return;
              }

              valorFijo = GeneralService.obtenerValorAbsoluto(valorFijo);
              this.terceroDeduccionSeleccionado.valorFijo = valorFijo;
            }

            if (
              this.terceroDeduccionSeleccionado.terceroDeduccionId <
              this.inicioId
            ) {
              this.terceroDeduccionSeleccionado.estadoModificacion =
                EstadoModificacion.Modificado;
            }

            //#endregion Descuento y Actividad Economica
          }
        }
      }

      //#endregion Moficar Deducción Actividad Economica
    }

    this.onLimpiarDeduccion();
  }

  onEliminarDeduccion() {
    if (
      this.terceroDeduccionSeleccionado &&
      this.terceroDeduccionSeleccionadoId > 0
    ) {
      this.alertify.confirm2(
        'Deducciones del tercero',
        '¿Esta seguro que desea eliminar la deducción seleccionada?',
        () => {
          let index = 0;
          let i = 0;
          this.listaTerceroDeducciones.forEach((item: TerceroDeduccionDto) => {
            if (
              item.terceroDeduccionId === this.terceroDeduccionSeleccionadoId
            ) {
              index = i;
            }
            i++;
          });

          if (index !== -1) {
            this.listaTerceroDeducciones.splice(index, 1);
          }

          this.terceroDeduccionSeleccionado.estadoModificacion =
            EstadoModificacion.Eliminado;
          this.listaTerceroDeduccionesEliminadas.push(
            this.terceroDeduccionSeleccionado
          );

          this.onLimpiarDeduccion();
        }
      );
    } else {
      this.alertify.warning('Debe seleccionar una deducción');
    }
  }

  onLimpiarDeduccion() {
    this.terceroDeduccionSeleccionado = null;
    this.terceroDeduccionSeleccionadoId = 0;
    this.deduccionId = 0;
    this.deduccion = null;
    this.actividadEconomicaId = 0;
    this.actividadEconomica = null;
    this.searchActividad = '';
    this.searchCodigoDeduccion = '';
    this.searchNombreDeduccion = '';
    this.habilitarBotonAgregar = false;
    this.nombreBotonAgregar = 'Agregar';
    this.accionAgregarDeduccion = true;
    this.actividadCtrl.patchValue('');
    this.valorFijoCtrl.patchValue('');
  }

  onLimpiarForm() {
    this.idModalidadContratoSelecionado = 0;
    this.modalidadContratoSeleccionado = null;
    this.idTipoCuentaXPagarSelecionado = 0;
    this.tipoCuentaXPagarSeleccionado = null;
    this.idTipoDocumentoSoporteSelecionado = 0;
    this.tipoCuentaXPagarSeleccionado = null;
    this.idTipoIvaSelecionado = 0;
    this.tipoIvaSeleccionado = null;
    this.idTipoPagoSelecionado = 0;
    this.tipoPagoSeleccionado = null;

    this.onLimpiarDeduccion();

    this.editForm.reset();
  }

  onGuardar() {
    if (
      this.idModalidadContratoSelecionado ===
        ModalidadContrato.ContratoPrestacionServicio.value ||
      this.idModalidadContratoSelecionado ===
        ModalidadContrato.ProveedorConDescuento.value
    ) {
      if (this.listaTerceroDeducciones.length === 0) {
        this.alertify.warning('Debe ingresar al menos una deducción');
        return;
      } else {
        if (!this.ValidarTerceroDeDeduccion()) {
          this.alertify.warning(
            'Debe vincularse un tercero a las posiciones de deducción seleccionadas'
          );
          return;
        }
      }
    }

    if (this.listaTerceroDeduccionesEliminadas.length > 0) {
      this.listaTerceroDeducciones.push(
        ...this.listaTerceroDeduccionesEliminadas
      );
    }

    if (this.editForm.valid) {
      const formValues = Object.assign({}, this.editForm.value);

      //#region Read dates

      let dateFechaInicio = null;
      let dateFechaFinal = null;
      const valueFechaInicio = this.editForm.get('fechaInicioCtrl').value;
      const valueFechaFinal = this.editForm.get('fechaFinalCtrl').value;

      if (GeneralService.isValidDate(valueFechaInicio)) {
        dateFechaInicio = valueFechaInicio;
      } else {
        if (valueFechaInicio && valueFechaInicio.indexOf('-') > -1) {
          dateFechaInicio = GeneralService.dateString2Date(valueFechaInicio);
        }
      }

      if (GeneralService.isValidDate(valueFechaFinal)) {
        dateFechaFinal = valueFechaFinal;
      } else {
        if (valueFechaFinal && valueFechaFinal.indexOf('-') > -1) {
          dateFechaFinal = GeneralService.dateString2Date(valueFechaFinal);
        }
      }

      let dateFechaInicioOtrosDescuentos = null;
      let dateFechaFinalOtrosDescuentos = null;

      const valueFechaInicioOtrosDescuentos = this.editForm.get(
        'fecIniOtrosDescuentosCtrl'
      ).value;
      const valueFechaFinalOtrosDescuentos = this.editForm.get(
        'fecFinOtrosDescuentosCtrl'
      ).value;

      if (GeneralService.isValidDate(valueFechaInicioOtrosDescuentos)) {
        dateFechaInicioOtrosDescuentos = valueFechaInicioOtrosDescuentos;
      } else {
        if (
          valueFechaInicioOtrosDescuentos &&
          valueFechaInicioOtrosDescuentos.indexOf('-') > -1
        ) {
          dateFechaInicioOtrosDescuentos = GeneralService.dateString2Date(
            valueFechaInicioOtrosDescuentos
          );
        }
      }

      if (GeneralService.isValidDate(valueFechaFinalOtrosDescuentos)) {
        dateFechaFinalOtrosDescuentos = valueFechaFinalOtrosDescuentos;
      } else {
        if (
          valueFechaFinalOtrosDescuentos &&
          valueFechaFinalOtrosDescuentos.indexOf('-') > -1
        ) {
          dateFechaFinalOtrosDescuentos = GeneralService.dateString2Date(
            valueFechaFinalOtrosDescuentos
          );
        }
      }

      //#endregion Read dates

      if (this.esCreacion) {
        const parametroNuevo: ParametroLiquidacionTercero = {
          parametroLiquidacionTerceroId: 0,
          modalidadContrato: this.idModalidadContratoSelecionado,
          tipoCuentaXPagarId: this.idTipoCuentaXPagarSelecionado,
          tipoDocumentoSoporteId: this.idTipoDocumentoSoporteSelecionado,
          tipoAdminPilaId: this.idTipoAdminPila,
          tipoIva:
            this.idTipoIvaSelecionado !== null ? this.idTipoIvaSelecionado : 0,
          tipoPago:
            this.idTipoPagoSelecionado !== null
              ? this.idTipoPagoSelecionado
              : 0,
          honorarioSinIva: GeneralService.obtenerValorAbsoluto(
            formValues.honorarioSinIvaCtrl
          ),
          tarifaIva: GeneralService.obtenerValorAbsoluto(
            formValues.tarifaIvaCtrl
          ),
          facturaElectronicaId: this.idFacturaElectronicaSeleccionado,
          subcontrataId: this.idSubcontrataSeleccionado,

          baseAporteSalud:
            formValues.baseAporteSaludCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.baseAporteSaludCtrl
                ),
          aporteSalud:
            formValues.aporteSaludCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(formValues.aporteSaludCtrl),
          aportePension:
            formValues.aportePensionCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.aportePensionCtrl
                ),
          riesgoLaboral:
            formValues.riesgoLaboralCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.riesgoLaboralCtrl
                ),
          fondoSolidaridad:
            formValues.fondoSolidaridadCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.fondoSolidaridadCtrl
                ),

          pensionVoluntaria:
            formValues.pensionVoluntariaCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.pensionVoluntariaCtrl
                ),
          dependiente:
            formValues.dependienteCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(formValues.dependienteCtrl),
          afc:
            formValues.afcCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(formValues.afcCtrl),
          medicinaPrepagada:
            formValues.medicinaPrepagadaCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.medicinaPrepagadaCtrl
                ),
          interesVivienda:
            formValues.interesesViviendaCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.interesesViviendaCtrl
                ),
          fechaInicioDescuentoInteresVivienda: dateFechaInicio,
          fechaFinalDescuentoInteresVivienda: dateFechaFinal,

          notaLegal1: formValues.notaLegal1Ctrl === '' ? false : true,
          notaLegal2: formValues.notaLegal2Ctrl === '' ? false : true,
          notaLegal3: formValues.notaLegal3Ctrl === '' ? false : true,
          notaLegal4: formValues.notaLegal4Ctrl === '' ? false : true,
          notaLegal5: formValues.notaLegal5Ctrl === '' ? false : true,
          notaLegal6: formValues.notaLegal6Ctrl === '' ? false : true,

          otrosDescuentos:
            formValues.otrosDescuentosCtrl === undefined
              ? 0
              : GeneralService.obtenerValorAbsoluto(
                  formValues.otrosDescuentosCtrl
                ),
          fechaInicioOtrosDescuentos: dateFechaInicioOtrosDescuentos,
          fechaFinalOtrosDescuentos: dateFechaFinalOtrosDescuentos,

          debito: '',
          credito: '',
          numeroCuenta: '',
          tipoCuenta: '',
          convenioFontic: 0,
          terceroId: this.tercero.terceroId,

          terceroDeducciones: this.listaTerceroDeducciones,
        };
        this.terceroService
          .RegistrarParametroLiquidacionTercero(parametroNuevo)
          .subscribe(
            () => {
              this.editForm.reset(parametroNuevo);
              this.alertify.success(
                'La parametrización de liquidación se registró correctamente'
              );
            },

            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.esCancelado.emit(true);
            }
          );
      } else {
        this.parametroLiquidacionSeleccionado.modalidadContrato =
          this.idModalidadContratoSelecionado;

        this.parametroLiquidacionSeleccionado.tipoIva =
          this.idTipoIvaSelecionado !== null ? this.idTipoIvaSelecionado : 0;
        this.parametroLiquidacionSeleccionado.tipoPago =
          this.idTipoPagoSelecionado !== null ? this.idTipoPagoSelecionado : 0;

        this.parametroLiquidacionSeleccionado.tipoCuentaXPagarId =
          this.idTipoCuentaXPagarSelecionado;
        this.parametroLiquidacionSeleccionado.facturaElectronicaId =
          this.idFacturaElectronicaSeleccionado;
        this.parametroLiquidacionSeleccionado.subcontrataId =
          this.idSubcontrataSeleccionado;
        this.parametroLiquidacionSeleccionado.tipoDocumentoSoporteId =
          this.idTipoDocumentoSoporteSelecionado;
        this.parametroLiquidacionSeleccionado.honorarioSinIva =
          GeneralService.obtenerValorAbsoluto(formValues.honorarioSinIvaCtrl);
        this.parametroLiquidacionSeleccionado.tarifaIva =
          GeneralService.obtenerValorAbsoluto(formValues.tarifaIvaCtrl);

        this.parametroLiquidacionSeleccionado.baseAporteSalud =
          formValues.baseAporteSaludCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(
                formValues.baseAporteSaludCtrl
              );
        this.parametroLiquidacionSeleccionado.aporteSalud =
          formValues.aporteSaludCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(formValues.aporteSaludCtrl);
        this.parametroLiquidacionSeleccionado.aportePension =
          formValues.aportePensionCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(formValues.aportePensionCtrl);
        this.parametroLiquidacionSeleccionado.riesgoLaboral =
          formValues.riesgoLaboralCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(formValues.riesgoLaboralCtrl);
        this.parametroLiquidacionSeleccionado.fondoSolidaridad =
          formValues.fondoSolidaridadCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(
                formValues.fondoSolidaridadCtrl
              );

        this.parametroLiquidacionSeleccionado.pensionVoluntaria =
          formValues.pensionVoluntariaCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(
                formValues.pensionVoluntariaCtrl
              );
        this.parametroLiquidacionSeleccionado.dependiente =
          formValues.dependienteCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(formValues.dependienteCtrl);
        this.parametroLiquidacionSeleccionado.afc =
          formValues.afcCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(formValues.afcCtrl);
        this.parametroLiquidacionSeleccionado.medicinaPrepagada =
          formValues.medicinaPrepagadaCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(
                formValues.medicinaPrepagadaCtrl
              );
        this.parametroLiquidacionSeleccionado.interesVivienda =
          formValues.interesesViviendaCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(
                formValues.interesesViviendaCtrl
              );
        this.parametroLiquidacionSeleccionado.fechaInicioDescuentoInteresVivienda =
          dateFechaInicio;
        this.parametroLiquidacionSeleccionado.fechaFinalDescuentoInteresVivienda =
          dateFechaFinal;

        this.parametroLiquidacionSeleccionado.otrosDescuentos =
          formValues.otrosDescuentosCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(
                formValues.otrosDescuentosCtrl
              );
        this.parametroLiquidacionSeleccionado.fechaInicioOtrosDescuentos =
          dateFechaInicioOtrosDescuentos;
        this.parametroLiquidacionSeleccionado.fechaFinalOtrosDescuentos =
          dateFechaFinalOtrosDescuentos;

        this.parametroLiquidacionSeleccionado.terceroDeducciones =
          this.listaTerceroDeducciones;

        this.parametroLiquidacionSeleccionado.notaLegal1 =
          formValues.notaLegal1Ctrl;
        this.parametroLiquidacionSeleccionado.notaLegal2 =
          formValues.notaLegal2Ctrl;
        this.parametroLiquidacionSeleccionado.notaLegal3 =
          formValues.notaLegal3Ctrl;
        this.parametroLiquidacionSeleccionado.notaLegal4 =
          formValues.notaLegal4Ctrl;
        this.parametroLiquidacionSeleccionado.notaLegal5 =
          formValues.notaLegal5Ctrl;
        this.parametroLiquidacionSeleccionado.notaLegal6 =
          formValues.notaLegal6Ctrl;

        this.parametroLiquidacionSeleccionado.tipoAdminPilaId =
          this.idTipoAdminPila;

        this.terceroService
          .ActualizarParametroLiquidacionTercero(
            this.parametroLiquidacionSeleccionado
          )
          .subscribe(
            () => {
              this.editForm.reset(this.parametroLiquidacionSeleccionado);
              this.alertify.success(
                'La parametrización de liquidación se modificó correctamente'
              );
            },

            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.esCancelado.emit(true);
            }
          );
      }
    }
  }

  ValidarTerceroDeDeduccion(): boolean {
    let resultado = true;
    for (const x of this.listaTerceroDeducciones) {
      if (x.deduccion && x.deduccion.deduccionId > 0) {
        if (x.terceroDeDeduccion === null || x.terceroDeDeduccion.id === 0) {
          resultado = false;

          if (!resultado) {
            break;
          }
        }
      }
    }
    return resultado;
  }

  onCancelar() {
    this.parametroLiquidacionSeleccionado = null;
    this.esCancelado.emit(true);
  }

  abrirPopup(index: number) {
    if (this.listaTerceroDeducciones.length > 0) {
      const terceroDeduccionSel = this.listaTerceroDeducciones[index];

      //#region Abrir Popup

      const initialState = {
        title: 'TERCERO DE LA DEDUCCION',
      };

      this.bsModalRef = this.modalService.show(
        PopupParametroLiquidacionTerceroComponent,
        Object.assign({ initialState }, { class: 'gray modal-lg' })
      );

      //#endregion Abrir Popup

      //#region Cargar información del popup (OnHidden event)

      const combine = combineLatest([this.modalService.onHidden]).subscribe(
        () => this.changeDetection.markForCheck()
      );

      this.subscriptions.push(
        this.modalService.onHidden.subscribe((reason: string) => {
          if (
            this.bsModalRef.content != null &&
            this.bsModalRef.content.tercero !== null
          ) {
            const terceroSeleccionado = this.bsModalRef.content
              .tercero as Tercero;
            terceroDeduccionSel.terceroDeDeduccion = new ValorSeleccion();
            terceroDeduccionSel.terceroDeDeduccion.id =
              terceroSeleccionado.terceroId;
            terceroDeduccionSel.terceroDeDeduccion.nombre =
              terceroSeleccionado.nombre;
            terceroDeduccionSel.terceroDeDeduccion.valor = '';
          }
          this.unsubscribe();
        })
      );

      this.subscriptions.push(combine);

      //#endregion Cargar información del popup (OnHidden event)
    }
  }

  onCheckChange(event) {
    /* Selected */
    this.terceroDeduccionSeleccionadoId = 0;
    if (event.target.checked) {
      this.accionAgregarDeduccion = false;
      this.habilitarBotonAgregar = true;
      this.nombreBotonAgregar = 'Actualizar';
      this.terceroDeduccionSeleccionadoId = +event.target.value;
      this.terceroDeduccionSeleccionado = this.listaTerceroDeducciones.filter(
        (x) => x.terceroDeduccionId === this.terceroDeduccionSeleccionadoId
      )[0];
      this.cargarTerceroDeduccion();
    }
  }

  cargarTerceroDeduccion() {
    if (this.terceroDeduccionSeleccionado) {
      this.editForm.patchValue({
        codigoActividadCtrl:
          this.terceroDeduccionSeleccionado.actividadEconomica.codigo,
        actividadCtrl:
          this.terceroDeduccionSeleccionado.actividadEconomica.nombre,
        codigoDeduccionCtrl: this.terceroDeduccionSeleccionado.deduccion.codigo,
        deduccionCtrl: this.terceroDeduccionSeleccionado.deduccion.nombre,
        valorFijoCtrl: GeneralService.obtenerFormatoMoney(
          this.terceroDeduccionSeleccionado.valorFijo
        ),
      });

      if (this.terceroDeduccionSeleccionado.deduccion) {
        this.deduccionId =
          this.terceroDeduccionSeleccionado.deduccion.deduccionId;
        this.habilitarValorFijoControl(
          this.terceroDeduccionSeleccionado.deduccion.esValorFijo
        );
      }
      if (this.terceroDeduccionSeleccionado.actividadEconomica) {
        this.actividadEconomicaId =
          this.terceroDeduccionSeleccionado.actividadEconomica.id;
      }
    }
  }

  habilitarValorFijoControl(valor: boolean) {
    if (valor) {
      this.valorFijoCtrl.enable();
    } else {
      this.valorFijoCtrl.disable();
    }
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  exportarExcel() {
    let fileName = '';

    this.terceroService.DescargarListaActividadEconomica().subscribe(
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
              fileName = 'ActividadEconomica.xlsx';
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
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/ADMINISTRACION_PARAMETROLIQUIDACIONTERCERO']);
      }
    );
  }

  //#region Controles

  get valorFijoCtrl() {
    return this.editForm.get('valorFijoCtrl');
  }

  get modalidadContratoCtrl() {
    return this.editForm.get('modalidadContratoCtrl');
  }
  get tipoPagoCtrl() {
    return this.editForm.get('tipoPagoCtrl');
  }
  get tipoIvaCtrl() {
    return this.editForm.get('tipoIvaCtrl');
  }
  get adminPilaCtrl() {
    return this.editForm.get('adminPilaCtrl');
  }
  get tipoCuentaXPagarCtrl() {
    return this.editForm.get('tipoCuentaXPagarCtrl');
  }
  get tipoDocumentoSoporteCtrl() {
    return this.editForm.get('tipoDocumentoSoporteCtrl');
  }
  get facturaElectronicaCtrl() {
    return this.editForm.get('facturaElectronicaCtrl');
  }
  get subcontrataCtrl() {
    return this.editForm.get('subcontrataCtrl');
  }
  get baseAporteSaludCtrl() {
    return this.editForm.get('baseAporteSaludCtrl');
  }
  get aporteSaludCtrl() {
    return this.editForm.get('aporteSaludCtrl');
  }
  get aportePensionCtrl() {
    return this.editForm.get('aportePensionCtrl');
  }
  get riesgoLaboralCtrl() {
    return this.editForm.get('riesgoLaboralCtrl');
  }
  get fondoSolidaridadCtrl() {
    return this.editForm.get('fondoSolidaridadCtrl');
  }
  get pensionVoluntariaCtrl() {
    return this.editForm.get('pensionVoluntariaCtrl');
  }
  get dependienteCtrl() {
    return this.editForm.get('dependienteCtrl');
  }
  get afcCtrl() {
    return this.editForm.get('afcCtrl');
  }
  get medicinaPrepagadaCtrl() {
    return this.editForm.get('medicinaPrepagadaCtrl');
  }
  get interesesViviendaCtrl() {
    return this.editForm.get('interesesViviendaCtrl');
  }
  get fechaInicioCtrl() {
    return this.editForm.get('fechaInicioCtrl');
  }
  get fechaFinalCtrl() {
    return this.editForm.get('fechaFinalCtrl');
  }
  get honorarioSinIvaCtrl() {
    return this.editForm.get('honorarioSinIvaCtrl');
  }
  get tarifaIvaCtrl() {
    return this.editForm.get('tarifaIvaCtrl');
  }
  get codigoDeduccionCtrl() {
    return this.editForm.get('codigoDeduccionCtrl');
  }
  get deduccionCtrl() {
    return this.editForm.get('deduccionCtrl');
  }
  get codigoActividadCtrl() {
    return this.editForm.get('codigoActividadCtrl');
  }
  get actividadCtrl() {
    return this.editForm.get('actividadCtrl');
  }

  //#endregion Controles

  //#region Listas

  cargarParametrosGenerales() {
    this.listaService
      .ObtenerParametrosGeneralesXTipo('ParametroLiquidacionTercero')
      .subscribe(
        (lista: ValorSeleccion[]) => {
          this.listaParametrosGeneral = lista;

          if (this.listaParametrosGeneral) {
            this.createDefaultForm();
          }
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  cargarListasResolver() {
    this.route.data.subscribe((data) => {
      this.listaModalidadContrato = data['modalidadContrato'];
    });

    this.route.data.subscribe((data) => {
      this.listaTipoCuentaXPagar = data['tipoCuentaPorPagar'];
    });

    this.route.data.subscribe((data) => {
      this.listaTipoDocumentoSoporte = data['tipoDocumentoSoporte'];
    });

    this.route.data.subscribe((data) => {
      this.listaTipoIva = data['tipoIva'];
    });

    this.route.data.subscribe((data) => {
      this.listaTipoPago = data['tipoPago'];
    });

    this.route.data.subscribe((data) => {
      this.listaAdminPila = data['adminPila'];
    });

    this.route.data.subscribe((data) => {
      this.listaFacturaElectronica = data['SIoNO'];
      this.listaSubcontrata = data['SIoNO'];
    });

  }

  cargarListas() {
    this.cargarListaModalidadContrato(TipoLista.ModalidadContrato.value);
    this.cargarlistaTipoCuentaXPagar(TipoLista.TipoCuentaXPagar.value);
    this.cargarlistaTipoDocumentoSoporte(TipoLista.TipoDocumentoSoporte.value);
    this.cargarlistaTipoIva(TipoLista.TipoIva.value);
    this.cargarlistaTipoPago(TipoLista.TipoPago.value);
    this.cargarlistaFacturaElectronica();
  }

  cargarListaModalidadContrato(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaModalidadContrato = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaTipoPago(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoPago = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaFacturaElectronica() {
    this.listaService.ObtenerListaSIoNO().subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaFacturaElectronica = lista;
        this.listaSubcontrata = lista;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  cargarlistaTipoIva(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoIva = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaTipoCuentaXPagar(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoCuentaXPagar = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  cargarlistaTipoDocumentoSoporte(tipo: number) {
    this.listaService.ObtenerListaXTipo(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaTipoDocumentoSoporte = lista;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {}
    );
  }

  //#endregion Listas
}
