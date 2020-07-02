import { Component, OnInit } from '@angular/core';
import { Usuario } from 'src/app/_models/usuario';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-usuario-detalle',
  templateUrl: './usuario-detalle.component.html',
  styleUrls: ['./usuario-detalle.component.css'],
})
export class UsuarioDetalleComponent implements OnInit {
  usuario: Usuario;
  constructor(private route: ActivatedRoute) {}

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.usuario = data['usuario'];
    });
  }
}
