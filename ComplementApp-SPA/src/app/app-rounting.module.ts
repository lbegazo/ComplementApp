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

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'usuarios',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: UsuarioMainComponent,
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
    path: 'archivo',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ArchivoMainComponent,
  },
  {
    path: 'factura',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: FacturaMainComponent,
  },
  {
    path: 'causacion',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CausacionyLiquidacionComponent,
    resolve: { planPagoResolver: PlanPagoResolver },
  },
  {
    path: 'cdp',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CdpMainComponent,
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [],
})
export class AppRoutingModule {}
