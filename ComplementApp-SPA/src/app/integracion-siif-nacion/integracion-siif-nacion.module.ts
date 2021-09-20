import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CargaGestionPresupuestalComponent } from './carga-gestion-presupuestal/carga-gestion-presupuestal.component';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileUploadModule } from 'ng2-file-upload';

@NgModule({
  declarations: [CargaGestionPresupuestalComponent],
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    FileUploadModule
  ],
  providers: [],
  exports: [CargaGestionPresupuestalComponent],
})
export class IntegracionSiifNacionModule {}
