import { LineaPlanPagoDto } from '../_dto/LineaPlanPagoDto';
import { Cdp } from './cdp';
import { PlanPago } from './planPago';

export interface FormaPagoCompromiso {
  cdp: Cdp;
  listaLineaPlanPago: LineaPlanPagoDto[];
}
