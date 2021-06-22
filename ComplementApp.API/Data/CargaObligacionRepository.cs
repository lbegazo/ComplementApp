using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class CargaObligacionRepository : ICargaObligacionRepository
    {
        private readonly DataContext _context;
        private readonly IListaRepository _repoLista;

        private readonly IHolidayService _holiday;

        public CargaObligacionRepository(DataContext context, IListaRepository listaRepository, IHolidayService holiday)
        {
            _holiday = holiday;
            _context = context;
            _repoLista = listaRepository;
        }
        public bool EliminarCargaObligacion()
        {
            try
            {
                if (!_context.CargaObligacion.Any())
                    return true;

                if (_context.CargaObligacion.Any())
                    return _context.CargaObligacion.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public bool InsertarListaCargaObligacion(IList<CargaObligacion> listaCdp)
        {
            try
            {
                _context.BulkInsert(listaCdp);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<CDPDto>> ObtenerListaCargaObligacion(string estado, UserParams userParams)
        {
            var lista = (from c in _context.CargaObligacion
                         join rp in _context.RubroPresupuestal on c.RubroPresupuestalId equals rp.RubroPresupuestalId
                         where c.Estado.ToLower() == estado.ToLower()
                         where c.PciId == userParams.PciId
                         select new CDPDto()
                         {
                             CdpId = c.CargaObligacionId,
                             Obligacion = c.Obligacion,
                             ValorInicial = c.ValorActual2,
                             NumeroIdentificacionTercero = c.NumeroIdentificacion,
                             NombreTercero = c.NombreRazonSocial,
                             IdentificacionRubro = rp.Identificacion,
                             NombreRubro = rp.Nombre,
                         })
                         .Distinct();

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<ICollection<CargaObligacionDto>> ObtenerListaCargaObligacionArchivoCabecera(int usuarioId, string estado, int pciId)
        {
            var fechaLimitePago = _holiday.GetNextWorkingDay(System.DateTime.Now, 2);
            // DateTime dateValue = new DateTime(2021, 10, 29);
            // var fechaLimitePago = _holiday.GetNextWorkingDay(dateValue, 2);
            var usuario = (from u in _context.Usuario
                           join c in _context.Cargo on u.CargoId equals c.CargoId
                           where u.UsuarioId == usuarioId
                           select new
                           {
                               Nombres = u.Nombres,
                               Apellidos = u.Apellidos,
                               CargoDescripcion = c.Nombre,
                           }
                          ).FirstOrDefault();

            var pci = (from p in _context.Pci
                       where p.PciId == pciId
                       select p).FirstOrDefault();

            var lista = await (from co in _context.CargaObligacion
                               join rp in _context.RubroPresupuestal on co.RubroPresupuestalId equals rp.RubroPresupuestalId
                               join td in _context.TipoDocumentoIdentidad on co.TipoDocumentoIdentidadId equals td.TipoDocumentoIdentidadId
                               join tds in _context.TipoDocumentoSoporte on co.TipoDocumentoSoporteId equals tds.TipoDocumentoSoporteId
                               join nap in _context.NivelAgrupacionPac on new
                               {
                                   RubroPresupuestalId = rp.PadreRubroId.Value,
                                   SituacionFondoId = co.SituacionFondoId.Value,
                                   FuenteFinanciacionId = co.FuenteFinanciacionId.Value,
                                   RecursoPresupuestalId = co.RecursoPresupuestalId.Value,
                                   pci = co.PciId
                               } equals
                                new
                                {
                                    RubroPresupuestalId = nap.RubroPresupuestalId,
                                    SituacionFondoId = nap.SituacionFondoId,
                                    FuenteFinanciacionId = nap.FuenteFinanciacionId,
                                    RecursoPresupuestalId = nap.RecursoPresupuestalId,
                                    pci = nap.PciId
                                }
                               where co.PciId == pciId
                               where co.Estado.ToLower() == estado.ToLower()

                               select new CargaObligacionDto()
                               {
                                   FechaRegistro = System.DateTime.Now,
                                   FechaPago = co.FechaRegistro,
                                   FechaLimitePago = fechaLimitePago,
                                   Obligacion = co.Obligacion,
                                   ValorActual = co.ValorActual,
                                   CodigoTipoBeneficiario = (pci.Nit == co.NumeroIdentificacion) ? "P" : "B",
                                   MedioPago = (co.MedioPago.ToLower() == "abono en cuenta") ? ("AC") :
                                                (((co.MedioPago.ToLower() == "giro") ? ("GR") :
                                                (((co.MedioPago.ToLower() == "cheque") ? ("CH") :
                                                (((co.MedioPago.ToLower() == "efectivo") ? ("EF") : (string.Empty))))))),
                                   CodigoPosicionPac = nap.Identificacion,
                                   CodigoDependenciaAfectacionPac = nap.DependenciaAfectacionPAC,
                                   CodigoPciTesoreria = nap.IdentificacionTesoreria,
                                   Tercero = new TerceroDto()
                                   {
                                       TipoDocumentoIdentidad = td.Codigo,
                                       NumeroIdentificacion = co.NumeroIdentificacion,
                                       Nombre = co.NombreRazonSocial
                                   },
                                   Pci = new Pci()
                                   {
                                       Identificacion = pci.Identificacion,
                                   },
                                   NumeroCuenta = co.NumeroCuenta,
                                   TipoCuenta = co.TipoCuenta.ToLower() == "ahorro" ? "AHR" : "CRR",
                                   TipoDocSoporteCompromiso = tds.Codigo,
                                   NumeroDocSoporteCompromiso = co.NumeroDocSoporteCompromiso,
                                   FechaDocSoporteCompromiso = co.FechaDocSoporteCompromiso,
                                   NombreFuncionario = usuario.Nombres + " " + usuario.Apellidos,
                                   CargoFuncionario = usuario.CargoDescripcion,
                                   Concepto = co.Concepto.Length > 200 ? co.Concepto.Substring(0, 200) : co.Concepto,
                               })
                               .Distinct()
                               .ToListAsync();

            return lista;
        }

        public async Task<ICollection<CargaObligacionDto>> ObtenerListaCargaObligacionArchivoDetalle(string estado, int pciId)
        {
            var lista = await (from co in _context.CargaObligacion
                               join rp in _context.RubroPresupuestal on co.RubroPresupuestalId equals rp.RubroPresupuestalId
                               join ff in _context.FuenteFinanciacion on co.FuenteFinanciacionId equals ff.FuenteFinanciacionId
                               join sf in _context.SituacionFondo on co.SituacionFondoId equals sf.SituacionFondoId
                               join re in _context.RecursoPresupuestal on co.RecursoPresupuestalId equals re.RecursoPresupuestalId
                               where co.PciId == pciId
                               where co.Estado.ToLower() == estado.ToLower()
                               select new CargaObligacionDto()
                               {
                                   Obligacion = co.Obligacion,
                                   CargaObligacionId = co.CargaObligacionId,
                                   Dependencia = co.Dependencia,
                                   ValorActual2 = co.ValorActual2,
                                   RubroPresupuestal = new RubroPresupuestalDto()
                                   {
                                       Identificacion = rp.Identificacion
                                   },
                                   FuenteFinanciacion = new FuenteFinanciacion()
                                   {
                                       Codigo = ff.Codigo
                                   },
                                   SituacionFondo = new SituacionFondo()
                                   {
                                       Codigo = sf.Codigo
                                   },
                                   RecursoPresupuestal = new RecursoPresupuestal()
                                   {
                                       Codigo = re.Codigo
                                   }
                               })
                               .Distinct()
                               .ToListAsync();

            return lista;
        }
    }
}