import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { esLocale } from 'ngx-bootstrap/locale';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { JwtModule } from '@auth0/angular-jwt';
import { TimeagoModule } from 'ngx-timeago';
import { FileUploadModule } from 'ng2-file-upload';
import { ModalModule } from 'ngx-bootstrap/modal';
import { CurrencyPipe, DatePipe, registerLocaleData } from '@angular/common';
import localeEsCo from '@angular/common/locales/es-CO';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { PaginationModule } from 'ngx-bootstrap/pagination';

// Angular Material
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatStepperModule } from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';

import { FlexLayoutModule } from '@angular/flex-layout';
import { AppComponent } from './app.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/memberlist.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { UsuarioDetalleResolver } from './_resolvers/usuario-detalle.resolver';
import { UsuarioListComponent } from './Usuario/usuario-list/usuario-list.component';
import { UsuarioEditComponent } from './Usuario/usuario-edit/usuario-edit.component';
import { UsuarioDetailComponent } from './Usuario/usuario-detail/usuario-detail.component';
import { UsuarioItemComponent } from './Usuario/usuario-list/usuario-item/usuario-item.component';
import { UsuarioStartComponent } from './Usuario/usuario-start/usuario-start.component';
import { UsuarioMainComponent } from './Usuario/usuario-main/usuario-main.component';
import { CdpMainComponent } from './solicitudCdp/cdp-main/cdp-main.component';
import { CdpEditComponent } from './solicitudCdp/cdp-edit/cdp-edit.component';
import { ArchivoMainComponent } from './archivo/archivo-main/archivo-main.component';
import { PreventUnsavedChangesUsuario } from './_guards/prevent-unsaved-changes-usuario.guard';
import { CdpHeaderComponent } from './solicitudCdp/cdp-header/cdp-header.component';
import { ItemComponent } from './solicitudCdp/cdp-edit/item/item.component';
import { TwoDigitDecimaNumberDirective } from './_directives/two-digit-decima-number.directive';
import { PopupCdpComponent } from './solicitudCdp/popup-cdp/popup-cdp.component';
import { NumberCommaDirective } from './_directives/number-comma.directive';
import { PreventUnsavedChangesFactura } from './_guards/prevent-unsaved-changes-factura.guard';
import { FacturaMainComponent } from './facturaCompromiso/factura-main/factura-main.component';
import { FacturaEditComponent } from './facturaCompromiso/factura-edit/factura-edit.component';
import { PopupBuscarFacturaComponent } from './facturaCompromiso/popup-buscar-factura/popup-buscar-factura.component';
import { CausacionyLiquidacionComponent } from './CausacionyLiquidacion/CausacionyLiquidacion.component';
import { MenuListItemComponent } from './menu-list-item/menu-list-item.component';
import { NavService } from './_services/nav.service';
import { AppRoutingModule } from './app-rounting.module';
import { TopNavComponent } from './top-nav/top-nav.component';
import { FormatoCausacionLiquidacionComponent } from './CausacionyLiquidacion/formato-causacion-liquidacion/formato-causacion-liquidacion.component';
import { PlanPagoResolver } from './_resolvers/planPago.resolver';
import { TransaccionResolver } from './_resolvers/transaccion.resolver';
import { CargaArchivoXmlComponent } from './carga-archivo-xml/carga-archivo-xml.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { ServerErrorComponent } from './server-error/server-error.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { RadicadoPagoComponent } from './reporte/radicado-pago/radicado-pago.component';
import { LiquidacionPagoComponent } from './reporte/liquidacion-pago/liquidacion-pago.component';
import { FormatoLiquidacionComponent } from './reporte/liquidacion-pago/formato-causacion-liquidacion/formato-liquidacion.component';

