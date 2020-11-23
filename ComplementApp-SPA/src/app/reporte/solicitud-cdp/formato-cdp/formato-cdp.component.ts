import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import * as jsPDF from 'jspdf';
import domtoimage from 'dom-to-image';

import {
  FormArray,
  FormBuilder,
  FormGroup,
  NgForm,
} from '@angular/forms';
import { SolicitudCDP } from 'src/app/_models/solicitudCDP';
import { TipoOperacion } from 'src/app/_models/tipoOperacion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { CdpService } from 'src/app/_services/cdp.service';
import { ListaService } from 'src/app/_services/lista.service';

@Component({
  selector: 'app-formato-cdp',
  templateUrl: './formato-cdp.component.html',
  styleUrls: ['./formato-cdp.component.scss'],
})
export class FormatoCdpComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('cdpNgForm', { static: true }) cdpNgForm: NgForm;
  today: number = Date.now();
  @Input() solicitudCDPSeleccionado: SolicitudCDP;
  @Output() esCancelado = new EventEmitter<boolean>();

  cambiosConfirmados = false;
  cdpForm = new FormGroup({});

  ordenadorDelGasto = 'ANDREY GEOVANNY RODRIGUEZ LEON';
  contratacion = 'HANS RONALD NIÑO GARCIA';

  constructor(
    private alertify: AlertifyService,
    private authService: AuthService,
    private fb: FormBuilder,
    private listaService: ListaService,
    private cdpService: CdpService
  ) {}

  ngOnInit() {
    this.createCdpForm();
  }

  createCdpForm() {}


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

  get esSolicitudInicial() {
    const idTipoOperacion = this.solicitudCDPSeleccionado?.tipoOperacion
      .tipoOperacionId;
    return idTipoOperacion === 4;
  }

  get esAplicaContrato() {
    return this.solicitudCDPSeleccionado?.aplicaContratoDescripcion.toUpperCase() ===
      'SI'
      ? true
      : false;
  }

  get mostrarTerceraFirma() {
    if (!this.esSolicitudInicial) {
      if (!this.esAplicaContrato) {
        return false;
      } else {
        return true;
      }
    } else {
      return false;
    }
  }

}
