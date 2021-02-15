import { CurrencyPipe } from '@angular/common';
import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { LineaPlanPagoDto } from 'src/app/_dto/LineaPlanPagoDto';
import { Cdp } from 'src/app/_models/cdp';
import { EstadoModificacion, Mes } from 'src/app/_models/enum';
import { FormaPagoCompromiso } from 'src/app/_models/formaPagoCompromiso';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { GeneralService } from 'src/app/_services/general.service';
import { PlanPagoService } from 'src/app/_services/planPago.service';

@Component({
  selector: 'app-plan-pago-edit',
  templateUrl: './plan-pago-edit.component.html',
  styleUrls: ['./plan-pago-edit.component.scss'],
})
export class PlanPagoEditComponent implements OnInit {
  @Input() esCreacion: boolean;
  @Input() cdpSeleccionado: Cdp;
  @Output() esCancelado = new EventEmitter<boolean>();
  @ViewChild('txtValorActualizado', { static: false })
  txtValorActualizado: ElementRef;

  @ViewChild('itemSeleccionado', { static: false })
  itemSeleccionado: ElementRef;

  nombreBoton = 'Registrar';
  editForm = new FormGroup({});
  arrayControls = new FormArray([]);

  valorTotal = 0;
  mesSeleccionadoId = 0;
  mesSeleccionadoDescripcion = '';
  valorSeleccionado = 0;
  viaticos = false;

  planPagoSeleccionado: LineaPlanPagoDto = null;

  formaPagoCompromiso: FormaPagoCompromiso;
  listaLineaPlanPago: LineaPlanPagoDto[] = [];
  listaLineaPlanPagoBD: LineaPlanPagoDto[] = [];

  constructor(
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private cp: CurrencyPipe,
    private planPagoService: PlanPagoService
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    if (this.esCreacion) {
      this.nombreBoton = 'Registrar';
    } else {
      this.ObtenerLineasPlanPagoXCompromiso(this.cdpSeleccionado.crp);
      this.nombreBoton = 'Guardar';
    }
  }

  createDefaultControls() {
    for (let index = 0; index < 5; index++) {
      this.addEnero();
      this.addFebrero();
      this.addMarzo();
      this.addAbril();
      this.addMayo();
      this.addJunio();
      this.addJulio();
      this.addAgosto();
      this.addSeptiembre();
      this.addOctubre();
      this.addNoviembre();
      this.addDiciembre();
    }
  }

  createEmptyForm() {
    this.editForm = this.fb.group({
      eneroCtrl: [''],
      febreroCtrl: [''],
      marzoCtrl: [''],
      abrilCtrl: [''],
      mayoCtrl: [''],
      junioCtrl: [''],
      julioCtrl: [''],
      agostoCtrl: [''],
      septiembreCtrl: [''],
      octubreCtrl: [''],
      noviembreCtrl: [''],
      diciembreCtrl: [''],
      valorCtrl: [''],
      viaticosCtrl: [''],
      planPagoControles: this.arrayControls,
    });
  }

  ObtenerLineasPlanPagoXCompromiso(crp: number) {
    this.planPagoService.ObtenerLineasPlanPagoXCompromiso(crp).subscribe(
      (lineas: LineaPlanPagoDto[]) => {
        if (lineas) {
          this.listaLineaPlanPago.push(...lineas);
          this.listaLineaPlanPagoBD.push(...lineas);
        } else {
          this.alertify.error(
            'No se pudo obtener información de los planes de pago para el compromiso seleccionado'
          );
        }
      },
      (error) => {
        this.alertify.error(
          'Hubó un error al obtener los planes de pago ' + error
        );
      },
      () => {
        this.createFullForm();
      }
    );
  }

  createFullForm() {
    if (this.listaLineaPlanPago && this.listaLineaPlanPago.length > 0) {
      const lineaPlanPago = this.listaLineaPlanPago[0];
      this.viaticos = lineaPlanPago.viaticos;
    }

    this.listaLineaPlanPago.forEach((x) => {
      this.arrayControls.push(
        new FormGroup({
          rubroControl: new FormControl(''),
        })
      );
    });
    this.editForm.patchValue({
      viaticosCtrl: this.viaticos,
    });
    this.editForm.setControl('planPagoControles', this.arrayControls);
    this.actualizarValorTotal();
  }

