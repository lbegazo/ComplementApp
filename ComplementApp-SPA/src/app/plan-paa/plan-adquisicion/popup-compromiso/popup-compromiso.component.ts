import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import {
  FormBuilder,
  FormArray,
  FormGroup,
  FormControl,
} from '@angular/forms';
import { ActividadEspecifica } from 'src/app/_models/actividadEspecifica';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { CdpService } from 'src/app/_services/cdp.service';
import { Cdp } from 'src/app/_models/cdp';


@Component({
  selector: 'app-popup-compromiso',
  templateUrl: './popup-compromiso.component.html',
  styleUrls: ['./popup-compromiso.component.scss']
})
export class PopupCompromisoComponent implements OnInit {

  title: string;
  listaCdp: Cdp[];
  cdpSeleccionado: Cdp;
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
    private cdpService: CdpService,
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
    this.cdpService
      .ObtenerListaCompromiso(
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Cdp[]>) => {
          this.listaCdp = documentos.result;
          this.pagination = documentos.pagination;

          if (
            this.listaCdp &&
            this.listaCdp.length > 0
          ) {
            for (const detalle of this.listaCdp) {
              this.arrayControls.push(
                new FormGroup({
                  rubroControl: new FormControl(''),
                })
              );
            }
          } else {
            this.alertify.warning('No existen compromisos');
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
      const crp = this.arrayRubro[0];
      this.cdpSeleccionado = this.listaCdp.filter(
        (x) => x.crp === crp
      )[0];
    }

    this.bsModalRef.hide();
    this.bsModalRef.content = this.cdpSeleccionado;
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }

}
