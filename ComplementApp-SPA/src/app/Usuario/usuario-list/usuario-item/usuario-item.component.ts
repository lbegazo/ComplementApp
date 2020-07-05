import { Component, OnInit, Input } from '@angular/core';
import { Usuario } from 'src/app/_models/usuario';

@Component({
  selector: 'app-usuario-item',
  templateUrl: './usuario-item.component.html',
  styleUrls: ['./usuario-item.component.css'],
})
export class UsuarioItemComponent implements OnInit {
  @Input() usuario: Usuario;
  @Input() index;
  constructor() {}

  ngOnInit() {}
}
