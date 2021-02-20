import { formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { Tercero } from 'src/app/_models/tercero';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';
import { ListaService } from 'src/app/_services/lista.service';
import { TerceroService } from 'src/app/_services/tercero.service';

@Component({
  selector: 'app-tercero-edit',
  templateUrl: './tercero-edit.component.html',
  styleUrls: ['./tercero-edit.component.scss'],
})
export class TerceroEditComponent implements OnInit {
  @Input() esCreacion: boolean;
  @Input() terceroSeleccionado: Tercero;
  @Output() esCancelado = new EventEmitter<boolean>();

  editForm = new FormGroup({});
  bsConfig: Partial<BsDaterangepickerConfig>;
  nombreBoton = 'Registrar';

  listaTipoDocumento: ValorSeleccion[] = [];

  idTipoDocumentoSeleccionado?: number;
  tipoDocumentoSeleccionado: ValorSeleccion = null;
  terceroExiste = false;

  constructor(
    private listaService: ListaService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private terceroService: TerceroService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    this.cargarListasResolver();

    if (!this.esCreacion) {
      this.createFullForm();
      this.nombreBoton = 'Guardar';
      this.tipoDocumentoCtrl.disable();
      this.numeroIdentificacionCtrl.disable();
    } else {
      this.nombreBoton = 'Registrar';
      this.tipoDocumentoCtrl.enable();
      this.numeroIdentificacionCtrl.enable();
    }
  }

  createEmptyForm() {
    this.editForm = this.fb.group({
      tipoDocumentoCtrl: [null, Validators.required],
      numeroIdentificacionCtrl: ['', Validators.required],
      nombreCtrl: ['', Validators.required],
      fechaExpedicionCtrl: [null],
      telefonoCtrl: [''],
      emailCtrl: ['', Validators.email],
      direccionCtrl: [''],
      declaranteRentaCtrl: [false],
      facturadorElectronicoCtrl: [false],
    });
  }

  createFullForm() {
    let numeroIdentificacion = '';
    let nombre = '';
    let email = '';
    let direccion = '';
    let telefono = '';
    let fechaExpedicion = null;
    let declaranteRenta = false;
    let facturadorElectronico = false;

    this.idTipoDocumentoSeleccionado =
      this.terceroSeleccionado.tipoDocumentoIdentidadId > 0
        ? this.terceroSeleccionado.tipoDocumentoIdentidadId
        : null;
    if (this.idTipoDocumentoSeleccionado !== null) {
      this.tipoDocumentoSeleccionado = this.listaTipoDocumento.filter(
        (x) => x.id === this.idTipoDocumentoSeleccionado
      )[0];
    }

    numeroIdentificacion = this.terceroSeleccionado.numeroIdentificacion;
    nombre = this.terceroSeleccionado.nombre;
    fechaExpedicion = this.terceroSeleccionado.fechaExpedicionDocumento;
    email = this.terceroSeleccionado.email;
    telefono = this.terceroSeleccionado.telefono;
    direccion = this.terceroSeleccionado.direccion;
    declaranteRenta = this.terceroSeleccionado.declaranteRenta;
    facturadorElectronico = this.terceroSeleccionado.facturadorElectronico;

    this.editForm.patchValue({
      tipoDocumentoCtrl: this.tipoDocumentoSeleccionado,
      numeroIdentificacionCtrl: numeroIdentificacion,
      nombreCtrl: nombre,
      fechaExpedicionCtrl:
        fechaExpedicion !== null
          ? formatDate(fechaExpedicion, 'dd-MM-yyyy', 'en')
          : '',
      emailCtrl: email,
      telefonoCtrl: telefono,
      direccionCtrl: direccion,
      declaranteRentaCtrl: declaranteRenta,
      facturadorElectronicoCtrl: facturadorElectronico,
    });
  }

  cargarListasResolver() {
    this.route.data.subscribe((data) => {
      this.listaTipoDocumento = data['tipoDocumentoIdentidad'];
    });
  }

  onLimpiarForm() {
    this.editForm.reset();
  }

  onTipoDocumento() {
    this.tipoDocumentoSeleccionado = this.tipoDocumentoCtrl
      .value as ValorSeleccion;
    this.idTipoDocumentoSeleccionado = +this.tipoDocumentoSeleccionado.id;
  }

  numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }

  onGuardar() {
    if (this.editForm.valid) {
      const formValues = Object.assign({}, this.editForm.value);

      //#region Read dates

      let dateFecha = null;
      const valueFechaInicio = this.editForm.get('fechaExpedicionCtrl').value;

      if (GeneralService.isValidDate(valueFechaInicio)) {
        dateFecha = valueFechaInicio;
      } else {
        if (valueFechaInicio && valueFechaInicio.indexOf('-') > -1) {
          dateFecha = GeneralService.dateString2Date(valueFechaInicio);
        }
      }

      //#endregion Read dates

      if (this.esCreacion) {
        const terceroNuevo: Tercero = {
          terceroId: 0,
          tipoDocumentoIdentidadId: this.idTipoDocumentoSeleccionado,
          numeroIdentificacion: formValues.numeroIdentificacionCtrl.trim(),
          nombre:
            formValues.nombreCtrl !== undefined
              ? formValues.nombreCtrl.trim()
              : '',
          fechaExpedicionDocumento: dateFecha,
          telefono: formValues.telefonoCtrl,
          email:
            formValues.emailCtrl !== undefined
              ? formValues.emailCtrl.trim()
              : '',
          direccion:
            formValues.direccionCtrl !== undefined
              ? formValues.direccionCtrl.trim()
              : '',
          declaranteRenta: formValues.declaranteRentaCtrl,
          facturadorElectronico: formValues.facturadorElectronicoCtrl,
          facturadorElectronicoDescripcion: '',
          declaranteRentaDescripcion: '',
          tipoDocumentoIdentidad: '',
          regimenTributario: '',
          modalidadContrato: 0,
        };

        this.terceroService.RegistrarTercero(terceroNuevo).subscribe(
          (response: any) => {
            if (!isNaN(response) && response > 0) {
              this.terceroExiste = false;
              this.editForm.reset(terceroNuevo);
              this.alertify.success('El tercero se registró correctamente');
            } else {
              this.terceroExiste = true;
              this.alertify.error('El tercero ya fue registrado');
            }
          },
          (error) => {
            this.alertify.error(error);
          },
          () => {
            if (!this.terceroExiste) {
              this.esCancelado.emit(true);
            }
          }
        );
      } else {
        this.terceroSeleccionado.nombre = formValues.nombreCtrl.trim();
        this.terceroSeleccionado.fechaExpedicionDocumento = dateFecha;
        this.terceroSeleccionado.telefono = formValues.telefonoCtrl;
        this.terceroSeleccionado.email =
          formValues.emailCtrl !== undefined ? formValues.emailCtrl.trim() : '';
        this.terceroSeleccionado.direccion =
          formValues.direccionCtrl !== undefined
            ? formValues.direccionCtrl.trim()
            : '';
        (this.terceroSeleccionado.declaranteRenta =
          formValues.declaranteRentaCtrl),
          (this.terceroSeleccionado.facturadorElectronico =
            formValues.facturadorElectronicoCtrl),
          this.terceroService
            .ActualizarTercero(this.terceroSeleccionado)
            .subscribe(
              () => {
                this.editForm.reset(this.terceroSeleccionado);
                this.alertify.success('El tercero se modificó correctamente');
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

  onCancelar() {
    this.terceroSeleccionado = null;
    this.esCancelado.emit(true);
  }

  //#region Controles

  get tipoDocumentoCtrl() {
    return this.editForm.get('tipoDocumentoCtrl');
  }

  get numeroIdentificacionCtrl() {
    return this.editForm.get('numeroIdentificacionCtrl');
  }

  //#endregion Controles
}
