import { formatDate } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
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
import { TerceroDeduccionDto } from 'src/app/_dto/terceroDeduccionDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { ActividadEconomica } from 'src/app/_models/actividadEconomica';
import { Deduccion } from 'src/app/_models/deduccion';
import {
  ModalidadContrato,
  PerfilUsuario,
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
  @Input() esCreacion: boolean;
  @Input() tercero: Tercero;
  @Output() esCancelado = new EventEmitter<boolean>();
  @ViewChild('staticTabs', { static: false }) staticTabs: TabsetComponent;

  searchDeduccion: string;
  suggestionsDeduccion$: Observable<Deduccion[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  baseUrlDeduccion = environment.apiUrl + 'lista/ObtenerListaDeducciones';
  deduccion: Deduccion;
  deduccionId = 0;

  searchActividad: string;
  suggestionsActividad$: Observable<ActividadEconomica[]>;
  baseUrlActividad =
    environment.apiUrl + 'lista/ObtenerListaActividadesEconomicas';
  actividadEconomica: ActividadEconomica;
  actividadEconomicaId = 0;

  arrayControls = new FormArray([]);

  parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  listaModalidadContrato: ValorSeleccion[] = [];
  listaTipoPago: ValorSeleccion[] = [];
  listaTipoIva: ValorSeleccion[] = [];
  listaTipoCuentaXPagar: ValorSeleccion[] = [];
  listaTipoDocumentoSoporte: ValorSeleccion[] = [];
  listaSupervisor: ValorSeleccion[] = [];
  listaFacturaElectronica: ValorSeleccion[] = [];
  listaSubcontrata: ValorSeleccion[] = [];

  editForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  nombreBoton = 'Registrar';
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

  idSupervisorSeleccionado?: number;
  supervisorSeleccionado: ValorSeleccion = null;

  listaTerceroDeducciones: TerceroDeduccionDto[] = [];
  listaParametrosGeneral: ValorSeleccion[] = [];

  constructor(
    private http: HttpClient,
    private listaService: ListaService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private terceroService: TerceroService,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef,
    private usuarioService: UsuarioService
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    this.cargarListas();

    this.cargarBusquedaDeducciones();

    this.cargarBusquedaActividad();

    if (this.esCreacion) {
      this.cargarParametrosGenerales();
      this.nombreBoton = 'Registrar';
      // this.editForm.reset();
    } else {
      this.obtenerParametrizacionTercero();
      this.nombreBoton = 'Guardar';
    }
  }

  cargarBusquedaDeducciones() {
    this.suggestionsDeduccion$ = new Observable(
      (observer: Observer<string>) => {
        observer.next(this.searchDeduccion);
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

  typeaheadOnSelectDeduccion(e: TypeaheadMatch): void {
    this.deduccion = e.item as Deduccion;
    if (this.deduccion) {
      this.deduccionId = this.deduccion.deduccionId;
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
    }
  }

  obtenerParametrizacionTercero() {
    if (this.tercero.terceroId > 0) {
      this.terceroService
        .ObtenerParametrizacionLiquidacionXTercero(this.tercero.terceroId)
        .subscribe((documento: ParametroLiquidacionTercero) => {
          if (documento) {
            this.parametroLiquidacionSeleccionado = documento;

            if (this.parametroLiquidacionSeleccionado) {
              this.createFullForm();
            }
          } else {
            this.alertify.error(
              'No se pudo obtener información de la parametrización del tercero'
            );
          }
        });
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
      tipoCuentaXPagarCtrl: [null, Validators.required],
      tipoDocumentoSoporteCtrl: [null, Validators.required],
      supervisorCtrl: [null, Validators.required],

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

      codigoDeduccionCtrl: [''],
      deduccionCtrl: [''],
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
    }

    this.editForm.patchValue({
      tarifaIvaCtrl: GeneralService.obtenerFormatoLongMoney(tarifaIvaC),
      baseAporteSaludCtrl: GeneralService.obtenerFormatoLongMoney(
        baseAporteSaludC
      ),
      aporteSaludCtrl: GeneralService.obtenerFormatoLongMoney(aporteSaludC),
      aportePensionCtrl: GeneralService.obtenerFormatoLongMoney(aportePensionC),
      riesgoLaboralCtrl: GeneralService.obtenerFormatoLongMoney(riesgoLaboralC),
      dependienteCtrl: GeneralService.obtenerFormatoLongMoney(dependientesC),
    });
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

    console.log(this.parametroLiquidacionSeleccionado.supervisorId);
    this.idSupervisorSeleccionado =
      this.parametroLiquidacionSeleccionado.supervisorId > 0
        ? this.parametroLiquidacionSeleccionado.supervisorId
        : null;
    if (this.idSupervisorSeleccionado !== null) {
      this.supervisorSeleccionado = this.listaSupervisor.filter(
        (x) => x.id === this.idSupervisorSeleccionado
      )[0];
    }

    this.idFacturaElectronicaSeleccionado = this.parametroLiquidacionSeleccionado.facturaElectronicaId;
    if (this.idFacturaElectronicaSeleccionado !== null) {
      this.facturaElectronicaSeleccionado = this.listaFacturaElectronica.filter(
        (x) => x.id === this.idFacturaElectronicaSeleccionado
      )[0];
    }

    this.idSubcontrataSeleccionado = this.parametroLiquidacionSeleccionado.subcontrataId;
    if (this.idSubcontrataSeleccionado !== null) {
      this.subcontrataSeleccionado = this.listaSubcontrata.filter(
        (x) => x.id === this.idSubcontrataSeleccionado
      )[0];
    }

    console.log(this.parametroLiquidacionSeleccionado.tipoCuentaPorPagar);
    this.idTipoCuentaXPagarSelecionado =
      this.parametroLiquidacionSeleccionado.tipoCuentaPorPagar > 0
        ? this.parametroLiquidacionSeleccionado.tipoCuentaPorPagar
        : null;
    if (this.idTipoCuentaXPagarSelecionado !== null) {
      this.tipoCuentaXPagarSeleccionado = this.listaTipoCuentaXPagar.filter(
        (x) => x.id === this.idTipoCuentaXPagarSelecionado
      )[0];
    }

    console.log(this.parametroLiquidacionSeleccionado.tipoDocumentoSoporte);
    this.idTipoDocumentoSoporteSelecionado =
      this.parametroLiquidacionSeleccionado.tipoDocumentoSoporte > 0
        ? this.parametroLiquidacionSeleccionado.tipoDocumentoSoporte
        : null;
    if (this.idTipoDocumentoSoporteSelecionado !== null) {
      this.tipoDocumentoSoporteSeleccionado = this.listaTipoDocumentoSoporte.filter(
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

    pensionVoluntariaC = this.parametroLiquidacionSeleccionado
      ?.pensionVoluntaria;
    dependienteC = this.parametroLiquidacionSeleccionado?.dependiente;
    afcC = this.parametroLiquidacionSeleccionado?.afc;
    medicinaPrepagadaC = this.parametroLiquidacionSeleccionado
      ?.medicinaPrepagada;

    otrosDescuentosC = this.parametroLiquidacionSeleccionado?.otrosDescuentos;
    fechaInicioOtrosDescuentos = this.parametroLiquidacionSeleccionado
      .fechaInicioOtrosDescuentos;
    fechaFinalOtrosDescuentos = this.parametroLiquidacionSeleccionado
      .fechaFinalOtrosDescuentos;

    interesesViviendaC = this.parametroLiquidacionSeleccionado?.interesVivienda;
    fechaInicio = this.parametroLiquidacionSeleccionado
      .fechaInicioDescuentoInteresVivienda;
    fechaFinal = this.parametroLiquidacionSeleccionado
      .fechaFinalDescuentoInteresVivienda;

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

      baseAporteSaludCtrl: GeneralService.obtenerFormatoLongMoney(
        baseAporteSaludC
      ),
      aporteSaludCtrl: GeneralService.obtenerFormatoLongMoney(aporteSaludC),
      aportePensionCtrl: GeneralService.obtenerFormatoLongMoney(aportePensionC),
      riesgoLaboralCtrl: GeneralService.obtenerFormatoLongMoney(riesgoLaboralC),
      fondoSolidaridadCtrl: GeneralService.obtenerFormatoMoney(
        fondoSolidaridadC
      ),

      pensionVoluntariaCtrl: GeneralService.obtenerFormatoMoney(
        pensionVoluntariaC
      ),
      dependienteCtrl: GeneralService.obtenerFormatoLongMoney(dependienteC),
      afcCtrl: GeneralService.obtenerFormatoMoney(afcC),
      medicinaPrepagadaCtrl: GeneralService.obtenerFormatoMoney(
        medicinaPrepagadaC
      ),

      interesesViviendaCtrl: GeneralService.obtenerFormatoMoney(
        interesesViviendaC
      ),
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
    this.idModalidadContratoSelecionado = +this.modalidadContratoSeleccionado
      .id;

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

    if (
      this.idModalidadContratoSelecionado ===
      ModalidadContrato.ProveedorSinDescuento.value
    ) {
      this.staticTabs.tabs[1].disabled = true;
    } else {
      this.staticTabs.tabs[1].disabled = false;
    }
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
    this.idTipoDocumentoSoporteSelecionado = +this
      .tipoDocumentoSoporteSeleccionado.id;
  }

  onFacturaElectronica() {
    this.facturaElectronicaSeleccionado = this.facturaElectronicaCtrl
      .value as ValorSeleccion;
    this.idFacturaElectronicaSeleccionado = +this.facturaElectronicaSeleccionado
      .id;
  }

  onSubcontrata() {
    this.subcontrataSeleccionado = this.subcontrataCtrl.value as ValorSeleccion;
    this.idSubcontrataSeleccionado = +this.subcontrataSeleccionado.id;
  }

  onSupervisor() {
    this.supervisorSeleccionado = this.supervisorCtrl.value as ValorSeleccion;
    this.idSupervisorSeleccionado = +this.supervisorSeleccionado.id;
  }

  onAgregarDeduccion() {
    if (
      this.deduccion &&
      this.deduccion.deduccionId > 0 &&
      this.actividadEconomica &&
      this.actividadEconomicaId > 0
    ) {
      const filtro = this.listaTerceroDeducciones.filter(
        (x) =>
          x.deduccion.id === this.deduccion.deduccionId &&
          x.actividadEconomica.id === this.actividadEconomicaId
      )[0];

      if (filtro) {
        this.alertify.error(
          'Ya existe la combinación deducción-activadad económica'
        );
        return;
      }

      const actividadT = new ValorSeleccion();
      actividadT.id = this.actividadEconomicaId;
      actividadT.codigo = this.actividadEconomica.codigo;
      actividadT.nombre = this.actividadEconomica.nombre;

      const deduccionT = new ValorSeleccion();
      deduccionT.id = this.deduccion.deduccionId;
      deduccionT.codigo = this.deduccion.codigo;
      deduccionT.nombre = this.deduccion.nombre;

      const terceroDeDeduccionT = new ValorSeleccion();
      if (this.deduccion.tercero && this.deduccion.tercero.terceroId > 0) {
        terceroDeDeduccionT.id = this.deduccion.tercero.terceroId;
        terceroDeDeduccionT.codigo = this.deduccion.tercero.numeroIdentificacion;
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
        deduccion: deduccionT,
        actividadEconomica: actividadT,
        tercero: terceroT,
        terceroDeDeduccion: terceroDeDeduccionT,
        tipoIdentificacion: 0,
        identificacionTercero: '',
        codigo: '',
        terceroDeduccionId: 0,
      };

      this.listaTerceroDeducciones.push(terceroDeduccion);

      this.arrayControls.push(
        new FormGroup({
          rubroControl: new FormControl(''),
        })
      );

      this.onLimpiarDeduccion();
    }
  }

  onEliminarDeduccion() {
    this.alertify.confirm2(
      'Deducciones del tercero',
      '¿Esta seguro que desea eliminar las deducciones?',
      () => {
        this.listaTerceroDeducciones = [];
      }
    );
  }

  onLimpiarDeduccion() {
    this.deduccionId = 0;
    this.deduccion = null;
    this.actividadEconomicaId = 0;
    this.actividadEconomica = null;
    this.searchActividad = '';
    this.searchDeduccion = '';
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
        this.alertify.error('Debe ingresar al menos una deducción');
        return;
      }
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
          tipoCuentaPorPagar: this.idTipoCuentaXPagarSelecionado,
          tipoIva: this.idTipoIvaSelecionado,
          tipoDocumentoSoporte: this.idTipoDocumentoSoporteSelecionado,
          tipoPago: this.idTipoPagoSelecionado,
          honorarioSinIva: GeneralService.obtenerValorAbsoluto(
            formValues.honorarioSinIvaCtrl
          ),
          tarifaIva: GeneralService.obtenerValorAbsoluto(
            formValues.tarifaIvaCtrl
          ),
          facturaElectronicaId: this.idFacturaElectronicaSeleccionado,
          subcontrataId: this.idSubcontrataSeleccionado,
          supervisorId: this.idSupervisorSeleccionado,

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
        this.parametroLiquidacionSeleccionado.modalidadContrato = this.idModalidadContratoSelecionado;
        this.parametroLiquidacionSeleccionado.tipoCuentaPorPagar = this.idTipoCuentaXPagarSelecionado;
        this.parametroLiquidacionSeleccionado.tipoIva = this.idTipoIvaSelecionado;
        this.parametroLiquidacionSeleccionado.tipoPago = this.idTipoPagoSelecionado;
        this.parametroLiquidacionSeleccionado.supervisorId = this.idSupervisorSeleccionado;
        this.parametroLiquidacionSeleccionado.facturaElectronicaId = this.idFacturaElectronicaSeleccionado;
        this.parametroLiquidacionSeleccionado.subcontrataId = this.idSubcontrataSeleccionado;
        this.parametroLiquidacionSeleccionado.tipoDocumentoSoporte = this.idTipoDocumentoSoporteSelecionado;
        this.parametroLiquidacionSeleccionado.honorarioSinIva = GeneralService.obtenerValorAbsoluto(
          formValues.honorarioSinIvaCtrl
        );
        this.parametroLiquidacionSeleccionado.tarifaIva = GeneralService.obtenerValorAbsoluto(
          formValues.tarifaIvaCtrl
        );

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
        this.parametroLiquidacionSeleccionado.fechaInicioDescuentoInteresVivienda = dateFechaInicio;
        this.parametroLiquidacionSeleccionado.fechaFinalDescuentoInteresVivienda = dateFechaFinal;

        this.parametroLiquidacionSeleccionado.otrosDescuentos =
          formValues.otrosDescuentosCtrl === undefined
            ? 0
            : GeneralService.obtenerValorAbsoluto(
                formValues.otrosDescuentosCtrl
              );
        this.parametroLiquidacionSeleccionado.fechaInicioOtrosDescuentos = dateFechaInicioOtrosDescuentos;
        this.parametroLiquidacionSeleccionado.fechaFinalOtrosDescuentos = dateFechaFinalOtrosDescuentos;

        this.parametroLiquidacionSeleccionado.terceroDeducciones = this.listaTerceroDeducciones;

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

  onCancelar() {
    this.parametroLiquidacionSeleccionado = null;
    this.esCancelado.emit(true);
  }

  abrirPopup(index: number) {
    if (this.listaTerceroDeducciones.length > 0) {
      const terceroDeduccionSeleccionado = this.listaTerceroDeducciones[index];

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

      const combine = combineLatest([
        this.modalService.onHidden,
      ]).subscribe(() => this.changeDetection.markForCheck());

      this.subscriptions.push(
        this.modalService.onHidden.subscribe((reason: string) => {
          if (
            this.bsModalRef.content != null &&
            this.bsModalRef.content.tercero !== null
          ) {
            const terceroSeleccionado = this.bsModalRef.content
              .tercero as Tercero;
            terceroDeduccionSeleccionado.terceroDeDeduccion = new ValorSeleccion();
            terceroDeduccionSeleccionado.terceroDeDeduccion.id =
              terceroSeleccionado.terceroId;
            terceroDeduccionSeleccionado.terceroDeDeduccion.nombre =
              terceroSeleccionado.nombre;
            terceroDeduccionSeleccionado.terceroDeDeduccion.valor = '';
          }
          this.unsubscribe();
        })
      );

      this.subscriptions.push(combine);

      //#endregion Cargar información del popup (OnHidden event)
    }
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
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
  get tipoCuentaXPagarCtrl() {
    return this.editForm.get('tipoCuentaXPagarCtrl');
  }
  get tipoDocumentoSoporteCtrl() {
    return this.editForm.get('tipoDocumentoSoporteCtrl');
  }
  get supervisorCtrl() {
    return this.editForm.get('supervisorCtrl');
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

  cargarListas() {
    this.cargarListaModalidadContrato(TipoLista.ModalidadContrato.value);
    this.cargarlistaTipoCuentaXPagar(TipoLista.TipoCuentaXPagar.value);
    this.cargarlistaTipoDocumentoSoporte(TipoLista.TipoDocumentoSoporte.value);
    this.cargarlistaTipoIva(TipoLista.TipoIva.value);
    this.cargarlistaTipoPago(TipoLista.TipoPago.value);
    this.cargarListaUsuarioXPerfil(PerfilUsuario.SupervisorContractual.value);
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

  cargarListaUsuarioXPerfil(tipo: number) {
    this.usuarioService.ObtenerListaUsuarioXPerfil(tipo).subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaSupervisor = lista;
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
