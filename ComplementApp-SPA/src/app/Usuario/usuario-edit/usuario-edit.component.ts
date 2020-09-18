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
  arrayPerfiles = new FormArray([]);
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
    this.route.params.subscribe((params: Params) => {
      this.idUsuario = +params['id'];
      this.editMode = params['id'] != null;
      this.initForm();
    });
  }

  private initForm() {
    this.cargarCargos();
    this.cargarAreas();
    this.cargarPerfiles();

    this.createRegisterForm();

    if (this.editMode) {
      this.usuarioService
        .ObtenerUsuario(this.idUsuario)
        .subscribe((usuario: Usuario) => {
          this.user = usuario;

          this.registerForm.get('username').setValue(this.user.username);
          this.registerForm.get('nombres').setValue(this.user.nombres);
          this.registerForm.get('apellidos').setValue(this.user.apellidos);
          this.registerForm.get('areaControl').setValue(this.user.areaId);
          this.registerForm.get('cargoControl').setValue(this.user.cargoId);

          this.areaSelected = this.user.areaId;
          this.cargoSelected = this.user.cargoId;
        });
    }
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        username: ['', Validators.required],
        nombres: ['', Validators.required],
        apellidos: ['', Validators.required],
        areaControl: [null, Validators.required],
        cargoControl: [null, Validators.required],
        perfilesControles: this.arrayPerfiles,
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(4),
            Validators.maxLength(8),
          ],
        ],
        confirmPassword: ['', Validators.required],
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
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  onRegister() {
    if (this.registerForm.valid) {
      if (!this.editMode) {
        this.user = Object.assign({}, this.registerForm.value);
        this.user.nombres = this.user.nombres.toUpperCase().trim();
        this.user.apellidos = this.user.apellidos.toUpperCase().trim();
        this.user.areaId = this.areaSelected;
        this.user.cargoId = this.cargoSelected;
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
        this.user = Object.assign({}, this.registerForm.value);
        this.user.nombres = this.user.nombres.toUpperCase().trim();
        this.user.apellidos = this.user.apellidos.toUpperCase().trim();
        this.user.areaId = this.areaSelected;
        this.user.cargoId = this.cargoSelected;

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
    return this.registerForm.get('username') as FormControl;
  }

  get passwordControl() {
    return this.registerForm.get('password') as FormControl;
  }

  get confirmPasswordControl() {
    return this.registerForm.get('confirmPassword') as FormControl;
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
        if (this.perfiles) {
          for (const detalle of this.perfiles) {
            this.arrayPerfiles.push(
              new FormGroup({
                perfilControl: new FormControl('', [Validators.required]),
              })
            );
          }
        }
      }
    );
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

      // console.log(event.target.value + ' ' + index);
      // if (index !== -1) {
      //   this.arrayRubro.splice(index, 1);
      // }
      // let i: number = 0;
      // this.rubrosControles.controls.forEach((ctrl: FormControl) => {
      //   if (ctrl.value === event.target.value) {
      //     // Remove the unselected element from the arrayForm
      //     this.arrayRubro.removeAt(i);
      //     return;
      //   }
      //   i++;
      // });
    }
  }
}
