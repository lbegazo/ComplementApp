import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './_guards/auth.guard';
import { HomeComponent } from './home/home.component';
import { UsuarioDetalleResolver } from './_resolvers/usuario-detalle.resolver';
import { PreventUnsavedChangesUsuario } from './_guards/prevent-unsaved-changes-usuario.guard';
import { FacturaMainComponent } from './facturaCompromiso/factura-main/factura-main.component';
import { CausacionyLiquidacionComponent } from './CausacionyLiquidacion/CausacionyLiquidacion.component';
import { ArchivoMainComponent } from './archivo/archivo-main/archivo-main.component';
import { TransaccionResolver } from './_resolvers/transaccion.resolver';
import { CargaArchivoXmlComponent } from './carga-archivo-xml/carga-archivo-xml.component';
import { ServerErrorComponent } from './server-error/server-error.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { RadicadoPagoComponent } from './reporte/radicado-pago/radicado-pago.component';
import { LiquidacionPagoComponent } from './reporte/liquidacion-pago/liquidacion-pago.component';

import { SolicitudCdpComponent } from './reporte/solicitud-cdp/solicitud-cdp.component';
import { RadicadoPagoMensualComponent } from './reporte/radicado-pago-mensual/radicado-pago-mensual.component';
import { ClavePresupuestalContableComponent } from './administracion/clave-presupuestal-contable/clave-presupuestal-contable.component';
import { ParametroLiquidacionTerceroComponent } from './administracion/parametro-liquidacion-tercero/parametro-liquidacion-tercero.component';

import { PlanPagoComponent } from './administracion/plan-pago/plan-pago.component';
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
import { TerceroComponent } from './administracion/tercero/tercero.component';
import { ListaTipoDocumentoIdentidadResolver } from './_resolvers/lista-TipoDocumentoIdentidad.resolver';
import { ContratoComponent } from './administracion/contrato/contrato.component';
import { ListaTipoContratoResolver } from './_resolvers/lista-TipoContrato.resolver';
import { DecretoEditComponent } from './plan-paa/decreto/decreto-edit/decreto-edit.component';
import { ListaPciResolver } from './_resolvers/lista-Pci.resolver';
import { ListaPerfilesResolver } from './_resolvers/lista-perfiles.resolver';
import { EjecucionPresupuestalComponent } from './plan-paa/ejecucion-presupuestal/ejecucion-presupuestal.component';
import { PlanAdquisicionComponent } from './plan-paa/plan-adquisicion/plan-adquisicion.component';
import { ListaDependenciaResolver } from './_resolvers/lista-Dependencia.resolver';
import { ListaUsuarioResolver } from './_resolvers/lista-Usuario.resolver';
import { SolicitudDisponibilidadPresupuestalComponent } from './SolicitudGestionPresupuestal/SolicitudDisponibilidadPresupuestal/solicitud-disponibilidad-presupuestal/solicitud-disponibilidad-presupuestal.component';
import { PlanAnualAdquisicionComponent } from './reporte/plan-anual-adquisicion/plan-anual-adquisicion/plan-anual-adquisicion.component';
import { InformePagoProveedorComponent } from './reporte/informe-pago-proveedor/informe-pago-proveedor.component';

