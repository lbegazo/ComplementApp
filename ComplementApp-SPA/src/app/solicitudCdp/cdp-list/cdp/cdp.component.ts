import { Component, OnInit, Input } from '@angular/core';
import { Cdp } from 'src/app/_models/cdp';

@Component({
  selector: 'app-cdp',
  templateUrl: './cdp.component.html',
  styleUrls: ['./cdp.component.css']
})
export class CdpComponent implements OnInit {
  @Input() cdp: Cdp;
  @Input() index;
  constructor() { }

  ngOnInit() {
  }

}
