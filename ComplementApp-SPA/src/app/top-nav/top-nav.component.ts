import {
  Component,
  OnInit,
  ViewChild,
  Input,
} from '@angular/core';
import { NavService } from '../_services/nav.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { Transaccion } from '../_models/transaccion';

@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.scss'],
})
export class TopNavComponent implements OnInit {
  @ViewChild('loginForm', { static: true }) loginForm: NgForm;
  @Input() transacciones: Transaccion[] = [];

  model: any = {};
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    public navService: NavService
  ) {}

  ngOnInit() {}

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    this.model = {};
    localStorage.clear();
    this.alertify.message('Su sesión ha sido cerrada');
    this.router.navigate(['/home']);
  }
}
