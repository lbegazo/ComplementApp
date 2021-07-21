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
import { Transaccion } from './_models/transaccion';
import { AlertifyService } from './_services/alertify.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

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
  subscription: Subscription;
  model: any = {};
  constructor(
    @Inject(DOCUMENT) private document: Document,
    private navService: NavService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    this.document.documentElement.lang = 'es';

    this.transacciones = [];
    this.subscription = this.authService.transaccionChanged.subscribe(
      (lista: Transaccion[]) => {
        this.transacciones = lista;
        this.guardarListaTransacciones();
      }
    );
  }

  ngAfterViewInit() {
    this.navService.appDrawer = this.appDrawer;
  }

  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Login correcto!!!');
        this.model = {};
      },
      () => {},
      () => {
        this.router.navigate(['/home']);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  guardarListaTransacciones() {
    localStorage.setItem('Transacciones', JSON.stringify(this.transacciones));
  }

  @HostListener('window:beforeunload', ['$event'])
  clearLocalStorage(event) {
    localStorage.clear();
    this.model = {};
  }

  //#region No Eliminar

  /*
  llenarNavItem() {
    this.navItems = [];
    if (this.transacciones && this.transacciones.length > 0) {
      for (const element of this.transacciones) {
        const newItem: NavItem = {
          displayName: element.nombre,
          route: element.ruta,
          iconName: element.icono,
        };

        if (element.hijos && element.hijos.length > 0) {
          newItem.children = [];
          for (const hijo of element.hijos) {
            const newItemHijo: NavItem = {
              displayName: hijo.nombre,
              route: hijo.ruta,
              iconName: hijo.icono,
            };

            if (hijo.hijos && hijo.hijos.length > 0) {
              newItemHijo.children = [];
              for (const nieto of hijo.hijos) {
                const newItemNieto: NavItem = {
                  displayName: hijo.nombre,
                  route: hijo.ruta,
                  iconName: hijo.icono,
                };
                newItemHijo.children.push(newItemNieto);
              }
            }

            newItem.children.push(newItemHijo);
          }
        }
        this.navItems.push(newItem);
      }
    } else {
      this.alertify.warning('El usuario no tiene transacciones configuradas');
    }
  }
  */

  //#endregion No eliminar
}
