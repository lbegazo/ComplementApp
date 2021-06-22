using System;
using System.Collections.Generic;
using ComplementApp.API.Interfaces.Service;

namespace ComplementApp.API.Services
{
    public class HolidayService : IHolidayService
    {
        public HashSet<DateTime> Holidays = null;
        private int year;
        private int easterMonth;
        private int easterDay;

        public HolidayService()
        {
            Holidays = new HashSet<DateTime>();
            year = DateTime.Now.Year;

            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int g = (8 * b + 13) / 25;
            int h = (19 * a + b - d - g + 15) % 30;
            int j = c / 4;
            int k = c % 4;
            int m = (a + 11 * h) / 319;
            int r = (2 * e + 2 * j - k - h + m + 32) % 7;
            this.easterMonth = (h - m + r + 90) / 25;
            this.easterDay = (h - m + r + this.easterMonth + 19) % 32;
            //this.easterMonth--;

            Holidays.Add(new DateTime(DateTime.Now.Year, 1, 1)); // Primero de Enero
            Holidays.Add(new DateTime(DateTime.Now.Year, 5, 1)); // Dia del trabajo 1 de mayo
            Holidays.Add(new DateTime(DateTime.Now.Year, 7, 20)); //Independencia 20 de Julio
            Holidays.Add(new DateTime(DateTime.Now.Year, 8, 7)); //Batalla de boyaca 7 de agosto
            Holidays.Add(new DateTime(DateTime.Now.Year, 12, 8)); //Maria inmaculada 8 de diciembre
            Holidays.Add(new DateTime(DateTime.Now.Year, 12, 25)); //Navidad 25 de diciembre

            calculateEmiliani(1, 6); // Reyes magos 6 de enero
            calculateEmiliani(3, 19); //San jose 19 de marzo
            calculateEmiliani(6, 29); //San pedro y san pablo 29 de junio
            calculateEmiliani(8, 15); //Asuncion 15 de agosto
            calculateEmiliani(10, 12); //Descubrimiento de america 12 de octubre
            calculateEmiliani(11, 1); //Todos los santos 1 de noviembre
            calculateEmiliani(11, 11); //Independencia de cartagena 11 de noviembre

            this.calculateOtherHoliday(-3, false); //jueves santos
            this.calculateOtherHoliday(-2, false); //viernes santo
            this.calculateOtherHoliday(40, true); //Asención del señor de pascua
            this.calculateOtherHoliday(60, true); //Corpus cristi
            this.calculateOtherHoliday(68, true); //Sagrado corazon
        }

        public DateTime GetNextWorkingDay(DateTime date, int days)
        {
            while (days > 0)
            {
                date = date.AddDays(1);

                if (!(IsWeekend(date) || IsHoliday(date)))
                {
                    days--;
                }
            }
            return date;
        }

        private bool IsHoliday(DateTime date)
        {
            var resultado = Holidays.Contains(date);
            return resultado;
        }

        private bool IsWeekend(DateTime date)
        {
            var resultado = date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;

            return resultado;
        }


        private void calculateOtherHoliday(int days, bool emiliani)
        {
            DateTime dateValue = new DateTime(this.year, this.easterMonth, this.easterDay);
            DateTime newDate = dateValue.AddDays(days);

            if (emiliani)
            {
                this.calculateEmiliani(newDate.Month, newDate.Day);
            }
            else
            {
                Holidays.Add(newDate);
            }

        }

        private void calculateEmiliani(int month, int day)
        {
            DateTime dateValue = new DateTime(this.year, month, day);
            DateTime newDate = new DateTime();
            int dayOfWeek = (int)dateValue.DayOfWeek == 0 ? 7 : (int)dateValue.DayOfWeek;
            switch (dayOfWeek)
            {
                case 1:
                    newDate = dateValue.AddDays(0);
                    break;
                case 2:
                    newDate = dateValue.AddDays(6);
                    break;
                case 3:
                    newDate = dateValue.AddDays(5);
                    break;
                case 4:
                    newDate = dateValue.AddDays(4);
                    break;
                case 5:
                    newDate = dateValue.AddDays(3);
                    break;
                case 6:
                    newDate = dateValue.AddDays(2);
                    break;
                case 7:
                    newDate = dateValue.AddDays(1);
                    break;
                default:
                    break;
            }
            Holidays.Add(newDate);
        }
    }
}