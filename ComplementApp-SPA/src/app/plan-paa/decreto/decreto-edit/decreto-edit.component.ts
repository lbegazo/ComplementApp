import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ValidarValorIngresado } from 'src/app/helpers/validarValorIngresado';
import { ActividadGeneralPrincipalDto } from 'src/app/_dto/actividadGeneralPrincipalDto';
import { ActividadGeneral } from 'src/app/_models/actividadGeneral';
import { Transaccion } from 'src/app/_models/transaccion';
import { ActividadGeneralService } from 'src/app/_services/actividadGeneral.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';

@Component({
  selector: 'app-decreto-edit',
  templateUrl: './decreto-edit.component.html',
  styleUrls: ['./decreto-edit.component.css'],
})
export class DecretoEditComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  listaActividadGeneral: ActividadGeneral[] = [];

  facturaHeaderForm = new FormGroup({});
  arrayControls = new FormArray([]);

  subscriptions: Subscription[] = [];

  valorTotalVigente = 0;
  valorTotalDisponible = 0;
  validacionDisponibleMenorVigente = false;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private actividadGeneralService: ActividadGeneralService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.onBuscarActividadesGenerales();

    this.facturaHeaderForm.setControl('rubrosControles', this.arrayControls);
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      rubrosControles: this.arrayControls,
    });
  }

  onBuscarActividadesGenerales() {
    this.actividadGeneralService.ObtenerActividadesGenerales().subscribe(
      (documentos: ActividadGeneral[]) => {
        this.listaActividadGeneral = documentos;
        this.crearControlesDeArray();
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        if (
          this.listaActividadGeneral &&
          this.listaActividadGeneral.length === 0
        ) {
          this.alertify.warning(
            'No existen rubros presupuestales a nivel decreto'
          );
        }
        this.facturaHeaderForm = this.fb.group({
          rubrosControles: this.arrayControls,
        });
      }
    );
  }

  onGuardar() {
    const actividadGeneralPrincipal: ActividadGeneralPrincipalDto = new ActividadGeneralPrincipalDto();

    if (!this.validarValoresIngresados) {
      if (this.facturaHeaderForm.valid) {
        let respuesta = 0;
        const arrayControles = this.facturaHeaderForm.get(
          'rubrosControles'
        ) as FormArray;
        if (arrayControles && arrayControles.length > 0) {
          actividadGeneralPrincipal.listaActividadGeneral = this.listaActividadGeneral;

          this.actividadGeneralService
            .RegistrarActividadesGenerales(actividadGeneralPrincipal)
            .subscribe(
              (response: any) => {
                if (!isNaN(response)) {
                  respuesta = +response;
                  this.alertify.success(
                    'Se registraron los valores de las apropiaciones asignadas a la entidad a nivel de decreto'
                  );
                } else {
                  this.alertify.error(
                    'No se pudo registrar los valores de las apropiaciones asignadas a la entidad'
                  );
                }
              },

              (error) => {
                this.alertify.error(
                  'Hubó un error al registrar los valores de las apropiaciones asignadas a la entidad ' +
                    error
                );
              },
              () => {}
            );
        }
      }
    } else {
      this.alertify.error(
        'El valor de la apropiación disponible debe ser igual o inferior al valor de la apropiación vigente del rubro.'
      );
    }
  }

  crearControlesDeArray() {
    if (this.listaActividadGeneral && this.listaActividadGeneral.length > 0) {
      for (const detalle of this.listaActividadGeneral) {
        const valorVigente = detalle.apropiacionVigente;
        const valorDisponible = detalle.apropiacionDisponible;

        this.arrayControls.push(
          new FormGroup({
            rubroControlVigente: new FormControl(
              GeneralService.obtenerFormatoLongMoney(valorVigente),
              [Validators.required]
            ),
            rubroControlDisponible: new FormControl(
              GeneralService.obtenerFormatoLongMoney(valorDisponible),
              [
                Validators.required,
                //ValidarValorIngresado.validacionValorIngresado(valorVigente),
              ]
            ),
          })
        );
      }
      this.actualizarValoresTotales();
    }
  }

  get validarValoresIngresados() {
    let respuesta = false;
    if (this.listaActividadGeneral && this.listaActividadGeneral.length > 0) {
      for (const detalle of this.listaActividadGeneral) {
        const valorVigente = +GeneralService.obtenerValorAbsoluto(
          detalle.apropiacionVigente
        );
        const valorDisponible = +GeneralService.obtenerValorAbsoluto(
          detalle.apropiacionDisponible
        );

        if (valorDisponible > valorVigente) {
          respuesta = true;
          return respuesta;
        }
      }
      return respuesta;
    }
  }

  actualizarValoresTotales() {
    let sum = 0;

    const arrayControles = this.rubrosControles;
    if (arrayControles && arrayControles.length > 0) {
      for (let index = 0; index < arrayControles.length; index++) {
        const item = arrayControles.at(index);
        //if ( item.value.rubroControlDisponible <= item.value.rubroControlVigente) {
        const itemDetalle = this.listaActividadGeneral[index];
        itemDetalle.apropiacionVigente = GeneralService.obtenerValorAbsoluto(
          item.value.rubroControlVigente
        );
        itemDetalle.apropiacionDisponible = GeneralService.obtenerValorAbsoluto(
          item.value.rubroControlDisponible
        );
        //}
      }
    }

    this.listaActividadGeneral.forEach(
      (a) => (sum += +a.apropiacionDisponible)
    );
    this.valorTotalDisponible = sum;
    sum = 0;

    this.listaActividadGeneral.forEach((a) => (sum += +a.apropiacionVigente));
    this.valorTotalVigente = sum;
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  get rubrosControles() {
    return this.facturaHeaderForm.get('rubrosControles') as FormArray;
  }
}
