import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  Input,
  Output,
  EventEmitter,
  ChangeDetectorRef,
} from '@angular/core';
import {
  NgForm,
  FormGroup,
  FormArray,
  FormControl,
  FormBuilder,
} from '@angular/forms';
import * as jsPDF from 'jspdf';
import domtoimage from 'dom-to-image';

import { AlertifyService } from 'src/app/_services/alertify.service';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';
import { FormatoSolicitudPagoDto } from 'src/app/_dto/formatoSolicitudPagoDto';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { PopupSolicitudPagoComponent } from './popup-solicitud-pago/popup-solicitud-pago.component';
import { combineLatest, Subscription } from 'rxjs';
import { PopupFacturaComponent } from './popup-factura/popup-factura.component';
import { PlanPago } from 'src/app/_models/planPago';
import { ListaService } from 'src/app/_services/lista.service';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';
import { SolicitudPagoService } from 'src/app/_services/solicitudPago.service';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PopupCargarArchivosComponent } from './popup-cargar-archivos/popup-cargar-archivos.component';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { TerceroService } from 'src/app/_services/tercero.service';
import { CdpService } from 'src/app/_services/cdp.service';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { Cdp } from 'src/app/_models/cdp';
import { RespuestaSolicitudPago } from 'src/app/_dto/respuestaSolicitudPago';

