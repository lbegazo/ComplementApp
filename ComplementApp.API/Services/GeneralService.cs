using System;
using ComplementApp.API.Interfaces.Repository;

namespace ComplementApp.API.Services
{
    public class GeneralService : IGeneralInterface
    {
        public DateTime ObtenerFechaHoraActual()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "SA Pacific Standard Time");
            return _localTime;
        }

        public decimal ObtenerValorRentaRedondeado(decimal valorRentaCalculado)
        {

            var modValorRentaCalculado = valorRentaCalculado % 100;

            if (modValorRentaCalculado < 50)
            {
                valorRentaCalculado = valorRentaCalculado - modValorRentaCalculado;
            }
            else
            {
                valorRentaCalculado = valorRentaCalculado + (100 - modValorRentaCalculado);
            }
            return valorRentaCalculado;
        }

        public decimal ObtenerValorRedondeadoCPS(decimal valor)
        {
            decimal resultado = 0;
            if (valor <= 100)
            {
                resultado = valor + (100 - valor);
            }
            else if (valor > 100 && valor < 10000)
            {
                resultado = ObtenerValorRedondeadoAl100XEncima(valor);
            }
            else if (valor > 10000)
            {
                resultado = ObtenerValorRedondeadoAl1000XEncima(valor);
            }

            return resultado;
        }

        public decimal ObtenerValorRedondeadoAl100XEncima(decimal valor)
        {
            decimal valorNuevo = 0;
            var modValorRentaCalculado = valor % 100;

            if (modValorRentaCalculado > 0)
            {
                valorNuevo = valor + (100 - modValorRentaCalculado);
            }
            else
            {
                valorNuevo = valor;
            }

            return valorNuevo;
        }

        public decimal ObtenerValorRedondeadoAl1000XEncima(decimal valor)
        {
            decimal valorNuevo = 0;
            var modValorRentaCalculado = valor % 1000;

            if (modValorRentaCalculado > 0)
            {
                valorNuevo = valor + (1000 - modValorRentaCalculado);
            }
            else
            {
                valorNuevo = valor;
            }

            return valorNuevo;
        }

        public string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

    }
}