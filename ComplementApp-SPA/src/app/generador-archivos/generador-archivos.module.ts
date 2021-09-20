import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CargaMasivaOrdenPagoComponent } from './carga-masiva-orden-pago/carga-masiva-orden-pago.component';
import { CuentaPorPagarComponent } from './cuenta-por-pagar/cuenta-por-pagar.component';
import { ObligacionPresupuestalComponent } from './obligacion-presupuestal/obligacion-presupuestal.component';
import { AdministracionArchivosComponent } from './administracion-archivos/administracion-archivos.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { BsDatepickerModule, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { FileUploadModule } from 'ng2-file-upload';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    CargaMasivaOrdenPagoComponent,
    CuentaPorPagarComponent,
    ObligacionPresupuestalComponent,
    AdministracionArchivosComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    PaginationModule.forRoot(),
    BsDatepickerModule.forRoot(),
    FileUploadModule,
    TypeaheadModule.forRoot(),
    SharedModule,
  ]
})
export class GeneradorArchivosModule {

  constructor(private bsLocaleService: BsLocaleService) {
    this.bsLocaleService.use('es'); // fecha en español, datepicker
  }
}
