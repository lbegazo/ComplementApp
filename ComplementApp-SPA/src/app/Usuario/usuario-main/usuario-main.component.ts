import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-usuario-main',
  templateUrl: './usuario-main.component.html',
  styleUrls: ['./usuario-main.component.css']
})
export class UsuarioMainComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  esAdministrador(){
    return this.authService.esAdministrador();
  }

}
