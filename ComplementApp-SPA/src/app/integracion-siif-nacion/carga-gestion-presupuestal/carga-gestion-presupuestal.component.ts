import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FileItem, FileUploader, ParsedResponseHeaders } from 'ng2-file-upload';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-carga-gestion-presupuestal',
  templateUrl: './carga-gestion-presupuestal.component.html',
  styleUrls: ['./carga-gestion-presupuestal.component.css'],
})
export class CargaGestionPresupuestalComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;
  respuesta = '';

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;
  baseUrl = environment.apiUrl;
  path: string;

  constructor(
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
        console.log(this.nombreTransaccion);
      }
    });

    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'documento/ActualizarDocumentosBase',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 20 * 1024 * 1024,
      queueLimit: 4,
    });

    this.uploader.onErrorItem = (item, response, status, headers) =>
      this.onErrorItem(item, response, status, headers);
    this.uploader.onSuccessItem = (item, response, status, headers) =>
      this.onSuccessItem(item, response, status, headers);

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
  }

  onSuccessItem(
    item: FileItem,
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    if (status === 200) {
      let documento = '';
      if (response === '1') {
        documento = 'CDPs';
      } else if (response === '2') {
        documento = 'Compromisos';
      } else if (response === '3') {
        documento = 'Obligaciones';
      } else if (response === '4') {
        documento = 'Ordenes de pago';
      }
      this.alertify.success(
        'El archivo ' + documento + ' fue cargado exitosamente'
      );
    } else {
      this.alertify.error('No se pudo cargar el archivo');
    }
  }

  onErrorItem(
    item: FileItem,
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    this.alertify.error('Ocurrió un error al ejecutar el proceso: ' + response);
  }
}
