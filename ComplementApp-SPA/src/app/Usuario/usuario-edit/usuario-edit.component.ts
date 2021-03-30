import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  ViewChild,
} from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormControl,
  NgForm,
  FormArray,
} from '@angular/forms';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { Area } from 'src/app/_models/area';
import { Cargo } from 'src/app/_models/cargo';
import { ListaService } from 'src/app/_services/lista.service';
import { Perfil } from 'src/app/_models/perfil';
import { ValorSeleccion } from 'src/app/_dto/valorSeleccion';

@Component({
  selector: 'app-usuario-edit',
  templateUrl: './usuario-edit.component.html',
  styleUrls: ['./usuario-edit.component.css'],
})
export class UsuarioEditComponent implements OnInit {
  @ViewChild('editForm', { static: true }) editForm: NgForm;
  idUsuario = 0;
  editMode = false;
  nombreBoton = 'Registrar';
  @Output() cancelRegisterEvent = new EventEmitter();
  user: Usuario = {
    usuarioId: 0,
    username: '',
    nombres: '',
    apellidos: '',
    password: '',
    cargoId: 0,
    areaId: 0,
    areaNombre: '',
    cargoNombre: '',
    numeroIdentificacion: '',
    fechaCreacion: new Date(),
    fechaUltimoAcceso: new Date(),
    nombreCompleto: '',
    perfiles: [],
    pciId: 0,
  };
  registerForm = new FormGroup({});
  arrayControls = new FormArray([]);

  areas: Area[] = [];
  cargos: Cargo[] = [];
  listaPci: ValorSeleccion[] = [];
  perfiles: Perfil[];
  areaSelected?: number;
  cargoSelected?: number;
  pciSelected?: number;
  arrayRubro: number[] = [];

  areaSeleccionada: Area = null;
  cargoSeleccionado: Cargo = null;
  pciSeleccionado: ValorSeleccion = null;

  isDataLoaded = false;

  constructor(
    private listaService: ListaService,
    private usuarioService: UsuarioService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.createEmptyForm();

    this.cargarListasResolver();

    this.route.params.subscribe((params: Params) => {
      this.idUsuario = +params['id'];
      this.editMode = params['id'] != null;
      this.initForm();
    });

    if (!this.editMode) {
      this.nombreBoton = 'Registrar';
      this.cargarPerfiles();
    } else {
      this.nombreBoton = 'Modificar';
    }
  }

  private initForm() {
    if (this.editMode) {
      this.obtenerUsuario();
    }
  }

  cargarListasResolver() {
    this.route.data.subscribe((data) => {
      this.areas = data['areas'];
    });

    this.route.data.subscribe((data) => {
      this.cargos = data['cargos'];
    });

    this.route.data.subscribe((data) => {
      this.listaPci = data['pcis'];
    });

    this.route.data.subscribe((data) => {
      this.perfiles = data['perfiles'];
    });
  }

