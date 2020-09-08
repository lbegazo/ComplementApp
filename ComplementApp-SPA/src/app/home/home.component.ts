import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Usuario } from '../_models/usuario';
import { environment } from 'src/environments/environment';
import { UsuarioService } from '../_services/usuario.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Transaccion } from '../_models/transaccion';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  baseUrl = environment.apiUrl + 'auth/';
  registerMode = false;
  values: any;
  userId: number;
  usuario: Usuario;

  constructor(
    private usuarioService: UsuarioService,
    public authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    //this.obtenerUsuario();
    //console.log(this.usuario);
    //console.log(this.authService.decodedToken);
  }

  obtenerUsuario() {
    if (this.authService.decodedToken != null) {
      const id = +this.authService.decodedToken.nameid;
      if (id > 0) {
        this.usuarioService.ObtenerUsuario(id).subscribe(
          (user: Usuario) => {
            this.usuario = user;
          },
          (error) => this.alertify.error(error)
        );
      }
    }
  }

  onRegister() {
    this.registerMode = true;
  }

  onCancelRegister(response: boolean) {
    this.registerMode = response;
  }
}
