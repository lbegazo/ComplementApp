import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { BsDaterangepickerConfig } from 'ngx-bootstrap/datepicker';
import { Router } from '@angular/router';
import { User } from '../_models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegisterEvent = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDaterangepickerConfig>;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-blue',
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dayOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
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
      this.authService.register(this.user).subscribe(
        () =>
          this.alertify.success('El usuario se registró satisfactoriamente'),
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
  }

  onCancel() {
    this.cancelRegisterEvent.emit(false);
    this.alertify.error('Cancelado');
  }

  // createRegisterFormUsingFormGroup() {
  //   this.registerForm = new FormGroup(
  //     {
  //       gender: new FormControl('male', Validators.required),
  //       username: new FormControl('', Validators.required),
  //       knownAs: new FormControl('', Validators.required),
  //       dayOfBirth: new FormControl(null, Validators.required),
  //       city: new FormControl('', Validators.required),
  //       country: new FormControl('', Validators.required),
  //       password: new FormControl('', [
  //         Validators.required,
  //         Validators.minLength(4),
  //         Validators.maxLength(8),
  //       ]),
  //       confirmPassword: new FormControl('', Validators.required),
  //     },
  //     this.passwordMatchValidator
  //   );
  // }
}
