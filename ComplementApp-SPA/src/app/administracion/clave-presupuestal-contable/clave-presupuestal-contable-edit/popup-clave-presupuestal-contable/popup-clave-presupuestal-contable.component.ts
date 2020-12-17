import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ClavePresupuestalContableDto } from 'src/app/_dto/clavePresupuestalContableDto';
import { RelacionContableDto } from 'src/app/_dto/relacionContableDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClavePresupuestalContableService } from 'src/app/_services/clavePresupuestalContable.service';

@Component({
  selector: 'app-popup-clave-presupuestal-contable',
  templateUrl: './popup-clave-presupuestal-contable.component.html',
  styleUrls: ['./popup-clave-presupuestal-contable.component.css'],
})
export class PopupClavePresupuestalContableComponent implements OnInit {
  title: string;
  clavePresupuestalContable: ClavePresupuestalContableDto;

  popupForm = new FormGroup({});
  arrayControls1 = new FormArray([]);
  arrayControls2 = new FormArray([]);

  listaUsoPresupuestal: ValorSeleccion[];
  listaRelacionContable: RelacionContableDto[];
  idUsoPresupuestalSelecionado = 0;
  idRelacionContableSelecionado = 0;
  usoPresupuestalSeleccionado: ValorSeleccion = null;
  relacionContableSeleccionado: RelacionContableDto;

  constructor(
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private clavePresupuestalService: ClavePresupuestalContableService
  ) {}

  ngOnInit() {
    this.createEmptyForm();
    this.cargarUsosPresupuestales();
    this.cargarRelacionesContable();
  }

  crearControlesArrayUsoPresupuestal() {
    if (this.listaUsoPresupuestal && this.listaUsoPresupuestal.length > 0) {
      for (const detalle of this.listaUsoPresupuestal) {
        this.arrayControls1.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning(
        'No existen Usos Presupuestales para el Rubro seleccionado'
      );
    }
  }

  crearControlesArrayRelacionContable() {
    if (this.listaRelacionContable && this.listaRelacionContable.length > 0) {
      for (const detalle of this.listaRelacionContable) {
        this.arrayControls2.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning(
        'No existen Usos Presupuestales para el Rubro seleccionado'
      );
    }
  }

  createEmptyForm() {
    this.popupForm = this.fb.group({
      planPagoControles: this.arrayControls1,
      relacionContableControles: this.arrayControls2,
    });
  }

  onCheckChange(event) {
    /* Selected */
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.idUsoPresupuestalSelecionado = +event.target.value;
      this.usoPresupuestalSeleccionado = this.listaUsoPresupuestal.filter(
        (x) => x.id === this.idUsoPresupuestalSelecionado
      )[0];
    }
  }

  onCheckChangeRelacionContable(event) {
    /* Selected */
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.idRelacionContableSelecionado = +event.target.value;
      this.relacionContableSeleccionado = this.listaRelacionContable.filter(
        (x) => x.relacionContableId === this.idRelacionContableSelecionado
      )[0];
    }
  }

  cargarUsosPresupuestales() {
    this.clavePresupuestalService
      .ObtenerUsosPresupuestalesXRubroPresupuestal(
        this.clavePresupuestalContable.rubroPresupuestal.id
      )
      .subscribe(
        (lista: ValorSeleccion[]) => {
          this.listaUsoPresupuestal = lista;
          this.crearControlesArrayUsoPresupuestal();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {}
      );
  }

  cargarRelacionesContable() {
    this.clavePresupuestalService
      .ObtenerRelacionesContableXRubroPresupuestal(
        this.clavePresupuestalContable.rubroPresupuestal.id
      )
      .subscribe(
        (lista: RelacionContableDto[]) => {
          this.listaRelacionContable = lista;
          this.crearControlesArrayRelacionContable();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {}
      );
  }

  onAceptar() {
    if (
      this.idUsoPresupuestalSelecionado > 0 &&
      this.idRelacionContableSelecionado > 0
    ) {
      this.bsModalRef.hide();
    }
  }
}