import { SolicitudCdpComponent } from './reporte/solicitud-cdp/solicitud-cdp.component';
import { FormatoCdpComponent } from './reporte/solicitud-cdp/formato-cdp/formato-cdp.component';
import { RadicadoPagoMensualComponent } from './reporte/radicado-pago-mensual/radicado-pago-mensual.component';
import { UsuarioDetalleParametroResolver } from './_resolvers/usuario-detalle-parametro.resolver';
import { PopupDatosAdicionalesComponent } from './CausacionyLiquidacion/popup-datos-adicionales/popup-datos-adicionales.component';
import { RelacionContableComponent } from './administracion/relacion-contable/relacion-contable.component';
import { ClavePresupuestalContableComponent } from './administracion/clave-presupuestal-contable/clave-presupuestal-contable.component';
import { ClavePresupuestalContableEditComponent } from './administracion/clave-presupuestal-contable/clave-presupuestal-contable-edit/clave-presupuestal-contable-edit.component';
import { ParametroLiquidacionTerceroComponent } from './administracion/parametro-liquidacion-tercero/parametro-liquidacion-tercero.component';
import { ParametroLiquidacionEditComponent } from './administracion/parametro-liquidacion-tercero/parametro-liquidacion-edit/parametro-liquidacion-edit.component';
import { LongDecimalMaskDirective } from './_directives/long-decimal-mask.directive';
import { PopupClavePresupuestalContableComponent } from './administracion/clave-presupuestal-contable/clave-presupuestal-contable-edit/popup-clave-presupuestal-contable/popup-clave-presupuestal-contable.component';

import { DecimalMaskDirective } from './_directives/decimal-mask.directive';
import { PopupParametroLiquidacionTerceroComponent } from './administracion/parametro-liquidacion-tercero/parametro-liquidacion-edit/popup-parametro-liquidacion-tercero/popup-parametro-liquidacion-tercero.component';
import { PlanPagoComponent } from './administracion/plan-pago/plan-pago.component';
import { PlanPagoEditComponent } from './administracion/plan-pago/plan-pago-edit/plan-pago-edit.component';
import { ListaAreasResolver } from './_resolvers/lista-areas.resolver';
import { ListaCargosResolver } from './_resolvers/lista-cargos.resolver';
import { ListaModalidadContratoResolver } from './_resolvers/lista-ModalidadContrato.resolver';
import { ListaTipoCuentaPorPagarResolver } from './_resolvers/lista-TipoCuentaPorPagar.resolver';
import { ListaTipoDocumentoSoporteResolver } from './_resolvers/lista-TipoDocumentoSoporte.resolver';
import { ListaTipoIvaResolver } from './_resolvers/lista-TipoIva.resolver';
import { ListaTipoPagoResolver } from './_resolvers/lista-TipoPago.resolver';
import { ListaSupervisorResolver } from './_resolvers/lista-Supervisor.resolver';
import { ListaSIoNOResolver } from './_resolvers/lista-SiONo.resolver';
import { ListaAdminPilaResolver } from './_resolvers/lista-AdminPila.resolver';
import { TerceroEditComponent } from './administracion/tercero/tercero-edit/tercero-edit.component';
import { TerceroComponent } from './administracion/tercero/tercero.component';
import { ListaTipoDocumentoIdentidadResolver } from './_resolvers/lista-TipoDocumentoIdentidad.resolver';
import { ContratoEditComponent } from './administracion/contrato/contrato-edit/contrato-edit.component';
import { ContratoComponent } from './administracion/contrato/contrato.component';
import { SpecialCharacterDirective } from './_directives/special-character.directive';
import { ListaTipoContratoResolver } from './_resolvers/lista-TipoContrato.resolver';
import { DecretoEditComponent } from './plan-paa/decreto/decreto-edit/decreto-edit.component';
import { ListaPciResolver } from './_resolvers/lista-Pci.resolver';
import { ListaPerfilesResolver } from './_resolvers/lista-perfiles.resolver';
import { EjecucionPresupuestalComponent } from './plan-paa/ejecucion-presupuestal/ejecucion-presupuestal.component';
import { PopupRubroDecretoComponent } from './plan-paa/ejecucion-presupuestal/popup-rubro-decreto/popup-rubro-decreto.component';
import { PlanAdquisicionComponent } from './plan-paa/plan-adquisicion/plan-adquisicion.component';
import { PopupActividadEspecificaComponent } from './plan-paa/plan-adquisicion/popup-actividad-especifica/popup-actividad-especifica.component';
import { PopupRubroPresupuestalComponent } from './plan-paa/plan-adquisicion/popup-rubro-presupuestal/popup-rubro-presupuestal.component';
import { ListaUsuarioResolver } from './_resolvers/lista-Usuario.resolver';
import { ListaDependenciaResolver } from './_resolvers/lista-Dependencia.resolver';
import { PopupCompromisoComponent } from './plan-paa/plan-adquisicion/popup-compromiso/popup-compromiso.component';
import { SolicitudDisponibilidadPresupuestalComponent } from './SolicitudGestionPresupuestal/SolicitudDisponibilidadPresupuestal/solicitud-disponibilidad-presupuestal/solicitud-disponibilidad-presupuestal.component';
import { SolicitudDisponibilidadPresupuestalEditComponent } from './SolicitudGestionPresupuestal/SolicitudDisponibilidadPresupuestal/solicitud-disponibilidad-presupuestal/solicitud-disponibilidad-presupuestal-edit/solicitud-disponibilidad-presupuestal-edit.component';
import { PlanAnualAdquisicionComponent } from './reporte/plan-anual-adquisicion/plan-anual-adquisicion/plan-anual-adquisicion.component';
import { PopupDetallePlanAdquisicionComponent } from './reporte/plan-anual-adquisicion/plan-anual-adquisicion/popup-detalle-plan-adquisicion/popup-detalle-plan-adquisicion.component';
import { InformePagoProveedorComponent } from './reporte/informe-pago-proveedor/informe-pago-proveedor.component';
import { DetalleInformePagoProveedorComponent } from './reporte/informe-pago-proveedor/detalle-informe-pago-proveedor/detalle-informe-pago-proveedor.component';

