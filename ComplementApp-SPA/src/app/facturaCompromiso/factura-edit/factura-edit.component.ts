import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  FormControl,
  Validators,
} from '@angular/forms';
import { PlanPago } from 'src/app/_models/planPago';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { PlanPagoService } from 'src/app/_services/planPago.service';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { Router, ActivatedRoute } from '@angular/router';
import { formatDate } from '@angular/common';
import { DetallePlanPago } from 'src/app/_models/detallePlanPago';

@Component({
  selector: 'app-factura-edit',
  templateUrl: './factura-edit.component.html',
  styleUrls: ['./factura-edit.component.css'],
})
export class FacturaEditComponent implements OnInit {
  @Input() planPagoId: number;
  @Input() esRadicarFactura: boolean;
  @Output() isSavedEvent = new EventEmitter<boolean>();
  planPagoSeleccionado: PlanPago;
  facturaForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  detallePlanPago: DetallePlanPago;


  constructor(
    private alertify: AlertifyService,
    private facturaService: PlanPagoService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    this.obtenerPlanPago();
  }

  createEmptyForm() {
    this.facturaForm = this.fb.group({
      numeroContratistaCtrl: ['', Validators.required],
      fechaContratistaCtrl: [null, Validators.required],
      numeroSupervisorCtrl: ['', Validators.required],
      fechaSupervisorCtrl: [null, Validators.required],
      numeroFacturaCtrl: ['', Validators.required],
      valorFacturaCtrl: ['', Validators.required],
      observacionesCtrl: ['', Validators.required],
      detalleCtrl: [],
    });
  }

  createFullForm() {
    //#region Modificar Factura

    if (
      this.planPagoSeleccionado &&
      this.planPagoSeleccionado?.planPagoId > 0
    ) {
      let numeroProveedor = '';
      let numeroSupervisor = '';
      let numeroFactura = '';
      let valorFactura = 0;
      let fechaProveedor = null;
      let fechaSupervisor = null;
      numeroProveedor = this.planPagoSeleccionado?.numeroRadicadoProveedor;
      numeroSupervisor = this.planPagoSeleccionado?.numeroRadicadoSupervisor;
      numeroFactura = this.planPagoSeleccionado?.numeroFactura;
      valorFactura = this.planPagoSeleccionado?.valorFacturado;
      fechaProveedor = this.planPagoSeleccionado?.fechaRadicadoProveedor;
      fechaSupervisor = this.planPagoSeleccionado?.fechaRadicadoSupervisor;

      this.facturaForm = this.fb.group({
        numeroContratistaCtrl: [numeroProveedor, Validators.required],
        fechaContratistaCtrl: [
          formatDate(fechaProveedor, 'dd-MM-yyyy', 'en'),
          Validators.required,
        ],
        numeroSupervisorCtrl: [numeroSupervisor, Validators.required],
        fechaSupervisorCtrl: [
          formatDate(fechaSupervisor, 'dd-MM-yyyy', 'en'),
          Validators.required,
        ],
        numeroFacturaCtrl: [numeroFactura, Validators.required],
        valorFacturaCtrl: [valorFactura, Validators.required],
        observacionesCtrl: [
          this.planPagoSeleccionado?.observaciones,
          Validators.required,
        ],
        detalleCtrl: [],
      });
    }
    //#endregion Modificar Factura
  }

  obtenerPlanPago() {
    this.facturaService.ObtenerPlanPago(this.planPagoId).subscribe(
      (documento: PlanPago) => {
        this.planPagoSeleccionado = documento;

        this.facturaService
          .ObtenerDetallePlanPago(this.planPagoId)
          .subscribe((response: DetallePlanPago) => {
            this.detallePlanPago = response;
          });
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        //#region Create form

        if (
          this.planPagoSeleccionado &&
          this.planPagoSeleccionado.planPagoId > 0
        ) {
          if (this.esRadicarFactura) {
            //#region RadicarFactura
            this.createEmptyForm();
            //#endregion RadicarFactura
          } else {
            this.createFullForm();
          }
        }

        //#endregion Create form
      }
    );
  }

  onGuardar() {
    if (this.facturaForm.valid) {
      //#region Read the form

      //#region Read dates

      let dateFechaProveedor = null;
      let dateFechaSupervisor = null;
      const valueFechaContratista = this.facturaForm.get('fechaContratistaCtrl')
        .value;
      const valueFechaSupervisor = this.facturaForm.get('fechaSupervisorCtrl')
        .value;

      if (this.isValidDate(valueFechaContratista)) {
        dateFechaProveedor = valueFechaContratista;
      } else {
        if (valueFechaContratista && valueFechaContratista.indexOf('-') > -1) {
          dateFechaProveedor = this.dateString2Date(valueFechaContratista);
        }
      }

      if (this.isValidDate(valueFechaSupervisor)) {
        dateFechaSupervisor = valueFechaSupervisor;
      } else {
        if (valueFechaSupervisor && valueFechaSupervisor.indexOf('-') > -1) {
          dateFechaSupervisor = this.dateString2Date(valueFechaSupervisor);
        }
      }

      //#endregion Read dates

      this.planPagoSeleccionado.numeroRadicadoProveedor = this.facturaForm.get(
        'numeroContratistaCtrl'
      ).value;
      this.planPagoSeleccionado.fechaRadicadoProveedor = dateFechaProveedor;
      this.planPagoSeleccionado.numeroRadicadoSupervisor = this.facturaForm.get(
        'numeroSupervisorCtrl'
      ).value;
      this.planPagoSeleccionado.fechaRadicadoSupervisor = dateFechaSupervisor;
      this.planPagoSeleccionado.numeroFactura = this.facturaForm.get(
        'numeroFacturaCtrl'
      ).value;
      this.planPagoSeleccionado.valorFacturado = this.facturaForm.get(
        'valorFacturaCtrl'
      ).value;
      this.planPagoSeleccionado.observaciones = this.facturaForm.get(
        'observacionesCtrl'
      ).value;
      this.planPagoSeleccionado.esRadicarFactura = this.esRadicarFactura;

      //#endregion Read the form

      this.facturaService
        .ActualizarPlanPago(this.planPagoSeleccionado)
        .subscribe(
          () => {
            this.facturaForm.reset(this.planPagoSeleccionado);
            this.alertify.success('El plan de pagos se modificó correctamente');
          },

          (error) => {
            this.alertify.error(error);
          },
          () => {
            this.isSavedEvent.emit(true);
          }
        );
    }
  }

  onCancelar() {
    this.alertify.error('Cancelado');
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  dateString2Date(dateString: string) {
    const day = +dateString.substr(0, 2);
    const month = +dateString.substr(3, 2) - 1;
    const year = +dateString.substr(6, 4);
    const dateFechaProveedor = new Date(year, month, day);
    return dateFechaProveedor;
  }

  isValidDate(d) {
    return d instanceof Date;
  }
}
