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
  Validators,
} from '@angular/forms';
import * as jsPDF from 'jspdf';
import domtoimage from 'dom-to-image';

import { AlertifyService } from 'src/app/_services/alertify.service';
import { TipoOperacion } from 'src/app/_models/tipoOperacion';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';
import { FormatoSolicitudPagoDto } from 'src/app/_dto/formatoSolicitudPagoDto';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { combineLatest, Subscription } from 'rxjs';
import { PlanPago } from 'src/app/_models/planPago';
import { ListaService } from 'src/app/_services/lista.service';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';
import { SolicitudPagoService } from 'src/app/_services/solicitudPago.service';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PopupSolicitudPagoAprobacionComponent } from './popup-solicitud-pago-aprobacion/popup-solicitud-pago-aprobacion.component';
import { EstadoSolicitudPago } from 'src/app/_models/enum';

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
    private changeDetection: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.createForm();
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

  abrirPopup(tipo: number) {
    const initialState = {
      title: 'REGISTRAR OBSERVACIONES',
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
        { class: 'gray modal-md' }
      )
    );

    const combine = combineLatest(this.modalService.onHidden).subscribe(() =>
      this.changeDetection.markForCheck()
    );

    this.subscriptions.push(
      this.modalService.onHidden.subscribe((reason: string) => {
        if (
          this.bsModalRef.content != null &&
          this.bsModalRef.content.observaciones != null
        ) {
          this.formatoSolicitudPago.observacionesModificacion = this.bsModalRef.content.observaciones;
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
