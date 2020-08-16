import { Directive, ElementRef, HostListener, OnDestroy } from '@angular/core';
import { NgControl } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { map } from 'rxjs/operators';

@Directive({
  selector: '[appTwoDigitDecimaNumber]',
})
export class TwoDigitDecimaNumberDirective {
  // Allow decimal numbers and negative values
  private regex: RegExp = new RegExp(/^\d*\.?\d{0,2}$/g);
  // Allow key codes for special events. Reflect :
  // Backspace, tab, end, home
  private specialKeys: Array<string> = [
    'Backspace',
    'Tab',
    'End',
    'Home',
    'ArrowLeft',
    'ArrowRight',
    'Del',
    'Delete',
  ];

  constructor(private el: ElementRef) {}

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    // Allow Backspace, tab, end, and home keys
    if (this.specialKeys.indexOf(event.key) !== -1) {
      return;
    }

    const current: string = this.el.nativeElement.value;
    const position = this.el.nativeElement.selectionStart;
    const next: string = [
      current.slice(0, position),
      event.key === 'Decimal' ? '.' : event.key,
      current.slice(position),
    ].join('');

    //console.log(next);
    if (next && !String(next).match(this.regex)) {
      event.preventDefault();
    }
  }
}
