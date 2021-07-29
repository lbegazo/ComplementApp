using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Dtos;
using System;
using ComplementApp.API.Helpers;

namespace ComplementApp.API.Data
{
    public class ListaRepository : IListaRepository
    {
        private readonly DataContext _context;
        public ListaRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Cargo>> ObtenerCargos()
        {
            return await _context.Cargo.ToListAsync();
        }

        public async Task<IEnumerable<Area>> ObtenerAreas()
        {
            return await _context.Area.ToListAsync();
        }

        public async Task<IEnumerable<TipoOperacion>> ObtenerListaTipoOperacion()
        {
            return await _context.TipoOperacion.ToListAsync();
        }

        public async Task<IEnumerable<TipoDetalleCDP>> ObtenerListaTipoDetalleModificacion()
        {
            return await _context.TipoDetalleModificacion.ToListAsync();
        }

        public async Task<IEnumerable<Tercero>> ObtenerListaTercero(string numeroIdentificacion, string nombre)
        {

            IQueryable<Tercero> listaFiltrada = null;

            var lista = (from d in _context.Tercero
                         select new Tercero()
                         {
                             TerceroId = d.TerceroId,
                             NumeroIdentificacion = d.NumeroIdentificacion,
                             Nombre = d.Nombre,
                         }).OrderBy(d => d.Nombre);

            if (!string.IsNullOrEmpty(numeroIdentificacion))
            {
                listaFiltrada = (from l in lista
                                 where l.NumeroIdentificacion.Contains(numeroIdentificacion)
                                 select l);
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                listaFiltrada = (from l in lista
                                 where l.Nombre.Contains(nombre)
                                 select l);
            }
            return await listaFiltrada.ToListAsync();
        }

        public async Task<IEnumerable<Perfil>> ObtenerListaPerfiles()
        {
            return await _context.Perfil
                        .Where(x => x.Estado == true)
                        .ToListAsync();
        }

        public async Task<IEnumerable<ParametroGeneral>> ObtenerParametrosGenerales()
        {
            return await _context.ParametroGeneral.ToListAsync();
        }

        public async Task<ValorSeleccion> ObtenerParametroGeneralXNombre(string nombre)
        {
            return await (from pg in _context.ParametroGeneral
                          where pg.Nombre == nombre
                          select new ValorSeleccion()
                          {
                              Id = pg.ParametroGeneralId,
                              Nombre = pg.Nombre,
                              Valor = pg.Valor,
                          })
                        .FirstOrDefaultAsync();
        }

        public async Task<ICollection<ValorSeleccion>> ObtenerParametrosGeneralesXTipo(string tipo)
        {
            var filtroUpper = tipo.ToUpper();

            return await (from pg in _context.ParametroGeneral
                          where pg.Tipo.ToUpper() == filtroUpper
                          select new ValorSeleccion()
                          {
                              Id = pg.ParametroGeneralId,
                              Nombre = pg.Nombre,
                              Valor = pg.Valor,
                              TipoDocumento = pg.Tipo
                          })
                        .ToListAsync();
        }

        public async Task<ICollection<CriterioCalculoReteFuente>> ObtenerListaCriterioCalculoReteFuente()
        {
            return await _context.CriterioCalculoReteFuente.ToListAsync();
        }

        public async Task<ICollection<Estado>> ObtenerListaEstado(string tipoDocumento)
        {
            return await _context.Estado.Where(x => x.TipoDocumento == tipoDocumento).ToListAsync();
        }

