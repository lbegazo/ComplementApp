import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FileItem, FileUploader, ParsedResponseHeaders } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { Transaccion } from 'src/app/_models/transaccion';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-archivo-main',
  templateUrl: './archivo-main.component.html',
  styleUrls: ['./archivo-main.component.css'],
})
export class ArchivoMainComponent implements OnInit {
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
      url: this.baseUrl + 'documento/upload',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['xls'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
      queueLimit: 3,
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
