import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Transaccion } from 'src/app/_models/transaccion';
import { ListaService } from 'src/app/_services/lista.service';

@Component({
  selector: 'app-cdp-main',
  templateUrl: './cdp-main.component.html',
  styleUrls: ['./cdp-main.component.css'],
})
export class CdpMainComponent implements OnInit {
  readonly codigoTransaccion = 'SOLICITUDES_CERTIFICADO';
  nombreTransaccion: string;
  transaccion: Transaccion;

  constructor(
    private listaService: ListaService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.transaccion = data['transaccion'];
      if (this.transaccion) {
        this.nombreTransaccion = this.transaccion.nombre;
      }
    });
  }
}