        public async Task<IEnumerable<UsuarioParaDetalleDto>> ObtenerListaUsuarioxFiltro(int pciId, string nombres, string apellidos)
        {
            IQueryable<UsuarioParaDetalleDto> listaFiltrada = null;

            var lista = (from u in _context.Usuario
                         where u.PciId == pciId
                         select new UsuarioParaDetalleDto()
                         {
                             UsuarioId = u.UsuarioId,
                             Nombres = u.Nombres,
                             Apellidos = u.Apellidos,
                             NombreCompleto = u.Nombres + " " + u.Apellidos,
                             Username = u.Username,
                             PciId = u.PciId.Value,
                         }).OrderBy(d => d.Nombres);

            if (!string.IsNullOrEmpty(nombres))
            {
                listaFiltrada = (from l in lista
                                 where l.PciId == pciId
                                 where l.Nombres.Contains(nombres)
                                 select l);
            }

            if (!string.IsNullOrEmpty(apellidos))
            {
                listaFiltrada = (from l in lista
                                 where l.PciId == pciId
                                 where l.Apellidos.Contains(apellidos)
                                 select l);
            }

            var listaUsuario = await listaFiltrada.ToListAsync();

            foreach (var usuario in listaUsuario)
            {
                Perfil perfil = await (from up in _context.UsuarioPerfil
                                       join p in _context.Perfil on up.PerfilId equals p.PerfilId
                                       where up.UsuarioId == usuario.UsuarioId
                                       select p).FirstOrDefaultAsync();
                usuario.Perfiles = new List<Perfil>();
                usuario.Perfiles.Add(perfil);
            }

            return listaUsuario;
        }

        public async Task<IEnumerable<UsoPresupuestal>> ObtenerListaUsoPresupuestalXRubro(int rubroPresupuestalId)
        {
            return await _context.UsoPresupuestal
                        .Where(x => x.RubroPresupuestalId == rubroPresupuestalId)
                        .ToListAsync();
        }

        public async Task<ICollection<ValorSeleccion>> ObtenerListaXTipo(TipoLista tipo)
        {
            List<ValorSeleccion> lista = new List<ValorSeleccion>();

            switch (tipo)
            {
                case TipoLista.ModalidadContrato:
                    {
                        lista = await (from m in _context.TipoModalidadContrato
                                       select new ValorSeleccion()
                                       {
                                           Id = m.TipoModalidadContratoId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       }).ToListAsync();
                        break;
                    }
                case TipoLista.TipoCuentaXPagar:
                    {
                        lista = await (from m in _context.TipoCuentaXPagar
                                       select new ValorSeleccion()
                                       {
                                           Id = m.TipoCuentaXPagarId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       })
                                       .OrderBy(m => m.Nombre)
                                       .ToListAsync();
                        break;
                    }
                case TipoLista.TipoIva:
                    {
                        lista = await (from m in _context.TipoIva
                                       select new ValorSeleccion()
                                       {
                                           Id = m.TipoIvaId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       }).ToListAsync();
                        break;
                    }
                case TipoLista.TipoPago:
                    {
                        lista = await (from m in _context.TipoDePago
                                       select new ValorSeleccion()
                                       {
                                           Id = m.TipoDePagoId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       }).ToListAsync();
                        break;
                    }
                case TipoLista.TipoDocumentoSoporte:
                    {
                        lista = await (from m in _context.TipoDocumentoSoporte
                                       select new ValorSeleccion()
                                       {
                                           Id = m.TipoDocumentoSoporteId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       })
                                       .OrderBy(m => m.Nombre)
                                       .ToListAsync();
                        break;
                    }
                case TipoLista.TipoAdminPila:
                    {
                        lista = await (from m in _context.TipoAdminPila
                                       select new ValorSeleccion()
                                       {
                                           Id = Int32.Parse(m.Codigo),
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       })
                                       .OrderBy(m => m.Nombre)
                                       .ToListAsync();
                        break;
                    }
                case TipoLista.TipoDocumentoIdentidad:
                    {
                        lista = await (from m in _context.TipoDocumentoIdentidad
                                       select new ValorSeleccion()
                                       {
                                           Id = m.TipoDocumentoIdentidadId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       })
                                       .OrderBy(m => m.Nombre)
                                       .ToListAsync();
                        break;
                    }
                case TipoLista.TipoContrato:
                    {
                        lista = await (from m in _context.TipoContrato
                                       select new ValorSeleccion()
                                       {
                                           Id = m.TipoContratoId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       })
                                        .OrderBy(m => m.Nombre)
                                        .ToListAsync();
                        break;
                    }
                case TipoLista.Pci:
                    {
                        lista = await (from m in _context.Pci
                                       select new ValorSeleccion()
                                       {
                                           Id = m.PciId,
                                           Codigo = m.Identificacion,
                                           Nombre = m.Identificacion + " " + m.Nombre,
                                       })
                                        .OrderBy(m => m.Nombre)
                                        .ToListAsync();
                        break;
                    }
                case TipoLista.FuenteFinanciacion:
                    {
                        lista = await (from m in _context.FuenteFinanciacion
                                       select new ValorSeleccion()
                                       {
                                           Id = m.FuenteFinanciacionId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       }).ToListAsync();
                        break;
                    }
                case TipoLista.SituacionFondo:
                    {
                        lista = await (from m in _context.SituacionFondo
                                       select new ValorSeleccion()
                                       {
                                           Id = m.SituacionFondoId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       }).ToListAsync();
                        break;
                    }
                case TipoLista.RecursoPresupuestal:
                    {
                        lista = await (from m in _context.RecursoPresupuestal
                                       select new ValorSeleccion()
                                       {
                                           Id = m.RecursoPresupuestalId,
                                           Codigo = m.Codigo,
                                           Nombre = m.Nombre,
                                       }).ToListAsync();
                        break;
                    }
                case TipoLista.MedioPago:
                    {
                        lista = ListaMedioPago();
                        break;
                    }
                default: break;
            }

            return lista;
        }

