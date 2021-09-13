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
import { FormatoSolicitudPagoDto } from 'src/app/_dto/formatoSolicitudPagoDto';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { combineLatest, Subscription } from 'rxjs';
import { SolicitudPagoService } from 'src/app/_services/solicitudPago.service';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PopupSolicitudPagoAprobacionComponent } from './popup-solicitud-pago-aprobacion/popup-solicitud-pago-aprobacion.component';
import { CdpService } from 'src/app/_services/cdp.service';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { DetalleFormatoSolicitudPagoDto } from 'src/app/_dto/detalleFormatoSolicitudPagoDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { PopupSolicitudPagoRechazoComponent } from './popup-solicitud-pago-rechazo/popup-solicitud-pago-rechazo.component';
import { ListaService } from 'src/app/_services/lista.service';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { Cdp } from 'src/app/_models/cdp';

@Component({
  selector: 'app-formato-solicitud-pago-aprobacion',
  templateUrl: './formato-solicitud-pago-aprobacion.component.html',
  styleUrls: ['./formato-solicitud-pago-aprobacion.component.css'],
})
export class FormatoSolicitudPagoAprobacionComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;

  @Input() formatoSolicitudPago: FormatoSolicitudPagoDto;
  @Input() parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  @Input() listaNotasLegales: ValorSeleccion[];
  @Output() esCancelado = new EventEmitter<boolean>();

  formatoSolicitudPagoId = 0;
  rubrosPresupuestales: DetalleCDP[] = [];
  cdps: Cdp[] = [];
  solicitudActualizada = false;

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);
  arrayRubrosControls = new FormArray([]);

  bsModalRef: BsModalRef;
  formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  subscriptions: Subscription[] = [];

  notaLegal: ValorSeleccion;
  notaLegal1 = '';
  notaLegal2 = '';
  notaLegal3 = '';
  notaLegal4 = '';
  notaLegal5 = '';

  constructor(
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private solicitudPagoService: SolicitudPagoService,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef,
    private cdpService: CdpService,
    private listaService: ListaService
  ) {}

  ngOnInit() {
    this.cargarNotaLegal();
    this.createEmptyForm();
    this.obtenerRubrosPresupuestales();
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

  cargarNotaLegal() {
    this.listaService.ObtenerParametroGeneralXNombre('NotaLegalANE').subscribe(
      (data: ValorSeleccion) => {
        this.notaLegal = data;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.cargarNotasLegales();
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

  abrirPopupAprobacion(tipo: number) {
    const initialState = {
      title: 'REGISTRAR DATOS ADICIONALES',
      rubrosPresupuestales: this.rubrosPresupuestales,
      formatoSolicitudPago: this.formatoSolicitudPago,
    };

    this.bsModalRef = this.modalService.show(
      PopupSolicitudPagoAprobacionComponent,
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
          this.bsModalRef.content.observaciones != null
        ) {
          this.formatoSolicitudPago.observacionesModificacion =
            this.bsModalRef.content.observaciones !== undefined
              ? this.bsModalRef.content.observaciones.trim()
              : '';
          this.formatoSolicitudPago.numeroRadicadoProveedor = this.bsModalRef.content.numeroContratista.trim();
          this.formatoSolicitudPago.fechaRadicadoProveedor = this.bsModalRef.content.fechaContratista;
          this.formatoSolicitudPago.numeroRadicadoSupervisor = this.bsModalRef.content.numeroSupervisor.trim();
          this.formatoSolicitudPago.fechaRadicadoSupervisor = this.bsModalRef.content.fechaSupervisor;
          this.formatoSolicitudPago.estadoId = tipo;
          this.solicitudActualizada = true;

          const listaDetalle: DetalleFormatoSolicitudPagoDto[] = [];

          if (
            this.bsModalRef.content.rubrosPresupuestales &&
            this.bsModalRef.content.rubrosPresupuestales.length
          ) {
            this.rubrosPresupuestales.forEach((element) => {
              if (element.valorSolicitud > 0) {
                const rubro = new ValorSeleccion();

                rubro.id = element.rubroPresupuestal.rubroPresupuestalId;
                rubro.codigo = element.rubroPresupuestal.identificacion;
                rubro.nombre = element.rubroPresupuestal.nombre;

                const item: DetalleFormatoSolicitudPagoDto = {
                  detalleFormatoSolicitudPagoId: 0,
                  formatoSolicitudPagoId: this.formatoSolicitudPago
                    .formatoSolicitudPagoId,
                  valorAPagar: element.valorSolicitud,
                  rubroPresupuestal: rubro,
                  dependencia: element.dependencia,
                  clavePresupuestalContableId: element.clavePresupuestalContableId,
                };

                listaDetalle.push(item);
              }
            });
          }

          this.formatoSolicitudPago.detallesFormatoSolicitudPago = listaDetalle;
          this.actualizarSolicitudPago();
        }
        this.unsubscribe();
      })
    );
    this.subscriptions.push(combine);
  }

  abrirPopupRechazo(tipo: number) {
    const initialState = {
      title: 'REGISTRAR OBSERVACIONES',
    };

    this.bsModalRef = this.modalService.show(
      PopupSolicitudPagoRechazoComponent,
      Object.assign(
        {
          animated: true,
          keyboard: true,
          backdrop: true,
          ignoreBackdropClick: false,
        },
        { initialState },
        { class: 'gray modal-md' }
      )
    );

    const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (
          this.bsModalRef.content != null &&
          this.bsModalRef.content.observaciones != null
        ) {
          this.formatoSolicitudPago.observacionesModificacion =
            this.bsModalRef.content.observaciones !== undefined
              ? this.bsModalRef.content.observaciones.trim()
              : '';
          this.formatoSolicitudPago.estadoId = tipo;
          this.solicitudActualizada = true;
          this.actualizarSolicitudPago();
        }
        this.unsubscribe();
      })
    );
    this.subscriptions.push(combine);
  }

  actualizarSolicitudPago() {
    if (this.formatoForm.valid) {
      this.solicitudPagoService
        .ActualizarFormatoSolicitudPago(this.formatoSolicitudPago)
        .subscribe(
          (response: any) => {
            if (!isNaN(response)) {
              this.formatoSolicitudPagoId = +response;
              this.alertify.success(
                'El Formato de Solicitud de Pago se modificó correctamente'
              );
            } else {
              this.alertify.error(
                'No se pudo actualizar el Formato de Solicitud de Pago'
              );
            }
          },
          (error) => {
            this.alertify.error(
              'Hubó un error al actualizar el Formato de Solicitud de Pago ' +
                error
            );
          },
          () => {}
        );
    }
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
      }
    }
  }

  exportarPDF() {
    const node = document.getElementById('content');

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
  }

  onCancelar() {
    this.esCancelado.emit(true);
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }
}
