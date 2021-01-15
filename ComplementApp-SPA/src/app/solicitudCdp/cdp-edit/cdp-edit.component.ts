import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import * as jsPDF from 'jspdf';
import domtoimage from 'dom-to-image';

import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { Cdp } from 'src/app/_models/cdp';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { AuthService } from 'src/app/_services/auth.service';
import { TipoOperacion } from 'src/app/_models/tipoOperacion';
import {
  FormBuilder,
  FormGroup,
  Validators,
  FormArray,
  FormControl,
  NgForm,
} from '@angular/forms';
import { ListaService } from 'src/app/_services/lista.service';
import { CdpService } from 'src/app/_services/cdp.service';
import { ValidarValorIngresado } from 'src/app/helpers/validarValorIngresado';
import { SolicitudCDP } from 'src/app/_models/solicitudCDP';
import { DetalleSolicitudCDP } from 'src/app/_models/detalleSolicitudCDP';
import { TipoDetalleCDP } from 'src/app/_models/tipoDetalleCDP';
import { RubroPresupuestal } from 'src/app/_models/rubroPresupuestal';
import { EstadoSolicitudCDP } from 'src/app/_models/enum';
import { Estado } from 'src/app/_models/estado';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { GeneralService } from 'src/app/_services/general.service';

@Component({
  selector: 'app-cdp-edit',
  templateUrl: './cdp-edit.component.html',
  styleUrls: ['./cdp-edit.component.css'],
})
export class CdpEditComponent implements OnInit {
  @ViewChild('content') content: ElementRef;
  @ViewChild('cdpNgForm', { static: true }) cdpNgForm: NgForm;
  usuario: Usuario;
  today: number = Date.now();
  @Input() cdp: Cdp;
  @Input() tipoOperacion: TipoOperacion;
  @Input() rubroPresupuestalesSeleccionado: DetalleCDP[];
  cambiosConfirmados = false;

  itemCdp: DetalleCDP;
  cdpForm = new FormGroup({});
  arrayControls = new FormArray([]);
  ordenadorDelGasto = 'ANDREY GEOVANNY RODRIGUEZ LEON';
  contratacion = 'HANS RONALD NIÑO GARCIA';
  detalleCdp: DetalleCDP[];

  listaTipoDetalle: TipoDetalleCDP[];
  idTipoDetalle: number;
  tipoDetalleSeleccionado: TipoDetalleCDP = { tipoDetalleCDPId: 0, nombre: '' };
  notaLegal: ValorSeleccion;

  constructor(
    private alertify: AlertifyService,
    private usuarioService: UsuarioService,
    private authService: AuthService,
    private fb: FormBuilder,
    private listaService: ListaService,
    private cdpService: CdpService,
    private generalService: GeneralService
  ) {}

  ngOnInit() {
    this.cargarNotaLegal();

    this.cargarInformacionUsuario();

    if (!this.esSolicitudInicial) {
      this.cargarTipoDetalle();
    }

    this.createCdpForm();
  }

  createCdpForm() {
    let objetoBien = '';
    const idTipoOperacion = this.tipoOperacion?.tipoOperacionId;

    if (!this.esSolicitudInicial) {
      //#region No Solicitud Inicial

      this.cdpService.ObtenerDetalleDeCDP(this.cdp?.cdp).subscribe(
        (documento: DetalleCDP[]) => {
          this.detalleCdp = documento;

          if (this.detalleCdp) {
            this.itemCdp = this.detalleCdp[0];

            for (const detalle of this.detalleCdp) {
              this.arrayControls.push(
                new FormGroup({
                  rubroControl: new FormControl('', [
                    Validators.required,
                    ValidarValorIngresado.valorIncorrecto(
                      idTipoOperacion,
                      detalle.saldoAct,
                      detalle.saldoCDP
                    ),
                  ]),
                })
              );
            }
          }
        },
        (error) => {
          this.alertify.error(error);
        }
      );

      objetoBien = this.cdp?.detalle4;

      this.cdpForm = this.fb.group({
        objetoBienControl: new FormControl(objetoBien, Validators.required),
        observacionesControl: new FormControl('', Validators.required),
        tipoDetalleControl: new FormControl(null, Validators.required),
        rubrosControles: this.arrayControls,
      });

      //#endregion No Solicitud Inicial
    } else {
      //#region Solicitud Inicial

      this.itemCdp = this.rubroPresupuestalesSeleccionado[0];
      this.detalleCdp = this.rubroPresupuestalesSeleccionado;

      if (this.detalleCdp) {
        this.itemCdp = this.detalleCdp[0];

        for (const detalle of this.detalleCdp) {
          this.arrayControls.push(
            new FormGroup({
              rubroControl: new FormControl('', [
                Validators.required,
                ValidarValorIngresado.valorIncorrecto(
                  idTipoOperacion,
                  detalle.saldoAct,
                  detalle.saldoCDP
                ),
              ]),
            })
          );
        }
      }

      this.cdpForm = this.fb.group({
        objetoBienControl: new FormControl(objetoBien, Validators.required),
        observacionesControl: new FormControl('', Validators.required),
        tipoDetalleControl: new FormControl(''),
        rubrosControles: this.arrayControls,
      });
      this.cdpForm.controls.tipoDetalleControl.disable();

      //#endregion Solicitud Inicial
    }
  }

