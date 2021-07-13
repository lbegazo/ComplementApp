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

import { AlertifyService } from 'src/app/_services/alertify.service';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';
import { FormatoSolicitudPagoDto } from 'src/app/_dto/formatoSolicitudPagoDto';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { PlanPago } from 'src/app/_models/planPago';
import { ListaService } from 'src/app/_services/lista.service';
import { FormatoSolicitudPago } from 'src/app/_models/formatoSolicitudPago';
import { SolicitudPagoService } from 'src/app/_services/solicitudPago.service';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { CdpService } from 'src/app/_services/cdp.service';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { Cdp } from 'src/app/_models/cdp';

@Component({
  selector: 'app-detalle-informe-pago-proveedor',
  templateUrl: './detalle-informe-pago-proveedor.component.html',
  styleUrls: ['./detalle-informe-pago-proveedor.component.scss'],
})
export class DetalleInformePagoProveedorComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;

  @Input() formatoSolicitudPago: FormatoSolicitudPagoDto;
  @Input() parametroLiquidacionSeleccionado: ParametroLiquidacionTercero;
  @Input() listaNotasLegales: ValorSeleccion[];
  @Input() listaInformacionPagador: ValorSeleccion[];
  @Output() esCancelado = new EventEmitter<boolean>();

  listaMeses: ValorSeleccion[] = [];
  mesSaludActual = '';
  formatoSolicitudPagoId = 0;
  numeroFactura = '';

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);
  arrayCompromisosControls = new FormArray([]);
  arrayRubrosControls = new FormArray([]);

  listaActividadEconomica: ValorSeleccion[];
  bsModalRef: BsModalRef;
  planPagoSeleccionada: PlanPago = null;
  formatoSolicitudPagoPopup: FormatoSolicitudPago = null;
  formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  idplanPagoSeleccionada = 0;

  notaLegal: ValorSeleccion;
  notaLegal1 = '';
  notaLegal2 = '';
  notaLegal3 = '';
  notaLegal4 = '';
  notaLegal5 = '';
  notaLegal6 = '';

  rubrosPresupuestales: DetalleCDP[] = [];
  cdps: Cdp[] = [];
  compromisos: Cdp[] = [];

  constructor(
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private liquidacionService: DetalleLiquidacionService,
    private solicitudPagoService: SolicitudPagoService,
    private listaService: ListaService,
    private cdpService: CdpService
  ) {}

  ngOnInit() {
    this.cargarNotaLegal();
    this.cargarNotasLegales();
    this.createEmptyForm();
    this.ObtenerRubrosPresupuestalesXNumeroContrato();
    this.cargarListaActividadEconomicaXTercero();
    this.cargarMeses();
  }

  createEmptyForm() {
    this.formatoForm = this.fb.group({
      deduccionControles: this.arrayControls,
      CompromisosControles: this.arrayCompromisosControls,
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

    if (
      this.formatoSolicitudPago.compromisos != null &&
      this.formatoSolicitudPago.compromisos.length > 0
    ) {
      this.compromisos = this.formatoSolicitudPago.compromisos;
      for (const detalle of this.compromisos) {
        this.arrayCompromisosControls.push(
          new FormGroup({
            compromisoControl: new FormControl('', []),
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
      CompromisoControles: this.arrayCompromisosControls,
      rubrosControles: this.arrayRubrosControls,
    });
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
            this.notaLegal1 = (
              this.listaNotasLegales[0] as ValorSeleccion
            ).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal2) {
          if (this.listaNotasLegales[1] != null) {
            this.notaLegal2 = (
              this.listaNotasLegales[1] as ValorSeleccion
            ).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal3) {
          if (this.listaNotasLegales[2] != null) {
            this.notaLegal3 = (
              this.listaNotasLegales[2] as ValorSeleccion
            ).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal4) {
          if (this.listaNotasLegales[3] != null) {
            this.notaLegal4 = (
              this.listaNotasLegales[3] as ValorSeleccion
            ).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal5) {
          if (this.listaNotasLegales[4] != null) {
            this.notaLegal5 = (
              this.listaNotasLegales[4] as ValorSeleccion
            ).valor;
          }
        }
        if (this.parametroLiquidacionSeleccionado.notaLegal6) {
          if (this.listaNotasLegales[5] != null) {
            this.notaLegal6 = (
              this.listaNotasLegales[5] as ValorSeleccion
            ).valor;
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

  ObtenerRubrosPresupuestalesXNumeroContrato() {
    this.cdpService
      .ObtenerRubrosPresupuestalesXNumeroContrato(
        this.formatoSolicitudPago.cdp.detalle6
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
}
