import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RegistroSolicitudPagoComponent } from './registro-solicitud-pago/registro-solicitud-pago.component';
import { FormatoSolicitudPagoComponent } from './registro-solicitud-pago/formato-solicitud-pago/formato-solicitud-pago.component';
import { PopupCargarArchivosComponent } from './registro-solicitud-pago/formato-solicitud-pago/popup-cargar-archivos/popup-cargar-archivos.component';
import { PopupFacturaComponent } from './registro-solicitud-pago/formato-solicitud-pago/popup-factura/popup-factura.component';
import { PopupSolicitudPagoComponent } from './registro-solicitud-pago/formato-solicitud-pago/popup-solicitud-pago/popup-solicitud-pago.component';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { PaginationModule } from 'ngx-bootstrap/pagination';
// tslint:disable-next-line: max-line-length
import { RegistroAprobacionSolicitudPagoComponent } from './registro-aprobacion-solicitud-pago/registro-aprobacion-solicitud-pago.component';
import { FormatoSolicitudPagoEditComponent } from './registro-aprobacion-solicitud-pago/formato-solicitud-pago-edit/formato-solicitud-pago-edit.component';
import { PopupSolicitudPagoEditComponent } from './registro-aprobacion-solicitud-pago/formato-solicitud-pago-edit/popup-solicitud-pago-edit/popup-solicitud-pago-edit.component';
import { SharedModule } from '../shared/shared.module';
import { AprobacionSolicitudPagoComponent } from './aprobacion-solicitud-pago/aprobacion-solicitud-pago.component';
import { FormatoSolicitudPagoAprobacionComponent } from './aprobacion-solicitud-pago/formato-solicitud-pago-aprobacion/formato-solicitud-pago-aprobacion.component';
import { PopupSolicitudPagoRechazoComponent } from './aprobacion-solicitud-pago/formato-solicitud-pago-aprobacion/popup-solicitud-pago-rechazo/popup-solicitud-pago-rechazo.component';
import { PopupSolicitudPagoAprobacionComponent } from './aprobacion-solicitud-pago/formato-solicitud-pago-aprobacion/popup-solicitud-pago-aprobacion/popup-solicitud-pago-aprobacion.component';

@NgModule({
  declarations: [
    RegistroSolicitudPagoComponent,
    FormatoSolicitudPagoComponent,
    PopupCargarArchivosComponent,
    PopupFacturaComponent,
    PopupSolicitudPagoComponent,
    RegistroAprobacionSolicitudPagoComponent,
    FormatoSolicitudPagoEditComponent,
    PopupSolicitudPagoEditComponent,
    AprobacionSolicitudPagoComponent,
    FormatoSolicitudPagoAprobacionComponent,
    PopupSolicitudPagoRechazoComponent,
    PopupSolicitudPagoAprobacionComponent,
  ],
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    FileUploadModule,
    BsDatepickerModule.forRoot(),
    TypeaheadModule.forRoot(),
    PaginationModule.forRoot(),
    BsDatepickerModule.forRoot(),
    SharedModule
  ],
  exports: [
    RegistroSolicitudPagoComponent,
    FormatoSolicitudPagoComponent,
    PopupCargarArchivosComponent,
    PopupFacturaComponent,
    PopupSolicitudPagoComponent,
  ],
})
export class TramitePagoModule {}