  createEmptyForm() {
    this.registerForm = this.fb.group(
      {
        usernameCtrl: ['', Validators.required],
        nombreCtrl: ['', Validators.required],
        apellidoCtrl: ['', Validators.required],
        areaControl: [0, Validators.required],
        cargoControl: [0, Validators.required],
        pciControl: [0, Validators.required],
        perfilesControles: this.arrayControls,
        passwordCtrl: [
          '',
          [
            Validators.required,
            Validators.minLength(4),
            Validators.maxLength(8),
          ],
        ],
        confirmPasswordCtrl: ['', Validators.required],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  obtenerUsuario() {
    this.usuarioService.ObtenerUsuario(this.idUsuario).subscribe(
      (usuario: Usuario) => {
        this.user = usuario;
      },
      (error) => {
        this.alertify.error(
          'Ocurrió un error al cargar información del usuario'
        );
      },
      () => {
        this.cargarInformacionUsuario();
      }
    );
  }

  cargarInformacionUsuario() {
    const userNameC = this.user.username;
    const nombresC = this.user.nombres;
    const apellidosC = this.user.apellidos;

    this.areaSelected = this.user.areaId;
    this.cargoSelected = this.user.cargoId;
    this.pciSelected = this.user.pciId;

    this.areaSeleccionada = this.areas.filter(
      (x) => x.areaId === this.areaSelected
    )[0];

    this.cargoSeleccionado = this.cargos.filter(
      (x) => x.cargoId === this.cargoSelected
    )[0];

    this.pciSeleccionado = this.listaPci.filter(
      (x) => x.id === this.pciSelected
    )[0];

    if (this.perfiles) {
      for (const perfil of this.perfiles) {
        perfil.checked = false;
        if (this.user.perfiles && this.user.perfiles.length > 0) {
          for (const item of this.user.perfiles) {
            if (perfil.perfilId === item.perfilId) {
              perfil.checked = true;
            }
          }
        }
      }

      for (const perfil of this.perfiles) {
        this.arrayControls.push(new FormControl(perfil.checked || false));
      }
    }

    this.registerForm.patchValue({
      usernameCtrl: userNameC,
      nombreCtrl: nombresC,
      apellidoCtrl: apellidosC,
      areaControl: this.areaSeleccionada,
      cargoControl: this.cargoSeleccionado,
      pciControl: this.pciSeleccionado,
    });

    this.registerForm.setControl('perfilesControles', this.arrayControls);

    if (this.editMode) {
      this.usernameControl.disable();
      this.passwordControl.disable();
      this.confirmPasswordControl.disable();
      //document.getElementById('btnGuardar').innerHTML = 'Guardar';
    }
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('passwordCtrl').value === g.get('confirmPasswordCtrl').value
      ? null
      : { mismatch: true };
  }

  onRegister() {
    if (this.registerForm.valid) {
      if (!this.editMode) {
        const formValues = Object.assign({}, this.registerForm.value);
        this.user.username = formValues.usernameCtrl.trim();
        this.user.nombres = formValues.nombreCtrl.toUpperCase().trim();
        this.user.apellidos = formValues.apellidoCtrl.toUpperCase().trim();
        this.user.password = formValues.passwordCtrl;
        this.user.areaId = this.areaSelected;
        this.user.cargoId = this.cargoSelected;
        this.user.pciId = this.pciSelected;
        this.setearPerfilesAUsuario(formValues);
        this.usuarioService.RegistrarUsuario(this.user).subscribe(
          () => {
            this.alertify.success('El usuario se registró satisfactoriamente');
          },
          (error) => {
            this.alertify.error(error);
          },
          () => {
            this.router.navigate(['/ADMINISTRACION_USUARIO']);
          }
        );
      } else {
        const formValues = Object.assign({}, this.registerForm.value);
        this.user.nombres = formValues.nombreCtrl.toUpperCase().trim();
        this.user.apellidos = formValues.apellidoCtrl.toUpperCase().trim();
        this.user.areaId = this.areaSelected;
        this.user.cargoId = this.areaSelected;
        this.user.pciId = this.pciSelected;
        this.setearPerfilesAUsuario(formValues);

        this.usuarioService
          .ActualizarUsuario(this.idUsuario, this.user)
          .subscribe(
            (response: boolean) => {},
            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.alertify.success('El usuario se actualizó correctamente');
              this.router.navigate(['../../'], { relativeTo: this.route });
            }
          );
      }
    }
  }

  onCancel() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  onSelectArea() {
    this.areaSeleccionada = this.areaControl.value as Area;
    this.areaSelected = this.areaSeleccionada.areaId;
  }

  get guardarControl() {
    return this.registerForm.get('btnGuardar');
  }

  get areaControl() {
    return this.registerForm.get('areaControl');
  }

  onSelectCargo() {
    this.cargoSeleccionado = this.cargoControl.value as Cargo;
    this.cargoSelected = this.cargoSeleccionado.cargoId;
  }

  onSelectPci() {
    this.pciSeleccionado = this.pciControl.value as ValorSeleccion;
    this.pciSelected = this.pciSeleccionado.id;
  }

  get pciControl() {
    return this.registerForm.get('pciControl');
  }

  get cargoControl() {
    return this.registerForm.get('cargoControl');
  }

  get usernameControl() {
    return this.registerForm.get('usernameCtrl') as FormControl;
  }

  get passwordControl() {
    return this.registerForm.get('passwordCtrl') as FormControl;
  }

  get confirmPasswordControl() {
    return this.registerForm.get('confirmPasswordCtrl') as FormControl;
  }

  get perfilesControl() {
    return (this.registerForm.get('perfilesControles') as FormArray).controls;
  }

  cargarPerfiles() {
    if (this.perfiles && this.perfiles.length > 0) {
      for (const perfil of this.perfiles) {
        this.arrayControls.push(new FormControl(false));
      }
    }
  }

  createPerfilesControles() {
    const arrayPerfiles = new FormArray([]);
    if (this.perfiles) {
      for (const perfil of this.perfiles) {
        arrayPerfiles.push(new FormControl(perfil.checked || false));
      }
    }
    return arrayPerfiles;
  }

  onCheckChange(event) {
    /* Selected */
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.arrayRubro?.push(+event.target.value);
    } else {
      /* unselected */
      let index = 0;
      let i = 0;
      this.arrayRubro.forEach((val: number) => {
        if (val === event.target.value) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.arrayRubro.splice(index, 1);
      }
    }
  }

  setearPerfilesAUsuario(formValues: any) {
    this.user.perfiles = [];
    if (formValues.perfilesControles) {
      for (
        let index = 0;
        index < formValues.perfilesControles.length;
        index++
      ) {
        const element = formValues.perfilesControles[index];
        if (element) {
          const perfil = this.perfiles[index];
          this.user.perfiles.push(perfil);
        }
      }
    }
  }
}
