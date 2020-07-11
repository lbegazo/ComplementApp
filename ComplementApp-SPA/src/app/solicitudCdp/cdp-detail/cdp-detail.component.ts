import { Component, OnInit } from '@angular/core';
import { Cdp } from 'src/app/_models/cdp';
import { CdpService } from 'src/app/_services/cdp.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-cdp-detail',
  templateUrl: './cdp-detail.component.html',
  styleUrls: ['./cdp-detail.component.css'],
})
export class CdpDetailComponent implements OnInit {
  cdp: Cdp;
  constructor(
    private cdpService: CdpService,
    private route: ActivatedRoute,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.cdp = data['cdp'];
    });
  }
}
