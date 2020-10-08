import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Transaccion } from 'src/app/_models/transaccion';
import { UsuarioService } from 'src/app/_services/usuario.service';

@Component({
  selector: 'app-usuario-main',
  templateUrl: './usuario-main.component.html',
  styleUrls: ['./usuario-main.component.css'],
})
export class UsuarioMainComponent implements OnInit {
  nombreTransaccion: string;
  transaccion: Transaccion;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });
  }
}
