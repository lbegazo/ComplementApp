import { Directive, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { NgControl } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { map } from 'rxjs/operators';

@Directive({
  selector: '[appNumberComma]',
  providers: [DecimalPipe],
})
export class NumberCommaDirective implements OnInit, OnDestroy {
  private regex: RegExp = new RegExp(/^\d*\.?\d{0,2}$/g);
  private subscription: Subscription;

  constructor(private ngControl: NgControl, private decimal: DecimalPipe) {}

  ngOnInit() {
    const control = this.ngControl.control;
    this.subscription = control.valueChanges
      .pipe(
        map((value) => {
          const parts = value.toString().split('.');
          parts[0] = this.decimal.transform(parts[0].replace(/,/g, ''));
          const part2 = parts[1];
          const result = parts.join('.');
          //console.log(part2);
          //if (part2 && part2.length < 3)
          {
            return result;
          }
        })
      )
      .subscribe((v) => {
        //console.log(v);
        control.setValue(v, { emitEvent: false });
      });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
