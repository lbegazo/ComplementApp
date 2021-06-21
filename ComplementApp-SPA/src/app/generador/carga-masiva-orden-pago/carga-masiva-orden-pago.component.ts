import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FileItem, FileUploader, ParsedResponseHeaders } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { Transaccion } from 'src/app/_models/transaccion';
import { ActivatedRoute } from '@angular/router';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { CargaObligacionService } from 'src/app/_services/cargaObligacion.service';
import { Cdp } from 'src/app/_models/cdp';
import { TipoArchivoObligacion } from 'src/app/_models/enum';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-carga-masiva-orden-pago',
  templateUrl: './carga-masiva-orden-pago.component.html',
  styleUrls: ['./carga-masiva-orden-pago.component.scss'],
})
export class CargaMasivaOrdenPagoComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;
  mostrarActualizarInformacion = false;

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;
  baseUrl = environment.apiUrl;
  path: string;

  facturaHeaderForm = new FormGroup({});
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

  arrayControls = new FormArray([]);
  listaPlanPago: Cdp[] = [];

  constructor(
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private cargaObligacionService: CargaObligacionService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.createForm();
    this.onBuscarFactura();

    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'cargaobligacion/upload',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['xls'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
      queueLimit: 1,
    });

    this.uploader.onErrorItem = (item, response, status, headers) =>
      this.onErrorItem(item, response, status, headers);
    this.uploader.onSuccessItem = (item, response, status, headers) =>
      this.onSuccessItem(item, response, status, headers);

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      planPagoControles: this.arrayControls,
    });
  }

  onBuscarFactura() {
    this.cargaObligacionService
      .ObtenerListaCargaObligacion(
        'Generada',
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Cdp[]>) => {
          this.listaPlanPago = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.facturaHeaderForm = this.fb.group({
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaPlanPago && this.listaPlanPago.length > 0) {
      for (const detalle of this.listaPlanPago) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning('No existen ordenes de pago');
    }
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  descargarArchivo(response) {
    let fileName = '';
    switch (response.type) {
      case HttpEventType.Response:
        if (response.body !== null) {
          const downloadedFile = new Blob([response.body], {
            type: response.body.type,
          });

          const nombreArchivo = response.headers.get('filename');

          if (nombreArchivo != null && nombreArchivo.length > 0) {
            fileName = nombreArchivo + '.txt';
          }

          const a = document.createElement('a');
          a.setAttribute('style', 'display:none;');
          document.body.appendChild(a);
          a.download = fileName;
          a.href = URL.createObjectURL(downloadedFile);
          a.target = '_blank';
          a.click();
          document.body.removeChild(a);
        }
        break;
    }
  }

  public DescargarArchivoDetalleLiquidacion() {

    //#region Cabecera

    let tipoArchivoObligacion = TipoArchivoObligacion.Cabecera.value;

    this.cargaObligacionService
      .DescargarArchivoCargaObligacion(tipoArchivoObligacion, 'Generada')
      .subscribe(
        (response) => {
          this.descargarArchivo(response);
        },
        (error) => {
          this.alertify.warning(error);
        },
        () => {
          //#region Item

          tipoArchivoObligacion = TipoArchivoObligacion.Item.value;

          this.cargaObligacionService
            .DescargarArchivoCargaObligacion(tipoArchivoObligacion, 'Generada')
            .subscribe(
              (response) => {
                this.descargarArchivo(response);
              },
              (error) => {
                this.alertify.warning(error);
              },
              () => {}
            );

          //#endregion Item
        }
      );

    //#endregion Cabecera
  }

  onSuccessItem(
    item: FileItem,
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    if (status === 200) {
      this.alertify.success('La base de datos se actualizó correctamente');
      this.mostrarActualizarInformacion = false;
      this.onBuscarFactura();
    } else {
      this.alertify.error('No se pudo actualizar la base de datos');
      this.mostrarActualizarInformacion = true;
    }
  }

  onErrorItem(
    item: FileItem,
    response: any,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    if (response) {
      const resultado = JSON.parse(response);
      this.alertify.error(
        'Ocurrió un error al ejecutar el proceso ' + resultado.message
      );
    }
  }
}
