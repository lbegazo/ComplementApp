import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/memberlist.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { UsuarioDetalleResolver } from './_resolvers/usuario-detalle.resolver';
import { UsuarioStartComponent } from './Usuario/usuario-start/usuario-start.component';
import { UsuarioEditComponent } from './Usuario/usuario-edit/usuario-edit.component';
import { UsuarioDetailComponent } from './Usuario/usuario-detail/usuario-detail.component';
import { UsuarioMainComponent } from './Usuario/usuario-main/usuario-main.component';
import { CdpEditComponent } from './solicitudCdp/cdp-edit/cdp-edit.component';
import { CdpMainComponent } from './solicitudCdp/cdp-main/cdp-main.component';
import { ArchivoMainComponent } from './archivo/archivo-main/archivo-main.component';
import { PreventUnsavedChangesUsuario } from './_guards/prevent-unsaved-changes-usuario.guard';

export const appRoutes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
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
    path: '',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    children: [
      {
        path: 'cdp',
        component: CdpMainComponent,
        children: [
          {
            path: ':id/edit',
            component: CdpEditComponent,
          },
        ],
      },
      {
        path: 'archivo',
        component: ArchivoMainComponent,
      },
      {
        path: 'members',
        component: MemberListComponent,
        resolve: { users: MemberListResolver },
      },
      {
        path: 'members/:id',
        component: MemberDetailComponent,
        resolve: { user: MemberDetailResolver },
      },
      {
        path: 'member/edit',
        component: MemberEditComponent,
        resolve: { user: MemberEditResolver },
        canDeactivate: [PreventUnsavedChanges],
      },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent },
    ],
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];
