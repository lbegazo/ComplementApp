import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  Input,
  Output,
  EventEmitter,
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

import { DetallePlanPago } from 'src/app/_models/detallePlanPago';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { PlanPagoService } from 'src/app/_services/planPago.service';

@Component({
  selector: 'app-formato-causacion-liquidacion',
  templateUrl: './formato-causacion-liquidacion.component.html',
  styleUrls: ['./formato-causacion-liquidacion.component.scss'],
})
export class FormatoCausacionLiquidacionComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('textoComprobanteContableElement')
  textoComprobanteContableEl: ElementRef<HTMLElement>;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;

  @Input() detallePlanPago: DetallePlanPago;
  @Input() formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  @Output() esCancelado = new EventEmitter<boolean>();
  liquidacionRegistrada = false;
  liquidacionRechazada = false;

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);

  constructor(
    private planPagoService: PlanPagoService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.createForm();
  }

  createForm() {
    if (this.formatoCausacionyLiquidacionPago.deducciones != null) {
      for (const detalle of this.formatoCausacionyLiquidacionPago.deducciones) {
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

  registrarLiquidacion() {
    let detalleLiquidacionId = 0;
    this.formatoCausacionyLiquidacionPago.textoComprobanteContable = this.textoComprobanteContable;

    this.planPagoService
      .RegistrarDetalleLiquidacion(this.formatoCausacionyLiquidacionPago)
      .subscribe(
        (response: any) => {
          if (!isNaN(response)) {
            detalleLiquidacionId = +response;
            this.alertify.success(
              'El formato de causación y liquidación se registro correctamente'
            );
            this.liquidacionRegistrada = true;
          } else {
            this.alertify.error(
              'No se pudo registrar el formato de causación y liquidación '
            );
          }
        },

        (error) => {
          this.alertify.error(
            'Hubó un error al registrar la liquidación ' + error
          );
        },
        () => {}
      );
  }

  rechazarLiquidacion() {
    let planPagoId = 0;
    this.alertify.confirm2(
      'Formato de Causación y Liquidación',
      '¿Esta seguro que desea rechazar el plan de pago?',
      () => {
        this.planPagoService
          .RechazarDetalleLiquidacion(
            this.formatoCausacionyLiquidacionPago.planPagoId
          )
          .subscribe(
            (response: any) => {
              if (!isNaN(response)) {
                planPagoId = +response;
                this.alertify.success(
                  'El formato de causación y liquidación se rechazó correctamente'
                );
                this.liquidacionRechazada = true;
              } else {
                this.alertify.error(
                  'No se pudo rechazar el formato de causación y liquidación '
                );
              }
            },

            (error) => {
              this.alertify.error(
                'Hubó un error al rechazar la liquidación ' + error
              );
            },
            () => {}
          );
      }
    );
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
          const pdfHeight = img.height - 10;

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
    this.detallePlanPago = null;
    this.esCancelado.emit(true);
  }

  get textoComprobanteContable() {
    return this.textoComprobanteContableEl.nativeElement.innerHTML;
  }
}
