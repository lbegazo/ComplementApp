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
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
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
import { DatePipe, registerLocaleData } from '@angular/common';
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
    MessagesComponent,
    ListsComponent,
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
    PreventUnsavedChanges,
    PreventUnsavedChangesUsuario,
    PreventUnsavedChangesFactura,
    NavService,
    DatePipe
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
  constructor(private bsLocaleService: BsLocaleService) {
    this.bsLocaleService.use('es'); // fecha en español, datepicker
  }
}
