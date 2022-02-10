import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ClavePresupuestalContableDto } from 'src/app/_dto/clavePresupuestalContableDto';
import { RelacionContableDto } from 'src/app/_dto/relacionContableDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClavePresupuestalContableService } from 'src/app/_services/clavePresupuestalContable.service';

@Component({
  selector: 'app-popup-uso-relacion-contable',
  templateUrl: './popup-uso-relacion-contable.component.html',
  styleUrls: ['./popup-uso-relacion-contable.component.scss'],
})
export class PopupUsoRelacionContableComponent implements OnInit {
  title: string;
  clavePresupuestalContable: ClavePresupuestalContableDto;

  displayedUsoColumns: string[] = ['selection', 'codigo', 'nombre'];
  displayedRelacionContableColumns: string[] = [
    'selection',
    'cuentaContableCodigo',
    'cuentaContableNombre',
    'atributoContableNombre',
    'tipoGastoNombre',
    'tipoOperacion',
    'usoContable',
  ];
  listaUsoPresupuestal: ValorSeleccion[] = [];
  listaRelacionContable: RelacionContableDto[] = [];
  usoPresupuestalSeleccionado: ValorSeleccion = null;
  relacionContableSeleccionado: RelacionContableDto = null;

  habilitaControlAceptar = false;

  constructor(
    public bsModalRef: BsModalRef,
    private alertify: AlertifyService,
    private clavePresupuestalService: ClavePresupuestalContableService
  ) {}

  ngOnInit() {
    this.cargarUsosPresupuestales();
    this.cargarRelacionesContable();
  }

  //#region Filtro Uso Presupuestal

  // tslint:disable-next-line: variable-name
  private _filterCodigoUso: string;
  // tslint:disable-next-line: variable-name
  private _filterNombreUso: string;
  // tslint:disable-next-line: variable-name
  private _filterNumeroContable: string;
  // tslint:disable-next-line: variable-name
  private _filterNombreContable: string;

  listaFiltradaUsoPresupuestal: ValorSeleccion[] = [];
  selection: SelectionModel<ValorSeleccion> =
    new SelectionModel<ValorSeleccion>(false, []);

  listaFiltradaRelacionContable: RelacionContableDto[] = [];
  selectionContable: SelectionModel<RelacionContableDto> =
    new SelectionModel<RelacionContableDto>(false, []);

  get filterCodigoUso() {
    return this._filterCodigoUso;
  }

  set filterCodigoUso(value: string) {
    this._filterCodigoUso = value;
    this.listaFiltradaUsoPresupuestal =
      this.filtrarCodigoListaUsoPresupuestal(value);
  }

  get filterNombreUso() {
    return this._filterNombreUso;
  }

  set filterNombreUso(value: string) {
    this._filterNombreUso = value;
    this.listaFiltradaUsoPresupuestal =
      this.filtrarNombreListaUsoPresupuestal(value);
  }

  filtrarCodigoListaUsoPresupuestal(val: string) {
    return this.listaUsoPresupuestal.filter((x: ValorSeleccion) =>
      x.codigo.includes(val)
    );
  }

  filtrarNombreListaUsoPresupuestal(val: string) {
    return this.listaUsoPresupuestal.filter((x: ValorSeleccion) =>
      x.nombre.includes(val)
    );
  }

  //#endregion Filtro Uso Presupuestal

  //#region Filtro Relación contable

  get filterNombreContable() {
    return this._filterNombreContable;
  }

  set filterNombreContable(value: string) {
    this._filterNombreContable = value;
    this.listaFiltradaRelacionContable =
      this.filtrarNombreListaRelacionContable(value);
  }

  get filterNumeroContable() {
    return this._filterNumeroContable;
  }

  set filterNumeroContable(value: string) {
    this._filterNumeroContable = value;
    this.listaFiltradaRelacionContable =
      this.filtrarNumeroListaRelacionContable(value);
  }

  filtrarNumeroListaRelacionContable(val: string) {
    return this.listaRelacionContable.filter((x: RelacionContableDto) =>
      x.cuentaContable.codigo.includes(val)
    );
  }

  filtrarNombreListaRelacionContable(val: string) {
    return this.listaRelacionContable.filter((x: RelacionContableDto) =>
      x.cuentaContable.nombre.includes(val)
    );
  }

  //#endregion Filtro Relación contable

  cargarUsosPresupuestales() {
    this.clavePresupuestalService
      .ObtenerUsosPresupuestalesXRubroPresupuestal(
        this.clavePresupuestalContable.rubroPresupuestal.id
      )
      .subscribe(
        (lista: ValorSeleccion[]) => {
          this.listaUsoPresupuestal = lista;
          this.listaFiltradaUsoPresupuestal = lista;
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.listaUsoPresupuestal?.forEach((row, i) => {
            row.seleccionable = true;
            row.seleccionado = false;
          });
          this.listaFiltradaUsoPresupuestal?.forEach((row, i) => {
            row.seleccionable = true;
            row.seleccionado = false;
          });
        }
      );
  }

  cargarRelacionesContable() {
    this.clavePresupuestalService
      .ObtenerRelacionesContableXRubroPresupuestal(
        this.clavePresupuestalContable.rubroPresupuestal.id
      )
      .subscribe(
        (lista: RelacionContableDto[]) => {
          this.listaRelacionContable = lista;
          this.listaFiltradaRelacionContable = lista;
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.listaRelacionContable?.forEach((row, i) => {
            row.seleccionable = true;
            row.seleccionado = false;
          });
          this.listaFiltradaRelacionContable?.forEach((row, i) => {
            row.seleccionable = true;
            row.seleccionado = false;
          });
        }
      );
  }

  selectUsoPresupuestal($event: any, row: ValorSeleccion) {
    // console.log(row);
    $event.preventDefault();
    if (row.seleccionable && !row.seleccionado) {
      this.listaFiltradaUsoPresupuestal?.forEach(
        // tslint:disable-next-line: no-shadowed-variable
        (row: ValorSeleccion) => (row.seleccionado = false)
      );
      row.seleccionado = true;
      this.selection.select(row);
      this.usoPresupuestalSeleccionado = row;

      this.habilitaControlAceptar = this.habilitarBotonAceptar();
    }
  }

  selectRelacionContable($event: any, row: RelacionContableDto) {
    // console.log(row);
    $event.preventDefault();
    if (row.seleccionable && !row.seleccionado) {
      this.listaFiltradaRelacionContable?.forEach(
        // tslint:disable-next-line: no-shadowed-variable
        (row: RelacionContableDto) => (row.seleccionado = false)
      );
      row.seleccionado = true;
      this.selectionContable.select(row);
      this.relacionContableSeleccionado = row;

      this.habilitaControlAceptar = this.habilitarBotonAceptar();
    }
  }

  habilitarBotonAceptar(): boolean {
    if (this.validarUsoPresupuestal && this.relacionContableSeleccionado) {
      return true;
    }
    return false;
  }

  onAceptar() {
    if (this.validarUsoPresupuestal && this.relacionContableSeleccionado) {
      this.bsModalRef.hide();
    }
  }

  get validarUsoPresupuestal(): boolean {
    if (this.listaUsoPresupuestal && this.listaUsoPresupuestal.length === 0) {
      return true;
    }
    if (this.listaUsoPresupuestal && this.listaUsoPresupuestal.length > 0) {
      if (this.usoPresupuestalSeleccionado) {
        return true;
      }
    }
    return false;
  }
}
