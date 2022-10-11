import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DecimalDirective } from './decimal.directive';
import { SpecialCharacterDirective } from './special-character.directive';
import { FormatoLiquidacionComponent } from './components/formato-liquidacion/formato-liquidacion.component';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    DecimalDirective,
    SpecialCharacterDirective,
    FormatoLiquidacionComponent
  ],
  imports: [CommonModule, BrowserModule, FormsModule, ReactiveFormsModule],
  exports: [DecimalDirective, SpecialCharacterDirective, FormatoLiquidacionComponent],
})
export class SharedModule {}
