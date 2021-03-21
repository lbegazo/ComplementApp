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
import { CdpDetalleResolver } from './_resolvers/cdp-detalle.resolver';
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
import { CuentaPorPagarComponent } from './generador/cuenta-por-pagar/cuenta-por-pagar.component';
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
import { RegistroSolicitudPagoComponent } from './tramites/registro-solicitud-pago/registro-solicitud-pago.component';
import { FormatoSolicitudPagoComponent } from './tramites/registro-solicitud-pago/formato-solicitud-pago/formato-solicitud-pago.component';
import { PopupSolicitudPagoComponent } from './tramites/registro-solicitud-pago/formato-solicitud-pago/popup-solicitud-pago/popup-solicitud-pago.component';
import { PopupFacturaComponent } from './tramites/registro-solicitud-pago/formato-solicitud-pago/popup-factura/popup-factura.component';
import { PopupCargarArchivosComponent } from './tramites/registro-solicitud-pago/formato-solicitud-pago/popup-cargar-archivos/popup-cargar-archivos.component';
import { AprobacionSolicitudPagoComponent } from './tramites/aprobacion-solicitud-pago/aprobacion-solicitud-pago.component';
import { FormatoSolicitudPagoAprobacionComponent } from './tramites/aprobacion-solicitud-pago/formato-solicitud-pago-aprobacion/formato-solicitud-pago-aprobacion.component';
import { PopupSolicitudPagoAprobacionComponent } from './tramites/aprobacion-solicitud-pago/formato-solicitud-pago-aprobacion/popup-solicitud-pago-aprobacion/popup-solicitud-pago-aprobacion.component';
import { PopupClavePresupuestalContableComponent } from './administracion/clave-presupuestal-contable/clave-presupuestal-contable-edit/popup-clave-presupuestal-contable/popup-clave-presupuestal-contable.component';
import { ObligacionPresupuestalComponent } from './generador/obligacion-presupuestal/obligacion-presupuestal.component';
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
import { PopupSolicitudPagoRechazoComponent } from './tramites/aprobacion-solicitud-pago/formato-solicitud-pago-aprobacion/popup-solicitud-pago-rechazo/popup-solicitud-pago-rechazo.component';
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
    CuentaPorPagarComponent,
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
    RegistroSolicitudPagoComponent,
    FormatoSolicitudPagoComponent,
    PopupSolicitudPagoComponent,
    PopupFacturaComponent,
    PopupCargarArchivosComponent,
    AprobacionSolicitudPagoComponent,
    FormatoSolicitudPagoAprobacionComponent,
    PopupSolicitudPagoAprobacionComponent,
    PopupSolicitudPagoRechazoComponent,
    ObligacionPresupuestalComponent,
    PlanPagoComponent,
    PlanPagoEditComponent,
    TerceroComponent,
    TerceroEditComponent,
    ContratoEditComponent,
    ContratoComponent,
    DecretoEditComponent,
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
    AppRoutingModule,
    NgxSpinnerModule,
    JwtModule.forRoot({
      config: {
        tokenGetter,
        whitelistedDomains: ['localhost:5000'],
        blacklistedRoutes: ['localhost:5000/api/auth'],
      },
    }),
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'es-Co' },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    AuthService,
    MemberDetailResolver,
    MemberListResolver,
    MemberEditResolver,
    CdpDetalleResolver,
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
