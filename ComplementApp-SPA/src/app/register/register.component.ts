import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  //@Input() valuesFromHome: any;
  @Output() cancelRegisterEvent = new EventEmitter();
  model: any = {};
  constructor(private auth: AuthService) { }

  ngOnInit() {
  }

  onRegister()
  {
    this.auth.register(this.model).subscribe(
      () => console.log('Registration successful'),
      error => console.log(error)
    );
  }

  onCancel()
  {
    this.cancelRegisterEvent.emit(false);
    console.log('Cancelled');
  }

  

}
