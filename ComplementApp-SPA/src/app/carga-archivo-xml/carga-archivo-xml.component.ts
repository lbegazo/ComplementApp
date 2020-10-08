import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FileItem, FileUploader, ParsedResponseHeaders } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { Transaccion } from '../_models/transaccion';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-carga-archivo-xml',
  templateUrl: './carga-archivo-xml.component.html',
  styleUrls: ['./carga-archivo-xml.component.scss'],
})
export class CargaArchivoXmlComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

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
      }
    });

    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'xmldocumento/upload',
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
      this.alertify.success('La base de datos se actualizó correctamente');
    } else {
      this.alertify.error('No se pudo actualizar la base de datos');
    }
  }

  onErrorItem(
    item: FileItem,
    response: string,
    status: number,
    headers: ParsedResponseHeaders
  ): any {
    this.alertify.error('Ocurrió un error al ejecutar el proceso ' + response);
  }
}
