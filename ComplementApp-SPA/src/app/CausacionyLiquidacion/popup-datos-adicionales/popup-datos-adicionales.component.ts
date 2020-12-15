import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';

@Component({
  selector: 'app-popup-datos-adicionales',
  templateUrl: './popup-datos-adicionales.component.html',
  styleUrls: ['./popup-datos-adicionales.component.scss'],
})
export class PopupDatosAdicionalesComponent implements OnInit {
  terId = 0;
  title: string;
  mostrarActividad = false;
  mostrarValor = false;
  valorFacturado = 0;
  listaActividades: ValorSeleccion[] = [];

  popupForm = new FormGroup({});

  idActividadSelecionado = 0;

  datosPopup: any[] = [];

  constructor(
    private liquidacionService: DetalleLiquidacionService,
    private alertify: AlertifyService,
    public bsModalRef: BsModalRef,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    if (!this.mostrarActividad) {
      this.actividadEconomicaCtrl.disable();
    }
    if (!this.mostrarValor) {
      this.valorCtrl.disable();
    }

    this.popupForm.reset();
  }

  createEmptyForm() {
    this.popupForm = this.fb.group({
      actividadEconomicaCtrl: ['', Validators.required],
      valorCtrl: ['', Validators.required],
    });
  }

  onSelectActividadEconomica() {
    this.idActividadSelecionado = this.actividadEconomicaCtrl.value.id;
  }

  onAceptar() {
    if (this.popupForm.valid) {
      let valorIngresado = 0;
      const formValues = Object.assign({}, this.popupForm.value);

      // Se inserta la actividad económica como primer elemento
      if (this.mostrarActividad) {
        if (this.idActividadSelecionado === 0) {
          this.alertify.warning('Debe seleccionar la actividad económica');
          return;
        }
        this.datosPopup.push(this.idActividadSelecionado);
      } else {
        this.datosPopup.push(this.idActividadSelecionado);
      }

      // Se inserta el valor registrado como segundo elemento
      if (this.mostrarValor) {
        const valor = formValues.valorCtrl;

        if (isNaN(+valor)) {
          this.alertify.warning('Debe ingresar un valor númerico');
          return;
        } else if (+valor === 0) {
          this.alertify.warning('El valor ingresado debe ser mayor a cero');
          return;
        } else {
          valorIngresado = +valor;

          if (valorIngresado > this.valorFacturado) {
            this.alertify.warning(
              'Debe ingresar un valor menor al valor total a cancelar ' +
                this.valorFacturado
            );
            return;
          } else {
            this.datosPopup.push(valor);
          }
        }
      } else {
        this.datosPopup.push(0);
      }

      this.bsModalRef.content = this.datosPopup;
      this.bsModalRef.hide();
    }
  }

  get actividadEconomicaCtrl() {
    return this.popupForm.get('actividadEconomicaCtrl');
  }

  get valorCtrl() {
    return this.popupForm.get('valorCtrl');
  }
}
