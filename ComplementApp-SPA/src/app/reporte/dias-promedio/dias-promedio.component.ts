import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Transaccion } from 'src/app/_models/transaccion';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-dias-promedio',
  templateUrl: './dias-promedio.component.html',
  styleUrls: ['./dias-promedio.component.css']
})
export class DiasPromedioComponent implements OnInit {

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

