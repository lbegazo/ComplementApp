import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/Operators';
import { Tercero } from 'src/app/_models/tercero';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-popup-parametro-liquidacion-tercero',
  templateUrl: './popup-parametro-liquidacion-tercero.component.html',
  styleUrls: ['./popup-parametro-liquidacion-tercero.component.scss'],
})
export class PopupParametroLiquidacionTerceroComponent implements OnInit {
  title: string;

  popupForm = new FormGroup({});
  suggestionsXCodigo$: Observable<Tercero[]>;
  suggestionsXNombre$: Observable<Tercero[]>;
  errorMessage: string;
  subscriptions: Subscription[] = [];
  search: string;
  searchNombre: string;
  baseUrl = environment.apiUrl + 'lista/ObtenerListaTercero';

  tercero: Tercero;
  terceroId = 0;

  constructor(
    private http: HttpClient,
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    this.cargarBusquedaTerceroXCodigo();

    this.cargarBusquedaTerceroXNombre();
  }

  createEmptyForm() {
    this.popupForm = this.fb.group({
      terceroCtrl: ['', Validators.required],
      terceroDescripcionCtrl: ['', Validators.required],
    });
  }

  cargarBusquedaTerceroXCodigo() {
    this.suggestionsXCodigo$ = new Observable((observer: Observer<string>) => {
      observer.next(this.search);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Tercero[]>(this.baseUrl, {
              params: { numeroIdentificacion: query },
            })
            .pipe(
              map((data: Tercero[]) => data || []),
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

  cargarBusquedaTerceroXNombre() {
    this.suggestionsXNombre$ = new Observable((observer: Observer<string>) => {
      observer.next(this.searchNombre);
    }).pipe(
      switchMap((query: string) => {
        if (query) {
          return this.http
            .get<Tercero[]>(this.baseUrl, {
              params: { nombre: query },
            })
            .pipe(
              map((data: Tercero[]) => data || []),
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

  // Selected value event
  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  typeaheadOnSelectXNombre(e: TypeaheadMatch): void {
    this.tercero = e.item as Tercero;
    if (this.tercero) {
      this.terceroId = this.tercero.terceroId;
    }
  }

  onLimpiar() {
    this.tercero = null;
    this.terceroId = 0;
    this.search = '';
  }

  onAceptar() {
    if (this.terceroId > 0) {
      this.bsModalRef.hide();
    }
  }
}
