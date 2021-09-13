import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FileItem, FileUploader, ParsedResponseHeaders } from 'ng2-file-upload';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-popup-cargar-archivos',
  templateUrl: './popup-cargar-archivos.component.html',
  styleUrls: ['./popup-cargar-archivos.component.scss'],
})
export class PopupCargarArchivosComponent implements OnInit {
  title: string;
  respuesta = '';

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;
  baseUrl = environment.apiUrl;
  path: string;

  constructor(
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    public bsModalRef: BsModalRef
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'documento/CargarArchivosParaSolicitudPago',
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
      this.alertify.success('El archivo fue anexado exitosamente');
    } else {
      this.alertify.error('No se pudo anexar el archivo');
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

  onAceptar() {
    this.bsModalRef.hide();
    this.respuesta = 'DONE';
  }
}
