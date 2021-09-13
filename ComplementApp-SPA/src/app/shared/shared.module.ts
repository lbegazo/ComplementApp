import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DecimalDirective } from './decimal.directive';
import { SpecialCharacterDirective } from './special-character.directive';

@NgModule({
  declarations: [DecimalDirective, SpecialCharacterDirective],
  imports: [CommonModule],
  exports: [
    DecimalDirective,
    SpecialCharacterDirective
  ]
})
export class SharedModule {}
