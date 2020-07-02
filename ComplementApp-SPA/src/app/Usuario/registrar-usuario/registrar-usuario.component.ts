import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Usuario } from 'src/app/_models/usuario';
import { Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-registrar-usuario',
  templateUrl: './registrar-usuario.component.html',
  styleUrls: ['./registrar-usuario.component.css']
})
export class RegistrarUsuarioComponent implements OnInit {

  @Output() cancelRegisterEvent = new EventEmitter();
  usuario: Usuario;
  registerForm: FormGroup;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        username: ['', Validators.required],
        nombres: ['', Validators.required],
        apellidos: [null, Validators.required],
        areaId: ['', Validators.required],
        cargoId: ['', Validators.required],
        password: [
          '',
          [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
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
      this.usuario = Object.assign({}, this.registerForm.value);
      this.authService.register(this.usuario).subscribe(
        () => this.alertify.success('La cuenta se registró satisfactoriamente'),
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.authService.login(this.usuario).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
  }

  onCancel() {
    this.cancelRegisterEvent.emit(false);
    this.alertify.error('Cancelled');
  }  
}
