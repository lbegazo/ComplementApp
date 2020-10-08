import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './_guards/auth.guard';
import { UsuarioMainComponent } from './Usuario/usuario-main/usuario-main.component';
import { UsuarioStartComponent } from './Usuario/usuario-start/usuario-start.component';
import { UsuarioDetailComponent } from './Usuario/usuario-detail/usuario-detail.component';
import { UsuarioEditComponent } from './Usuario/usuario-edit/usuario-edit.component';
import { HomeComponent } from './home/home.component';
import { UsuarioDetalleResolver } from './_resolvers/usuario-detalle.resolver';
import { PreventUnsavedChangesUsuario } from './_guards/prevent-unsaved-changes-usuario.guard';
import { FacturaMainComponent } from './facturaCompromiso/factura-main/factura-main.component';
import { CausacionyLiquidacionComponent } from './CausacionyLiquidacion/CausacionyLiquidacion.component';
import { CdpMainComponent } from './solicitudCdp/cdp-main/cdp-main.component';
import { ArchivoMainComponent } from './archivo/archivo-main/archivo-main.component';
import { PlanPagoResolver } from './_resolvers/planPago.resolver';
import { TransaccionResolver } from './_resolvers/transaccion.resolver';
import { CargaArchivoXmlComponent } from './carga-archivo-xml/carga-archivo-xml.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'ADMINISTRACION_USUARIO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: UsuarioMainComponent,
    resolve: { transaccion: TransaccionResolver },
    children: [
      { path: '', component: UsuarioStartComponent },
      { path: 'new', component: UsuarioEditComponent },
      {
        path: ':id',
        component: UsuarioDetailComponent,
        resolve: { usuario: UsuarioDetalleResolver },
      },
      {
        path: ':id/edit',
        component: UsuarioEditComponent,
        canDeactivate: [PreventUnsavedChangesUsuario],
      },
    ],
  },
  {
    path: 'PLAN_CARGAMASIVA',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ArchivoMainComponent,
    resolve: { transaccion: TransaccionResolver },
  },  
  {
    path: 'TRAMITE_RADICADO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: FacturaMainComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'TRAMITE_LIQUIDACION',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CausacionyLiquidacionComponent,
    resolve: {
      planPagoResolver: PlanPagoResolver,
      transaccion: TransaccionResolver,
    },
  },
  {
    path: 'SOLICITUDES_CERTIFICADO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CdpMainComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'INTEGRACION_GESTIONPRESUPUESTAL',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CargaArchivoXmlComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [],
})
export class AppRoutingModule {}
