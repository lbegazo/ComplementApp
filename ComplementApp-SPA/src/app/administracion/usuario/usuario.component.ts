import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/Operators';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { Area } from 'src/app/_models/area';
import { Cargo } from 'src/app/_models/cargo';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Perfil } from 'src/app/_models/perfil';
import { Transaccion } from 'src/app/_models/transaccion';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-usuario',
  templateUrl: './usuario.component.html',
  styleUrls: ['./usuario.component.scss'],
})
export class UsuarioComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  search: string;
  searchNombre: string;
  suggestions$: Observable<Usuario[]>;
  suggestionsXNombre$: Observable<Usuario[]>;

  errorMessage: string;
  subscriptions: Subscription[] = [];
  esCreacion = false;
  mostrarCabecera = true;

  listaUsuario: Usuario[] = [];
  usuario: Usuario;
  usuarioSeleccionado: Usuario;

  areas: Area[] = [];
  cargos: Cargo[] = [];
  listaPci: ValorSeleccion[] = [];
  perfiles: Perfil[];

  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

  facturaHeaderForm = new FormGroup({});

  usuarioId = 0;
  arrayControls = new FormArray([]);
  baseUrl = environment.apiUrl + 'lista/ObtenerListaUsuarioXFiltro';

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private usuarioService: UsuarioService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.cargarListasResolver();

    this.createForm();

    this.cargarBusquedaUsuarioXNombres();

    this.cargarBusquedaUsuarioXApellidos();

    this.onBuscarUsuario();
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      rbtRadicarFacturaCtrl: ['1'],
      terceroCtrl: [''],
      terceroDescripcionCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  cargarListasResolver() {
    this.route.data.subscribe((data) => {
      this.areas = data['areas'];
    });

    this.route.data.subscribe((data) => {
      this.cargos = data['cargos'];
    });

    this.route.data.subscribe((data) => {
      this.listaPci = data['pcis'];
    });

    this.route.data.subscribe((data) => {
      this.perfiles = data['perfiles'];
    });
  }

  onCreacion(event) {
    this.limpiarVariables();
    this.esCreacion = true;
    this.mostrarCabecera = false;
  }

  onModificacion(event) {
    this.limpiarVariables();
    this.esCreacion = false;
    this.onBuscarUsuario();
  }

  cargarBusquedaUsuarioXNombres() {
    this.suggestions$ = new Observable((observer: Observer<string>) => {
      observer.next(this.search);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Usuario[]>(this.baseUrl, {
              params: { nombres: query },
            })
            .pipe(
              map((data: Usuario[]) => data || []),
              tap(
                () => noop,
                (err) => {
                  // in case of http error
                  this.errorMessage =
                    (err && err.message) ||
                    'Algo salió mal, consulte a su administrador';
                }
              )
            );
        }

        return of([]);
      })
    );
  }

  cargarBusquedaUsuarioXApellidos() {
    this.suggestionsXNombre$ = new Observable((observer: Observer<string>) => {
      observer.next(this.searchNombre);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Usuario[]>(this.baseUrl, {
              params: { apellidos: query },
            })
            .pipe(
              map((data: Usuario[]) => data || []),
              tap(
                () => noop,
                (err) => {
                  // in case of http error
                  this.errorMessage =
                    (err && err.message) ||
                    'Algo salió mal, consulte a su administrador';
                }
              )
            );
        }

        return of([]);
      })
    );
  }

  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.usuario = e.item as Usuario;
    if (this.usuario) {
      this.usuarioId = this.usuario.usuarioId;
    }
  }

  typeaheadOnSelectXNombre(e: TypeaheadMatch): void {
    this.usuario = e.item as Usuario;
    if (this.usuario) {
      this.usuarioId = this.usuario.usuarioId;
    }
  }

  onCheckChange(event) {
    /* Selected */
    this.usuarioId = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.usuarioId = +event.target.value;

      this.usuarioSeleccionado = this.listaUsuario.filter(
        (x) => x.usuarioId === this.usuarioId
      )[0];
    }
  }

  onBuscarUsuario() {
    this.usuarioService
      .ObtenerUsuariosXFiltro(
        this.esCreacion ? 2 : 1,
        this.usuarioId,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Usuario[]>) => {
          this.listaUsuario = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          if (this.listaUsuario && this.listaUsuario.length === 0) {
            this.alertify.warning('No existen terceros registrados');
          }
          const tipo = this.esCreacion ? '2' : '1';
          this.facturaHeaderForm = this.fb.group({
            rbtRadicarFacturaCtrl: [tipo],
            terceroCtrl: [''],
            terceroDescripcionCtrl: [''],
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaUsuario && this.listaUsuario.length > 0) {
      for (const detalle of this.listaUsuario) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl('', [Validators.required]),
          })
        );
      }
    }
  }

  onModificar() {
    if (this.usuarioId > 0) {
      if (!this.esCreacion) {
        this.usuarioService.ObtenerUsuario(this.usuarioId).subscribe(
          (documento: Usuario) => {
            if (documento) {
              this.usuarioSeleccionado = documento;
              this.mostrarCabecera = false;
            } else {
              this.alertify.error(
                'No se pudo obtener información para el usuario'
              );
              this.mostrarCabecera = true;
            }
          },
          (error) => {
            this.alertify.error(error);
          }
        );
      } else {
        this.mostrarCabecera = false;
      }
    }
  }

  HabilitarCabecera($event) {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };

    this.mostrarCabecera = true;
    this.onLimpiarFactura();
  }

  onLimpiarFactura() {
    this.limpiarVariables();
    this.onBuscarUsuario();
  }

  limpiarVariables() {
    this.esCreacion = false;
    this.listaUsuario = [];
    this.usuario = null;
    this.usuarioId = 0;
    this.search = '';
    this.usuarioSeleccionado = null;
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarUsuario();
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }
}
