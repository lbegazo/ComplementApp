import { formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { Cdp } from 'src/app/_models/cdp';
import { Contrato } from 'src/app/_models/contrato';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ContratoService } from 'src/app/_services/contrato.service';
import { GeneralService } from 'src/app/_services/general.service';
import { ListaService } from 'src/app/_services/lista.service';

@Component({
  selector: 'app-contrato-edit',
  templateUrl: './contrato-edit.component.html',
  styleUrls: ['./contrato-edit.component.scss'],
})
export class ContratoEditComponent implements OnInit {
  @Input() esCreacion: boolean;
  @Input() cdpSeleccionado: Cdp;
  @Input() contratoSeleccionado: Contrato;
  @Output() esCancelado = new EventEmitter<boolean>();

  editForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  nombreBoton = 'Registrar';
  contratoExiste = false;

  listaTipoModalidadContrato: ValorSeleccion[] = [];
  listaSupervisor: ValorSeleccion[] = [];

  idTipoModalidadContratoSeleccionado?: number;
  tipoModalidadContratoSeleccionado: ValorSeleccion = null;

  idSupervisor1Seleccionado?: number;
  supervisor1Seleccionado: ValorSeleccion = null;

  idSupervisor2Seleccionado?: number;
  supervisor2Seleccionado: ValorSeleccion = null;

  constructor(
    private listaService: ListaService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private contratoService: ContratoService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.cargarListasResolver();

    this.createEmptyForm();

    if (!this.esCreacion) {
      if (this.contratoSeleccionado !== undefined) {
        this.createFullForm();
      }
      this.nombreBoton = 'Guardar';
    } else {
      this.nombreBoton = 'Registrar';
    }
  }

  createEmptyForm() {
    this.editForm = this.fb.group({
      tipoModalidadContratoCtrl: [null, Validators.required],
      numeroContratoCtrl: ['', Validators.required],
      fechaInicioCtrl: [null, Validators.required],
      fechaFinalCtrl: [null, Validators.required],
      fechaExpedicionPolizaCtrl: [null, Validators.required],
      supervisor1Ctrl: [null, Validators.required],
      supervisor2Ctrl: [null],
    });
  }

  createFullForm() {
    console.log(this.contratoSeleccionado);
    console.log(this.listaSupervisor);
    let numeroContrato = 0;
    let fechaInicio = null;
    let fechaFinal = null;
    let fechaExpedicionPoliza = null;

    this.idTipoModalidadContratoSeleccionado =
      this.contratoSeleccionado.tipoModalidadContratoId > 0
        ? this.contratoSeleccionado.tipoModalidadContratoId
        : null;
    if (this.idTipoModalidadContratoSeleccionado !== null) {
      this.tipoModalidadContratoSeleccionado = this.listaTipoModalidadContrato.filter(
        (x) => x.id === this.idTipoModalidadContratoSeleccionado
      )[0];
    }

    this.idSupervisor1Seleccionado =
      this.contratoSeleccionado.supervisor1Id > 0
        ? this.contratoSeleccionado.supervisor1Id
        : null;
    if (this.idSupervisor1Seleccionado !== null) {
      this.supervisor1Seleccionado = this.listaSupervisor.filter(
        (x) => x.id === this.idSupervisor1Seleccionado
      )[0];
    }

    this.idSupervisor2Seleccionado =
      this.contratoSeleccionado.supervisor2Id > 0
        ? this.contratoSeleccionado.supervisor2Id
        : null;
    if (this.idSupervisor2Seleccionado !== null) {
      this.supervisor2Seleccionado = this.listaSupervisor.filter(
        (x) => x.id === this.idSupervisor2Seleccionado
      )[0];
    }

    numeroContrato = this.contratoSeleccionado.numeroContrato;
    fechaInicio = this.contratoSeleccionado.fechaInicio;
    fechaFinal = this.contratoSeleccionado.fechaFinal;
    fechaExpedicionPoliza = this.contratoSeleccionado.fechaExpedicionPoliza;

    this.editForm.patchValue({
      tipoModalidadContratoCtrl: this.tipoModalidadContratoSeleccionado,
      numeroContratoCtrl: numeroContrato,
      supervisor1Ctrl: this.supervisor1Seleccionado,
      supervisor2Ctrl: this.supervisor2Seleccionado,
      fechaInicioCtrl:
        fechaInicio !== null ? formatDate(fechaInicio, 'dd-MM-yyyy', 'en') : '',
      fechaFinalCtrl:
        fechaInicio !== null ? formatDate(fechaFinal, 'dd-MM-yyyy', 'en') : '',
      fechaExpedicionPolizaCtrl:
        fechaExpedicionPoliza !== null
          ? formatDate(fechaExpedicionPoliza, 'dd-MM-yyyy', 'en')
          : '',
    });
  }

  cargarListasResolver() {
    this.route.data.subscribe((data) => {
      this.listaSupervisor = data['supervisor'];
    });
    this.route.data.subscribe((data) => {
      this.listaTipoModalidadContrato = data['modalidadContrato'];
    });
  }

