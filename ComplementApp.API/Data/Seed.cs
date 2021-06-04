using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace ComplementApp.API.Data
{
    public class Seed
    {
        public static void CargarDataInicial(DataContext context)
        {
            SeedParametroSistema(context);
            SeedPci(context);

            SeedTipoContrato(context);
            SeedRubroPresupuestal(context);
            SeedCargo(context);
            SeedArea(context);
            SeedTipoOperacion(context);
            SeedTipoDetalleModificacion(context);
            SeedEstado(context);
            SeedTercero(context);
            SeedActividadGeneral(context);
            SeedUsoPresupuestal(context);

            SeedPerfil(context);
            SeedTransaccion(context);
            SeedPerfilTransaccion(context);
            SeedUsuarioPerfil(context);

            SeedUsuario(context);
            SeedDependencia(context);
            SeedActividadEspecifica(context);

            SeedTipoBaseDeduccion(context);
            SeedParametroGeneral(context);
            SeedDeduccion(context);
            SeedTerceroDeducciones(context);
            SeedParametroLiquidacionTercero(context);
            SeedCriterioCalculoReteFuente(context);

            SeedActividadEconomica(context);
            SeedTipoGasto(context);
            SeedSituacionFondo(context);
            SeedFuenteFinanciacion(context);
            SeedRecursoPresupuestal(context);
            SeedAtributoContable(context);

            SeedModalidadContrato(context);
            SeedTipoDePago(context);
            SeedTipoIva(context);
            SeedTipoCuentaXPagar(context);
            SeedTipoDocumentoSoporte(context);
            SeedTipoDocumentoIdentidad(context);

            SeedTipoAdminPila(context);
        }

        private static void SeedParametroSistema(DataContext context)
        {
            if (File.Exists("Data/SeedFiles/_ParametroSistema.json"))
            {
                string valor = string.Empty;

                var data = File.ReadAllText("Data/SeedFiles/_ParametroSistema.json");
                var parametros = JsonConvert.DeserializeObject<List<ParametroSistema>>(data);
                foreach (var item in parametros)
                {
                    var parametroBD = obtenerParametroSistema(context, item.Nombre);

                    if (parametroBD == null)
                    {
                        valor = Encrypt(item.Valor);
                        item.Valor = valor;
                        context.ParametroSistema.Add(item);
                    }
                    else
                    {
                        valor = Encrypt(item.Valor);
                        parametroBD.Valor = valor;
                    }
                }
                context.SaveChanges();
            }
        }

        private static void SeedPci(DataContext context)
        {
            Pci itemNuevo = null;

            if (!context.Pci.Any())
            {
                if (File.Exists("Data/SeedFiles/_Pci.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_Pci.json");
                    var items = JsonConvert.DeserializeObject<List<Pci>>(data);
                    foreach (var item in items)
                    {
                        var itemBd = obtenerPci(context, item.Identificacion);

                        if (itemBd == null)
                        {
                            itemNuevo = new Pci();
                            itemNuevo.Nombre = item.Nombre;
                            itemNuevo.Identificacion = item.Identificacion;
                            itemNuevo.Estado = true;

                            context.Pci.Add(itemNuevo);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        private static void SeedTipoContrato(DataContext context)
        {
            TipoContrato valor = null;
            List<TipoContrato> lista = new List<TipoContrato>();

            if (!context.TipoContrato.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoContrato.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoContrato.json");
                    var items = JsonConvert.DeserializeObject<List<TipoContrato>>(data);
                    foreach (var item in items)
                    {
                        valor = new TipoContrato();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoContrato.AddRange(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedTipoAdminPila(DataContext context)
        {
            TipoAdminPila valor = null;
            List<TipoAdminPila> lista = new List<TipoAdminPila>();

            if (!context.TipoAdminPila.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoAdminPila.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoAdminPila.json");
                    var items = JsonConvert.DeserializeObject<List<TipoAdminPila>>(data);
                    foreach (var item in items)
                    {
                        valor = new TipoAdminPila();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoAdminPila.AddRange(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedTipoDocumentoIdentidad(DataContext context)
        {
            TipoDocumentoIdentidad valor = null;
            List<TipoDocumentoIdentidad> lista = new List<TipoDocumentoIdentidad>();

            if (!context.TipoDocumentoIdentidad.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoDocumentoIdentidad.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoDocumentoIdentidad.json");
                    var items = JsonConvert.DeserializeObject<List<TipoDocumentoIdentidad>>(data);
                    foreach (var item in items)
                    {
                        valor = new TipoDocumentoIdentidad();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoDocumentoIdentidad.AddRange(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedModalidadContrato(DataContext context)
        {
            TipoModalidadContrato valor = null;
            List<TipoModalidadContrato> lista = new List<TipoModalidadContrato>();

            if (!context.TipoModalidadContrato.Any())
            {
                if (File.Exists("Data/SeedFiles/_ModalidadContrato.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_ModalidadContrato.json");
                    var items = JsonConvert.DeserializeObject<List<TipoModalidadContrato>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new TipoModalidadContrato();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoModalidadContrato.AddRange(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedTipoDePago(DataContext context)
        {
            TipoDePago valor = null;
            List<TipoDePago> lista = new List<TipoDePago>();

            if (!context.TipoDePago.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoDePago.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoDePago.json");
                    var items = JsonConvert.DeserializeObject<List<TipoDePago>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new TipoDePago();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoDePago.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedTipoIva(DataContext context)
        {
            TipoIva valor = null;
            List<TipoIva> lista = new List<TipoIva>();

            if (!context.TipoIva.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoIva.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoIva.json");
                    var items = JsonConvert.DeserializeObject<List<TipoIva>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new TipoIva();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoIva.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedTipoCuentaXPagar(DataContext context)
        {
            TipoCuentaXPagar valor = null;
            List<TipoCuentaXPagar> lista = new List<TipoCuentaXPagar>();

            if (!context.TipoCuentaXPagar.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoCuentaXPagar.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoCuentaXPagar.json");
                    var items = JsonConvert.DeserializeObject<List<TipoCuentaXPagar>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new TipoCuentaXPagar();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoCuentaXPagar.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedTipoDocumentoSoporte(DataContext context)
        {
            TipoDocumentoSoporte valor = null;
            List<TipoDocumentoSoporte> lista = new List<TipoDocumentoSoporte>();

            if (!context.TipoDocumentoSoporte.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoDocumentoSoporte.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoDocumentoSoporte.json");
                    var items = JsonConvert.DeserializeObject<List<TipoDocumentoSoporte>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new TipoDocumentoSoporte();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoDocumentoSoporte.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedAtributoContable(DataContext context)
        {
            AtributoContable valor = null;
            List<AtributoContable> lista = new List<AtributoContable>();

            if (!context.AtributoContable.Any())
            {
                if (File.Exists("Data/SeedFiles/_AtributoContable.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_AtributoContable.json");
                    var items = JsonConvert.DeserializeObject<List<AtributoContable>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new AtributoContable();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.AtributoContable.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedRecursoPresupuestal(DataContext context)
        {
            RecursoPresupuestal valor = null;
            List<RecursoPresupuestal> lista = new List<RecursoPresupuestal>();

            if (!context.RecursoPresupuestal.Any())
            {
                if (File.Exists("Data/SeedFiles/_RecursoPresupuestal.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_RecursoPresupuestal.json");
                    var items = JsonConvert.DeserializeObject<List<RecursoPresupuestal>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new RecursoPresupuestal();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.RecursoPresupuestal.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedTipoGasto(DataContext context)
        {
            TipoGasto valor = null;
            List<TipoGasto> lista = new List<TipoGasto>();

            if (!context.TipoGasto.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoGasto.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoGasto.json");
                    var items = JsonConvert.DeserializeObject<List<TipoGasto>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new TipoGasto();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.TipoGasto.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedSituacionFondo(DataContext context)
        {
            SituacionFondo valor = null;
            List<SituacionFondo> lista = new List<SituacionFondo>();

            if (!context.SituacionFondo.Any())
            {
                if (File.Exists("Data/SeedFiles/_SituacionFondo.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_SituacionFondo.json");
                    var items = JsonConvert.DeserializeObject<List<SituacionFondo>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new SituacionFondo();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.SituacionFondo.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedFuenteFinanciacion(DataContext context)
        {
            FuenteFinanciacion valor = null;
            List<FuenteFinanciacion> lista = new List<FuenteFinanciacion>();

            if (!context.FuenteFinanciacion.Any())
            {
                if (File.Exists("Data/SeedFiles/_FuenteFinanciacion.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_FuenteFinanciacion.json");
                    var items = JsonConvert.DeserializeObject<List<FuenteFinanciacion>>(data);
                    var items2 = items.OrderBy(x => x.Codigo);
                    foreach (var item in items2)
                    {
                        valor = new FuenteFinanciacion();
                        valor.Nombre = item.Nombre;
                        valor.Codigo = item.Codigo;
                        context.FuenteFinanciacion.Add(valor);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedActividadEconomica(DataContext context)
        {
            ActividadEconomica actividad = null;
            List<ActividadEconomica> lista = new List<ActividadEconomica>();

            if (!context.ActividadEconomica.Any())
            {
                if (File.Exists("Data/SeedFiles/_ActividadEconomica.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_ActividadEconomica.json");
                    var items = JsonConvert.DeserializeObject<List<ActividadEconomica>>(data);
                    foreach (var item in items)
                    {
                        actividad = new ActividadEconomica();
                        actividad.Nombre = item.Nombre;
                        actividad.Codigo = item.Codigo;
                        lista.Add(actividad);
                    }
                    context.ActividadEconomica.AddRange(lista);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedCriterioCalculoReteFuente(DataContext context)
        {
            if (!context.CriterioCalculoReteFuente.Any())
            {
                CriterioCalculoReteFuente nuevoItem = null;
                List<CriterioCalculoReteFuente> lista = new List<CriterioCalculoReteFuente>();

                if (File.Exists("Data/SeedFiles/_CriterioReteFuente.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_CriterioReteFuente.json");
                    var items = JsonConvert.DeserializeObject<List<CriterioCalculoReteFuente>>(data);
                    foreach (var item in items)
                    {
                        nuevoItem = new CriterioCalculoReteFuente();
                        nuevoItem.Tarifa = item.Tarifa;
                        nuevoItem.Desde = item.Desde;
                        nuevoItem.Hasta = item.Hasta;
                        nuevoItem.Factor = item.Factor;
                        lista.Add(nuevoItem);
                    }
                    context.CriterioCalculoReteFuente.AddRange(lista);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedTerceroDeducciones(DataContext context)
        {
            if (!context.TerceroDeducciones.Any())
            {
                TerceroDeduccion nuevoItem = null;
                List<TerceroDeduccion> lista = new List<TerceroDeduccion>();
                if (File.Exists("Data/SeedFiles/_TerceroDeduccion.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TerceroDeduccion.json");
                    var items = JsonConvert.DeserializeObject<List<TerceroDeduccionDto>>(data);
                    foreach (var item in items)
                    {
                        var terceroBD = obtenerTercero(context, item.TipoIdentificacion, item.IdentificacionTercero);
                        var deduccionBD = obtenerDeduccion(context, item.Codigo);

                        if (terceroBD != null && deduccionBD != null)
                        {
                            var terceroDeduccion = obtenerTerceroDeduccion(context, terceroBD.TerceroId, deduccionBD.DeduccionId);

                            if (terceroDeduccion == null)
                            {
                                nuevoItem = new TerceroDeduccion();
                                nuevoItem.TerceroId = terceroBD.TerceroId;
                                nuevoItem.DeduccionId = deduccionBD.DeduccionId;
                                lista.Add(nuevoItem);
                            }
                        }
                    }
                    context.TerceroDeducciones.AddRange(lista);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedParametroLiquidacionTercero(DataContext context)
        {
            if (!context.ParametroLiquidacionTercero.Any())
            {
                ParametroLiquidacionTercero nuevoItem = null;
                List<ParametroLiquidacionTercero> lista = new List<ParametroLiquidacionTercero>();
                DateTime fecha;

                if (File.Exists("Data/SeedFiles/_ParametroLiquidacionTercero.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_ParametroLiquidacionTercero.json");
                    var items = JsonConvert.DeserializeObject<List<ParametroLiquidacionTerceroDto>>(data);
                    foreach (var item in items)
                    {
                        var terceroBD = obtenerTercero(context, item.TipoDocumentoIdentidadId, item.IdentificacionTercero);
                        if (terceroBD != null)
                        {
                            nuevoItem = new ParametroLiquidacionTercero();
                            nuevoItem.TerceroId = terceroBD.TerceroId;
                            nuevoItem.Afc = item.Afc;
                            nuevoItem.AportePension = item.AportePension;
                            nuevoItem.AporteSalud = item.AporteSalud;
                            nuevoItem.BaseAporteSalud = item.BaseAporteSalud;
                            nuevoItem.Dependiente = item.Dependiente;

                            if (!string.IsNullOrEmpty(item.FechaFinalDescuentoInteresViviendaDes))
                            {
                                if (DateTime.TryParse(item.FechaFinalDescuentoInteresViviendaDes, out fecha))
                                    nuevoItem.FechaFinalDescuentoInteresVivienda = fecha;
                            }

                            if (!string.IsNullOrEmpty(item.FechaInicioDescuentoInteresViviendaDes))
                                if (DateTime.TryParse(item.FechaInicioDescuentoInteresViviendaDes, out fecha))
                                    nuevoItem.FechaInicioDescuentoInteresVivienda = fecha;

                            nuevoItem.FondoSolidaridad = item.FondoSolidaridad;
                            nuevoItem.HonorarioSinIva = item.HonorarioSinIva;
                            nuevoItem.InteresVivienda = item.InteresVivienda;
                            nuevoItem.MedicinaPrepagada = item.MedicinaPrepagada;
                            nuevoItem.ModalidadContrato = item.ModalidadContrato;
                            nuevoItem.PensionVoluntaria = item.PensionVoluntaria;
                            nuevoItem.RiesgoLaboral = item.RiesgoLaboral;
                            nuevoItem.TarifaIva = item.TarifaIva;
                            nuevoItem.TipoCuentaXPagarId = item.TipoCuentaXPagarId;
                            nuevoItem.TipoDocumentoSoporteId = item.TipoDocumentoSoporteId;
                            nuevoItem.TipoIva = item.TipoIva;
                            nuevoItem.TipoPago = item.TipoPago;
                            lista.Add(nuevoItem);
                        }
                    }
                    context.ParametroLiquidacionTercero.AddRange(lista);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedDeduccion(DataContext context)
        {
            Deduccion nuevoItem = null;
            List<Deduccion> lista = new List<Deduccion>();
            if (File.Exists("Data/SeedFiles/_Deduccion.json"))
            {
                var data = File.ReadAllText("Data/SeedFiles/_Deduccion.json");
                var items = JsonConvert.DeserializeObject<List<DeduccionDto>>(data);
                foreach (var item in items)
                {
                    var itemBD = obtenerDeduccion(context, item.Codigo);
                    if (itemBD == null)
                    {
                        nuevoItem = new Deduccion();
                        nuevoItem.Codigo = item.Codigo;
                        nuevoItem.Nombre = item.Nombre;
                        nuevoItem.Tarifa = item.Tarifa;
                        nuevoItem.Gmf = item.GmfDescripcion == "0" ? false : true;
                        nuevoItem.estado = item.Estado == "0" ? false : true;
                        var tipoBase = obtenerTipoBaseDeduccion(context, item.TipoBase);

                        if (tipoBase != null)
                            nuevoItem.TipoBaseDeduccionId = tipoBase.TipoBaseDeduccionId;

                        lista.Add(nuevoItem);
                    }
                }
                context.Deduccion.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedParametroGeneral(DataContext context)
        {
            ParametroGeneral nuevoItem = null;
            List<ParametroGeneral> lista = new List<ParametroGeneral>();
            if (!context.ParametroGeneral.Any())
            {
                if (File.Exists("Data/SeedFiles/_ParametroGeneral.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_ParametroGeneral.json");
                    var items = JsonConvert.DeserializeObject<List<ParametroGeneral>>(data);
                    foreach (var item in items)
                    {
                        var itemBD = obtenerParametroGeneral(context, item.Nombre);
                        if (itemBD == null)
                        {
                            nuevoItem = new ParametroGeneral();
                            nuevoItem.Nombre = item.Nombre;
                            nuevoItem.Descripcion = item.Descripcion;
                            nuevoItem.Valor = item.Valor;
                            nuevoItem.Tipo = item.Tipo;
                            lista.Add(nuevoItem);
                        }
                    }
                    context.ParametroGeneral.AddRange(lista);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedTipoBaseDeduccion(DataContext context)
        {
            TipoBaseDeduccion nuevoItem = null;
            if (File.Exists("Data/SeedFiles/_TipoBaseDeduccion.json"))
            {
                if (!context.TipoBaseDeduccion.Any())
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoBaseDeduccion.json");
                    var lista = JsonConvert.DeserializeObject<List<TipoBaseDeduccion>>(data);
                    foreach (var item in lista)
                    {
                        var itemBD = obtenerTipoBaseDeduccion(context, item.Nombre);
                        if (itemBD == null)
                        {
                            nuevoItem = new TipoBaseDeduccion();
                            nuevoItem.Nombre = item.Nombre;
                            context.TipoBaseDeduccion.Add(nuevoItem);
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        private static void SeedUsuario(DataContext context)
        {
            //if (!context.Usuario.Any())
            {
                if (File.Exists("Data/SeedFiles/_Usuario.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_Usuario.json");
                    var users = JsonConvert.DeserializeObject<List<Usuario>>(data);
                    foreach (var user in users)
                    {
                        var usuario = obtenerUsuario(context, user.Username);

                        if (usuario == null)
                        {
                            byte[] passwordHash, passwordSalt;
                            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

                            user.Username = user.Username.ToLower();
                            user.PasswordHash = passwordHash;
                            user.PasswordSalt = passwordSalt;
                            context.Usuario.Add(user);
                        }
                        else
                        {
                            byte[] passwordHash, passwordSalt;
                            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

                            usuario.PasswordHash = passwordHash;
                            usuario.PasswordSalt = passwordSalt;
                        }
                        context.SaveChanges();
                    }

                }
            }
        }
        private static void SeedRubroPresupuestal(DataContext context)
        {
            RubroPresupuestal rubro = null;
            RubroPresupuestal rubroPapa = null;

            if (!context.RubroPresupuestal.Any())
            {
                if (File.Exists("Data/SeedFiles/_RubroPresupuestalSeed.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_RubroPresupuestalSeed.json");
                    var items = JsonConvert.DeserializeObject<List<RubroPresupuestalDto>>(data);
                    foreach (var item in items)
                    {
                        var rubroBd = obtenerRubroPresupuestal(context, item.Identificacion);

                        if (rubroBd == null)
                        {
                            rubro = new RubroPresupuestal();

                            if (string.IsNullOrEmpty(item.IdentificacionPadre))
                            {
                                rubro.PadreRubroId = 0;
                            }
                            else
                            {
                                rubroPapa = obtenerRubroPresupuestal(context, item.IdentificacionPadre);
                                rubro.PadreRubroId = rubroPapa.RubroPresupuestalId;
                            }
                            rubro.Nombre = item.Nombre;
                            rubro.Identificacion = item.Identificacion;

                            context.RubroPresupuestal.Add(rubro);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }
        private static void SeedArea(DataContext context)
        {
            if (!context.Area.Any())
            {
                if (File.Exists("Data/SeedFiles/_AreaSeed.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_AreaSeed.json");
                    var items = JsonConvert.DeserializeObject<List<Area>>(data);
                    foreach (var item in items)
                    {
                        context.Area.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }
        private static void SeedCargo(DataContext context)
        {
            if (!context.Cargo.Any())
            {
                if (File.Exists("Data/SeedFiles/_CargoSeed.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_CargoSeed.json");
                    var items = JsonConvert.DeserializeObject<List<Cargo>>(data);
                    foreach (var item in items)
                    {
                        context.Cargo.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }
        private static void SeedTipoOperacion(DataContext context)
        {
            if (!context.TipoOperacion.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoOperacionSeed.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoOperacionSeed.json");
                    var items = JsonConvert.DeserializeObject<List<TipoOperacion>>(data);
                    foreach (var item in items)
                    {
                        context.TipoOperacion.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }
        private static void SeedTipoDetalleModificacion(DataContext context)
        {
            if (!context.TipoDetalleModificacion.Any())
            {
                if (File.Exists("Data/SeedFiles/_TipoDetalleModificacion.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_TipoDetalleModificacion.json");
                    var items = JsonConvert.DeserializeObject<List<TipoDetalleCDP>>(data);
                    foreach (var item in items)
                    {
                        context.TipoDetalleModificacion.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }

        private static void SeedTercero(DataContext context)
        {
            if (!context.Tercero.Any())
            {
                if (File.Exists("Data/SeedFiles/_Tercero.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_Tercero.json");
                    var items = JsonConvert.DeserializeObject<List<Tercero>>(data);
                    foreach (var item in items)
                    {
                        context.Tercero.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }

        //La tabla estado puede tener nueva información
        private static void SeedEstado(DataContext context)
        {
            Estado nuevoEstado = null;
            List<Estado> lista = new List<Estado>();
            //if (!context.Estado.Any())
            {
                if (File.Exists("Data/SeedFiles/_Estado.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_Estado.json");
                    var items = JsonConvert.DeserializeObject<List<Estado>>(data);
                    foreach (var item in items)
                    {
                        var estado = obtenerEstado(context, item.Nombre, item.TipoDocumento);
                        if (estado == null)
                        {
                            nuevoEstado = new Estado();
                            nuevoEstado.Nombre = item.Nombre;
                            nuevoEstado.Descripcion = item.Descripcion;
                            nuevoEstado.TipoDocumento = item.TipoDocumento;
                            lista.Add(nuevoEstado);
                        }
                    }
                    context.Estado.AddRange(lista);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedActividadGeneral(DataContext context)
        {
            if (!context.ActividadGeneral.Any())
            {
                if (File.Exists("Data/SeedFiles/_ActividadGeneral.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_ActividadGeneral.json");
                    var items = JsonConvert.DeserializeObject<List<ActividadGeneral>>(data);
                    foreach (var item in items)
                    {
                        context.ActividadGeneral.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }

        private static void SeedActividadEspecifica(DataContext context)
        {
            var listaActEspecifica = new List<ActividadEspecifica>();
            ActividadEspecifica actividad = null;

            if (!context.ActividadEspecifica.Any())
            {
                if (File.Exists("Data/SeedFiles/_ActividadEspecifica.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_ActividadEspecifica.json");
                    var items = JsonConvert.DeserializeObject<List<ActividadEspecificaDto>>(data);
                    foreach (var item in items)
                    {

                        actividad = new ActividadEspecifica();
                        actividad.ActividadGeneral = obtenerActividadGeneral(context, item.ActividadGeneral);
                        actividad.RubroPresupuestal = obtenerRubroPresupuestal(context, item.RubroPresupuestal);
                        actividad.Nombre = item.Nombre;
                        actividad.ValorApropiacionVigente = item.ValorApropiacionVigente;
                        actividad.SaldoPorProgramar = item.SaldoPorProgramar;
                        listaActEspecifica.Add(actividad);
                    }
                    context.ActividadEspecifica.AddRange(listaActEspecifica);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedDependencia(DataContext context)
        {
            var listaDependencia = new List<Dependencia>();
            Dependencia dependencia = null;

            if (!context.Dependencia.Any())
            {
                if (File.Exists("Data/SeedFiles/_Dependencia.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_Dependencia.json");
                    var items = JsonConvert.DeserializeObject<List<DependenciaDto>>(data);
                    foreach (var item in items)
                    {
                        dependencia = new Dependencia();
                        dependencia.Area = obtenerArea(context, item.Area);
                        dependencia.Nombre = item.Nombre;
                        listaDependencia.Add(dependencia);
                    }
                    context.Dependencia.AddRange(listaDependencia);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedUsoPresupuestal(DataContext context)
        {
            var lista = new List<UsoPresupuestal>();
            UsoPresupuestal uso = null;

            if (!context.UsoPresupuestal.Any())
            {
                if (File.Exists("Data/SeedFiles/_UsoPresupuestal.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_UsoPresupuestal.json");
                    var items = JsonConvert.DeserializeObject<List<UsoPresupuestalDto>>(data);
                    foreach (var item in items)
                    {
                        uso = new UsoPresupuestal();
                        uso.RubroPresupuestal = obtenerRubroPresupuestal(context, item.RubroPresupuestal);
                        uso.Nombre = item.Nombre;
                        uso.Identificacion = item.Identificacion;
                        uso.MarcaAusteridad = item.MarcaAusteridad == "NO" ? false : true;
                        lista.Add(uso);
                    }
                    context.UsoPresupuestal.AddRange(lista);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any())
            {
                if (File.Exists("Data/SeedFiles/_UserSeedData.json"))
                {
                    var userData = System.IO.File.ReadAllText("Data/SeedFiles/_UserSeedData.json");
                    var users = JsonConvert.DeserializeObject<List<User>>(userData);

                    foreach (var user in users)
                    {
                        byte[] passwordHash, passwordSalt;
                        CreatePasswordHash("veritas", out passwordHash, out passwordSalt);

                        user.Username = user.Username.ToLower();
                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                        context.Users.Add(user);
                    }
                    context.SaveChanges();
                }
            }
        }

        //La tabla perfil puede tener nueva información
        private static void SeedPerfil(DataContext context)
        {
            Perfil nuevo = null;
            List<Perfil> lista = new List<Perfil>();
            if (File.Exists("Data/SeedFiles/_Perfil.json"))
            {
                var data = File.ReadAllText("Data/SeedFiles/_Perfil.json");
                var items = JsonConvert.DeserializeObject<List<Perfil>>(data);
                foreach (var item in items)
                {
                    var perfil = obtenerPerfil(context, item.Nombre);
                    if (perfil == null)
                    {
                        nuevo = new Perfil();
                        nuevo.Codigo = item.Codigo;
                        nuevo.Nombre = item.Nombre;
                        nuevo.Descripcion = item.Descripcion;
                        nuevo.Estado = true;
                        lista.Add(nuevo);
                    }
                }
                context.Perfil.AddRange(lista);
                context.SaveChanges();
            }
        }

        //La tabla transaccion puede tener nueva información
        private static void SeedTransaccion(DataContext context)
        {
            Transaccion tran = null;
            Transaccion tranPapa = null;
            
            if (!context.Transaccion.Any())
            {
                if (File.Exists("Data/SeedFiles/_Transaccion.json"))
                {
                    var data = File.ReadAllText("Data/SeedFiles/_Transaccion.json");
                    var items = JsonConvert.DeserializeObject<List<TransaccionDto>>(data);
                    foreach (var item in items)
                    {
                        var tranBD = obtenerTransaccion(context, item.Codigo);
                        if (tranBD == null)
                        {
                            tran = new Transaccion();
                            tran.Codigo = item.Codigo;
                            tran.Nombre = item.Nombre;
                            tran.Descripcion = item.Descripcion;
                            tran.Icono = item.Icono;
                            tran.Ruta = item.Ruta;
                            tran.Estado = true;
                            if (string.IsNullOrEmpty(item.CodigoPadreTransaccion))
                            {
                                tran.PadreTransaccionId = 0;
                            }
                            else
                            {
                                tranPapa = obtenerTransaccion(context, item.CodigoPadreTransaccion);
                                tran.PadreTransaccionId = tranPapa.TransaccionId;
                            }
                            context.Transaccion.Add(tran);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        //La tabla perfil puede tener nueva información
        private static void SeedPerfilTransaccion(DataContext context)
        {
            PerfilTransaccion nuevo = null;
            List<PerfilTransaccion> lista = new List<PerfilTransaccion>();
            if (File.Exists("Data/SeedFiles/_PerfilTransaccion.json"))
            {
                var data = File.ReadAllText("Data/SeedFiles/_PerfilTransaccion.json");
                var items = JsonConvert.DeserializeObject<List<PerfilTransaccionDto>>(data);
                foreach (var item in items)
                {
                    var perfil = obtenerPerfil(context, item.Perfil);
                    var transaccion = obtenerTransaccion(context, item.Transaccion);

                    if (perfil != null && transaccion != null)
                    {
                        nuevo = new PerfilTransaccion();
                        nuevo.PerfilId = perfil.PerfilId;
                        nuevo.TransaccionId = transaccion.TransaccionId;

                        var perfilTransaccionBD = context.PerfilTransaccion
                                                    .Where(p => p.PerfilId == nuevo.PerfilId
                                                            && p.TransaccionId == nuevo.TransaccionId)
                                                    .FirstOrDefault();

                        if (perfilTransaccionBD == null)
                        {
                            lista.Add(nuevo);
                        }
                    }
                }
                context.PerfilTransaccion.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedUsuarioPerfil(DataContext context)
        {
            UsuarioPerfil nuevo = null;
            List<UsuarioPerfil> lista = new List<UsuarioPerfil>();
            if (File.Exists("Data/SeedFiles/_UsuarioPerfil.json"))
            {
                var data = File.ReadAllText("Data/SeedFiles/_UsuarioPerfil.json");
                var items = JsonConvert.DeserializeObject<List<UsuarioPerfilDto>>(data);
                foreach (var item in items)
                {
                    var usuario = obtenerUsuario(context, item.Usuario.ToLower());
                    var perfil = obtenerPerfil(context, item.Perfil.ToLower());

                    if (perfil != null && usuario != null)
                    {
                        nuevo = new UsuarioPerfil();
                        nuevo.PerfilId = perfil.PerfilId;
                        nuevo.UsuarioId = usuario.UsuarioId;

                        var usuarioPerfilBD = context.UsuarioPerfil
                                                .Where(p => p.PerfilId == nuevo.PerfilId
                                                        && p.UsuarioId == nuevo.UsuarioId)
                                                .FirstOrDefault();

                        if (usuarioPerfilBD == null)
                        {
                            lista.Add(nuevo);
                        }
                    }
                }
                context.UsuarioPerfil.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static ActividadGeneral obtenerActividadGeneral(DataContext context, string nombre)
        {
            var actividad = (from ag in context.ActividadGeneral
                             join rp in context.RubroPresupuestal on ag.RubroPresupuestalId equals rp.RubroPresupuestalId
                             where (rp.Nombre.Trim().ToUpper() == nombre.Trim().ToUpper())
                             select ag).FirstOrDefault();
            return actividad;
        }

        private static RubroPresupuestal obtenerRubroPresupuestal(DataContext context, string Identificacion)
        {
            return context.RubroPresupuestal.Where(x => x.Identificacion == Identificacion).FirstOrDefault();
        }

        private static Pci obtenerPci(DataContext context, string Identificacion)
        {
            return context.Pci.Where(x => x.Identificacion == Identificacion).FirstOrDefault();
        }

        private static ActividadEconomica obtenerActividadEconomica(DataContext context, string codigo)
        {
            return context.ActividadEconomica.Where(x => x.Codigo == codigo).FirstOrDefault();
        }

        private static Area obtenerArea(DataContext context, string nombre)
        {
            return context.Area.Where(x => x.Nombre.ToLower() == nombre.ToLower()).FirstOrDefault();
        }

        private static Estado obtenerEstado(DataContext context, string nombre, string tipoDocumento)
        {
            return context.Estado.Where(x => x.Nombre.ToLower() == nombre.ToLower()
                                        && x.TipoDocumento.ToLower() == tipoDocumento).FirstOrDefault();
        }

        private static Perfil obtenerPerfil(DataContext context, string nombre)
        {
            return context.Perfil.Where(x => x.Nombre.ToLower() == nombre.ToLower()).FirstOrDefault();
        }

        private static Transaccion obtenerTransaccion(DataContext context, string codigo)
        {
            return context.Transaccion.Where(x => x.Codigo.ToLower() == codigo.ToLower()).FirstOrDefault();
        }

        private static Usuario obtenerUsuario(DataContext context, string username)
        {
            return context.Usuario.Where(x => x.Username.ToLower() == username.ToLower()).FirstOrDefault();
        }

        private static ParametroGeneral obtenerParametroGeneral(DataContext context, string nombre)
        {
            return context.ParametroGeneral.Where(x => x.Nombre == nombre).FirstOrDefault();
        }

        private static Deduccion obtenerDeduccion(DataContext context, string codigo)
        {
            return context.Deduccion.Where(x => x.Codigo == codigo).FirstOrDefault();
        }

        private static TipoBaseDeduccion obtenerTipoBaseDeduccion(DataContext context, string nombre)
        {
            return context.TipoBaseDeduccion.Where(x => x.Nombre == nombre).FirstOrDefault();
        }

        private static Tercero obtenerTercero(DataContext context, int tipoIdentificacion, string identificacion)
        {
            return context.Tercero.Where(x => x.TipoIdentificacion == tipoIdentificacion && x.NumeroIdentificacion == identificacion).FirstOrDefault();
        }

        private static TerceroDeduccion obtenerTerceroDeduccion(DataContext context, int terceroId, int deduccionId)
        {
            return context.TerceroDeducciones.Where(x => x.TerceroId == terceroId && x.DeduccionId == deduccionId).FirstOrDefault();
        }

        private static ParametroSistema obtenerParametroSistema(DataContext context, string nombre)
        {
            return context.ParametroSistema.Where(x => x.Nombre.ToLower() == nombre.ToLower()).FirstOrDefault();
        }

        private static string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }



    }
}