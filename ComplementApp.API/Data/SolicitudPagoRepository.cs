using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using System.Globalization;
using ComplementApp.API.Interfaces.Repository;

namespace ComplementApp.API.Data
{
    public class SolicitudPagoRepository : ISolicitudPagoRepository
    {
        private readonly DataContext _context;
        private readonly IGeneralInterface _generalInterface;

        public SolicitudPagoRepository(DataContext context, IGeneralInterface generalInterface)
        {
            _context = context;
            this._generalInterface = generalInterface;
        }

        #region Registro de Solicitud de Pago

        public async Task<PagedList<CDPDto>> ObtenerCompromisosParaSolicitudRegistroPago(int usuarioId, int perfilId,
                                                                                      int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;
            var usuario = await _context.Usuario.Where(x => x.UsuarioId == usuarioId).FirstOrDefaultAsync();
            var listaPerfilxUsuario = await _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).ToListAsync();

            if (listaPerfilxUsuario != null && listaPerfilxUsuario.Count > 0)
            {
                var listaPerfilId = listaPerfilxUsuario.Select(x => x.PerfilId).ToList();

                if (listaPerfilId.Contains((int)PerfilUsuario.Administrador)
                    || listaPerfilId.Contains((int)PerfilUsuario.CoordinadorFinanciero)
                    || listaPerfilId.Contains((int)PerfilUsuario.RegistradorContable))
                {
                    #region Administrador y Coordinador financiero

                    lista = (from c in _context.DocumentoCompromiso
                             join t in _context.Tercero on c.TerceroId equals t.TerceroId
                             where c.PciId == userParams.PciId
                             where c.SaldoPorUtilizar > 0 //Saldo Disponible
                             where c.TerceroId == terceroId || terceroId == null
                             select new CDPDto()
                             {
                                 Crp = c.NumeroDocumento,
                                 Detalle4 = c.Observaciones.Length > 200 ? c.Observaciones.Substring(0, 200) + "..." : c.Observaciones,
                                 NumeroIdentificacionTercero = t.NumeroIdentificacion,
                                 NombreTercero = t.Nombre,
                                 TerceroId = c.TerceroId.Value,
                             })
                          .Distinct()
                          .OrderBy(x => x.Crp);

                    #endregion Administrador y Coordinador financiero
                }
                else if (listaPerfilId.Contains((int)PerfilUsuario.SupervisorContractual))
                {
                    #region SupervisorContractual

                    lista = (from c in _context.DocumentoCompromiso
                             join t in _context.Tercero on c.TerceroId equals t.TerceroId
                             join p in _context.ParametroLiquidacionTercero on t.TerceroId equals p.TerceroId into ParametroTercero
                             from pt in ParametroTercero.DefaultIfEmpty()
                             join con in _context.Contrato on c.NumeroDocumento equals con.Crp into Contrato
                             from contra in Contrato.DefaultIfEmpty()
                             where c.PciId == pt.PciId
                             where c.PciId == contra.PciId
                             where c.PciId == userParams.PciId
                             where c.SaldoPorUtilizar > 0 //Saldo Disponible
                             where contra.Supervisor1Id == usuarioId
                             where c.TerceroId == terceroId || terceroId == null
                             where pt.ModalidadContrato != (int)ModalidadContrato.ContratoPrestacionServicio
                             select new CDPDto()
                             {
                                 Crp = c.NumeroDocumento,
                                 Detalle4 = c.Observaciones.Length > 200 ? c.Observaciones.Substring(0, 200) + "..." : c.Observaciones,
                                 NumeroIdentificacionTercero = t.NumeroIdentificacion,
                                 NombreTercero = t.Nombre,
                                 TerceroId = c.TerceroId.Value,
                             })
                            .Distinct()
                            .OrderBy(x => x.Crp);

                    #endregion SupervisorContractual
                }
                else if (listaPerfilId.Contains((int)PerfilUsuario.Contratista))
                {
                    #region Contratista

                    terceroId = usuario.TerceroId;

                    lista = (from c in _context.DocumentoCompromiso
                             join t in _context.Tercero on c.TerceroId equals t.TerceroId
                             where c.PciId == userParams.PciId
                             where c.SaldoPorUtilizar > 0 //Saldo Disponible
                             where c.TerceroId == terceroId
                             select new CDPDto()
                             {
                                 Crp = c.NumeroDocumento,
                                 Detalle4 = c.Observaciones.Length > 200 ? c.Observaciones.Substring(0, 200) + "..." : c.Observaciones,
                                 NumeroIdentificacionTercero = t.NumeroIdentificacion,
                                 NombreTercero = t.Nombre,
                                 TerceroId = c.TerceroId.Value,
                             })
                            .Distinct()
                            .OrderBy(x => x.Crp);

                    #endregion Contratista
                }
            }
            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CDPDto>> ObtenerListaCompromisoConContrato(int usuarioId, string numeroContrato,
                                                                                long? crp, int? terceroId,
                                                                                UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;

            lista = (from co in _context.Contrato
                     join c in _context.DocumentoCompromiso on new {    Crp=co.Crp, 
                                                                        NumeroContrato = co.NumeroContrato.Trim(), 
                                                                        co.PciId } equals
                                                                new {   Crp=c.NumeroDocumento, 
                                                                        NumeroContrato = c.NumeroDocumentoSoporte.Trim(), 
                                                                        c.PciId }
                     join t in _context.Tercero on c.TerceroId equals t.TerceroId
                     where co.Crp == crp || crp == null
                     where co.NumeroContrato == numeroContrato || numeroContrato == null
                     where c.TerceroId == terceroId || terceroId == null
                     select new CDPDto()
                     {
                         Crp = c.NumeroDocumento,
                         Detalle6 = c.NumeroDocumentoSoporte, //número de contrato
                         Detalle4 = c.Observaciones.Length > 100 ? c.Observaciones.Substring(0, 100) + "..." : c.Observaciones,
                         NumeroIdentificacionTercero = t.NumeroIdentificacion,
                         NombreTercero = t.Nombre,
                         TerceroId = c.TerceroId.Value,
                     })
                    .Distinct()
                    .OrderBy(x => x.Detalle6)
                    .ThenBy(x => x.Crp);

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CDPDto>> ObtenerSolicitudesPagoParaAprobar(int usuarioId, int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;

            var listaPerfilxUsuario = await _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).ToListAsync();
            if (listaPerfilxUsuario != null && listaPerfilxUsuario.Count > 0)
            {
                var listaPerfilId = listaPerfilxUsuario.Select(x => x.PerfilId).ToList();


                if (listaPerfilId.Contains((int)PerfilUsuario.Administrador))
                {
                    lista = (from s in _context.FormatoSolicitudPago
                             join c in _context.DocumentoCompromiso on s.Crp equals c.NumeroDocumento
                             join t in _context.Tercero on c.TerceroId equals t.TerceroId
                             where s.EstadoId == (int)EstadoSolicitudPago.Generado
                             where c.PciId == userParams.PciId
                             where s.PciId == c.PciId
                             where c.SaldoPorUtilizar > 0 //Saldo Disponible                     
                             where c.TerceroId == terceroId || terceroId == null
                             select new CDPDto()
                             {
                                 CdpId = s.FormatoSolicitudPagoId,
                                 Crp = c.NumeroDocumento,
                                 Detalle4 = c.Observaciones.Length > 100 ? c.Observaciones.Substring(0, 100) + "..." : c.Observaciones,
                                 NumeroIdentificacionTercero = t.NumeroIdentificacion,
                                 NombreTercero = t.Nombre,
                                 FormatoSolicitudPagoId = s.FormatoSolicitudPagoId
                             })
                            .Distinct()
                            .OrderBy(x => x.CdpId);
                }
                else
                {
                    lista = (from s in _context.FormatoSolicitudPago
                             join c in _context.DocumentoCompromiso on s.Crp equals c.NumeroDocumento
                             join t in _context.Tercero on c.TerceroId equals t.TerceroId
                             where s.SupervisorId == usuarioId
                             where s.EstadoId == (int)EstadoSolicitudPago.Generado
                             where c.PciId == userParams.PciId
                             where s.PciId == c.PciId
                             where c.SaldoPorUtilizar > 0 //Saldo Disponible                     
                             where c.TerceroId == terceroId || terceroId == null
                             select new CDPDto()
                             {
                                 CdpId = s.FormatoSolicitudPagoId,
                                 Crp = c.NumeroDocumento,
                                 Detalle4 = c.Observaciones.Length > 100 ? c.Observaciones.Substring(0, 100) + "..." : c.Observaciones,
                                 NumeroIdentificacionTercero = t.NumeroIdentificacion,
                                 NombreTercero = t.Nombre,
                                 FormatoSolicitudPagoId = s.FormatoSolicitudPagoId
                             })
                            .Distinct()
                            .OrderBy(x => x.CdpId);
                }
            }

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CDPDto>> ObtenerListaSolicitudPagoAprobada(int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;

            lista = (from s in _context.FormatoSolicitudPago
                     join c in _context.DocumentoCompromiso on s.Crp equals c.NumeroDocumento
                     join t in _context.Tercero on c.TerceroId equals t.TerceroId
                     join pp in _context.PlanPago on s.PlanPagoId equals pp.PlanPagoId
                     where s.EstadoId == (int)EstadoSolicitudPago.Aprobado
                     where c.PciId == userParams.PciId
                     where s.PciId == c.PciId
                     where s.PciId == pp.PciId
                     where c.TerceroId == terceroId || terceroId == null
                     select new CDPDto()
                     {
                         CdpId = s.FormatoSolicitudPagoId,
                         Crp = c.NumeroDocumento,
                         Detalle4 = c.Observaciones.Length > 100 ? c.Observaciones.Substring(0, 100) + "..." : c.Observaciones,
                         TerceroId = s.TerceroId,
                         NumeroIdentificacionTercero = t.NumeroIdentificacion,
                         NombreTercero = t.Nombre,
                         FormatoSolicitudPagoId = s.FormatoSolicitudPagoId,
                         PlanPagoId = s.PlanPagoId,
                         ValorFacturado = s.ValorFacturado,
                         NumeroRadicadoSupervisor = pp.NumeroRadicadoSupervisor,
                         FechaRadicadoSupervisor = pp.FechaRadicadoSupervisor,
                     })
                    .Distinct()
                    .OrderBy(x => x.CdpId);

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId)
        {
            return await _context.FormatoSolicitudPago.FirstOrDefaultAsync(u => u.FormatoSolicitudPagoId == formatoSolicitudPagoId);
        }

        public async Task<DetalleFormatoSolicitudPago> ObtenerDetalleFormatoSolicitudPagoBase(int detalleFormatoSolicitudPagoId)
        {
            return await _context.DetalleFormatoSolicitudPago.FirstOrDefaultAsync(u => u.DetalleFormatoSolicitudPagoId == detalleFormatoSolicitudPagoId);
        }

        public async Task<List<DetalleFormatoSolicitudPago>> ObtenerDetalleSolicitudPagoLiquidacionIds(List<int> listaLiquidacionId)
        {
            var lista = await (from cpc in _context.DetalleFormatoSolicitudPago
                               join sp in _context.FormatoSolicitudPago on cpc.FormatoSolicitudPagoId equals sp.FormatoSolicitudPagoId
                               join dl in _context.DetalleLiquidacion on new { sp.Crp, FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId, sp.PciId } equals
                                                                         new { dl.Crp, FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value, dl.PciId }
                               where listaLiquidacionId.Contains(dl.DetalleLiquidacionId)
                               where sp.PlanPagoId == dl.PlanPagoId
                               select new DetalleFormatoSolicitudPago()
                               {
                                   DetalleFormatoSolicitudPagoId = cpc.DetalleFormatoSolicitudPagoId,
                                   RubroPresupuestalId = cpc.RubroPresupuestalId,
                                   Dependencia = cpc.Dependencia,
                                   FormatoSolicitudPago = new FormatoSolicitudPago()
                                   {
                                       FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value,
                                       Crp = dl.Crp,
                                       PciId = dl.PciId,
                                   }

                               })
                               .Distinct()
                               .ToListAsync();

            return lista;
        }

        public async Task<List<long>> ObtenerListaCompromisoXNumeroContrato(string numeroContrato)
        {
            var listaCompromiso = (from c in _context.Contrato
                                   where c.NumeroContrato.Trim() == numeroContrato.Trim()
                                   select c.Crp).Distinct();

            return await listaCompromiso.ToListAsync();
        }

        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(long crp, int pciId)
        {
            //cusba
            var lista = (from c in _context.DocumentoCompromiso 
                        join t in _context.Tercero on c.TerceroId equals t.TerceroId
                        join ctro in _context.Contrato on c.NumeroDocumento equals ctro.Crp into FiltroContrato
                            from filtroContrato in FiltroContrato.DefaultIfEmpty()
                        join sup in _context.Usuario on filtroContrato.Supervisor1Id equals sup.UsuarioId into FiltroSupervisor
                            from filtroSupervisor in FiltroSupervisor.DefaultIfEmpty()                         
                         where c.NumeroDocumento == crp
                         where c.PciId == pciId
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             Crp = c.NumeroDocumento,
                             Fecha = c.FechaRegistro, //Fecha compromiso
                             Detalle4 = c.Observaciones, //objeto contrato
                             TerceroId = c.TerceroId.Value,
                             ValorInicial = c.ValorInicial,
                             Operacion = c.ValorOperacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorActual, //valor actual
                             SaldoActual = c.SaldoPorUtilizar, //saldo actual
                             Detalle7 = c.TipoDocumentoSoporte,
                         });

            var listaAgrupada = (from i in lista
                                 group i by new
                                 {
                                     i.Cdp,
                                     i.Crp,
                                     i.Fecha,
                                     i.Detalle4,
                                     i.TerceroId,
                                     i.ValorInicial,
                                     i.Operacion,
                                     i.ValorTotal,
                                     i.SaldoActual,
                                     i.Detalle7,
                                 }
                          into grp
                                 select new CDPDto()
                                 {
                                     Cdp = grp.Key.Cdp,
                                     Crp = grp.Key.Crp,
                                     Fecha = grp.Key.Fecha,
                                     Detalle4 = grp.Key.Detalle4,
                                     Detalle7 = grp.Key.Detalle7,
                                     TerceroId = grp.Key.TerceroId,
                                     ValorInicial = grp.Sum(i => i.ValorInicial),
                                     Operacion = grp.Sum(i => i.Operacion),
                                     ValorTotal = grp.Sum(i => i.ValorTotal),
                                     SaldoActual = grp.Sum(i => i.SaldoActual)
                                 });

            var formato = await (from c in listaAgrupada
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join co in _context.Contrato on c.Crp equals co.Crp
                                 join p in _context.ParametroLiquidacionTercero on t.TerceroId equals p.TerceroId into ParametroTercero
                                 from pt in ParametroTercero.DefaultIfEmpty()
                                 join ap in _context.TipoAdminPila on pt.TipoAdminPilaId equals ap.TipoAdminPilaId into AdminPila
                                 from adminPila in AdminPila.DefaultIfEmpty()

                                 join sup1 in _context.Usuario on co.Supervisor1Id equals sup1.UsuarioId into Supervisor1
                                 from super1 in Supervisor1.DefaultIfEmpty()
                                 join carg1 in _context.Cargo on super1.CargoId equals carg1.CargoId into Cargo1
                                 from cargoSuper1 in Cargo1.DefaultIfEmpty()
                                 join tercSup1 in _context.Tercero on super1.TerceroId equals tercSup1.TerceroId into TerceroSupervisor1
                                 from tercSuper1 in TerceroSupervisor1.DefaultIfEmpty()

                                 join sup2 in _context.Usuario on co.Supervisor2Id equals sup2.UsuarioId into Supervisor2
                                 from super2 in Supervisor2.DefaultIfEmpty()
                                 join carg2 in _context.Cargo on super2.CargoId equals carg2.CargoId into Cargo2
                                 from cargoSuper2 in Cargo2.DefaultIfEmpty()
                                 join tercSup2 in _context.Tercero on super2.TerceroId equals tercSup2.TerceroId into TerceroSupervisor2
                                 from tercSuper2 in TerceroSupervisor2.DefaultIfEmpty()
                                 where pt.PciId == pciId
                                 where co.PciId == pciId

                                 select new FormatoSolicitudPagoDto()
                                 {
                                     FormatoSolicitudPagoId = 666,
                                     FechaSistema = _generalInterface.ObtenerFechaHoraActual(),
                                     TipoAdminPila = adminPila.TipoAdminPilaId > 0 ? adminPila.Nombre : string.Empty,

                                     Cdp = new CDPDto()
                                     {
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         Detalle7 = c.Detalle7, //modalidad de contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                         SupervisorId = super1.UsuarioId,
                                     },
                                     Contrato = new ContratoDto()
                                     {
                                         ContratoId = co.ContratoId,
                                         NumeroContrato = co.NumeroContrato,
                                         Crp = co.Crp,
                                         FechaInicio = co.FechaInicio,
                                         FechaFinal = co.FechaFinal,
                                         FechaRegistro = co.FechaRegistro,
                                         FechaExpedicionPoliza = co.FechaExpedicionPoliza,

                                         Supervisor1 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = super1.UsuarioId > 0 ? super1.UsuarioId : 0,
                                             Nombres = super1.Nombres,
                                             Apellidos = super1.Apellidos,
                                             NombreCompleto = co.Supervisor1Id > 0 ? super1.Nombres + " " + super1.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper1.CargoId > 0 ? cargoSuper1.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper1.NumeroIdentificacion,
                                         },
                                         Supervisor2 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = co.Supervisor2Id.HasValue ? co.Supervisor2Id.Value : 0,
                                             Nombres = super2.Nombres,
                                             Apellidos = super2.Apellidos,
                                             NombreCompleto = co.Supervisor2Id > 0 ? super2.Nombres + " " + super2.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper2.CargoId > 0 ? cargoSuper2.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper2.NumeroIdentificacion,
                                         }
                                     },
                                     Tercero = new TerceroDto()
                                     {
                                         TerceroId = t.TerceroId,
                                         Nombre = t.Nombre,
                                         NumeroIdentificacion = t.NumeroIdentificacion,
                                         Email = t.Email,
                                         Telefono = t.Telefono,
                                         Direccion = t.Direccion.Length > 45 ? t.Direccion.Substring(0, 45) + "..." : t.Direccion,
                                         FechaExpedicionDocumento = t.FechaExpedicionDocumento.HasValue ? (t.FechaExpedicionDocumento.Value.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaExpedicionDocumento.Value : null) : null,
                                         RegimenTributario = t.RegimenTributario,
                                         DeclaranteRentaDescripcion = t.DeclaranteRenta ? "SI" : "NO",
                                         FacturadorElectronicoDescripcion = pt.FacturaElectronica ? "SI" : "NO"
                                     },

                                 }).FirstOrDefaultAsync();