  get rubrosControles() {
    return this.cdpForm.get('rubrosControles') as FormArray;
  }

  cargarTipoDetalle() {
    this.listaService.ObtenerListaTipoDetalle().subscribe(
      (lista: TipoDetalleCDP[]) => {
        this.listaTipoDetalle = lista;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  onSelectTipoDetalle() {
    if (!this.esSolicitudInicial) {
      this.tipoDetalleSeleccionado = this.tipoDetalleControl
        .value as TipoDetalleCDP;
      this.idTipoDetalle = +this.tipoDetalleSeleccionado.tipoDetalleCDPId;
    } else {
      this.tipoDetalleSeleccionado = { tipoDetalleCDPId: 0, nombre: '' };
    }
  }

  EliminarRubroPresupuestal(index: number) {
    if (this.detalleCdp.length > 1) {
      this.detalleCdp.splice(index, 1);
      this.arrayControls.removeAt(index);
    } else {
      this.alertify.warning('Debe existir por lo menos un rubro presupuestal');
    }
  }

  confirmarCambios() {
    this.alertify.confirm2(
      'Solicitud de CDP',
      '¿Esta seguro que desea confirmar la solicitud de CDP?',
      () => {
        this.cambiosConfirmados = true;

        const arrayControles = this.cdpForm.get('rubrosControles') as FormArray;
        if (arrayControles && arrayControles.length > 0) {
          for (let index = 0; index < arrayControles.length; index++) {
            const item = arrayControles.at(index);
            const itemDetalle = this.detalleCdp[index];
            itemDetalle.valorSolicitud = GeneralService.obtenerValorAbsoluto(
              item.value.rubroControl
            );
          }
        }

        this.guardarSolicitudCDP();
      }
    );
  }

  guardarSolicitudCDP() {
    const solicitudCDP: SolicitudCDP = new SolicitudCDP();
    const listaDetalleSolicitudCDP: DetalleSolicitudCDP[] = [];

    if (this.cdpForm.valid) {
      let solicitudCDPId = 0;

      //#region Setear datos
      solicitudCDP.tipoOperacion = new TipoOperacion();
      solicitudCDP.tipoOperacion.tipoOperacionId = this.tipoOperacion.tipoOperacionId;
      solicitudCDP.tipoOperacion.codigo = this.tipoOperacion.codigo;
      solicitudCDP.tipoOperacion.nombre = this.tipoOperacion.nombre;

      solicitudCDP.estadoSolicitudCDP = new Estado();
      solicitudCDP.estadoSolicitudCDP.estadoId =
        EstadoSolicitudCDP.Generado.value;
      solicitudCDP.estadoSolicitudCDP.nombre = 'test';
      solicitudCDP.estadoSolicitudCDP.tipoDocumento = 'test';
      solicitudCDP.estadoSolicitudCDP.descripcion = 'test';

      solicitudCDP.usuarioId = this.usuario.usuarioId;
      solicitudCDP.numeroActividad = this.itemCdp.idArchivo;

      solicitudCDP.aplicaContrato =
        this.itemCdp.aplicaContrato === 'SI' ? true : false;

      solicitudCDP.nombreBienServicio = this.itemCdp.planDeCompras;
      solicitudCDP.proyectoInversion = this.itemCdp.proyecto;
      solicitudCDP.actividadProyectoInversion = this.itemCdp.actividadBpin;

      if (!this.esSolicitudInicial) {
        solicitudCDP.cdp = this.cdp.cdp;
        solicitudCDP.estadoCDP = this.cdp.detalle1;
        solicitudCDP.objetoBienServicioContratado = this.cdp.detalle4;
        solicitudCDP.tipoDetalleCDP = new TipoDetalleCDP();
        solicitudCDP.tipoDetalleCDP = this.tipoDetalleSeleccionado;
      } else {
        solicitudCDP.objetoBienServicioContratado = this.cdpForm.get(
          'objetoBienControl'
        ).value;
      }

      solicitudCDP.observaciones = this.cdpForm.get(
        'observacionesControl'
      ).value;

      this.detalleCdp.forEach((element) => {
        const item: DetalleSolicitudCDP = new DetalleSolicitudCDP();

        item.rubroPresupuestal = new RubroPresupuestal();
        item.rubroPresupuestal.rubroPresupuestalId =
          element.rubroPresupuestalId;
        item.rubroPresupuestal.nombre = 'test';
        item.rubroPresupuestal.identificacion = 'test';
        item.rubroPresupuestal.padreRubroId = 1;
        item.saldoActividad = element.saldoAct;

        if (!this.esSolicitudInicial) {
          item.valorCDP = element.valorCDP;
          item.saldoCDP = element.saldoCDP;
        } else {
          item.valorActividad = element.saldoAct;
        }

        listaDetalleSolicitudCDP.push(item);
      });

      const arrayControl = this.cdpForm.get('rubrosControles') as FormArray;
      if (arrayControl && arrayControl.length > 0) {
        for (let index = 0; index < arrayControl.length; index++) {
          const item = arrayControl.at(index);
          const itemDetalle = listaDetalleSolicitudCDP[index];
          itemDetalle.valorSolicitud = GeneralService.obtenerValorAbsoluto(
            item.value.rubroControl
          );
        }
      }

      solicitudCDP.detalleSolicitudCDPs = listaDetalleSolicitudCDP;

      //#endregion Setear datos

      this.cdpService.RegistrarSolicitudCDP(solicitudCDP).subscribe(
        (response: any) => {
          if (!isNaN(response)) {
            solicitudCDPId = +response;
            this.alertify.success(
              'La Solicitud de CDP se registró correctamente'
            );
          } else {
            this.alertify.error('No se pudo registrar la Solicitud de CDP');
          }
        },

        (error) => {
          this.alertify.error(
            'Hubó un error al registrar la liquidación ' + error
          );
        },
        () => {}
      );
    }
  }

  exportarPDF() {
    const node = document.getElementById('content');

    let img;
    let filename;
    let newImage;

    domtoimage
      .toPng(node, { bgcolor: '#fff' })

      .then((dataUrl) => {
        img = new Image();
        img.src = dataUrl;
        newImage = img.src;

        img.onload = () => {
          const pdfWidth = img.width - 20;
          const pdfHeight = img.height - 10;

          let doc;
          doc = new jsPDF('l', 'px', [pdfWidth, pdfHeight]);

          const width = doc.internal.pageSize.getWidth();
          const height = doc.internal.pageSize.getHeight();

          doc.addImage(newImage, 'PNG', 0, 0, width, height);
          filename = 'documento' + '.pdf';
          doc.save(filename);
        };
      })
      .catch((error) => {
        // Error Handling
      });
  }

  cargarInformacionUsuario() {
    this.usuarioService
      .ObtenerUsuario(this.authService.decodedToken.nameid)
      .subscribe(
        (user: Usuario) => {
          this.usuario = user;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  cargarNotaLegal() {
    this.listaService.ObtenerParametroGeneralXNombre('NotaLegalANE').subscribe(
      (data: ValorSeleccion) => {
        this.notaLegal = data;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  get esSolicitudInicial() {
    const idTipoOperacion = this.tipoOperacion?.tipoOperacionId;
    return idTipoOperacion === 4;
  }

  get esAplicaContrato() {
    return this.itemCdp?.aplicaContrato.toUpperCase() === 'SI' ? true : false;
  }

  get mostrarTerceraFirma() {
    if (!this.esSolicitudInicial) {
      if (!this.esAplicaContrato) {
        return false;
      } else {
        return true;
      }
    } else {
      return false;
    }
  }

  get objetoBienControl() {
    return this.cdpForm.get('objetoBienControl');
  }

  get tipoDetalleControl() {
    return this.cdpForm.get('tipoDetalleControl');
  }
}
