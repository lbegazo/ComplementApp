using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class ActividadGeneralPrincipalDto
    {
        public ICollection<ActividadGeneral> ListaActividadGeneral { get; set; }
    }
}