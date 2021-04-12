import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormBuilder, FormArray, FormGroup, FormControl } from '@angular/forms';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { RubroPresupuestal } from 'src/app/_models/rubroPresupuestal';
import { ListaService } from 'src/app/_services/lista.service';

@Component({
  selector: 'app-popup-rubro-presupuestal',
  templateUrl: './popup-rubro-presupuestal.component.html',
  styleUrls: ['./popup-rubro-presupuestal.component.scss'],
})
export class PopupRubroPresupuestalComponent implements OnInit {
  title: string;
  rubroPapaId: number;

  listaRubroPresupuestal: RubroPresupuestal[];
  rubroPresupuestalSeleccionado: RubroPresupuestal;
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
    private listaService: ListaService,
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
    this.listaService
      .ObtenerListaRubroPresupuestalPorPapa(
        this.rubroPapaId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<RubroPresupuestal[]>) => {
          this.listaRubroPresupuestal = documentos.result;
          this.pagination = documentos.pagination;

          if (
            this.listaRubroPresupuestal &&
            this.listaRubroPresupuestal.length > 0
          ) {
            for (const detalle of this.listaRubroPresupuestal) {
              this.arrayControls.push(
                new FormGroup({
                  rubroControl: new FormControl(''),
                })
              );
            }
          } else {
            this.alertify.warning('No existen rubros presupuestales');
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
      this.rubroPresupuestalSeleccionado = this.listaRubroPresupuestal.filter(
        (x) => x.rubroPresupuestalId === id
      )[0];
    }

    this.bsModalRef.hide();
    this.bsModalRef.content = this.rubroPresupuestalSeleccionado;
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }
}
