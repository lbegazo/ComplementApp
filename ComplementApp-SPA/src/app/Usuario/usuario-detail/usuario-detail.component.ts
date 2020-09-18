import { Component, OnInit } from '@angular/core';
import { Usuario } from 'src/app/_models/usuario';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-usuario-detail',
  templateUrl: './usuario-detail.component.html',
  styleUrls: ['./usuario-detail.component.css'],
})
export class UsuarioDetailComponent implements OnInit {
  usuario: Usuario;
  constructor(
    public authService: AuthService,
    private usuarioService: UsuarioService,
    private route: ActivatedRoute,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.usuario = data['usuario'];
    });
  }

  onModificarUsuario() {
    this.router.navigate(['edit'], { relativeTo: this.route });
  }

  onEliminarUsuario() {
    this.alertify.confirm2(
      'Mantenimiento usuario',
      '¿Esta seguro que desea eliminar este registro?',
      () => {
        this.usuarioService.EliminarUsuario(this.usuario.usuarioId).subscribe(
          (next) => {
            this.alertify.success('El usuario se eliminó satisfactoriamente');
          },
          (error) => {
            this.alertify.error(error);
          },
          () => {
            this.router.navigate(['/usuarios']);
          }
        );
      }
    );
  }

}
