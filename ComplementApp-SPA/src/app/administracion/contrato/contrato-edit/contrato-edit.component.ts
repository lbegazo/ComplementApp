import { formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { Cdp } from 'src/app/_models/cdp';
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
  @Output() esCancelado = new EventEmitter<boolean>();

  editForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  nombreBoton = 'Registrar';

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
    this.createEmptyForm();

    this.cargarListasResolver();
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
    //this.editForm.reset();
  }

  onLimpiarForm() {
    this.editForm.reset();
  }

  onCancelar() {
    //this.terceroSeleccionado = null;
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
    return this.editForm.get('supervisor1Ctr2');
  }

  //#endregion Controles
}