@Component({
  selector: 'app-formato-solicitud-pago',
  templateUrl: './formato-solicitud-pago.component.html',
  styleUrls: ['./formato-solicitud-pago.component.scss'],
})
export class FormatoSolicitudPagoComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;

  @Input() formatoSolicitudPago: FormatoSolicitudPagoDto;
  @Input() parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  @Input() listaNotasLegales: ValorSeleccion[];
  @Output() esCancelado = new EventEmitter<boolean>();

  listaMeses: ValorSeleccion[] = [];
  mesSaludActual = '';
  formatoSolicitudPagoId = 0;
  numeroFactura = '';

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);
  arrayRubrosControls = new FormArray([]);

  listaActividadEconomica: ValorSeleccion[];
  bsModalRef: BsModalRef;
  planPagoSeleccionada: PlanPago = null;
  formatoSolicitudPagoPopup: FormatoSolicitudPago = null;
  formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  idplanPagoSeleccionada = 0;
  subscriptions: Subscription[] = [];

  habilitaPlanDePagoSeleccionado = false;
  habilitaDatosRegistrados = false;
  habilitaArchivosCargados = false;
  solicitudRegistrada = false;

  notaLegal: ValorSeleccion;
  notaLegal1 = '';
  notaLegal2 = '';
  notaLegal3 = '';
  notaLegal4 = '';
  notaLegal5 = '';
  notaLegal6 = '';

  rubrosPresupuestales: DetalleCDP[] = [];
  cdps: Cdp[] = [];

  constructor(
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private liquidacionService: DetalleLiquidacionService,
    private solicitudPagoService: SolicitudPagoService,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef,
    private listaService: ListaService,
    private cdpService: CdpService
  ) {}

  ngOnInit() {
    this.cargarNotaLegal();
    this.cargarNotasLegales();
    this.createEmptyForm();
    this.obtenerRubrosPresupuestales();
    this.cargarListaActividadEconomicaXTercero();
    this.cargarMeses();
  }

  createEmptyForm() {
    this.formatoForm = this.fb.group({
      deduccionControles: this.arrayControls,
      rubrosControles: this.arrayRubrosControls,
    });
  }

  createForm() {
    if (
      this.formatoSolicitudPago.pagosRealizados != null &&
      this.formatoSolicitudPago.pagosRealizados.length > 0
    ) {
      this.cdps = this.formatoSolicitudPago.pagosRealizados;
      for (const detalle of this.cdps) {
        this.arrayControls.push(
          new FormGroup({
            deduccionControl: new FormControl('', []),
          })
        );
      }
    }

    if (this.rubrosPresupuestales && this.rubrosPresupuestales.length > 0) {
      for (const detalle of this.rubrosPresupuestales) {
        this.arrayRubrosControls.push(
          new FormGroup({
            rubroControl: new FormControl('', []),
          })
        );
      }
    }

    this.formatoForm = this.fb.group({
      deduccionControles: this.arrayControls,
      rubrosControles: this.arrayRubrosControls,
    });
  }

  seleccionarPlanPago() {
    //#region Abrir Popup
    const initialState = {
      title: 'SELECCIONE EL PLAN DE PAGO',
      crp: this.formatoSolicitudPago.cdp.crp,
    };

    this.bsModalRef = this.modalService.show(
      PopupFacturaComponent,
      Object.assign({ initialState }, { class: 'gray modal-lg' })
    );

    this.bsModalRef.content.closeBtnName = 'Aceptar';

    //#endregion Abrir Popup

    //#region Cargar información del popup (OnHidden event)

    const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (
          this.bsModalRef.content != null &&
          this.bsModalRef.content.facturaSeleccionada != null
        ) {
          this.planPagoSeleccionada = this.bsModalRef.content
            .facturaSeleccionada as PlanPago;
          this.habilitaPlanDePagoSeleccionado = true;
        }

        this.unsubscribe();
      })
    );

    this.subscriptions.push(combine);

    //#endregion Cargar información del popup (OnHidden event)
  }

  registrarSeguridadSocial() {
    const initialState = {
      title: 'Plan de Pago',
      listaActividadEconomica: this.listaActividadEconomica,
      listaMeses: this.listaMeses,
      formatoSolicitudPagoEdit: this.formatoSolicitudPagoPopup,
      parametroLiquidacionTercero: this.parametroLiquidacionSeleccionado,
      planPagoSeleccionada: this.planPagoSeleccionada,
    };

    this.bsModalRef = this.modalService.show(
      PopupSolicitudPagoComponent,
      Object.assign(
        {
          animated: true,
          keyboard: true,
          backdrop: true,
          ignoreBackdropClick: false,
        },
        { initialState },
        { class: 'gray modal-lg' }
      )
    );

    const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (
          this.bsModalRef.content != null &&
          this.bsModalRef.content.formatoSolicitudPago != null
        ) {
          this.formatoSolicitudPagoPopup = this.bsModalRef.content
            .formatoSolicitudPago as FormatoSolicitudPago;

          this.cargarDatosSeguridadSocial();
          this.habilitaDatosRegistrados = true;
          // Temporal: Disabled el botón cargar archivos
          // this.habilitaArchivosCargados = true;
        }

        this.unsubscribe();
      })
    );
    this.subscriptions.push(combine);
  }

  cargarArchivos() {
    const initialState = {
      title: 'Documentos requeridos para el pago',
      listaActividadEconomica: this.listaActividadEconomica,
    };

    this.bsModalRef = this.modalService.show(
      PopupCargarArchivosComponent,
      Object.assign(
        {
          animated: true,
          keyboard: true,
          backdrop: true,
          ignoreBackdropClick: false,
        },
        { initialState },
        { class: 'gray modal-lg' }
      )
    );

    const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (
          this.bsModalRef.content != null &&
          this.bsModalRef.content.respuesta !== null
        ) {
          const resultado = this.bsModalRef.content.respuesta;
          if (resultado.toString() === 'DONE') {
            this.habilitaArchivosCargados = true;
          }
        }

        this.unsubscribe();
      })
    );
    this.subscriptions.push(combine);
  }

  registrarSolicitudPago() {
    if (this.formatoForm.valid) {
      this.formatoSolicitudPagoPopup.terceroId = this.formatoSolicitudPago.tercero.terceroId;
      this.formatoSolicitudPagoPopup.planPagoId = this.planPagoSeleccionada.planPagoId;
      this.formatoSolicitudPagoPopup.crp = this.formatoSolicitudPago.cdp.crp;
      this.formatoSolicitudPagoPopup.supervisorId = this.formatoSolicitudPago.cdp.supervisorId;

      this.solicitudPagoService
        .RegistrarFormatoSolicitudPago(this.formatoSolicitudPagoPopup)
        .subscribe(
          (response: RespuestaSolicitudPago) => {
            if (response) {
              const formatoId = response.formatoSolicitudPagoId;
              this.numeroFactura = response.numeroFactura;

              if (!isNaN(formatoId)) {
                this.formatoSolicitudPagoId = +formatoId;
                this.alertify.success(
                  'El Formato de Solicitud de Pago se registró correctamente'
                );
                this.solicitudRegistrada = true;
              } else {
                this.alertify.error(
                  'No se pudo registrar el Formato de Solicitud de Pago'
                );
              }
            }
          },
          (error) => {
            this.alertify.error(
              'Hubó un error al registrar el Formato de Solicitud de Pago ' +
                error
            );
          }
        );
    }
  }

  exportarPDF() {
    const node = document.getElementById('content');
    const node2 = document.getElementById('content2');

    let img;
    let filename;
    let newImage;

    domtoimage
      .toPng(node, { bgcolor: '#fff' })

      .then((dataUrl) => {
        img = new Image();
        img.src = dataUrl;
        newImage = img.src;

        img.onload = () => {
          const pdfWidth = img.width - 20;
          const pdfHeight = img.height;

          let doc;

          doc = new jsPDF('l', 'px', [pdfWidth, pdfHeight]);

          const width = doc.internal.pageSize.getWidth();
          const height = doc.internal.pageSize.getHeight();

          doc.addImage(newImage, 'PNG', 0, 0, width, height);
          filename = 'documento' + '.pdf';
          doc.save(filename);
        };
      })
      .catch((error) => {
        // Error Handling
      });

    let img2;
    let filename2;
    let newImage2;

    domtoimage
      .toPng(node2, { bgcolor: '#fff' })

      .then((dataUrl) => {
        img2 = new Image();
        img2.src = dataUrl;
        newImage2 = img2.src;

        img2.onload = () => {
          const pdfWidth = img2.width - 20;
          const pdfHeight = img2.height;

          let doc;

          doc = new jsPDF('l', 'px', [pdfWidth, pdfHeight]);

          const width = doc.internal.pageSize.getWidth();
          const height = doc.internal.pageSize.getHeight();

          doc.addImage(newImage2, 'PNG', 0, 0, width, height);
          filename2 = 'documento' + '.pdf';
          doc.save(filename2);
        };
      })
      .catch((error) => {
        // Error Handling
      });
  }

  onCancelar() {
    this.esCancelado.emit(true);
  }

  cargarDatosSeguridadSocial() {
    if (this.formatoSolicitudPagoPopup) {
      this.solicitudPagoService
        .ObtenerSeguridadSocialParaSolicitudPago(
          this.planPagoSeleccionada.planPagoId,
          this.formatoSolicitudPagoPopup.baseCotizacion,
          this.formatoSolicitudPagoPopup.actividadEconomicaId
        )
        .subscribe((response: FormatoCausacionyLiquidacionPago) => {
          this.formatoCausacionyLiquidacionPago = response;
        });
    }
  }

  cargarListaActividadEconomicaXTercero() {
    this.liquidacionService
      .ObtenerListaActividadesEconomicaXTercero(
        this.formatoSolicitudPago.tercero.terceroId
      )
      .subscribe(
        (lista: ValorSeleccion[]) => {
          this.listaActividadEconomica = lista;
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {}
      );
  }

  cargarMeses() {
    this.listaService.ObtenerListaMeses().subscribe(
      (lista: ValorSeleccion[]) => {
        this.listaMeses = lista;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  cargarNotasLegales() {
    if (this.listaNotasLegales) {
      if (this.parametroLiquidacionSeleccionado) {
        if (this.parametroLiquidacionSeleccionado.notaLegal1) {
          if (this.listaNotasLegales[0] != null) {
            this.notaLegal1 = (this
              .listaNotasLegales[0] as ValorSeleccion).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal2) {
          if (this.listaNotasLegales[1] != null) {
            this.notaLegal2 = (this
              .listaNotasLegales[1] as ValorSeleccion).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal3) {
          if (this.listaNotasLegales[2] != null) {
            this.notaLegal3 = (this
              .listaNotasLegales[2] as ValorSeleccion).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal4) {
          if (this.listaNotasLegales[3] != null) {
            this.notaLegal4 = (this
              .listaNotasLegales[3] as ValorSeleccion).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal5) {
          if (this.listaNotasLegales[4] != null) {
            this.notaLegal5 = (this
              .listaNotasLegales[4] as ValorSeleccion).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal6) {
          if (this.listaNotasLegales[5] != null) {
            this.notaLegal6 = (this
              .listaNotasLegales[5] as ValorSeleccion).valor;
          }
        }
      }
    }
  }

  cargarNotaLegal() {
    this.listaService.ObtenerParametroGeneralXNombre('NotaLegalANE').subscribe(
      (data: ValorSeleccion) => {
        this.notaLegal = data;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  obtenerRubrosPresupuestales() {
    this.cdpService
      .ObtenerRubrosPresupuestalesPorCompromiso(
        this.formatoSolicitudPago.cdp.crp
      )
      .subscribe(
        (lista: DetalleCDP[]) => {
          if (lista) {
            this.rubrosPresupuestales = lista;
          }
        },
        () => {},
        () => {
          this.createForm();
        }
      );
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }
}
