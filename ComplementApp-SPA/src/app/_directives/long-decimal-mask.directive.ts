import { Directive, ElementRef, HostListener } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
  selector: '[appLongDecimalMask]',
})
export class LongDecimalMaskDirective {
  constructor(private el: ElementRef, public model: NgControl) {}

  @HostListener('input', ['$event']) onEvent($event) {
    // console.log('keypress: ' + $event.keypress);
    const valArray = this.el.nativeElement.value.split(',');
    for (let i = 0; i < valArray.length; ++i) {
      valArray[i] = valArray[i].replace(/\D/g, '');
    }
    let newVal = '';
    if (valArray.length === 0) {
      newVal = '';
    } else {
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
        newVal += ',' + valArray[1].substring(0, 5);
      }
    }
    // set the new value
    this.model.control.setValue(newVal);
  }
}