  onModalidadContrato() {
    this.tipoModalidadContratoSeleccionado = this.tipoModalidadContratoCtrl
      .value as ValorSeleccion;
    this.idTipoModalidadContratoSeleccionado = +this
      .tipoModalidadContratoSeleccionado.id;
  }

  onSupervisor1() {
    this.supervisor1Seleccionado = this.supervisor1Ctrl.value as ValorSeleccion;
    this.idSupervisor1Seleccionado = +this.supervisor1Seleccionado.id;
  }

  onSupervisor2() {
    this.supervisor2Seleccionado = this.supervisor2Ctrl.value as ValorSeleccion;
    this.idSupervisor2Seleccionado = +this.supervisor2Seleccionado.id;
  }

  onGuardar() {
    if (this.editForm.valid) {
      const formValues = Object.assign({}, this.editForm.value);

      //#region Read dates

      let dateFechaInicio = null;
      const valueFechaInicio = this.editForm.get('fechaInicioCtrl').value;

      if (GeneralService.isValidDate(valueFechaInicio)) {
        dateFechaInicio = valueFechaInicio;
      } else {
        if (valueFechaInicio && valueFechaInicio.indexOf('-') > -1) {
          dateFechaInicio = GeneralService.dateString2Date(valueFechaInicio);
        }
      }

      let dateFechaFinal = null;
      const valueFechaFinal = this.editForm.get('fechaFinalCtrl').value;

      if (GeneralService.isValidDate(valueFechaFinal)) {
        dateFechaFinal = valueFechaFinal;
      } else {
        if (valueFechaFinal && valueFechaFinal.indexOf('-') > -1) {
          dateFechaFinal = GeneralService.dateString2Date(valueFechaFinal);
        }
      }

      let dateFechaExpedicionPoliza = null;
      const valueFecha = this.editForm.get('fechaExpedicionPolizaCtrl').value;

      if (GeneralService.isValidDate(valueFecha)) {
        dateFechaExpedicionPoliza = valueFecha;
      } else {
        if (valueFecha && valueFecha.indexOf('-') > -1) {
          dateFechaExpedicionPoliza = GeneralService.dateString2Date(
            valueFecha
          );
        }
      }

      //#endregion Read dates

      if (this.esCreacion) {
        const contratoNuevo: Contrato = {
          contratoId: 0,
          tipoModalidadContratoId: this.idTipoModalidadContratoSeleccionado,
          supervisor1Id: this.idSupervisor1Seleccionado,
          supervisor2Id:
            this.idSupervisor2Seleccionado != null
              ? this.idSupervisor2Seleccionado
              : null,
          numeroContrato: formValues.numeroContratoCtrl,
          fechaInicio: dateFechaInicio,
          fechaFinal: dateFechaFinal,
          fechaExpedicionPoliza: dateFechaExpedicionPoliza,
          crp: this.cdpSeleccionado.crp,
        };

        this.contratoService.RegistrarContrato(contratoNuevo).subscribe(
          (response: any) => {
            if (!isNaN(response) && response > 0) {
              this.contratoExiste = false;
              this.editForm.reset(contratoNuevo);
              this.alertify.success('El contrato se registró correctamente');
            } else {
              this.contratoExiste = true;
              this.alertify.error('El contrato ya fue registrado');
            }
          },
          (error) => {
            this.alertify.error(error);
          },
          () => {
            if (!this.contratoExiste) {
              this.esCancelado.emit(true);
            }
          }
        );
      } else {
        this.contratoSeleccionado.tipoModalidadContratoId = this.idTipoModalidadContratoSeleccionado;
        this.contratoSeleccionado.numeroContrato =
          formValues.numeroContratoCtrl;
        this.contratoSeleccionado.fechaInicio = dateFechaInicio;
        this.contratoSeleccionado.fechaFinal = dateFechaFinal;
        this.contratoSeleccionado.fechaExpedicionPoliza = dateFechaExpedicionPoliza;
        this.contratoSeleccionado.supervisor1Id = this.idSupervisor1Seleccionado;
        this.contratoSeleccionado.supervisor2Id =
          this.idSupervisor2Seleccionado !== null
            ? this.idSupervisor2Seleccionado
            : null;

        this.contratoService
          .ActualizarContrato(this.contratoSeleccionado)
          .subscribe(
            () => {
              this.editForm.reset(this.contratoSeleccionado);
              this.alertify.success('El contrato se modificó correctamente');
            },

            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.esCancelado.emit(true);
            }
          );
      }
    }
  }

  onLimpiarForm() {
    this.editForm.reset();
  }

  onCancelar() {
    this.contratoSeleccionado = null;
    this.esCancelado.emit(true);
  }

  numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }

  //#region Controles

  get tipoModalidadContratoCtrl() {
    return this.editForm.get('tipoModalidadContratoCtrl');
  }

  get supervisor1Ctrl() {
    return this.editForm.get('supervisor1Ctrl');
  }

  get supervisor2Ctrl() {
    return this.editForm.get('supervisor2Ctrl');
  }

  //#endregion Controles
}