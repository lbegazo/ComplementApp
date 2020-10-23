using System;
using ComplementApp.API.Interfaces;

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
    }
}