import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { NavService } from '../_services/nav.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { Transaccion } from '../_models/transaccion';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.scss'],
})
export class TopNavComponent implements OnInit {
  jwtHelper = new JwtHelperService();
  @ViewChild('loginForm', { static: true }) loginForm: NgForm;
  @Input() esQA: boolean;

  model: any = {};
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    public navService: NavService
  ) {}

  ngOnInit() {
    // console.log(this.esQA);
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    this.authService.logout();
    this.model = {};
    this.alertify.message('Su sesión ha sido cerrada');
    this.router.navigate(['/home']);
  }
}
