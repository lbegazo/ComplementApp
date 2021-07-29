import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { combineLatest, Subscription } from 'rxjs';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';
import { Cdp } from 'src/app/_models/cdp';
import { TipoDocumento } from 'src/app/_models/enum';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { SolicitudCDP } from 'src/app/_models/solicitudCDP';
import { SolicitudCDPDto } from 'src/app/_models/solicitudCDPDto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CdpService } from 'src/app/_services/cdp.service';
import { ClavePresupuestalContableService } from 'src/app/_services/clavePresupuestalContable.service';
import { SolicitudCdpService } from 'src/app/_services/solicitudCdp.service';

@Component({
  selector: 'app-vincular-cdp-solicitud-edit',
  templateUrl: './vincular-cdp-solicitud-edit.component.html',
  styleUrls: ['./vincular-cdp-solicitud-edit.component.scss'],
})
export class VincularCdpSolicitudEditComponent implements OnInit {
  @Input() esCreacion: boolean;
  @Input() solicitudCdpSeleccionado: SolicitudCDPDto;
  @Output() esCancelado = new EventEmitter<boolean>();

  solicitudCDPId: number;
  solicitudRegistrada = false;
  nombreBoton = 'Registrar';

  listaCdp: Cdp[] = [];
  cdp = 0;

  editForm = new FormGroup({});
  arrayControls = new FormArray([]);
  subscriptions: Subscription[] = [];
  bsModalRef: BsModalRef;

  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
    maxSize: 10,
  };

  constructor(
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private changeDetection: ChangeDetectorRef,
    private cdpService: CdpService,
    private solicitudService: SolicitudCdpService
  ) {}

  ngOnInit() {
    this.solicitudCDPId = this.solicitudCdpSeleccionado.solicitudCDPId;

    this.createForm();

    if (!this.esCreacion) {
      this.nombreBoton = 'Actualizar';
    } else {
      this.nombreBoton = 'Registrar';
    }

    this.onBuscarFactura();
  }

  createForm() {
    this.editForm = this.fb.group({
      planPagoControles: this.arrayControls,
    });
  }

  onBuscarFactura() {
    this.cdpService
      .ObtenerListaCdpParaVinculacion(
        TipoDocumento.Cdp.valueOf(),
        0,
        this.pagination.currentPage,
        this.pagination.itemsPerPage
      )
      .subscribe(
        (documentos: PaginatedResult<Cdp[]>) => {
          this.listaCdp = documentos.result;
          this.pagination = documentos.pagination;

          this.crearControlesDeArray();
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.editForm = this.fb.group({
            planPagoControles: this.arrayControls,
          });
        }
      );
  }

  crearControlesDeArray() {
    if (this.listaCdp && this.listaCdp.length > 0) {
      for (const detalle of this.listaCdp) {
        this.arrayControls.push(
          new FormGroup({
            rubroControl: new FormControl(''),
          })
        );
      }
    } else {
      this.alertify.warning(
        'No existen CDPs para vincular a la solicitud seleccionada'
      );
    }
  }

  onCheckChange(event) {
    /* Selected */
    this.cdp = 0;
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.cdp = +event.target.value;
    }
  }

  onGuardar() {
    if (this.editForm.valid) {
      const solicitudCDP: SolicitudCDP = new SolicitudCDP();
      solicitudCDP.solicitudCDPId =
        this.solicitudCdpSeleccionado.solicitudCDPId;
      solicitudCDP.cdp = this.cdp;

      this.solicitudService.ActualizarSolicitudCDP(solicitudCDP).subscribe(
        () => {
          this.alertify.success(
            'Se le vinculó el número de CDP a la solicitud seleccionada'
          );
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

  onCancelar() {
    this.esCancelado.emit(true);
  }

  unsubscribe() {
    this.subscriptions.forEach((subscription: Subscription) => {
      subscription.unsubscribe();
    });
    this.subscriptions = [];
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.onBuscarFactura();
  }
}
