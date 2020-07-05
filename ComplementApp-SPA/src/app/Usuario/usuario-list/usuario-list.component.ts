import { Component, OnInit } from '@angular/core';
import { Usuario } from 'src/app/_models/usuario';
import { Router, ActivatedRoute } from '@angular/router';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-usuario-list',
  templateUrl: './usuario-list.component.html',
  styleUrls: ['./usuario-list.component.css'],
})
export class UsuarioListComponent implements OnInit {
  usuarios: Usuario[];

  constructor(
    private usuarioService: UsuarioService,
    private router: Router,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.usuarioService.ObtenerUsuarios().subscribe(
      (users: Usuario[]) => {
        this.usuarios = users;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  addUsuario() {
    this.router.navigate(['./new'], { relativeTo: this.route });
  }
}