  onCheckChange(event) {
    /* Selected */

    if (event.target.checked) {
      // Add a new control in the arrayForm
      const id = +event.target.value;
      this.planPagoSeleccionado = this.listaLineaPlanPago.filter(
        (x) => x.planPagoId === id
      )[0];

      this.valorSeleccionado = this.planPagoSeleccionado.valor;

      this.editForm.patchValue({
        valorCtrl: GeneralService.obtenerFormatoLongMoney(
          this.valorSeleccionado
        ),
      });
      this.valorCtrl.enable();
      this.txtValorActualizado.nativeElement.focus();
    }
  }

  onAgregar() {
    const formValues = Object.assign({}, this.editForm.value);

    if (formValues.eneroCtrl !== '') {
      this.agregarNuevoItem(Mes.ENERO, formValues.eneroCtrl);
    }
    if (formValues.febreroCtrl !== '') {
      this.agregarNuevoItem(Mes.FEBRERO, formValues.febreroCtrl);
    }
    if (formValues.marzoCtrl !== '') {
      this.agregarNuevoItem(Mes.MARZO, formValues.marzoCtrl);
    }
    if (formValues.abrilCtrl !== '') {
      this.agregarNuevoItem(Mes.ABRIL, formValues.abrilCtrl);
    }
    if (formValues.mayoCtrl !== '') {
      this.agregarNuevoItem(Mes.MAYO, formValues.mayoCtrl);
    }
    if (formValues.junioCtrl !== '') {
      this.agregarNuevoItem(Mes.JUNIO, formValues.junioCtrl);
    }
    if (formValues.julioCtrl !== '') {
      this.agregarNuevoItem(Mes.JULIO, formValues.julioCtrl);
    }
    if (formValues.agostoCtrl !== '') {
      this.agregarNuevoItem(Mes.AGOSTO, formValues.agostoCtrl);
    }
    if (formValues.septiembreCtrl !== '') {
      this.agregarNuevoItem(Mes.SEPTIEMBRE, formValues.septiembreCtrl);
    }
    if (formValues.octubreCtrl !== '') {
      this.agregarNuevoItem(Mes.OCTUBRE, formValues.octubreCtrl);
    }
    if (formValues.noviembreCtrl !== '') {
      this.agregarNuevoItem(Mes.NOVIEMBRE, formValues.noviembreCtrl);
    }
    if (formValues.diciembreCtrl !== '') {
      this.agregarNuevoItem(Mes.DICIEMBRE, formValues.diciembreCtrl);
    }

    this.limpiarControles();
  }

  onActualizar() {
    const formValues = Object.assign({}, this.editForm.value);
    const valor = GeneralService.obtenerValorAbsoluto(formValues.valorCtrl);

    const planPagoActualizar = this.listaLineaPlanPago.filter(
      (x) => x.planPagoId === this.planPagoSeleccionado.planPagoId
    )[0];
    planPagoActualizar.valor = valor;
    planPagoActualizar.estadoModificacion = EstadoModificacion.Modificado;

    this.planPagoSeleccionado = null;
    this.actualizarValorTotal();
    this.limpiarControles();
    this.valorCtrl.disable();
    this.itemSeleccionado.nativeElement.checked = false;
  }

  onEliminar() {
    this.alertify.confirm2(
      'Plan de Pago',
      '¿Esta seguro que desea eliminar el Plan de Pago?',
      () => {
        const index = this.listaLineaPlanPago.indexOf(
          this.planPagoSeleccionado,
          0
        );
        this.listaLineaPlanPago.splice(index, 1);

        this.planPagoSeleccionado = null;
        this.actualizarValorTotal();
        this.limpiarControles();
        this.valorCtrl.disable();
      }
    );
  }

  agregarNuevoItem(mes: number, valorRegistrado: any) {
    const valor = GeneralService.obtenerValorAbsoluto(valorRegistrado);
    this.valorTotal = +this.valorTotal + +valor;
    const mesDesc = this.obtenerMes(mes);
    const idInterno = this.listaLineaPlanPago.length;

    const item: LineaPlanPagoDto = {
      id: idInterno,
      planPagoId: idInterno,
      mesDescripcion: mesDesc,
      mesId: mes,
      valor: +valor,
      estadoModificacion: EstadoModificacion.Insertado,
      viaticos: false,
    };
    this.listaLineaPlanPago.push(item);
    this.arrayControls.push(
      new FormGroup({
        rubroControl: new FormControl(''),
      })
    );
    this.actualizarValorTotal();

    if (this.validarValorTotalSuperior()) {
      this.alertify.warning(
        'La suma de los planes de pago es superior al valor del CRP'
      );
    }
  }

