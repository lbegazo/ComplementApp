import { formatDate } from '@angular/common';
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
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { noop, Observable, Observer, of, Subscription } from 'rxjs';
import { EnvioParametroDto } from 'src/app/_dto/envioParametroDto';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { FormatoCausacionyLiquidacionPago } from 'src/app/_models/formatoCausacionyLiquidacionPago';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { DetalleLiquidacionService } from 'src/app/_services/detalleLiquidacion.service';

@Component({
  selector: 'app-administracion-archivos',
  templateUrl: './administracion-archivos.component.html',
  styleUrls: ['./administracion-archivos.component.css'],
})
export class AdministracionArchivosComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;
  subscriptions: Subscription[] = [];
  arrayControls = new FormArray([]);

  listaPlanPago: FormatoCausacionyLiquidacionPago[] = [];

  facturaHeaderForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };
  liquidacionesSeleccionadas: number[] = [];
  seleccionaTodas = false;

  listaTipoArchivo: ValorSeleccion[] = [];
  idTipoArchivoSelecionado = 0;
  tipoArchivoSeleccionado: ValorSeleccion;

  listaArchivo: ValorSeleccion[] = [];
  idArchivoSelecionado = 0;
  archivoSeleccionado: ValorSeleccion;
  dateObj = new Date();

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private liquidacionService: DetalleLiquidacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarListasResolver();

    this.onLimpiarFactura();
  }

  createForm() {
    this.facturaHeaderForm = this.fb.group({
      fechaGeneracionCtrl: ['', Validators.required],
      tipoArchivoCtrl: [null, Validators.required],
      archivoCtrl: [null, Validators.required],
      planPagoControles: this.arrayControls,
    });

    this.facturaHeaderForm.patchValue({
      fechaGeneracionCtrl: this.dateObj,
    });
  }

  cargarListasResolver() {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });

    this.route.data.subscribe((data) => {
      this.listaTipoArchivo = data['tipoArchivo'];
    });
  }

  onSelectTipoArchivo() {
    this.tipoArchivoSeleccionado = this.tipoArchivoControl
      .value as ValorSeleccion;
    this.idTipoArchivoSelecionado = this.tipoArchivoSeleccionado.id;

    this.cargarArchivos();
  }

  onSelectArchivo() {
    this.archivoSeleccionado = this.archivoControl.value as ValorSeleccion;
    this.idArchivoSelecionado = this.archivoSeleccionado.id;

    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };
  }

  cargarArchivos() {
    const formValues = Object.assign({}, this.facturaHeaderForm.value);
    const valueFechaInicio = formValues.fechaGeneracionCtrl;

    if (this.idTipoArchivoSelecionado > 0) {
      const parametros: EnvioParametroDto = {
        fechaGeneracion: valueFechaInicio,
        tipoArchivo: this.idTipoArchivoSelecionado,
      };
      this.liquidacionService.ObtenerListaArchivoCreados(parametros).subscribe(
        (documentos: ValorSeleccion[]) => {
          this.listaArchivo = documentos;
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {}
      );
    }
  }

  onBuscarFactura() {
    this.liquidacionService
      .ObtenerDocumentosParaAdministracionArchivo(
        this.idArchivoSelecionado,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<FormatoCausacionyLiquidacionPago[]>) => {
          this.listaPlanPago = documentos.result;
          this.pagination = documentos.pagination;
          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          // this.facturaHeaderForm = this.fb.group({
          //   fechaGeneracionCtrl: ['', Validators.required],
          //   tipoArchivoCtrl: [null, Validators.required],
          //   archivoCtrl: [null, Validators.required],
          //   planPagoControles: this.arrayControls,
          // });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaPlanPago && this.listaPlanPago.length > 0) {
      if (this.seleccionaTodas) {
        this.listaPlanPago.forEach((item) => {
          item.esSeleccionada = this.seleccionaTodas;
        });

        this.listaPlanPago.forEach((val: FormatoCausacionyLiquidacionPago) => {
          if (
            this.liquidacionesSeleccionadas?.indexOf(
              val.detalleLiquidacionId
            ) === -1
          ) {
            this.liquidacionesSeleccionadas.push(val.detalleLiquidacionId);
          }
        });
      } else {
        if (
          this.liquidacionesSeleccionadas &&
          this.liquidacionesSeleccionadas.length > 0
        ) {
          this.listaPlanPago.forEach(
            (val: FormatoCausacionyLiquidacionPago) => {
              if (
                this.liquidacionesSeleccionadas?.indexOf(
                  val.detalleLiquidacionId
                ) > -1
              ) {
                val.esSeleccionada = true;
              }
            }
          );
        }
      }

      for (const detalle of this.listaPlanPago) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning('No se pudieron obtener Liquidaciones');
    }
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }

  onLimpiarFactura() {
    this.listaPlanPago = [];
    this.liquidacionesSeleccionadas = [];
    this.seleccionaTodas = false;
    this.idArchivoSelecionado = 0;
    this.idTipoArchivoSelecionado = 0;
    this.archivoSeleccionado = null;
    this.tipoArchivoSeleccionado = null;
    this.listaArchivo = [];

    this.pagination = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0,
      totalPages: 0,
      maxSize: 10,
    };

    this.createForm();
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  onCheckChange(event) {
    /* Selected */
    let valor = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      valor = +event.target.value;

      if (this.liquidacionesSeleccionadas?.indexOf(valor) === -1) {
        this.liquidacionesSeleccionadas?.push(+valor);
      }
    } else {
      /* unselected */
      valor = +event.target.value;
      let index = 0;
      let i = 0;
      this.liquidacionesSeleccionadas.forEach((val: number) => {
        if (val === valor) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.liquidacionesSeleccionadas.splice(index, 1);
      }
    }

    if (this.pagination) {
      if (
        this.pagination.totalItems === this.liquidacionesSeleccionadas.length
      ) {
        this.seleccionaTodas = true;
      } else {
        this.seleccionaTodas = false;
      }
    }
  }

  onCheckAllChange(event) {
    const checked = event.target.checked;
    if (checked) {
      this.seleccionaTodas = true;
      this.listaPlanPago.forEach((item) => (item.esSeleccionada = checked));
      this.liquidacionesSeleccionadas = [];

      this.listaPlanPago.forEach((val: FormatoCausacionyLiquidacionPago) => {
        this.liquidacionesSeleccionadas.push(val.detalleLiquidacionId);
      });
    } else {
      this.seleccionaTodas = false;
      this.listaPlanPago.forEach((item) => (item.esSeleccionada = checked));
      this.liquidacionesSeleccionadas = [];
    }
  }

  actualizarListaLiquidacionDeArchivo() {
    let listaIds: number[] = [];

    const esSeleccionarTodas = this.seleccionaTodas ? 1 : 0;
    let listaCadenaIds = '';

    if (this.facturaHeaderForm.valid) {
      //#region Obtener lista de ids: listaCadenaIds

      if (!this.seleccionaTodas) {
        if (
          this.liquidacionesSeleccionadas &&
          this.liquidacionesSeleccionadas.length > 0
        ) {
          listaIds = this.liquidacionesSeleccionadas.filter(
            (v, i) => this.liquidacionesSeleccionadas.indexOf(v) === i
          );
          listaCadenaIds = listaIds.join();
        }
      }

      //#endregion Obtener lista de ids

      this.liquidacionService
        .ActualizarListaLiquidacionDeArchivo(
          this.idArchivoSelecionado,
          listaCadenaIds,
          esSeleccionarTodas
        )
        .subscribe(
          (response) => {
            if (response) {
              this.alertify.success(
                'Se actualizaron las liquidaciones asociados al archivo seleccionado'
              );
            }
          },
          (error) => {
            this.alertify.error(
              'Hubo un error al actualizar las liquidaciones'
            );
          },
          () => {
            this.onLimpiarFactura();
            this.router.navigate(['/GENERADOR_ADMINISTRACIONARCHIVO']);
          }
        );
    }
  }

  get tipoArchivoControl() {
    return this.facturaHeaderForm.get('tipoArchivoCtrl');
  }

  get archivoControl() {
    return this.facturaHeaderForm.get('archivoCtrl');
  }
}
