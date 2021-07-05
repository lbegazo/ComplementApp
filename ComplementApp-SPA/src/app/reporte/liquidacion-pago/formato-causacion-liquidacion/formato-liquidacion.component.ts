import { ElementRef, Input, Output, ViewChild } from '@angular/core';
import { Component, EventEmitter, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  NgForm,
} from '@angular/forms';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { PlanPagoService } from 'src/app/_services/planPago.service';
import * as jsPDF from 'jspdf';
import domtoimage from 'dom-to-image';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';

@Component({
  selector: 'app-formato-liquidacion',
  templateUrl: './formato-liquidacion.component.html',
  styleUrls: ['./formato-liquidacion.component.scss'],
})
export class FormatoLiquidacionComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('textoComprobanteContableElement')
  textoComprobanteContableEl: ElementRef<HTMLElement>;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;

  @Input() formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  @Input() sePuedeRechazar: boolean;
  @Output() esCancelado = new EventEmitter<boolean>();
  liquidacionRechazada = false;

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);

  constructor(
    private liquidacionService: DetalleLiquidacionService,
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
    this.esCancelado.emit(true);
  }

  rechazarLiquidacion(){
    let mensaje = '';
    let planPagoId = 0;
    this.alertify.confirm2(
      'Formato de Causación y Liquidación',
      '¿Esta seguro que desea rechazar la liquidación?',
      () => {
        mensaje = window.prompt('Motivo de rechazo: ', '');
        if (mensaje.length === 0) {
          this.alertify.warning('Debe ingresar el motivo de rechazo');
        } else {
          this.liquidacionService
            .RechazarLiquidacion(
              this.formatoCausacionyLiquidacionPago.detalleLiquidacionId,
              this.formatoCausacionyLiquidacionPago.planPagoId,
              this.formatoCausacionyLiquidacionPago.formatoSolicitudPagoId,
              mensaje
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
              () => {
                this.onCancelar();
              }
            );
        }
      }
    );
  }

  get textoComprobanteContable() {
    return this.textoComprobanteContableEl.nativeElement.innerHTML;
  }
}