  actualizarValorTotal() {
    let sum = 0;
    this.listaLineaPlanPago.forEach((a) => (sum += +a.valor));
    this.valorTotal = sum;
  }

  obtenerMes(mesId: number): string {
    return Mes[mesId];
  }

  onGuardar() {
    if (this.validarValoresTotales()) {
      const viaticos = this.viaticosCtrl.value as boolean;

      this.listaLineaPlanPago.forEach((x) => {
        x.viaticos = viaticos;
        x.estadoModificacion = EstadoModificacion.Modificado;
      });

      this.formaPagoCompromiso = {
        cdp: this.cdpSeleccionado,
        listaLineaPlanPago: this.listaLineaPlanPago,
      };

      if (!this.esCreacion) {
        this.actualizarListaLineaPlanPago();
      }

      this.planPagoService
        .RegistrarFormaPagoCompromiso(
          this.esCreacion ? 1 : 2,
          this.formaPagoCompromiso
        )
        .subscribe(
          (response: any) => {
            if (!isNaN(response)) {
              if (this.esCreacion) {
                this.alertify.success(
                  'Los planes de pago se registraron satisfactoriamente'
                );
              } else {
                this.alertify.success(
                  'Los planes de pago se actualizaron correctamente'
                );
              }
            } else {
              this.alertify.error('No se pudo registrar los planes de pago');
            }
          },
          (error) => {
            this.alertify.error(
              'Hubó un error al registrar los planes de pago ' + error
            );
          },
          () => {
            this.esCancelado.emit(true);
          }
        );
    } else {
      this.alertify.warning(
        'La suma total de los planes de pago registrados es: ' +
          this.cp.transform(this.valorTotal, 'COP', 'symbol', '1.2-2') +
          ' y no es igual al saldo del compromiso'
      );
    }
  }

  onCancelar() {
    this.esCancelado.emit(true);
  }

  validarValoresTotales(): boolean {
    if (this.valorTotal === this.cdpSeleccionado.valorTotal) {
      return true;
    }
    return false;
  }

  validarValorTotalSuperior(): boolean {
    if (this.valorTotal > this.cdpSeleccionado.valorTotal) {
      return true;
    }
    return false;
  }

  actualizarListaLineaPlanPago() {
    this.listaLineaPlanPago.forEach((x) => {
      const planPago = this.listaLineaPlanPagoBD.filter(
        (y) => y.planPagoId === x.planPagoId
      )[0];

      if (planPago === null || planPago === undefined) {
        x.estadoModificacion = EstadoModificacion.Insertado;
      }
    });
  }

  //#region Meses

  newEnero(): FormGroup {
    return this.fb.group({
      eneroCtrl: new FormControl(''),
    });
  }
  newFebrero(): FormGroup {
    return this.fb.group({
      febreroCtrl: new FormControl(''),
    });
  }
  newMarzo(): FormGroup {
    return this.fb.group({
      marzoCtrl: new FormControl(''),
    });
  }
  newAbril(): FormGroup {
    return this.fb.group({
      abrilCtrl: new FormControl(''),
    });
  }
  newMayo(): FormGroup {
    return this.fb.group({
      mayoCtrl: new FormControl(''),
    });
  }
  newJunio(): FormGroup {
    return this.fb.group({
      junioCtrl: new FormControl(''),
    });
  }
  newJulio(): FormGroup {
    return this.fb.group({
      julioCtrl: new FormControl(''),
    });
  }
  newAgosto(): FormGroup {
    return this.fb.group({
      agostoCtrl: new FormControl(''),
    });
  }
  newSeptiembre(): FormGroup {
    return this.fb.group({
      septiembreCtrl: new FormControl(''),
    });
  }
  newOctubre(): FormGroup {
    return this.fb.group({
      octubreCtrl: new FormControl(''),
    });
  }
  newNoviembre(): FormGroup {
    return this.fb.group({
      noviembreCtrl: new FormControl(''),
    });
  }
  newDiciembre(): FormGroup {
    return this.fb.group({
      diciembreCtrl: new FormControl(''),
    });
  }

  addEnero() {
    this.listaEnero().push(this.newEnero());
  }

  addFebrero() {
    this.listaFebrero().push(this.newFebrero());
  }

  addMarzo() {
    this.listaMarzo().push(this.newMarzo());
  }

  addAbril() {
    this.listaAbril().push(this.newAbril());
  }

  addMayo() {
    this.listaMayo().push(this.newMayo());
  }