        public async Task<ICollection<ValorSeleccion>> ObtenerListaXTipoyPci(int pciId, TipoLista tipo)
        {
            List<ValorSeleccion> lista = new List<ValorSeleccion>();

            switch (tipo)
            {
                case TipoLista.Dependencia:
                    {
                        lista = await (from m in _context.Dependencia
                                       where m.PciId == pciId
                                       select new ValorSeleccion()
                                       {
                                           Id = m.DependenciaId,
                                           Nombre = m.Nombre,
                                       }).ToListAsync();
                        break;
                    }
                case TipoLista.Usuario:
                    {
                        lista = await (from m in _context.Usuario
                                       where m.PciId == pciId
                                       select new ValorSeleccion()
                                       {
                                           Id = m.UsuarioId,
                                           Nombre = m.Nombres + ' ' + m.Apellidos,
                                       }).ToListAsync();
                        break;
                    }
                default: break;
            }

            return lista;
        }

        public async Task<IEnumerable<Deduccion>> ObtenerListaDeducciones(string codigo, string descripcion)
        {
            IQueryable<Deduccion> listaFiltrada = null;

            var lista = (from d in _context.Deduccion
                         join t in _context.Tercero on d.TerceroId equals t.TerceroId into TerceroDeduccion
                         from ded in TerceroDeduccion.DefaultIfEmpty()
                         select new Deduccion()
                         {
                             DeduccionId = d.DeduccionId,
                             Codigo = d.Codigo,
                             Nombre = d.Nombre,
                             Tarifa = d.Tarifa,
                             EsValorFijo = d.EsValorFijo,
                             Tercero = new Tercero()
                             {
                                 TerceroId = d.TerceroId > 0 ? ded.TerceroId : 0,
                                 NumeroIdentificacion = d.TerceroId > 0 ? ded.NumeroIdentificacion : string.Empty,
                                 Nombre = d.TerceroId > 0 ? ded.Nombre : string.Empty,
                             }
                         }).OrderBy(d => d.Nombre);

            if (!string.IsNullOrEmpty(codigo))
            {
                listaFiltrada = (from l in lista
                                 where l.Codigo.Contains(codigo)
                                 select l);
            }

            if (!string.IsNullOrEmpty(descripcion))
            {
                listaFiltrada = (from l in lista
                                 where l.Nombre.Contains(descripcion)
                                 select l);
            }


            return await listaFiltrada.ToListAsync();
        }

