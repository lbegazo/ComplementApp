import { Component, OnInit, OnDestroy } from '@angular/core';
import { Usuario } from 'src/app/_models/usuario';
import { Router, ActivatedRoute } from '@angular/router';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-usuario-list',
  templateUrl: './usuario-list.component.html',
  styleUrls: ['./usuario-list.component.css'],
})
export class UsuarioListComponent implements OnInit, OnDestroy {
  usuarios: Usuario[];
  subscription: Subscription;

  constructor(
    private usuarioService: UsuarioService,
    private router: Router,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.subscription = this.usuarioService.usuarioChanged.subscribe(
      (users: Usuario[]) => {
        this.usuarios = users;
      }
    );

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

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
