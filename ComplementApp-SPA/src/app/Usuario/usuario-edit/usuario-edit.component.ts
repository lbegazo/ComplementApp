import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Usuario } from 'src/app/_models/usuario';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { UsuarioService } from 'src/app/_services/usuario.service';
import { Area } from 'src/app/_models/area';
import { Cargo } from 'src/app/_models/cargo';

@Component({
  selector: 'app-usuario-edit',
  templateUrl: './usuario-edit.component.html',
  styleUrls: ['./usuario-edit.component.css'],
})
export class UsuarioEditComponent implements OnInit {
  @Output() cancelRegisterEvent = new EventEmitter();
  user: Usuario = {
    id: 0,
    username: '',
    nombres: '',
    apellidos: '',
    password: '',
    cargoId: 0,
    areaId: 0,
    areaDescripcion: '',
    cargoDescripcion: '',
    esAdministrador: 0,
    fechaCreacion: new Date(),
    fechaUltimoAcceso: new Date(),
  };
  registerForm: FormGroup;
  areas: Area[];
  cargos: Cargo[];
  areaSelected: Area = { id: 0, codigo: '', descripcion: '' };
  cargoSelected: Cargo = { id: 0, codigo: '', descripcion: '' };

  constructor(
    private usuarioService: UsuarioService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.cargarCargos();
    this.cargarAreas();
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        username: ['', Validators.required],
        nombres: ['', Validators.required],
        apellidos: ['', Validators.required],
        areaControl: [null, Validators.required],
        cargoControl: [null, Validators.required],
        EsAdministradorControl: [false],
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
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  onRegister() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);

      // this.user.username = 'test';
      // this.user.nombres = 'test';
      // this.user.apellidos = 'test';
      this.user.areaId = this.areaSelected.id;
      this.user.cargoId = this.cargoSelected.id;
      this.user.esAdministrador = this.registerForm.get(
        'EsAdministradorControl'
      ).value
        ? 1
        : 0;
      console.log(this.user);
      this.usuarioService.RegistrarUsuario(this.user).subscribe(
        () =>
          this.alertify.success('El usuario se registró satisfactoriamente'),
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.router.navigate(['/usuarios/']);
        }
      );
    }
  }

  onCancel() {
    this.cancelRegisterEvent.emit(false);
    this.alertify.error('Cancelado');
  }

  onSelectArea() {
    this.areaSelected = this.areaControl.value;
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

  cargarCargos() {
    this.usuarioService.ObtenerCargos().subscribe(
      (cargos: Cargo[]) => {
        this.cargos = cargos;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  cargarAreas() {
    this.usuarioService.ObtenerAreas().subscribe(
      (result: Area[]) => {
        this.areas = result;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