        public async Task<IEnumerable<ActividadEconomica>> ObtenerListaActividadesEconomicas(string codigo)
        {
            return await _context.ActividadEconomica
                            .Where(t => t.Codigo.Contains(codigo))
                            .ToListAsync();
        }

        public async Task<Pci> ObtenerPci(int pciId)
        {
            return await _context.Pci
                        .Where(x => x.PciId == pciId)
                        .FirstOrDefaultAsync();
        }

        public async Task<PagedList<RubroPresupuestal>> ObtenerListaRubroPresupuestalPorPapa(int rubroPresupuestalId, UserParams userParams)
        {
            var lista = (from rp in _context.RubroPresupuestal
                         where rp.PadreRubroId == rubroPresupuestalId
                         select rp)
                        .Distinct()
                        .OrderBy(t => t.Identificacion);

            return await PagedList<RubroPresupuestal>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IEnumerable<RubroPresupuestal>> ObtenerListaRubroPresupuestal(string identificacion, string nombre)


        {
            IQueryable<RubroPresupuestal> listaFiltrada = null;
            var tieneFiltro = false;

            var lista = (from d in _context.RubroPresupuestal
                         select new RubroPresupuestal
                         {
                             RubroPresupuestalId = d.RubroPresupuestalId,
                             Identificacion = d.Identificacion,
                             Nombre = d.Nombre,
                             PadreRubroId = d.PadreRubroId,
                         }).OrderBy(d => d.Nombre);

            if (!string.IsNullOrEmpty(identificacion))
            {
                tieneFiltro = true;
                listaFiltrada = (from l in lista
                                 where l.Identificacion.Contains(identificacion)
                                 select l);
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                tieneFiltro = true;
                listaFiltrada = (from l in lista
                                 where l.Nombre.Contains(nombre)
                                 select l);
            }

            if (tieneFiltro)
                return await listaFiltrada.ToListAsync();
            else
                return await lista.ToListAsync();
        }

        public async Task<IEnumerable<SolicitudCDPParaPrincipalDto>> ObtenerListaSolicitudCDP(string numeroSolicitud)
        {
            IQueryable<SolicitudCDPParaPrincipalDto> listaFiltrada = null;

            var lista = (from d in _context.SolicitudCDP
                         select new SolicitudCDPParaPrincipalDto()
                         {
                             SolicitudCDPId = d.SolicitudCDPId,
                             ObjetoBienServicioContratado = d.ObjetoBienServicioContratado,
                         }).OrderBy(d => d.SolicitudCDPId);

            if (!string.IsNullOrEmpty(numeroSolicitud))
            {
                // listaFiltrada = lista
                //                  .Where(x => EF.Functions.Like(x.SolicitudCDPId.ToString(), numeroSolicitud));

                listaFiltrada = (from l in lista
                                 where l.SolicitudCDPId.ToString().Contains(numeroSolicitud)
                                 select l);

            }

            return await listaFiltrada.ToListAsync();
        }
        private List<ValorSeleccion> ListaMedioPago()
        {
            List<ValorSeleccion> lista = new List<ValorSeleccion>()
            {
             new ValorSeleccion()
             {
                Id = 1,
                Codigo="AC",
                Nombre="Abono en Cuenta",
             },
             new ValorSeleccion()
             {
                Id = 2,
                Codigo="AC",
                Nombre="Entre Entidades CUN",
             },
             new ValorSeleccion()
             {
                Id = 3,
                Codigo="GR",
                Nombre="Giro",
             },
             new ValorSeleccion()
             {
                Id = 4,
                Codigo="CH",
                Nombre="Cheque",
             },
             new ValorSeleccion()
             {
                Id = 5,
                Codigo="AC",
                Nombre="Transpaso a Pagaduria",
             },
             new ValorSeleccion()
             {
                Id = 5,
                Codigo="EF",
                Nombre="Efectivo",
             },
            };

            return lista;
        }
    }
}