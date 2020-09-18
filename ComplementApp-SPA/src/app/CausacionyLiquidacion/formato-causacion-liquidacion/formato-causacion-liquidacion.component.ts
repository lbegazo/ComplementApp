import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  Input,
  Output,
  EventEmitter,
} from '@angular/core';
import { NgForm, FormGroup } from '@angular/forms';
import { PlanPago } from 'src/app/_models/planPago';
import { DetallePlanPago } from 'src/app/_models/detallePlanPago';
import { ParametroGeneral } from 'src/app/_models/parametroGeneral';
import { ParametroLiquidacionTercero } from 'src/app/_models/parametroLiquidacionTercero';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';

@Component({
  selector: 'app-formato-causacion-liquidacion',
  templateUrl: './formato-causacion-liquidacion.component.html',
  styleUrls: ['./formato-causacion-liquidacion.component.scss'],
})
export class FormatoCausacionLiquidacionComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;
  formatoForm = new FormGroup({});
  @Input() detallePlanPago: DetallePlanPago;
  @Input() formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  @Output() esCancelado = new EventEmitter<boolean>();

  //#region Variables de valores calculados

  valorUvtParametro = 0;
  valorSMLV = 0;

  honorarioCalculado = 0;
  honorarioUvtCalculado = 0;
  valorIva = 0;

  //#endregion

  constructor() {}

  ngOnInit() {
    //this.obtenerValoresParametrosGenerales();
  }

  onCancelar() {
    this.detallePlanPago = null;
    this.esCancelado.emit(true);
  }

  // private obtenerValoresParametrosGenerales() {
  //   let varValorUvt = 0;
  //   let varValorSMLV = 0;
  //   if (this.parametrosGenerales) {
  //     varValorUvt = +this.parametrosGenerales.filter(
  //       (x) => x.nombre.toUpperCase() === 'VALORUVT'
  //     )[0];

  //     if (varValorUvt) {
  //       this.valorUvtParametro = +varValorUvt;
  //     }

  //     varValorSMLV = +this.parametrosGenerales.filter(
  //       (x) => x.nombre.toUpperCase() === 'SALARIOMINIMO'
  //     )[0];

  //     if (varValorSMLV) {
  //       this.valorSMLV = +varValorSMLV;
  //     }
  //   }
  //}
}
