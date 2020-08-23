import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import * as jsPDF from 'jspdf';
import domtoimage from 'dom-to-image';
import html2canvas from 'html2canvas';

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
import { TipoDetalle } from 'src/app/_models/tipoDetalle';
import { CdpService } from 'src/app/_services/cdp.service';
import { ValidarValorIngresado } from 'src/app/helpers/validarValorIngresado';

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

  listaTipoDetalle: TipoDetalle[];
  idTipoDetalle: number;
  tipoDetalleSelected: TipoDetalle;

  constructor(
    private alertify: AlertifyService,
    private usuarioService: UsuarioService,
    private authService: AuthService,
    private fb: FormBuilder,
    private listaService: ListaService,
    private cdpService: CdpService
  ) {}

  ngOnInit() {
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
        tipoDetalleControl: new FormControl('', Validators.required),
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

    //#region No Eliminar

    // this.cdpForm.addControl(
    //   'objetoBienControl',
    //   new FormControl(objetoBien, Validators.required)
    // );
    // this.cdpForm.addControl(
    //   'observacionesControl',
    //   new FormControl('', Validators.required)
    // );
    // this.cdpForm.addControl(
    //   'tipoDetalleControl',
    //   new FormControl('', Validators.required)
    // );

    // this.detalleCdp?.forEach((x) => {
    //   this.cdpForm.addControl(
    //     x?.id.toString(),
    //     new FormControl('', Validators.required)
    //   );
    // });

    //#endregion No Eliminar
  }

  get rubrosControles() {
    return this.cdpForm.get('rubrosControles') as FormArray;
  }

  cargarTipoDetalle() {
    this.listaService.ObtenerListaTipoDetalle().subscribe(
      (lista: TipoDetalle[]) => {
        this.listaTipoDetalle = lista;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  onSelectTipoDetalle() {
    this.tipoDetalleSelected = this.tipoDetalleControl.value as TipoDetalle;
    this.idTipoDetalle = +this.tipoDetalleSelected.tipoDetalleCDPId;
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

        const arrayControl = this.cdpForm.get('rubrosControles') as FormArray;
        if (arrayControl) {
          for (let index = 0; index < arrayControl.length; index++) {
            const item = arrayControl.at(index);
            const itemDetalle = this.detalleCdp[index];
            itemDetalle.valorSolicitud = item.value.rubroControl;
          }
        }

        //this.cdpForm.controls.tipoDetalleControl.disable();
      }
    );
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
          let pdfWidth = img.width - 20;
          let pdfHeight = img.height - 10;

          // FileSaver.saveAs(dataUrl, 'my-pdfimage.png'); // Save as Image

          let doc;

          // if (pdfWidth > pdfHeight) {
          //   doc = new jsPDF('l', 'px', [pdfWidth, pdfHeight]);
          // } else {
          //   doc = new jsPDF('p', 'px', [pdfWidth, pdfHeight]);
          // }
          doc = new jsPDF('l', 'px', [pdfWidth, pdfHeight]);

          const width = doc.internal.pageSize.getWidth();
          const height = doc.internal.pageSize.getHeight();

          doc.addImage(newImage, 'PNG', 0, 0, width, height);
          filename = 'mypdf_' + '.pdf';
          doc.save(filename);
        };
      })
      .catch((error) => {
        // Error Handling
      });
  }

  // exportarPDF() {
  //   // parentdiv is the html element which has to be converted to PDF
  //   const data = document.getElementById('content');
  //   html2canvas(data).then((canvas) => {
  //     var pdf = new jsPDF('p', 'pt', [canvas.width, canvas.height]);
  //     //var pdf = new jsPDF('p', 'mm', 'a4');

  //     var imgData = canvas.toDataURL('image/jpeg', 1.0);
  //     pdf.addImage(imgData, 0, 0, canvas.width, canvas.height);
  //     pdf.save('converteddoc.pdf');
  //   });
  // }

  // exportarPDF() {
  //   const data = document.getElementById('content');
  //   html2canvas(data).then((canvas) => {
  //     // Few necessary setting options
  //     let imgWidth = 208;
  //     let pageHeight = 295;
  //     let imgHeight = (canvas.height * imgWidth) / canvas.width;
  //     let heightLeft = imgHeight;
  //     //console.log(imgHeight);

  //     const contentDataURL = canvas.toDataURL('image/png');
  //     let pdf = new jsPDF('p', 'mm', 'a4'); // A4 size page of PDF
  //     let position = 0;
  //     pdf.addImage(contentDataURL, 'PNG', 0, 0, imgWidth, imgHeight);
  //     pdf.save('MYPdf.pdf'); // Generated PDF
  //   });
  // }

  // public openPDF(): void {
  //   let DATA = document.getElementById('content');
  //   let doc = new jsPDF('p', 'pt', 'a4');
  //   doc.fromHTML(DATA.innerHTML, 15, 15);
  //   doc.output('dataurlnewwindow');
  // }

  // exportarPDF() {
  //   let DATA = this.content.nativeElement;
  //   let doc = new jspdf('p', 'pt', 'a4');

  //   let handleElement = {
  //     '#editor': function (element, renderer) {
  //       return true;
  //     },
  //   };
  //   doc.fromHTML(DATA.innerHTML, 15, 15, {
  //     width: 200,
  //     elementHandlers: handleElement,
  //   });

  //   doc.save('angular-demo.pdf');
  // }

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
