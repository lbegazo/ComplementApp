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
  @Output() esCancelado = new EventEmitter<boolean>();
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

  get textoComprobanteContable() {
    return this.textoComprobanteContableEl.nativeElement.innerHTML;
  }
}
