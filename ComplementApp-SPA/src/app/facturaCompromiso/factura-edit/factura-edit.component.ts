import { Component, OnInit, Input } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  FormControl,
  Validators,
} from '@angular/forms';
import { PlanPago } from 'src/app/_models/planPago';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FacturaService } from 'src/app/_services/factura.service';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { Router, ActivatedRoute } from '@angular/router';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-factura-edit',
  templateUrl: './factura-edit.component.html',
  styleUrls: ['./factura-edit.component.css'],
})
export class FacturaEditComponent implements OnInit {
  @Input() planPagoId: number;
  @Input() esRadicarFactura: boolean;
  planPagoSeleccionado: PlanPago;
  facturaForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;

  constructor(
    private alertify: AlertifyService,
    private facturaService: FacturaService,
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
      numeroProveedor = this.planPagoSeleccionado?.numeroRadicadoProveedor;
      numeroSupervisor = this.planPagoSeleccionado?.numeroRadicadoSupervisor;
      numeroFactura = this.planPagoSeleccionado?.numeroFactura;
      valorFactura = this.planPagoSeleccionado?.valorFacturado;

      this.facturaForm = this.fb.group({
        numeroContratistaCtrl: [numeroProveedor, Validators.required],
        fechaContratistaCtrl: [
          formatDate(
            this.planPagoSeleccionado?.fechaRadicadoProveedor,
            'dd-MM-yyyy',
            'en'
          ),
          Validators.required,
        ],
        numeroSupervisorCtrl: [numeroSupervisor, Validators.required],
        fechaSupervisorCtrl: [
          formatDate(
            this.planPagoSeleccionado?.fechaRadicadoSupervisor,
            'dd-MM-yyyy',
            'en'
          ),
          Validators.required,
        ],
        numeroFacturaCtrl: [numeroFactura, Validators.required],
        valorFacturaCtrl: [valorFactura, Validators.required],
        observacionesCtrl: [
          this.planPagoSeleccionado?.observaciones,
          Validators.required,
        ],
      });
    }
    //#endregion Modificar Factura
  }

  obtenerPlanPago() {
    this.facturaService.ObtenerPlanPago(this.planPagoId).subscribe(
      (documento: PlanPago) => {
        this.planPagoSeleccionado = documento;

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
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }


  onGuardar() {
    if (this.facturaForm.valid) {
      this.planPagoSeleccionado.numeroRadicadoProveedor = this.facturaForm.get(
        'numeroContratistaCtrl'
      ).value;
      this.planPagoSeleccionado.fechaRadicadoProveedor = this.facturaForm.get(
        'fechaContratistaCtrl'
      ).value;
      this.planPagoSeleccionado.numeroRadicadoSupervisor = this.facturaForm.get(
        'numeroSupervisorCtrl'
      ).value;
      this.planPagoSeleccionado.fechaRadicadoSupervisor = this.facturaForm.get(
        'fechaSupervisorCtrl'
      ).value;
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

      this.facturaService
        .ActualizarPlanPago(this.planPagoId, this.planPagoSeleccionado)
        .subscribe(
          () => {
            this.alertify.success('El plan de pagos se modificó correctamente');
            this.facturaForm.reset(this.planPagoSeleccionado);
          },

          (error) => {
            this.alertify.error(error);
          },
          () => {
            this.router.navigate(['../'], { relativeTo: this.route });
          }
        );
    }
  }

  onCancelar() {
    this.alertify.error('Cancelado');
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}
