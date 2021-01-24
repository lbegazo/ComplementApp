using System;

namespace ComplementApp.API.Interfaces
{
    public interface IGeneralInterface
    {
         DateTime ObtenerFechaHoraActual();
         decimal ObtenerValorRentaRedondeado(decimal valorRentaCalculado);
         decimal ObtenerValorRedondeadoCPS(decimal valor);
         decimal ObtenerValorRedondeadoAl100XEncima(decimal valor);
         decimal ObtenerValorRedondeadoAl1000XEncima(decimal valor);
    }
}