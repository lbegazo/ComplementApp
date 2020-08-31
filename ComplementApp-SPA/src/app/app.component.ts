import {
  Component,
  OnInit,
  HostListener,
  OnDestroy,
  Inject,
} from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();

  constructor(
    private authServices: AuthService,
    @Inject(DOCUMENT) private document: Document
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.authServices.decodedToken = this.jwtHelper.decodeToken(token);
    this.document.documentElement.lang = 'es';
  }

  @HostListener('window:beforeunload', ['$event'])
  clearLocalStorage(event) {
    localStorage.clear();
  }
}