import { UsuarioComponent } from './administracion/usuario/usuario.component';
import { UsuarioEditNewComponent } from './administracion/usuario/usuario-edit-new/usuario-edit-new.component';
import { NegativeDecimalMaskDirective } from './_directives/negative-decimal-mask.directive';
import { VincularCdpSolicitudComponent } from './SolicitudGestionPresupuestal/VincularCdpASolicitud/vincular-cdp-solicitud/vincular-cdp-solicitud.component';
import { VincularCdpSolicitudEditComponent } from './SolicitudGestionPresupuestal/VincularCdpASolicitud/vincular-cdp-solicitud/vincular-cdp-solicitud-edit/vincular-cdp-solicitud-edit.component';
import { GraficoTestComponent } from './grafico/test/graficoTest/graficoTest.component';
import { PopupDetallePlanHistoricoComponent } from './reporte/plan-anual-adquisicion/plan-anual-adquisicion/popup-detalle-plan-historico/popup-detalle-plan-historico.component';
import { MetaEjecucionPresupuestalComponent } from './reporte/meta-ejecucion-presupuestal/meta-ejecucion-presupuestal.component';
import { IntegracionSiifNacionModule } from './integracion-siif-nacion/integracion-siif-nacion.module';
import { TramitePagoModule } from './tramite-pago/tramite-pago.module';
import { GeneradorArchivosModule } from './generador-archivos/generador-archivos.module';
import { ListaTipoArchivoResolver } from './_resolvers/lista-TipoArchivo.resolver';
import { PopupUsoRelacionContableComponent } from './administracion/clave-presupuestal-contable/clave-presupuestal-contable-edit/popup-uso-relacion-contable/popup-uso-relacion-contable.component';

