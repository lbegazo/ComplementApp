import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormBuilder, FormArray, FormGroup, FormControl } from '@angular/forms';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Cdp } from 'src/app/_models/cdp';
import { CdpService } from 'src/app/_services/cdp.service';
import { HttpEventType } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-popup-detalle-plan-historico',
  templateUrl: './popup-detalle-plan-historico.component.html',
  styleUrls: ['./popup-detalle-plan-historico.component.scss']
})
export class PopupDetallePlanHistoricoComponent implements OnInit {

  title: string;
  cdp: number;
  instancia: number;
  listaCdp: Cdp[];
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
    private fb: FormBuilder,
    private router: Router
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
      .ObtenerDetallePlanAnualAdquisicion(
        this.cdp,
        this.instancia,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Cdp[]>) => {
          this.listaCdp = documentos.result;
          this.pagination = documentos.pagination;

          if (this.listaCdp && this.listaCdp.length > 0) {
            for (const detalle of this.listaCdp) {
              this.arrayControls.push(
                new FormGroup({
                  rubroControl: new FormControl(''),
                })
              );
            }
          } else {
            this.alertify.warning(
              'No se pudo obtener información para el plan anual de adquisiciones histórico para el cdp: ' +
                this.cdp
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

  onAceptar() {
    this.bsModalRef.hide();
  }

  exportarExcel() {
    let fileName = '';

    this.cdpService.DescargarDetallePlanAnualAdquisicion(this.cdp, this.instancia).subscribe(
      (response) => {
        switch (response.type) {
          case HttpEventType.Response:
            const downloadedFile = new Blob([response.body], {
              type: response.body.type,
            });

            const nombreArchivo = response.headers.get('filename');

            if (nombreArchivo != null && nombreArchivo.length > 0) {
              fileName = nombreArchivo;
            }

            const a = document.createElement('a');
            a.setAttribute('style', 'display:none;');
            document.body.appendChild(a);
            a.download = fileName;
            a.href = URL.createObjectURL(downloadedFile);
            a.target = '_blank';
            a.click();
            document.body.removeChild(a);
            break;
        }
      },
      (error) => {
        this.alertify.warning(error);
      },
      () => {}
    );
  }

  get planPagoControles() {
    return this.popupForm.get('planPagoControles') as FormArray;
  }
}