  addJunio() {
    this.listaJunio().push(this.newJunio());
  }
  addJulio() {
    this.listaJulio().push(this.newJulio());
  }
  addAgosto() {
    this.listaAgosto().push(this.newAgosto());
  }
  addSeptiembre() {
    this.listaSeptiembre().push(this.newSeptiembre());
  }
  addOctubre() {
    this.listaOctubre().push(this.newOctubre());
  }
  addNoviembre() {
    this.listaNoviembre().push(this.newNoviembre());
  }
  addDiciembre() {
    this.listaDiciembre().push(this.newDiciembre());
  }

  //#endregion Meses

  //#region Lista de FormArray

  listaEnero(): FormArray {
    return this.editForm.get('listaEnero') as FormArray;
  }

  listaFebrero(): FormArray {
    return this.editForm.get('listaFebrero') as FormArray;
  }

  listaMarzo(): FormArray {
    return this.editForm.get('listaMarzo') as FormArray;
  }

  listaAbril(): FormArray {
    return this.editForm.get('listaAbril') as FormArray;
  }

  listaMayo(): FormArray {
    return this.editForm.get('listaMayo') as FormArray;
  }

  listaJunio(): FormArray {
    return this.editForm.get('listaJunio') as FormArray;
  }
  listaJulio(): FormArray {
    return this.editForm.get('listaJulio') as FormArray;
  }
  listaAgosto(): FormArray {
    return this.editForm.get('listaAgosto') as FormArray;
  }
  listaSeptiembre(): FormArray {
    return this.editForm.get('listaSeptiembre') as FormArray;
  }
  listaOctubre(): FormArray {
    return this.editForm.get('listaOctubre') as FormArray;
  }
  listaNoviembre(): FormArray {
    return this.editForm.get('listaNoviembre') as FormArray;
  }
  listaDiciembre(): FormArray {
    return this.editForm.get('listaDiciembre') as FormArray;
  }

  //#endregion Lista de FormArray

  //#region Lectura de controles

  // leerValoresEnero() {
  //   let valor = 0;
  //   if (this.listaEnero() && this.listaEnero().length > 0) {
  //     for (let index = 0; index < this.listaEnero().length; index++) {
  //       const item = this.listaEnero().at(index);

  //       if (item.value.eneroCtrl !== '') {
  //         valor = GeneralService.obtenerValorAbsoluto(item.value.eneroCtrl);
  //         this.valorTotalEnero = +this.valorTotalEnero + +valor;

  //       }
  //     }
  //   }
  // }

  //#endregion Lectura de controles

  limpiarValores() {
    this.valorTotal = 0;

    this.limpiarControles();

    this.listaLineaPlanPago = [];
  }

  limpiarControles() {
    this.eneroCtrl.reset('');
    this.febreroCtrl.reset('');
    this.marzoCtrl.reset('');
    this.abrilCtrl.reset('');
    this.mayoCtrl.reset('');
    this.junioCtrl.reset('');
    this.julioCtrl.reset('');
    this.agostoCtrl.reset('');
    this.septiembreCtrl.reset('');
    this.octubreCtrl.reset('');
    this.noviembreCtrl.reset('');
    this.diciembreCtrl.reset('');
    this.valorCtrl.reset('');
  }

  //#region Controles

  get valorCtrl() {
    return this.editForm.get('valorCtrl');
  }

  get eneroCtrl() {
    return this.editForm.get('eneroCtrl');
  }

  get febreroCtrl() {
    return this.editForm.get('febreroCtrl');
  }

  get marzoCtrl() {
    return this.editForm.get('marzoCtrl');
  }

  get abrilCtrl() {
    return this.editForm.get('abrilCtrl');
  }

  get mayoCtrl() {
    return this.editForm.get('mayoCtrl');
  }

  get junioCtrl() {
    return this.editForm.get('junioCtrl');
  }

  get julioCtrl() {
    return this.editForm.get('julioCtrl');
  }

  get agostoCtrl() {
    return this.editForm.get('agostoCtrl');
  }

  get septiembreCtrl() {
    return this.editForm.get('septiembreCtrl');
  }

  get octubreCtrl() {
    return this.editForm.get('octubreCtrl');
  }

  get noviembreCtrl() {
    return this.editForm.get('noviembreCtrl');
  }

  get diciembreCtrl() {
    return this.editForm.get('diciembreCtrl');
  }

  get viaticosCtrl() {
    return this.editForm.get('viaticosCtrl');
  }

  //#endregion Controles
}