defineLocale('es', esLocale);
registerLocaleData(localeEsCo, 'es-Co');

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    MemberCardComponent,
    MemberDetailComponent,
    MemberEditComponent,
    UsuarioMainComponent,
    UsuarioListComponent,
    UsuarioListComponent,
    UsuarioEditComponent,
    UsuarioDetailComponent,
    UsuarioItemComponent,
    UsuarioStartComponent,
    CdpMainComponent,
    CdpEditComponent,
    ArchivoMainComponent,
    CdpHeaderComponent,
    ItemComponent,
    TwoDigitDecimaNumberDirective,
    LongDecimalMaskDirective,
    DecimalMaskDirective,
    NegativeDecimalMaskDirective,
    SpecialCharacterDirective,
    PopupCdpComponent,
    NumberCommaDirective,
    FacturaMainComponent,
    FacturaEditComponent,
    PopupBuscarFacturaComponent,
    CausacionyLiquidacionComponent,
    MenuListItemComponent,
    TopNavComponent,
    FormatoCausacionLiquidacionComponent,
    CargaArchivoXmlComponent,
    ServerErrorComponent,
    NotFoundComponent,
    RadicadoPagoComponent,
    LiquidacionPagoComponent,
    FormatoLiquidacionComponent,
    SolicitudCdpComponent,
    FormatoCdpComponent,
    RadicadoPagoMensualComponent,
    PopupDatosAdicionalesComponent,
    ClavePresupuestalContableComponent,
    ClavePresupuestalContableEditComponent,
    PopupClavePresupuestalContableComponent,
    RelacionContableComponent,
    ParametroLiquidacionTerceroComponent,
    ParametroLiquidacionEditComponent,
    PopupParametroLiquidacionTerceroComponent,
    PlanPagoComponent,
    PlanPagoEditComponent,
    TerceroComponent,
    TerceroEditComponent,
    ContratoEditComponent,
    ContratoComponent,
    DecretoEditComponent,
    EjecucionPresupuestalComponent,
    PopupRubroDecretoComponent,
    PlanAdquisicionComponent,
    PopupActividadEspecificaComponent,
    PopupRubroPresupuestalComponent,
    PopupCompromisoComponent,
    SolicitudDisponibilidadPresupuestalComponent,
    SolicitudDisponibilidadPresupuestalEditComponent,
    PlanAnualAdquisicionComponent,
    PopupDetallePlanAdquisicionComponent,
    PopupDetallePlanHistoricoComponent,
    InformePagoProveedorComponent,
    DetalleInformePagoProveedorComponent,
    UsuarioComponent,
    UsuarioEditNewComponent,
    VincularCdpSolicitudComponent,
    VincularCdpSolicitudEditComponent,
    GraficoTestComponent,
    MetaEjecucionPresupuestalComponent,
    PopupUsoRelacionContableComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    FileUploadModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    ModalModule.forRoot(),
    BsDatepickerModule.forRoot(),
    TimeagoModule.forRoot(),
    TypeaheadModule.forRoot(),
    PaginationModule.forRoot(),

    MatIconModule,
    MatListModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    FlexLayoutModule,
    MatTableModule,
    MatStepperModule,
    MatInputModule,
    MatFormFieldModule,
    MatRadioModule,
    AppRoutingModule,
    NgxSpinnerModule,
    JwtModule.forRoot({
      config: {
        tokenGetter,
        whitelistedDomains: ['localhost:5000'],
        blacklistedRoutes: ['localhost:5000/api/auth'],
      },
    }),
    IntegracionSiifNacionModule,
    TramitePagoModule,
    GeneradorArchivosModule,
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'es-Co' },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    AuthService,
    MemberDetailResolver,
    MemberListResolver,
    MemberEditResolver,
    UsuarioDetalleResolver,
    PlanPagoResolver,
    TransaccionResolver,
    UsuarioDetalleParametroResolver,
    ListaAreasResolver,
    ListaCargosResolver,
    ListaModalidadContratoResolver,
    ListaTipoCuentaPorPagarResolver,
    ListaTipoDocumentoSoporteResolver,
    ListaTipoIvaResolver,
    ListaTipoPagoResolver,
    ListaSupervisorResolver,
    ListaSIoNOResolver,
    ListaAdminPilaResolver,
    ListaTipoDocumentoIdentidadResolver,
    ListaTipoContratoResolver,
    ListaPciResolver,
    ListaPerfilesResolver,
    ListaUsuarioResolver,
    ListaDependenciaResolver,
    ListaTipoArchivoResolver,
    PreventUnsavedChanges,
    PreventUnsavedChangesUsuario,
    PreventUnsavedChangesFactura,
    NavService,
    DatePipe,
    CurrencyPipe,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
  constructor(private bsLocaleService: BsLocaleService) {
    this.bsLocaleService.use('es'); // fecha en español, datepicker
  }
}
