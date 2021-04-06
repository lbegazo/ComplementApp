import { Component, OnInit, Input } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import {
  FormBuilder,
  FormArray,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { ActividadGeneralService } from 'src/app/_services/actividadGeneral.service';
import { ActividadGeneral } from 'src/app/_models/actividadGeneral';

@Component({
  selector: 'app-popup-rubro-decreto',
  templateUrl: './popup-rubro-decreto.component.html',
  styleUrls: ['./popup-rubro-decreto.component.scss'],
})
export class PopupRubroDecretoComponent implements OnInit {
  title: string;
  radicarFactura: boolean;
  closeBtnName = 'Aceptar';
  list: any[] = [];
  listaActividadGeneral: ActividadGeneral[];
  actividadGeneralSeleccionado: ActividadGeneral;
  arrayControls = new FormArray([]);
  popupForm = new FormGroup({});
  arrayRubro: number[] = [];

  constructor(
    public bsModalRef: BsModalRef,
    private actividadService: ActividadGeneralService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.cargarPlanesPago();
  }

  cargarPlanesPago() {
    this.actividadService.ObtenerActividadesGenerales().subscribe(
      (documentos: ActividadGeneral[]) => {
        this.listaActividadGeneral = documentos;

        if (
          this.listaActividadGeneral &&
          this.listaActividadGeneral.length > 0
        ) {
          for (const detalle of this.listaActividadGeneral) {
            this.arrayControls.push(
              new FormGroup({
                rubroControl: new FormControl('', [Validators.required]),
              })
            );
          }
        } else {
          this.alertify.warning(
            'No existen rubros presupuestales a nivel decreto'
          );
        }
      },
      (error) => {
        this.alertify.error(error);
      }
    );

    this.popupForm = this.fb.group({
      planPagoControles: this.arrayControls,
    });
  }

  onCheckChange(event) {
    /* Selected */
    this.arrayRubro = [];
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.arrayRubro?.push(+event.target.value);
    } else {
      /* unselected */
      let index = 0;
      let i = 0;
      this.arrayRubro.forEach((val: number) => {
        if (val === event.target.value) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.arrayRubro.splice(index, 1);
      }
    }
  }

  onAceptar() {
    if (this.arrayRubro && this.arrayRubro.length > 0) {
      const id = this.arrayRubro[0];
      this.actividadGeneralSeleccionado = this.listaActividadGeneral.filter(
        (x) => x.actividadGeneralId === id
      )[0];
    }

    this.bsModalRef.hide();
    this.bsModalRef.content = this.actividadGeneralSeleccionado;
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }
}
