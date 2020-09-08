import {
  Component,
  OnInit,
  HostListener,
  Inject,
  ElementRef,
  ViewChild,
  VERSION,
  AfterViewInit,
  OnDestroy,
} from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { DOCUMENT } from '@angular/common';
import { NavService } from './_services/nav.service';
import { NavItem } from './_models/nav-item';
import { Transaccion } from './_models/transaccion';
import { AlertifyService } from './_services/alertify.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, AfterViewInit, OnDestroy {
  jwtHelper = new JwtHelperService();
  @ViewChild('appDrawer') appDrawer: ElementRef;
  version = VERSION;
  transacciones: Transaccion[] = [];
  navItems: NavItem[] = [];
  subscription: Subscription;

  constructor(
    @Inject(DOCUMENT) private document: Document,
    private navService: NavService,
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    this.document.documentElement.lang = 'es';

    this.transacciones = [];
    this.navItems = [];
    this.subscription = this.authService.transaccionChanged.subscribe(
      (lista: Transaccion[]) => {
        this.transacciones = lista;
        this.llenarNavItem();
      }
    );
  }

  ngAfterViewInit() {
    this.navService.appDrawer = this.appDrawer;
  }

  obtenerListaTransaccionXUsuario() {}

  llenarNavItem() {
    this.navItems = [];
    if (this.transacciones && this.transacciones.length > 0) {
      for (let x = 0; x < this.transacciones.length; x++) {
        const element = this.transacciones[x];
        const newItem: NavItem = {
          displayName: element.nombre,
          route: element.ruta,
          iconName: element.icono,
        };

        // if (element.hijos && element.hijos.length > 0) {
        //   newItem.children = [];
        //   for (let y = 0; y < element.hijos.length; y++) {
        //     const hijo = element.hijos[y];
        //     const newItemHijo: NavItem = {
        //       displayName: hijo.nombre,
        //       route: hijo.ruta,
        //       iconName: hijo.icono,
        //     };
        //     newItem.children.push(newItemHijo);
        //   }
        // }
        this.navItems.push(newItem);
      }
    } else {
      this.alertify.warning('El usuario no tiene transacciones configuradas');
    }
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  @HostListener('window:beforeunload', ['$event'])
  clearLocalStorage(event) {
    localStorage.clear();
  }
}
