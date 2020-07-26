import { Component, OnInit, Input } from '@angular/core';
import { DetalleCDP } from 'src/app/_models/detalleCDP';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {
  @Input() item: DetalleCDP;
  constructor() { }

  ngOnInit() {
  }

}
