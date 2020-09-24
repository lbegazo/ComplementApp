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

@Component({
  selector: 'app-usuario-edit',
  templateUrl: './usuario-edit.component.html',
  styleUrls: ['./usuario-edit.component.css'],
})
export class UsuarioEditComponent implements OnInit {
  @ViewChild('editForm', { static: true }) editForm: NgForm;
  idUsuario = 0;
  editMode = false;
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
    fechaCreacion: new Date(),
    fechaUltimoAcceso: new Date(),
    perfiles: [],
  };
  registerForm = new FormGroup({});

  areas: Area[];
  cargos: Cargo[];
  perfiles: Perfil[];
  areaSelected = 0;
  cargoSelected = 0;
  arrayRubro: number[] = [];

  constructor(
    private listaService: ListaService,
    private usuarioService: UsuarioService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    //Cargar datos de controles
    this.cargarListas();

    this.createEmptyForm();

    this.route.params.subscribe((params: Params) => {
      this.idUsuario = +params['id'];
      this.editMode = params['id'] != null;
      this.initForm();
    });
  }

  private initForm() {
    if (this.editMode) {
      this.obtenerUsuario();
    }
  }

  createEmptyForm() {
    this.registerForm = this.fb.group(
      {
        usernameCtrl: ['', Validators.required],
        nombreCtrl: ['', Validators.required],
        apellidoCtrl: ['', Validators.required],
        areaControl: ['', Validators.required],
        cargoControl: ['', Validators.required],
        perfilesControles: this.createPerfilesControles(),
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

  private cargarListas() {
    this.cargarCargos();
    this.cargarAreas();
    this.cargarPerfiles();
  }

  cargarInformacionUsuario() {
    const userNameC = this.user.username;
    const nombresC = this.user.nombres;
    const apellidosC = this.user.apellidos;
    const areaIdC = this.user.areaId;
    const cargoIdC = this.user.cargoId;

    this.areaSelected = this.user.areaId;
    this.cargoSelected = this.user.cargoId;

    // this.registerForm.get('username').setValue(this.user.username);
    // this.registerForm.get('nombres').setValue(this.user.nombres);
    // this.registerForm.get('apellidos').setValue(this.user.apellidos);
    // this.registerForm.get('areaControl').setValue(this.user.areaId);
    // this.registerForm.get('cargoControl').setValue(this.user.cargoId);

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
    }

    this.registerForm = this.fb.group(
      {
        usernameCtrl: [userNameC, Validators.required],
        nombreCtrl: [nombresC, Validators.required],
        apellidoCtrl: [apellidosC, Validators.required],
        areaControl: [areaIdC, Validators.required],
        cargoControl: [cargoIdC, Validators.required],
        perfilesControles: this.createPerfilesControles(),
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

    if (this.editMode) {
      this.usernameControl.disable();
      this.passwordControl.disable();
      this.confirmPasswordControl.disable();
      document.getElementById('btnGuardar').innerHTML = 'Guardar';
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
        this.user.areaId = formValues.areaControl;
        this.user.cargoId = formValues.cargoControl;
        this.setearPerfilesAUsuario(formValues);
        this.usuarioService.RegistrarUsuario(this.user).subscribe(
          () => {
            this.alertify.success('El usuario se registró satisfactoriamente');
          },
          (error) => {
            this.alertify.error(error);
          },
          () => {
            this.router.navigate(['/usuarios']);
          }
        );
      } else {
        const formValues = Object.assign({}, this.registerForm.value);
        this.user.nombres = formValues.nombreCtrl.toUpperCase().trim();
        this.user.apellidos = formValues.apellidoCtrl.toUpperCase().trim();
        this.user.areaId = formValues.areaControl;
        this.user.cargoId = formValues.cargoControl;
        this.setearPerfilesAUsuario(formValues);

        this.usuarioService
          .ActualizarUsuario(this.idUsuario, this.user)
          .subscribe(
            () => {
              this.alertify.success('El usuario se actualizó correctamente');
              this.editForm.reset(this.user);
            },

            (error) => {
              this.alertify.error(error);
            },
            () => {
              this.router.navigate(['/usuarios']);
            }
          );
      }
    }
  }

  onCancel() {
    this.alertify.error('Cancelado');
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  onSelectArea() {
    this.areaSelected = this.areaControl.value;
  }

  get guardarControl() {
    return this.registerForm.get('btnGuardar');
  }

  get areaControl() {
    return this.registerForm.get('areaControl');
  }

  onSelectCargo() {
    this.cargoSelected = this.cargoControl.value;
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

  cargarCargos() {
    this.listaService.ObtenerCargos().subscribe(
      (cargos: Cargo[]) => {
        this.cargos = cargos;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  cargarAreas() {
    this.listaService.ObtenerAreas().subscribe(
      (result: Area[]) => {
        this.areas = result;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  cargarPerfiles() {
    this.listaService.ObtenerListaPerfiles().subscribe(
      (result: Perfil[]) => {
        this.perfiles = result;
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.createEmptyForm();
      }
    );
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
