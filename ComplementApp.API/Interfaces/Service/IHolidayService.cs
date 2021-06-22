using System;

namespace ComplementApp.API.Interfaces.Service
{
    public interface IHolidayService
    {
         DateTime GetNextWorkingDay(DateTime date, int day);         
    }
}