            return formato;
        }


        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXCompromiso(long crp)
        {
            var lista = (from c in _context.DocumentoCompromiso
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.NumeroDocumento == crp
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             Crp = c.NumeroDocumento,
                             Fecha = c.FechaRegistro, //Fecha compromiso
                             Detalle4 = c.Observaciones, //objeto contrato
                             Detalle6 = c.NumeroDocumentoSoporte.Trim(), //Numero de contrato
                             TerceroId = c.TerceroId.Value,
                             ValorInicial = c.ValorInicial,
                             Operacion = c.ValorOperacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorActual, //valor actual
                             SaldoActual = c.SaldoPorUtilizar, //saldo actual
                             Detalle7 = c.TipoDocumentoSoporte,
                         });

            var listaAgrupada = (from i in lista
                                 group i by new
                                 {
                                     i.Cdp,
                                     i.Crp,
                                     i.Fecha,
                                     i.Detalle4,
                                     i.Detalle6,
                                     i.Detalle7,
                                     i.TerceroId,
                                     i.ValorInicial,
                                     i.Operacion,
                                     i.ValorTotal,
                                     i.SaldoActual
                                 }
                          into grp
                                 select new CDPDto()
                                 {
                                     Cdp = grp.Key.Cdp,
                                     Crp = grp.Key.Crp,
                                     Fecha = grp.Key.Fecha,
                                     Detalle4 = grp.Key.Detalle4,
                                     Detalle6 = grp.Key.Detalle6,
                                     Detalle7 = grp.Key.Detalle7,
                                     TerceroId = grp.Key.TerceroId,
                                     ValorInicial = grp.Sum(i => i.ValorInicial),
                                     Operacion = grp.Sum(i => i.Operacion),
                                     ValorTotal = grp.Sum(i => i.ValorTotal),
                                     SaldoActual = grp.Sum(i => i.SaldoActual)
                                 });

            var formato = await (from c in listaAgrupada
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join co in _context.Contrato on c.Crp equals co.Crp
                                 join p in _context.ParametroLiquidacionTercero on t.TerceroId equals p.TerceroId into ParametroTercero
                                 from pt in ParametroTercero.DefaultIfEmpty()
                                 join ap in _context.TipoAdminPila on pt.TipoAdminPilaId equals ap.TipoAdminPilaId into AdminPila
                                 from adminPila in AdminPila.DefaultIfEmpty()

                                 join sup1 in _context.Usuario on co.Supervisor1Id equals sup1.UsuarioId into Supervisor1
                                 from super1 in Supervisor1.DefaultIfEmpty()
                                 join carg1 in _context.Cargo on super1.CargoId equals carg1.CargoId into Cargo1
                                 from cargoSuper1 in Cargo1.DefaultIfEmpty()
                                 join tercSup1 in _context.Tercero on super1.TerceroId equals tercSup1.TerceroId into TerceroSupervisor1
                                 from tercSuper1 in TerceroSupervisor1.DefaultIfEmpty()

                                 join sup2 in _context.Usuario on co.Supervisor2Id equals sup2.UsuarioId into Supervisor2
                                 from super2 in Supervisor2.DefaultIfEmpty()
                                 join carg2 in _context.Cargo on super2.CargoId equals carg2.CargoId into Cargo2
                                 from cargoSuper2 in Cargo2.DefaultIfEmpty()
                                 join tercSup2 in _context.Tercero on super2.TerceroId equals tercSup2.TerceroId into TerceroSupervisor2
                                 from tercSuper2 in TerceroSupervisor2.DefaultIfEmpty()

                                 select new FormatoSolicitudPagoDto()
                                 {
                                     FormatoSolicitudPagoId = 666,
                                     FechaSistema = _generalInterface.ObtenerFechaHoraActual(),
                                     TipoAdminPila = adminPila.TipoAdminPilaId > 0 ? adminPila.Nombre : string.Empty,

                                     Cdp = new CDPDto()
                                     {
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         Detalle6 = c.Detalle6.Trim(), //número de contrato
                                         Detalle7 = c.Detalle7.Trim(), //modalidad de contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                         SupervisorId = super1.UsuarioId
                                     },
                                     Contrato = new ContratoDto()
                                     {
                                         ContratoId = co.ContratoId,
                                         NumeroContrato = co.NumeroContrato,
                                         Crp = co.Crp,
                                         FechaInicio = co.FechaInicio,
                                         FechaFinal = co.FechaFinal,
                                         FechaRegistro = co.FechaRegistro,
                                         FechaExpedicionPoliza = co.FechaExpedicionPoliza,

                                         Supervisor1 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = super1.UsuarioId > 0 ? super1.UsuarioId : 0,
                                             Nombres = super1.Nombres,
                                             Apellidos = super1.Apellidos,
                                             NombreCompleto = co.Supervisor1Id > 0 ? super1.Nombres + " " + super1.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper1.CargoId > 0 ? cargoSuper1.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper1.NumeroIdentificacion,
                                         },
                                         Supervisor2 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = co.Supervisor2Id.HasValue ? co.Supervisor2Id.Value : 0,
                                             Nombres = super2.Nombres,
                                             Apellidos = super2.Apellidos,
                                             NombreCompleto = co.Supervisor2Id > 0 ? super2.Nombres + " " + super2.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper2.CargoId > 0 ? cargoSuper2.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper2.NumeroIdentificacion,
                                         }
                                     },
                                     Tercero = new TerceroDto()
                                     {
                                         TerceroId = t.TerceroId,
                                         Nombre = t.Nombre,
                                         NumeroIdentificacion = t.NumeroIdentificacion,
                                         Email = t.Email,
                                         Telefono = t.Telefono,
                                         Direccion = t.Direccion.Length > 45 ? t.Direccion.Substring(0, 45) + "..." : t.Direccion,
                                         FechaExpedicionDocumento = t.FechaExpedicionDocumento.HasValue ? (t.FechaExpedicionDocumento.Value.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaExpedicionDocumento.Value : null) : null,
                                         RegimenTributario = t.RegimenTributario,
                                         DeclaranteRentaDescripcion = t.DeclaranteRenta ? "SI" : "NO",
                                         FacturadorElectronicoDescripcion = pt.FacturaElectronica ? "SI" : "NO"
                                     },

                                 }).FirstOrDefaultAsync();

            return formato;
        }

        public async Task<List<CDPDto>> ObtenerInformacionFinancieraXListaCompromiso(string numeroContrato, List<long> listaCrp)
        {
            var lista = (from c in _context.DocumentoCompromiso
                         join co in _context.Contrato on new { Crp=c.NumeroDocumento, c.PciId } equals new { Crp=co.Crp, co.PciId }
                         join pci in _context.Pci on c.PciId equals pci.PciId
                         where c.NumeroDocumentoSoporte.Trim() == numeroContrato
                         where listaCrp.Contains(c.NumeroDocumento)
                         select new CDPDto()
                         {
                             Crp = c.NumeroDocumento,
                             Fecha = c.FechaDocumentoSoporte,
                             PciId = c.PciId.Value,
                             Pci = pci.Identificacion,
                             ValorInicial = c.ValorInicial,
                             Operacion = c.ValorOperacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorActual, //valor actual
                             SaldoActual = c.SaldoPorUtilizar, //saldo actual
                         });

            var listaAgrupada = await (from i in lista
                                       group i by new
                                       {
                                           i.Crp,
                                           i.Fecha,
                                           i.PciId,
                                           i.Pci,
                                       }
                          into grp
                                       select new CDPDto()
                                       {
                                           Crp = grp.Key.Crp,
                                           Fecha = grp.Key.Fecha,
                                           PciId = grp.Key.PciId,
                                           Pci = grp.Key.Pci,
                                           ValorInicial = grp.Sum(i => i.ValorInicial),
                                           Operacion = grp.Sum(i => i.Operacion),
                                           ValorTotal = grp.Sum(i => i.ValorTotal),
                                           SaldoActual = grp.Sum(i => i.SaldoActual)
                                       }).ToListAsync();
            return listaAgrupada;
        }

        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXId(int formatoSolicitudPagoId)
        {
            var lista = (from s in _context.FormatoSolicitudPago
                         join c in _context.DocumentoCompromiso on s.Crp equals c.NumeroDocumento
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where s.FormatoSolicitudPagoId == formatoSolicitudPagoId
                         where s.PciId == c.PciId
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             Crp = c.NumeroDocumento,
                             Fecha = c.FechaRegistro, //Fecha compromiso
                             Detalle4 = c.Observaciones, //objeto contrato
                             TerceroId = c.TerceroId.Value,
                             Detalle7 = c.TipoDocumentoSoporte,//Modalidad de contrato
                             ValorInicial = c.ValorInicial,
                             Operacion = c.ValorOperacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorActual, //valor actual
                             SaldoActual = c.SaldoPorUtilizar , //saldo actual
                         });

            var listaAgrupada = (from i in lista
                                 group i by new
                                 {
                                     i.Cdp,
                                     i.Crp,
                                     i.Fecha,
                                     i.Detalle4,
                                     i.TerceroId,
                                     i.Detalle7,
                                     i.ValorInicial,
                                     i.Operacion,
                                     i.ValorTotal,
                                     i.SaldoActual
                                 }
                          into grp
                                 select new CDPDto()
                                 {
                                     Cdp = grp.Key.Cdp,
                                     Crp = grp.Key.Crp,
                                     Fecha = grp.Key.Fecha,
                                     Detalle4 = grp.Key.Detalle4,
                                     TerceroId = grp.Key.TerceroId,
                                     Detalle7 = grp.Key.Detalle7,
                                     ValorInicial = grp.Sum(i => i.ValorInicial),
                                     Operacion = grp.Sum(i => i.Operacion),
                                     ValorTotal = grp.Sum(i => i.ValorTotal),
                                     SaldoActual = grp.Sum(i => i.SaldoActual)
                                 });

            var formato = await (from s in _context.FormatoSolicitudPago
                                 join c in listaAgrupada on s.Crp equals c.Crp
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join ae in _context.ActividadEconomica on s.ActividadEconomicaId equals ae.ActividadEconomicaId
                                 join pp in _context.PlanPago on s.PlanPagoId equals pp.PlanPagoId
                                 join co in _context.Contrato on c.Crp equals co.Crp
                                 join plt in _context.ParametroLiquidacionTercero on t.TerceroId equals plt.TerceroId into ParametroTercero
                                 from par in ParametroTercero.DefaultIfEmpty()
                                 join ap in _context.TipoAdminPila on par.TipoAdminPilaId equals ap.TipoAdminPilaId into AdminPila
                                 from adminPila in AdminPila.DefaultIfEmpty()

                                 join sup1 in _context.Usuario on co.Supervisor1Id equals sup1.UsuarioId into Supervisor1
                                 from super1 in Supervisor1.DefaultIfEmpty()
                                 join carg1 in _context.Cargo on super1.CargoId equals carg1.CargoId into Cargo1
                                 from cargoSuper1 in Cargo1.DefaultIfEmpty()
                                 join tercSup1 in _context.Tercero on super1.TerceroId equals tercSup1.TerceroId into TerceroSupervisor1
                                 from tercSuper1 in TerceroSupervisor1.DefaultIfEmpty()

                                 join sup2 in _context.Usuario on co.Supervisor2Id equals sup2.UsuarioId into Supervisor2
                                 from super2 in Supervisor2.DefaultIfEmpty()
                                 join carg2 in _context.Cargo on super2.CargoId equals carg2.CargoId into Cargo2
                                 from cargoSuper2 in Cargo2.DefaultIfEmpty()
                                 join tercSup2 in _context.Tercero on super2.TerceroId equals tercSup2.TerceroId into TerceroSupervisor2
                                 from tercSuper2 in TerceroSupervisor2.DefaultIfEmpty()

                                 where s.FormatoSolicitudPagoId == formatoSolicitudPagoId
                                 where s.PciId == pp.PciId
                                 where s.PciId == co.PciId
                                 where s.PciId == par.PciId
                                 select new FormatoSolicitudPagoDto()
                                 {
                                     FormatoSolicitudPagoId = s.FormatoSolicitudPagoId,
                                     PlanPagoId = s.PlanPagoId,
                                     FechaInicio = s.FechaInicio,
                                     FechaFinal = s.FechaFinal,
                                     FechaSistema = s.FechaRegistro.Value,
                                     ValorFacturado = s.ValorFacturado,
                                     MesId = s.MesId,
                                     MesDescripcion = DateTimeFormatInfo.CurrentInfo.GetMonthName(s.MesId).ToUpper(),
                                     NumeroPlanilla = s.NumeroPlanilla,
                                     NumeroFactura = s.NumeroFactura,
                                     Observaciones = s.Observaciones,
                                     BaseCotizacion = s.BaseCotizacion,
                                     TipoAdminPila = adminPila.TipoAdminPilaId > 0 ? adminPila.Nombre : string.Empty,

                                     Cdp = new CDPDto()
                                     {
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         Detalle7 = c.Detalle7, //Modalidad de Contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                     },
                                     ActividadEconomica = new ValorSeleccion()
                                     {
                                         Id = ae.ActividadEconomicaId,
                                         Codigo = ae.Codigo,
                                         Nombre = ae.Nombre,
                                     },
                                     Contrato = new ContratoDto()
                                     {
                                         ContratoId = co.ContratoId,
                                         NumeroContrato = co.NumeroContrato,
                                         Crp = co.Crp,
                                         FechaInicio = co.FechaInicio,
                                         FechaFinal = co.FechaFinal,
                                         FechaRegistro = co.FechaRegistro,
                                         FechaExpedicionPoliza = co.FechaExpedicionPoliza,

                                         Supervisor1 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = super1.UsuarioId > 0 ? super1.UsuarioId : 0,
                                             Nombres = super1.Nombres,
                                             Apellidos = super1.Apellidos,
                                             NombreCompleto = co.Supervisor1Id > 0 ? super1.Nombres + " " + super1.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper1.CargoId > 0 ? cargoSuper1.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper1.NumeroIdentificacion,
                                         },
                                         Supervisor2 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = co.Supervisor2Id.HasValue ? co.Supervisor2Id.Value : 0,
                                             Nombres = co.Supervisor2Id.HasValue ? super2.Nombres : string.Empty,
                                             Apellidos = super2.UsuarioId > 0 ? super2.Apellidos : string.Empty,
                                             NombreCompleto = super2.UsuarioId > 0 ? super2.Nombres + " " + super2.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper2.CargoId > 0 ? cargoSuper2.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper2.NumeroIdentificacion,
                                         }
                                     },
                                     Tercero = new TerceroDto()
                                     {
                                         TerceroId = t.TerceroId,
                                         Nombre = t.Nombre,
                                         NumeroIdentificacion = t.NumeroIdentificacion,
                                         Email = t.Email,
                                         Telefono = t.Telefono,
                                         Direccion = t.Direccion.Length > 45 ? t.Direccion.Substring(0, 45) + "..." : t.Direccion,
                                         FechaExpedicionDocumento = t.FechaExpedicionDocumento.HasValue ? (t.FechaExpedicionDocumento.Value.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaExpedicionDocumento.Value : null) : null,
                                         RegimenTributario = t.RegimenTributario,
                                         DeclaranteRentaDescripcion = t.DeclaranteRenta ? "SI" : "NO",
                                         FacturadorElectronicoDescripcion = par.FacturaElectronica ? "SI" : "NO",
                                         ModalidadContrato = par.TerceroId > 0 ? par.ModalidadContrato : 0,
                                     },
                                     PlanPago = new PlanPagoDto()
                                     {
                                         PlanPagoId = pp.PlanPagoId,
                                         NumeroPago = pp.NumeroPago,
                                         ValorFacturado = pp.ValorFacturado.HasValue ? pp.ValorFacturado.Value : 0,
                                         ViaticosDescripcion = pp.Viaticos ? "SI" : "NO",
                                     }
                                 }).FirstOrDefaultAsync();
            return formato;
        }

        public async Task<List<CDPDto>> ObtenerPagosRealizadosXCompromiso(long crp, int pciId)
        {
            var lista = await (from c in _context.DocumentoOrdenPago
                                join co in _context.DocumentoCompromiso on c.Compromiso equals co.NumeroDocumento
                                join ctro in _context.Contrato on co.NumeroDocumento equals ctro.Crp into FiltroContrato
                                 from filtroContrato in FiltroContrato.DefaultIfEmpty()
                                join sup in _context.Usuario on filtroContrato.Supervisor1Id equals sup.UsuarioId into FiltroSupervisor
                                 from filtroSupervisor in FiltroSupervisor.DefaultIfEmpty()
                               where c.PciId == pciId
                               where c.Compromiso == crp
                               where c.Estado.ToUpper() == "PAGADA"
                               select new CDPDto()
                               {
                                   Cdp = c.Cdp,
                                   Crp = co.NumeroDocumento,
                                   OrdenPago = c.NumeroDocumento,
                                   Obligacion = long.Parse(c.Obligaciones),
                                   Detalle1 = c.Estado.ToUpper(), //Estado OP
                                   Detalle2 = co.DependenciaDescripcion, //Codigo Dependencia
                                   Fecha = c.FechaPago, //Fecha Orden Pago
                                   Detalle5 = string.Concat(filtroSupervisor.Nombres, filtroSupervisor.Apellidos), //Supervisor
                                   Detalle4 = co.Observaciones, //objeto contrato
                                   ValorInicial = c.ValorBruto, //Valor Bruto
                                   Operacion = c.ValorDeduccion, //Valor deducciones
                                   ValorTotal = c.ValorNeto, //valor neto
                                   IdentificacionRubro = c.IdentificacionRubroPresupuestal, //Rubro Presupuestal

                               })
                                 .Distinct()
                                 .OrderBy(c => c.OrdenPago)
                                 .ToListAsync();

            return lista;
        }

        public async Task<List<CDPDto>> ObtenerPagosRealizadosXListaCompromiso(List<long> listaCrp)
        {
            List<CDPDto> listaFinal = new List<CDPDto>();

            var lista = await (from c in _context.DocumentoOrdenPago
                                join co in _context.DocumentoCompromiso on c.Compromiso equals co.NumeroDocumento
                                join ctro in _context.Contrato on c.NumeroDocumento equals ctro.Crp into FiltroContrato
                                 from filtroContrato in FiltroContrato.DefaultIfEmpty()
                                join sup in _context.Usuario on filtroContrato.Supervisor1Id equals sup.UsuarioId into FiltroSupervisor
                                 from filtroSupervisor in FiltroSupervisor.DefaultIfEmpty()
                               where listaCrp.Contains(c.Compromiso)
                               where c.Estado.ToUpper() == "PAGADA"
                               select new CDPDto()
                               {
                                   Cdp = c.Cdp,
                                   Crp = co.NumeroDocumento,
                                   OrdenPago = c.NumeroDocumento,
                                   Obligacion = long.Parse(c.Obligaciones),
                                   Detalle1 = c.Estado.ToUpper(), //Estado OP
                                   Detalle2 = co.DependenciaDescripcion, //Codigo Dependencia
                                   Fecha = c.FechaPago, //Fecha Orden Pago
                                   Detalle5 = string.Concat(filtroSupervisor.Nombres, filtroSupervisor.Apellidos), //Supervisor
                                   Detalle4 = co.Observaciones, //objeto contrato
                                   ValorInicial = c.ValorBruto, //Valor Bruto
                                   Operacion = c.ValorDeduccion, //Valor deducciones
                                   ValorTotal = c.ValorNeto, //valor neto
                                   IdentificacionRubro = c.IdentificacionRubroPresupuestal, //Rubro Presupuestal

                               })
                                 .Distinct()
                                 .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                listaFinal = lista
                                 .OrderBy(c => c.Crp)
                                 .ThenBy(c => c.OrdenPago)
                                 .ToList();

            }

            return listaFinal;
        }


        public async Task<Numeracion> ObtenerUltimaNumeracionDisponible(int pciId)
        {
            Numeracion numeracion = new Numeracion();
            var lista = await (from c in _context.Numeracion
                               where c.PciId == pciId
                               where c.Utilizado == false
                               select c)
                              .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                numeracion = lista.OrderBy(x => x.Consecutivo).First();
            }
            return numeracion;
        }

        public async Task<Numeracion> ObtenerNumeracionBase(int numeracionId)
        {
            var numeracion = await (from c in _context.Numeracion
                                    where c.NumeracionId == numeracionId
                                    select c)
                              .FirstOrDefaultAsync();


            return numeracion;
        }

        public async Task<Numeracion> ObtenerNumeracionxNumeroFactura(string numeroFactura)
        {
            var numeracion = await (from c in _context.Numeracion
                                    where c.NumeroConsecutivo == numeroFactura
                                    select c)
                              .FirstOrDefaultAsync();

            return numeracion;
        }


        #endregion Registro de Solicitud de Pago

        #region Proceso Liquidación Masiva
        public async Task<List<FormatoSolicitudPagoDto>> ObtenerListaSolicitudPagoXId(List<int> listaSolicitudPagoId)
        {
            return await (from sp in _context.FormatoSolicitudPago
                          join c in _context.PlanPago on new { sp.PlanPagoId, sp.PciId } equals new { c.PlanPagoId, c.PciId }
                          join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                          join t in _context.Tercero on c.TerceroId equals t.TerceroId
                          join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                          from pl in parametroLiquidacion.DefaultIfEmpty()
                          where c.PciId == pl.PciId
                          where (listaSolicitudPagoId.Contains(sp.FormatoSolicitudPagoId))
                          where (sp.EstadoId == (int)EstadoSolicitudPago.Aprobado)
                          select new FormatoSolicitudPagoDto()
                          {
                              PlanPagoId = c.PlanPagoId,
                              FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId,
                              //   Cdp = c.Cdp,
                              //   Crp = c.Crp,
                              //   AnioPago = c.AnioPago,
                              //   MesPago = c.MesPago,
                              //   ValorAPagar = c.ValorAPagar,
                              //   NumeroPago = c.NumeroPago,
                              //   EstadoPlanPagoId = c.EstadoPlanPagoId,
                              //   NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                              FechaRadicadoSupervisor = c.FechaRadicadoSupervisor.Value,
                              //   ValorFacturado = c.ValorFacturado,
                              Tercero = new TerceroDto()
                              {
                                  TerceroId = c.TerceroId,
                                  NumeroIdentificacion = t.NumeroIdentificacion,
                                  Nombre = t.Nombre,
                                  ModalidadContrato = pl.ModalidadContrato,
                                  TipoPago = pl.TipoPago,
                                  TipoIva = pl.TipoIva.HasValue ? pl.TipoIva.Value : 0,
                              },
                              PlanPago = new PlanPagoDto()
                              {
                                  PlanPagoId = c.PlanPagoId,
                                  Viaticos = c.Viaticos,
                              }
                          })
                        .OrderBy(c => c.FechaRadicadoSupervisor)
                        .ToListAsync();
        }

        public async Task<PagedList<FormatoSolicitudPagoDto>> ObtenerListaSolicitudPagoPaginada(int? terceroId, List<int> listaEstadoId, UserParams userParams)
        {
            var lista = (from sp in _context.FormatoSolicitudPago
                         join c in _context.PlanPago on new { sp.PlanPagoId, sp.PciId } equals new { c.PlanPagoId, c.PciId }
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where c.PciId == pl.PciId
                         where c.PciId == userParams.PciId
                         where pl.PciId == userParams.PciId
                         where (sp.TerceroId == terceroId || terceroId == null)
                         where (sp.EstadoId == (int)EstadoSolicitudPago.Aprobado)
                         //where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         select new FormatoSolicitudPagoDto()
                         {
                             PlanPagoId = c.PlanPagoId,
                             FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId,
                             //  Cdp = c.Cdp,
                             //  AnioPago = c.AnioPago,
                             //  MesPago = c.MesPago,
                             //  ValorAPagar = c.ValorAPagar,
                             //  Viaticos = c.Viaticos,
                             //  NumeroPago = c.NumeroPago,
                             //  EstadoPlanPagoId = c.EstadoPlanPagoId,
                             //  NumeroRadicadoSupervisor = c.NumeroRadicadoSupervisor,
                             FechaRadicadoSupervisor = c.FechaRadicadoSupervisor.Value,
                             //  ValorFacturado = c.ValorFacturado,
                             Tercero = new TerceroDto()
                             {
                                 TerceroId = c.TerceroId,
                                 NumeroIdentificacion = t.NumeroIdentificacion,
                                 Nombre = t.Nombre,
                                 ModalidadContrato = pl.ModalidadContrato,
                                 TipoPago = pl.TipoPago,
                                 TipoIva = pl.TipoIva.HasValue ? pl.TipoIva.Value : 0,
                             },
                             PlanPago = new PlanPagoDto()
                             {
                                 PlanPagoId = c.PlanPagoId,
                                 Viaticos = c.Viaticos
                             }
                         })
                         .Distinct()
                        .OrderBy(c => c.FechaRadicadoSupervisor);

            return await PagedList<FormatoSolicitudPagoDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<ICollection<FormatoSolicitudPago>> ObtenerListaFormatoSolicitudPagoBase(List<int> listaSolicitudPagoId)
        {
            return await (from s in _context.FormatoSolicitudPago
                          where listaSolicitudPagoId.Contains(s.FormatoSolicitudPagoId)
                          where s.EstadoId == (int)EstadoSolicitudPago.Aprobado
                          select s)
                         .ToListAsync();
        }
        #endregion Proceso Liquidación Masiva

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}