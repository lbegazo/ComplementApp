import { Component, OnInit, ViewChild, OnDestroy, Input } from '@angular/core';
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
  @Input() numeroItems = 0;

  model: any = {};
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    public navService: NavService
  ) {}

  ngOnInit() {
    console.log(this.numeroItems);
  }

  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Login correcto!!!');
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

  logout() {
    this.model = {};
    // localStorage.removeItem('token');
    // localStorage.removeItem('transacciones');
    localStorage.clear();
    this.alertify.message('Su sesión ha sido cerrada');
    this.router.navigate(['/home']);
  }
}
