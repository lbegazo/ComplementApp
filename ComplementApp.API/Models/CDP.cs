using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TB_CDP")]
    public class CDP
    {
        public int Id { get; set; }
        public string Dependencia { get; set; }

        public int Proy { get; set; }

        public int Pro { get; set; }

        public int Cdp { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; }
        public string Rubro { get; set; }

        public decimal Valor { get; set; }

        public decimal Saldo { get; set; }

        public string Tipo { get; set; }
    }
}