import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import {
  FormBuilder,
  FormArray,
  FormGroup,
  FormControl,
} from '@angular/forms';
import { ActividadGeneralService } from 'src/app/_services/actividadGeneral.service';
import { ActividadEspecifica } from 'src/app/_models/actividadEspecifica';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';

@Component({
  selector: 'app-popup-actividad-especifica',
  templateUrl: './popup-actividad-especifica.component.html',
  styleUrls: ['./popup-actividad-especifica.component.css'],
})
export class PopupActividadEspecificaComponent implements OnInit {
  title: string;
  listaActividadEspecifica: ActividadEspecifica[];
  actividadEspecificaSeleccionado: ActividadEspecifica;
  arrayControls = new FormArray([]);
  popupForm = new FormGroup({});
  arrayRubro: number[] = [];

  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

  constructor(
    public bsModalRef: BsModalRef,
    private actividadService: ActividadGeneralService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.cargarPlanesPago();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.cargarPlanesPago();
  }

  cargarPlanesPago() {
    this.actividadService
      .ObtenerListaActividadEspecifica(
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<ActividadEspecifica[]>) => {
          this.listaActividadEspecifica = documentos.result;
          this.pagination = documentos.pagination;

          if (
            this.listaActividadEspecifica &&
            this.listaActividadEspecifica.length > 0
          ) {
            for (const detalle of this.listaActividadEspecifica) {
              this.arrayControls.push(
                new FormGroup({
                  rubroControl: new FormControl(''),
                })
              );
            }
          } else {
            this.alertify.warning('No existen actividades específicas');
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
      this.actividadEspecificaSeleccionado = this.listaActividadEspecifica.filter(
        (x) => x.actividadEspecificaId === id
      )[0];
    }

    this.bsModalRef.hide();
    this.bsModalRef.content = this.actividadEspecificaSeleccionado;
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }
}
