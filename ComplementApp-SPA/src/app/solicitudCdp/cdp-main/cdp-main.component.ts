import { Component, OnInit } from '@angular/core';
import { ListaService } from 'src/app/_services/lista.service';

@Component({
  selector: 'app-cdp-main',
  templateUrl: './cdp-main.component.html',
  styleUrls: ['./cdp-main.component.css']
})
export class CdpMainComponent implements OnInit {
  readonly codigoTransaccion = 'CDP';
  nombreTransaccion: string;

  constructor(private listaService: ListaService) { }

  ngOnInit() {
    this.obtenerNombreTransaccion();
  }

  private obtenerNombreTransaccion() {
    this.nombreTransaccion = this.listaService.obtenerNombreTransaccionPorCodigo(
      this.codigoTransaccion
    );
  }
}
