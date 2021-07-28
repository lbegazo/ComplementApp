import { Directive, ElementRef, HostListener } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
  selector: '[appNegativeDecimalMask]'
})
export class NegativeDecimalMaskDirective {

  constructor(private el: ElementRef, public model: NgControl) {}

  @HostListener('input', ['$event']) onEvent($event) {
    // console.log('keypress: ' + $event.keypress);
    let esNegativo = false;
    const valArray = this.el.nativeElement.value.split(',');
    for (let i = 0; i < valArray.length; ++i) {
      valArray[i] = valArray[i].replace(/(?!^-)[^0-9]/g, '');
    }
    let newVal = '';
    if (valArray.length === 0) {
      newVal = '';
    } else {
      if (valArray[0].includes('-')) {
        esNegativo = true;
        valArray[0] = valArray[0].replace('-', '');
      }
      const matches = valArray[0].match(/[0-9]{3}/gim);
      if (matches !== null && valArray[0].length > 3) {
        const commaGroups = Array.from(
          Array.from(valArray[0])
            .reverse()
            .join('')
            .match(/[0-9]{3}/gim)
            .join('.')
        )
          .reverse()
          .join('');

        const replacement = valArray[0].replace(
          commaGroups.replace(/\D/g, ''),
          ''
        );

        newVal =
          (replacement.length > 0 ? replacement + '.' : '') + commaGroups;
      } else {
        newVal = valArray[0];
      }

      if (valArray.length > 1) {
        newVal += ',' + valArray[1].substring(0, 2);
      }
    }
    if (esNegativo) {
      if (valArray[0].length > 0) {
        newVal = '-' + newVal;
      } else {
        newVal = '-';
      }
    }
    // set the new value
    this.model.control.setValue(newVal);
  }

}
