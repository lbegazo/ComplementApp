import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { NavService } from '../_services/nav.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { Transaccion } from '../_models/transaccion';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.scss'],
})
export class TopNavComponent implements OnInit {
  @ViewChild('loginForm', { static: true }) loginForm: NgForm;

  model: any = {};
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    public navService: NavService
  ) {}

  ngOnInit() {}

  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Login correctoo!!!');
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/home']);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  esAdministrador() {
    return this.authService.esAdministrador();
  }

  logout() {
    this.model = {};
    localStorage.removeItem('token');
    this.alertify.message('Su sesión ha sido cerrada');
    this.router.navigate(['/home']);
  }
}
