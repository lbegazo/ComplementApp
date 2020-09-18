import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { ListaService } from 'src/app/_services/lista.service';

@Component({
  selector: 'app-usuario-main',
  templateUrl: './usuario-main.component.html',
  styleUrls: ['./usuario-main.component.css'],
})
export class UsuarioMainComponent implements OnInit {
  readonly codigoTransaccion = 'USUARIO';
  nombreTransaccion: string;

  constructor(
    private authService: AuthService,
    private listaService: ListaService
  ) {}

  ngOnInit() {
    this.obtenerNombreTransaccion();
  }

  private obtenerNombreTransaccion() {
    this.nombreTransaccion = this.listaService.obtenerNombreTransaccionPorCodigo(
      this.codigoTransaccion
    );
  }
}