import { UsuarioComponent } from './administracion/usuario/usuario.component';
import { VincularCdpSolicitudComponent } from './SolicitudGestionPresupuestal/VincularCdpASolicitud/vincular-cdp-solicitud/vincular-cdp-solicitud.component';
import { MetaEjecucionPresupuestalComponent } from './reporte/meta-ejecucion-presupuestal/meta-ejecucion-presupuestal.component';
import { CargaGestionPresupuestalComponent } from './integracion-siif-nacion/carga-gestion-presupuestal/carga-gestion-presupuestal.component';
import { RegistroAprobacionSolicitudPagoComponent } from './tramite-pago/registro-aprobacion-solicitud-pago/registro-aprobacion-solicitud-pago.component';
import { CuentaPorPagarComponent } from './generador-archivos/cuenta-por-pagar/cuenta-por-pagar.component';
import { ObligacionPresupuestalComponent } from './generador-archivos/obligacion-presupuestal/obligacion-presupuestal.component';
import { CargaMasivaOrdenPagoComponent } from './generador-archivos/carga-masiva-orden-pago/carga-masiva-orden-pago.component';
import { AdministracionArchivosComponent } from './generador-archivos/administracion-archivos/administracion-archivos.component';
import { ListaTipoArchivoResolver } from './_resolvers/lista-TipoArchivo.resolver';
import { AprobacionSolicitudPagoComponent } from './tramite-pago/aprobacion-solicitud-pago/aprobacion-solicitud-pago.component';
import { RegistroSolicitudPagoComponent } from './tramite-pago/registro-solicitud-pago/registro-solicitud-pago.component';
import { SeguimientoMetasAusteridadComponent } from './reporte/seguimiento-metas-austeridad/seguimiento-metas-austeridad.component';
import { DiasPromedioComponent } from './reporte/dias-promedio/dias-promedio.component';
// tslint:disable-next-line: max-line-length
import { ConsultaEjecucionPresupuestalComponent } from './reporte/consulta-ejecucion-presupuestal/consulta-ejecucion-presupuestal.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    component: HomeComponent,
  },
  // {
  //   path: 'ADMINISTRACION_USUARIO',
  //   canActivate: [AuthGuard],
  //   runGuardsAndResolvers: 'always',
  //   component: UsuarioMainComponent,
  //   resolve: { transaccion: TransaccionResolver },
  //   children: [
  //     { path: '', component: UsuarioStartComponent },
  //     {
  //       path: 'new',
  //       component: UsuarioEditComponent,
  //       resolve: {
  //         areas: ListaAreasResolver,
  //         cargos: ListaCargosResolver,
  //         pcis: ListaPciResolver,
  //         perfiles: ListaPerfilesResolver,
  //       },
  //     },
  //     {
  //       path: ':id',
  //       component: UsuarioDetailComponent,
  //       resolve: { usuario: UsuarioDetalleParametroResolver },
  //     },
  //     {
  //       path: ':id/edit',
  //       component: UsuarioEditComponent,
  //       resolve: {
  //         areas: ListaAreasResolver,
  //         cargos: ListaCargosResolver,
  //         pcis: ListaPciResolver,
  //         perfiles: ListaPerfilesResolver,
  //       },

  //       // canDeactivate: [PreventUnsavedChangesUsuario],
  //     },
  //   ],
  // },
  {
    path: 'ADMINISTRACION_USUARIO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: UsuarioComponent,
    resolve: {
      transaccion: TransaccionResolver,
      tipoDocumentoIdentidad: ListaTipoDocumentoIdentidadResolver,
      areas: ListaAreasResolver,
      cargos: ListaCargosResolver,
      pcis: ListaPciResolver,
      perfiles: ListaPerfilesResolver,
    },
  },
  {
    path: 'ADMINISTRACION_PARAMETROLIQUIDACIONTERCERO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ParametroLiquidacionTerceroComponent,
    resolve: {
      transaccion: TransaccionResolver,
      modalidadContrato: ListaModalidadContratoResolver,
      tipoCuentaPorPagar: ListaTipoCuentaPorPagarResolver,
      tipoDocumentoSoporte: ListaTipoDocumentoSoporteResolver,
      tipoIva: ListaTipoIvaResolver,
      tipoPago: ListaTipoPagoResolver,
      SIoNO: ListaSIoNOResolver,
      adminPila: ListaAdminPilaResolver,
    },
  },
  {
    path: 'ADMINISTRACION_CLAVEPRESUPUESTALCONTABLE',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ClavePresupuestalContableComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'ADMINISTRACION_PLANPAGO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: PlanPagoComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'ADMINISTRACION_TERCERO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: TerceroComponent,
    resolve: {
      transaccion: TransaccionResolver,
      tipoDocumentoIdentidad: ListaTipoDocumentoIdentidadResolver,
    },
  },
  {
    path: 'ADMINISTRACION_CONTRATO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ContratoComponent,
    resolve: {
      transaccion: TransaccionResolver,
      supervisor: ListaSupervisorResolver,
      tipoContrato: ListaTipoContratoResolver,
    },
  },

  {
    path: 'PLAN_CARGAMASIVA',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ArchivoMainComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'PLAN_REGISTRARDECRETO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: DecretoEditComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'PLAN_EJECUCIONPRESUPUESTAL',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: EjecucionPresupuestalComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'PLAN_ADQUISICIONANUAL',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: PlanAdquisicionComponent,
    resolve: {
      transaccion: TransaccionResolver,
      responsable: ListaUsuarioResolver,
      dependencia: ListaDependenciaResolver,
    },
  },

  {
    path: 'SOLICITUDES_CERTIFICADO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: SolicitudDisponibilidadPresupuestalComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'SOLICITUDES_VINCULARCDP',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: VincularCdpSolicitudComponent,
    resolve: { transaccion: TransaccionResolver },
  },

  {
    path: 'GENERADOR_CUENTAPORPAGAR',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CuentaPorPagarComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'GENERADOR_OBLIGACIONES',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ObligacionPresupuestalComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'GENERADOR_ORDENPAGO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CargaMasivaOrdenPagoComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  {
    path: 'GENERADOR_ADMINISTRACIONARCHIVO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: AdministracionArchivosComponent,
    resolve: {
      transaccion: TransaccionResolver,
      tipoArchivo: ListaTipoArchivoResolver,
    },
  },
  {
    path: 'TRAMITE_REGISTRAR',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: RegistroSolicitudPagoComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'TRAMITE_APROBAR',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: AprobacionSolicitudPagoComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'TRAMITE_REGISTRARAPROBAR',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: RegistroAprobacionSolicitudPagoComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
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
      transaccion: TransaccionResolver,
    },
  },
  {
    path: 'CONSULTAS_RADICADOPAGO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: RadicadoPagoComponent,
    resolve: {
      transaccion: TransaccionResolver,
    },
  },
  {
    path: 'CONSULTAS_LIQUIDACION',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: LiquidacionPagoComponent,
    resolve: {
      transaccion: TransaccionResolver,
    },
  },
  {
    path: 'CONSULTAS_SOLICITUDCDP',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: SolicitudCdpComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'CONSULTAS_RADICADOPAGOMENSUAL',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: RadicadoPagoMensualComponent,
    resolve: {
      transaccion: TransaccionResolver,
    },
  },
  {
    path: 'CONSULTAS_PLANANUALADQUISICION',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: PlanAnualAdquisicionComponent,
    resolve: {
      transaccion: TransaccionResolver,
    },
  },
  {
    path: 'CONSULTAS_INFORMEPAGO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: InformePagoProveedorComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'CONSULTAS_METAS',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: MetaEjecucionPresupuestalComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'CONSULTAS_AUSTERIDADGASTO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: SeguimientoMetasAusteridadComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'CONSULTAS_EJECUCIONPRESUPUESTAL',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: ConsultaEjecucionPresupuestalComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'CONSULTAS_DIASPROMEDIO',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: DiasPromedioComponent,
    resolve: {
      transaccion: TransaccionResolver,
      usuarioLogueado: UsuarioDetalleResolver,
    },
  },
  {
    path: 'INTEGRACION_GESTIONPRESUPUESTAL',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    component: CargaGestionPresupuestalComponent,
    resolve: { transaccion: TransaccionResolver },
  },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [],
})
export class AppRoutingModule {}
