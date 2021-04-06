using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class ActividadPrincipalDto
    {
        public ICollection<ActividadGeneral> ListaActividadGeneral { get; set; }
        public ICollection<ActividadEspecifica> ListaActividadEspecifica { get; set; }
    }
}