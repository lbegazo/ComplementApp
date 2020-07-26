import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { DetalleCDP } from 'src/app/_models/detalleCDP';
import { CdpService } from 'src/app/_services/cdp.service';
import {
  FormGroup,
  FormArray,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-popup-cdp',
  templateUrl: './popup-cdp.component.html',
  styleUrls: ['./popup-cdp.component.css'],
})
export class PopupCdpComponent implements OnInit {
  title: string;
  closeBtnName: string;
  list: any[] = [];
  detalleCdp: DetalleCDP[];
  arrayControls = new FormArray([]);
  popupCDPForm = new FormGroup({});
  arrayRubro: number[] = [];

  constructor(
    public bsModalRef: BsModalRef,
    private cdpService: CdpService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.cargarDetalleCDP();
  }

  onCheckChange(event) {
    /* Selected */
    if (event.target.checked) {
      // Add a new control in the arrayForm
      this.arrayRubro?.push(+event.target.value);
    } else {
      /* unselected */
      let index = 0;
      let i = 0;
      this.arrayRubro.forEach((val: number) => {
        if (val === event.target.value) {
          index = i;
        }
        i++;
      });

      if (index !== -1) {
        this.arrayRubro.splice(index, 1);
      }

      // console.log(event.target.value + ' ' + index);
      // if (index !== -1) {
      //   this.arrayRubro.splice(index, 1);
      // }
      // let i: number = 0;
      // this.rubrosControles.controls.forEach((ctrl: FormControl) => {
      //   if (ctrl.value === event.target.value) {
      //     // Remove the unselected element from the arrayForm
      //     this.arrayRubro.removeAt(i);
      //     return;
      //   }
      //   i++;
      // });
    }
  }

  cargarDetalleCDP() {
    this.cdpService.ObtenerDetalleDeCDP(0).subscribe(
      (documento: DetalleCDP[]) => {
        this.detalleCdp = documento;

        if (this.detalleCdp) {
          for (const detalle of this.detalleCdp) {
            this.arrayControls.push(
              new FormGroup({
                rubroControl: new FormControl('', [Validators.required]),
              })
            );
          }
        }
      },
      (error) => {
        this.alertify.error(error);
      }
    );

    this.popupCDPForm = this.fb.group({
      rubrosControles: this.arrayControls,
    });
  }

  onAceptar() {
    // console.log('Popup ' + this.arrayRubro);
    this.bsModalRef.hide();
    this.bsModalRef.content = this.arrayRubro;
  }

  get rubrosControles() {
    return this.popupCDPForm.get('rubrosControles') as FormArray;
  }
}
