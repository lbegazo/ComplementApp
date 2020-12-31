import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  ChangeDetectorRef,
} from '@angular/core';
import { FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

import { TipoOperacion } from 'src/app/_models/tipoOperacion';
import { ListaService } from 'src/app/_services/lista.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { Cdp } from 'src/app/_models/cdp';
import { CdpService } from 'src/app/_services/cdp.service';
import { PopupCdpComponent } from '../popup-cdp/popup-cdp.component';
import { Subscription, combineLatest } from 'rxjs';

@Component({
  selector: 'app-cdp-header',
  templateUrl: './cdp-header.component.html',
  styleUrls: ['./cdp-header.component.css'],
})
export class CdpHeaderComponent implements OnInit {
  //@ViewChild('editForm', { static: true }) editForm: NgForm;
  @ViewChild('cdpChildForm', { static: true }) cdpChildForm: NgForm;
  today: number = Date.now();
  registerForm: FormGroup;
  listaTO: TipoOperacion[];
  idTipoOperacionSelecionado: number;
  tipoOperacionSelecionado: TipoOperacion = null;
  cdp: Cdp;

  // Solicitud Inicial
  rubroPresupuestalSinCdp: DetalleCDP[];
  rubroPresupuestalSinCdpSeleccionado: DetalleCDP[] = [];
  arrRubroPresupuestalSeleccionado: number[];
  bsModalRef: BsModalRef;

  subscriptions: Subscription[] = [];

  constructor(
    private listaService: ListaService,
    private alertify: AlertifyService,
    private cdpService: CdpService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private modalService: BsModalService,
    private changeDetection: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.createRegisterForm();
    this.cargarTipoOperacion();
    this.registerForm.reset();
  }

  onLimpiarCDP(form: FormGroup) {
    this.tipoOperacionSelecionado = null;
    this.idTipoOperacionSelecionado = null;
    this.cdp = null;
    this.rubroPresupuestalSinCdp = [];
    this.rubroPresupuestalSinCdpSeleccionado = [];
    this.habilitarControles();
    form.reset();
  }

  onBuscarCDP() {
    if (!this.esSolicitudInicial) {
      //#region No solicitud Inicial
      const numCdp = this.cdpControl.value;
      //const numCdp = 120;
      this.cdpService.ObtenerCDP(numCdp).subscribe(
        (documento: Cdp) => {
          this.cdp = documento;

          if (this.cdpSeleccionado) {
            this.deshabilitarControles();
            //this.editForm.reset();
          }
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          if (this.cdp == null) {
            this.alertify.warning('No se encontró el CDP');
          }
        }
      );

      //#endregion No solicitud Inicial
    } else {
      //#region Solicitud inicial

      this.cargarRubrosPresupuestalesSinCdp();

      //#region Abrir Popup de rubros presupuestales

      const initialState = {
        title: 'SELECCIONE LOS RUBROS PRESUPUESTALES',
      };

      this.bsModalRef = this.modalService.show(
        PopupCdpComponent,
        Object.assign({ initialState }, { class: 'gray modal-lg' })
      );

      this.bsModalRef.content.closeBtnName = 'Aceptar';

      //#endregion Abrir Popup de rubros presupuestales

      //#region Cargar información del popup (OnHidden event)

      const combine = combineLatest([this.modalService.onHidden]).subscribe(() =>
        this.changeDetection.markForCheck()
      );

      this.subscriptions.push(
        this.modalService.onHidden.subscribe((reason: string) => {
          if (this.bsModalRef.content != null) {
            this.arrRubroPresupuestalSeleccionado = this.bsModalRef.content;
          }

          this.obtenerRubrosPresupuestalesSinCdp();

          if (this.rubroPresupuestalSinCdpSeleccionado) {
            this.deshabilitarControles();
            //this.editForm.reset();
          }

          // console.log(this.rubroPresupuestalSinCdpSeleccionado);

          this.unsubscribe();
        })
      );

      this.subscriptions.push(combine);

      //#endregion Cargar información del popup (OnHidden event)

      //#endregion Solicitud inicial
    }
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  cargarCDP() {
    this.router.navigate(['edit'], { relativeTo: this.route });
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      cdpControl: ['', Validators.required],
      tOperacionControl: ['', Validators.required],
    });
  }

  cargarTipoOperacion() {
    this.listaService.ObtenerListaTipoOperacion().subscribe(
      (lista: TipoOperacion[]) => {
        this.listaTO = lista;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  //#region Solicitud inicial

  cargarInformacionCDP() {}

  cargarRubrosPresupuestalesSinCdp() {
    this.cdpService.ObtenerDetalleDeCDP(0).subscribe(
      (documento: DetalleCDP[]) => {
        this.rubroPresupuestalSinCdp = documento;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  obtenerRubrosPresupuestalesSinCdp() {
    if (this.arrRubroPresupuestalSeleccionado.length > 0) {
      for (
        let index = 0;
        index < this.rubroPresupuestalSinCdp.length;
        index++
      ) {
        const rubroPre = this.rubroPresupuestalSinCdp[index];

        this.arrRubroPresupuestalSeleccionado.forEach((elem: number) => {
          if ((this.rubroPresupuestalSinCdp[index] as DetalleCDP).id === elem) {
            this.rubroPresupuestalSinCdpSeleccionado.push(
              this.rubroPresupuestalSinCdp[index] as DetalleCDP
            );
          }
        });
      }
    }
  }

  //#endregion Solicitud inicial

  onSelectTipoOperacion() {
    this.tipoOperacionSelecionado = this.tOperacionControl
      .value as TipoOperacion;
    this.idTipoOperacionSelecionado = +this.tipoOperacionSelecionado
      .tipoOperacionId;

    if (this.idTipoOperacionSelecionado === 4) {
      // Solicitud Inicial
      this.cdpControl.disable();
    } else {
      this.cdpControl.enable();
    }
  }

  get tOperacionControl() {
    return this.registerForm.get('tOperacionControl');
  }

  get cdpControl() {
    return this.registerForm.get('cdpControl');
  }

  get esSolicitudInicial() {
    return this.idTipoOperacionSelecionado === 4;
  }

  get habilitaBotonLimpiar() {
    if (!this.esSolicitudInicial) {
      return this.cdpSeleccionado;
    } else {
      return this.rubroPresupuestalSeleccionado;
    }
  }

  get rubroPresupuestalSeleccionado(): boolean {
    if (this.rubroPresupuestalSinCdpSeleccionado) {
      return true;
    }
    return false;
  }

  get cdpSeleccionado(): boolean {
    if (this.cdp != null) {
      return true;
    }
    return false;
  }

  private deshabilitarControles() {
    this.tOperacionControl.disable();
    this.cdpControl.disable();
  }

  private habilitarControles() {
    this.tOperacionControl.enable();
    this.cdpControl.enable();
  }
}
