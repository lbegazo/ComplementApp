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
  Validators,
} from '@angular/forms';
import * as jsPDF from 'jspdf';
import domtoimage from 'dom-to-image';

import { DetallePlanPago } from 'src/app/_models/detallePlanPago';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { PlanPagoService } from 'src/app/_services/planPago.service';
import { ListaService } from 'src/app/_services/lista.service';
import { TipoOperacion } from 'src/app/_models/tipoOperacion';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';

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
  listaMeses: TipoOperacion[] = [];
  mesSaludActual = '';

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);

  constructor(
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private liquidacionService: DetalleLiquidacionService,
  ) {}

  ngOnInit() {
    this.cargarMesesAnio();
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
      tOperacionControl: ['', Validators.required],
    });

    if (
      this.detallePlanPago.modalidadContrato !== 1 ||
      (this.detallePlanPago.modalidadContrato === 1 &&
        this.detallePlanPago.viaticos)
    ) {
      this.tOperacionControl.disable();
    }
  }

  registrarLiquidacion() {
    if (this.formatoForm.valid) {
      let detalleLiquidacionId = 0;
      this.formatoCausacionyLiquidacionPago.textoComprobanteContable = this.textoComprobanteContable;
      this.formatoCausacionyLiquidacionPago.modalidadContrato = this.detallePlanPago.modalidadContrato;

      this.liquidacionService
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
  }

  rechazarLiquidacion() {
    let mensaje = '';
    let planPagoId = 0;
    this.alertify.confirm2(
      'Formato de Causación y Liquidación',
      '¿Esta seguro que desea rechazar el plan de pago?',
      () => {
        mensaje = window.prompt('Motivo de rechazo: ', '');
        if (mensaje.length === 0) {
          this.alertify.warning('Debe ingresar el motivo de rechazo');
        } else {
          this.liquidacionService
            .RechazarDetalleLiquidacion(
              this.formatoCausacionyLiquidacionPago.planPagoId,
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
              () => {}
            );
        }
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

  onSeleccionarMesActual() {
    if (this.tOperacionControl) {
      this.formatoCausacionyLiquidacionPago.numeroMesSaludActual = (this
        .tOperacionControl.value as TipoOperacion).tipoOperacionId;
      this.mesSaludActual = (this.tOperacionControl
        .value as TipoOperacion).nombre.toUpperCase();
    }
  }

  get tOperacionControl() {
    return this.formatoForm.get('tOperacionControl');
  }

  cargarMesesAnio() {
    let mes: TipoOperacion = { tipoOperacionId: 0, codigo: '', nombre: '' };

    for (let index = 1; index <= 12; index++) {
      mes = { tipoOperacionId: 0, codigo: '', nombre: '' };
      mes.tipoOperacionId = index;

      switch (index) {
        case 1:
          mes.nombre = 'Enero';
          break;
        case 2:
          mes.nombre = 'Febrero';
          break;
        case 3:
          mes.nombre = 'Marzo';
          break;
        case 4:
          mes.nombre = 'Abril';
          break;
        case 5:
          mes.nombre = 'Mayo';
          break;
        case 6:
          mes.nombre = 'Junio';
          break;
        case 7:
          mes.nombre = 'Julio';
          break;
        case 8:
          mes.nombre = 'Agosto';
          break;
        case 9:
          mes.nombre = 'Septiembre';
          break;
        case 10:
          mes.nombre = 'Octubre';
          break;
        case 11:
          mes.nombre = 'Noviembre';
          break;
        case 12:
          mes.nombre = 'Diciembre';
          break;
        default:
          break;
      }
      this.listaMeses.push(mes);
    }
  }
}
