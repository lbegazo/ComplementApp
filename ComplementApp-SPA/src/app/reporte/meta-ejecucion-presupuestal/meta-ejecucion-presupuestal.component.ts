import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Transaccion } from 'src/app/_models/transaccion';
import { Usuario } from 'src/app/_models/usuario';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-meta-ejecucion-presupuestal',
  templateUrl: './meta-ejecucion-presupuestal.component.html',
  styleUrls: ['./meta-ejecucion-presupuestal.component.css'],
})
export class MetaEjecucionPresupuestalComponent implements OnInit {
  transaccion: Transaccion;
  nombreTransaccion: string;

  constructor(
    private http: HttpClient,
    private alertify: AlertifyService,
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
