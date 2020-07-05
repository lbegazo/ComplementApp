import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { JwtModule } from '@auth0/angular-jwt';
import { RouterModule } from '@angular/router';
import { TimeagoModule } from 'ngx-timeago';

import { appRoutes } from './routes';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
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

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
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
    UsuarioStartComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    BsDatepickerModule.forRoot(),
    TimeagoModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    JwtModule.forRoot({
      config: {
        tokenGetter,
        whitelistedDomains: ['localhost:5000'],
        blacklistedRoutes: ['localhost:5000/api/auth'],
      },
    }),
  ],
  providers: [
    ErrorInterceptorProvider,
    AuthService,
    MemberDetailResolver,
    MemberListResolver,
    MemberEditResolver,
    UsuarioDetalleResolver,
    PreventUnsavedChanges,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
