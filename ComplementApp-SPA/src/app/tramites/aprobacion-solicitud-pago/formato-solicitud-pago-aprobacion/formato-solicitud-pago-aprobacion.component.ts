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

@Component({
  selector: 'app-formato-solicitud-pago-aprobacion',
  templateUrl: './formato-solicitud-pago-aprobacion.component.html',
  styleUrls: ['./formato-solicitud-pago-aprobacion.component.css'],
})
export class FormatoSolicitudPagoAprobacionComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;

  @Input() formatoSolicitudPago: FormatoSolicitudPagoDto;
  @Output() esCancelado = new EventEmitter<boolean>();
  formatoSolicitudPagoId = 0;
  rubrosPresupuestales: DetalleCDP[] = [];
  solicitudActualizada = false;

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);

  bsModalRef: BsModalRef;
  formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  subscriptions: Subscription[] = [];

  constructor(
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private solicitudPagoService: SolicitudPagoService,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef,
    private cdpService: CdpService
  ) {}

  ngOnInit() {
    this.createForm();

    this.obtenerRubrosPresupuestales();
  }

  createForm() {
    if (this.formatoSolicitudPago.pagosRealizados != null) {
      for (const detalle of this.formatoSolicitudPago.pagosRealizados) {
        this.arrayControls.push(
          new FormGroup({
            deduccionControl: new FormControl('', []),
          })
        );
      }
    }

    this.formatoForm = this.fb.group({
      deduccionControles: this.arrayControls,
    });
  }

  obtenerRubrosPresupuestales() {
    this.cdpService
      .ObtenerRubrosPresupuestalesPorCompromiso(
        this.formatoSolicitudPago.cdp.crp
      )
      .subscribe((lista: DetalleCDP[]) => {
        if (lista) {
          this.rubrosPresupuestales = lista;
        }
      });
  }

  abrirPopup(tipo: number) {
    const initialState = {
      title: 'REGISTRAR DATOS ADICIONALES',
      rubrosPresupuestales: this.rubrosPresupuestales,
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
          this.formatoSolicitudPago.observacionesModificacion = this.bsModalRef.content.observaciones;
          this.formatoSolicitudPago.numeroRadicadoProveedor = this.bsModalRef.content.numeroContratista;
          this.formatoSolicitudPago.fechaRadicadoProveedor = this.bsModalRef.content.fechaContratista;
          this.formatoSolicitudPago.numeroRadicadoSupervisor = this.bsModalRef.content.numeroSupervisor;
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

                rubro.id = element.rubroPresupuestalId;
                rubro.codigo = element.identificacionRubro;
                rubro.nombre = element.rubroNombre;

                const item: DetalleFormatoSolicitudPagoDto = {
                  detalleFormatoSolicitudPagoId: 0,
                  formatoSolicitudPagoId: this.formatoSolicitudPago
                    .formatoSolicitudPagoId,
                  valorAPagar: element.valorSolicitud,
                  rubroPresupuestal: rubro,
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
