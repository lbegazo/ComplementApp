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
import { PlanPago } from 'src/app/_models/planPago';
import { DetallePlanPago } from 'src/app/_models/detallePlanPago';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-formato-causacion-liquidacion',
  templateUrl: './formato-causacion-liquidacion.component.html',
  styleUrls: ['./formato-causacion-liquidacion.component.scss'],
})
export class FormatoCausacionLiquidacionComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('formatoNgForm', { static: true }) formatoNgForm: NgForm;

  @Input() detallePlanPago: DetallePlanPago;
  @Input() formatoCausacionyLiquidacionPago: FormatoCausacionyLiquidacionPago;
  @Output() esCancelado = new EventEmitter<boolean>();

  formatoForm = new FormGroup({});
  arrayControls = new FormArray([]);

  constructor(private alertify: AlertifyService, private fb: FormBuilder) {}

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

  onCancelar() {
    this.detallePlanPago = null;
    this.esCancelado.emit(true);
  }
}
