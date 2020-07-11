import { Component, OnInit } from '@angular/core';
import { CdpService } from 'src/app/_services/cdp.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Cdp } from 'src/app/_models/cdp';

@Component({
  selector: 'app-cdp-list',
  templateUrl: './cdp-list.component.html',
  styleUrls: ['./cdp-list.component.css'],
})
export class CdpListComponent implements OnInit {
  listaCdp: Cdp[];
  constructor(
    private cdpService: CdpService,
    private router: Router,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.cdpService.ObtenerListaCDP().subscribe(
      (cdps: Cdp[]) => {
        this.listaCdp = cdps;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
