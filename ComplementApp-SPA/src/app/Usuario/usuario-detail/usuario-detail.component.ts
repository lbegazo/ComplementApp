import { Component, OnInit } from '@angular/core';
import { Usuario } from 'src/app/_models/usuario';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-usuario-detail',
  templateUrl: './usuario-detail.component.html',
  styleUrls: ['./usuario-detail.component.css'],
})
export class UsuarioDetailComponent implements OnInit {
  usuario: Usuario;
  constructor(
    private usuarioServices: UsuarioService,
    private route: ActivatedRoute,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.usuario = data['usuario'];
    });
    // const id = +this.route.snapshot.params['id'];
    // this.usuarioServices.ObtenerUsuario(id).subscribe(
    //   (usuario: Usuario) => {
    //     this.usuario = usuario;
    //   },
    //   (error) => {
    //     this.alertify.error(error);
    //   }
    // );
  }

  onEditUsuario() {
    this.router.navigate(['edit'], { relativeTo: this.route });
  }
}
