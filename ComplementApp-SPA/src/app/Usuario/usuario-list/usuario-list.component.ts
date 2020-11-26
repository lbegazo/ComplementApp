import { Component, OnInit, OnDestroy } from '@angular/core';
import { Usuario } from 'src/app/_models/usuario';
import { Router, ActivatedRoute } from '@angular/router';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Subscription } from 'rxjs';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';

@Component({
  selector: 'app-usuario-list',
  templateUrl: './usuario-list.component.html',
  styleUrls: ['./usuario-list.component.css'],
})
export class UsuarioListComponent implements OnInit, OnDestroy {
  usuarios: Usuario[];
  subscription: Subscription;
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

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

    this.cargarUsuarios();
  }

  addUsuario() {
    this.router.navigate(['./new'], { relativeTo: this.route });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.cargarUsuarios();
  }

  cargarUsuarios() {
    this.usuarioService
      .ObtenerUsuarios(
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Usuario[]>) => {
          this.usuarios = documentos.result;
          this.pagination = documentos.pagination;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
}
