using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using Newtonsoft.Json;

namespace ComplementApp.API.Data
{
    public class Seed
    {
        public static void CargarDataInicial(DataContext context)
        {
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
        }

        private static void SeedAtributoContable(DataContext context)
        {
            AtributoContable valor = null;
            List<AtributoContable> lista = new List<AtributoContable>();

            if (!context.AtributoContable.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_AtributoContable.json");
                var items = JsonConvert.DeserializeObject<List<AtributoContable>>(data);
                foreach (var item in items)
                {
                    valor = new AtributoContable();
                    valor.Nombre = item.Nombre;
                    valor.Codigo = item.Codigo;
                    lista.Add(valor);
                }
                context.AtributoContable.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedRecursoPresupuestal(DataContext context)
        {
            RecursoPresupuestal valor = null;
            List<RecursoPresupuestal> lista = new List<RecursoPresupuestal>();

            if (!context.RecursoPresupuestal.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_RecursoPresupuestal.json");
                var items = JsonConvert.DeserializeObject<List<RecursoPresupuestal>>(data);
                foreach (var item in items)
                {
                    valor = new RecursoPresupuestal();
                    valor.Nombre = item.Nombre;
                    valor.Codigo = item.Codigo;
                    lista.Add(valor);
                }
                context.RecursoPresupuestal.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedTipoGasto(DataContext context)
        {
            TipoGasto valor = null;
            List<TipoGasto> lista = new List<TipoGasto>();

            if (!context.TipoGasto.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_TipoGasto.json");
                var items = JsonConvert.DeserializeObject<List<TipoGasto>>(data);
                foreach (var item in items)
                {
                    valor = new TipoGasto();
                    valor.Nombre = item.Nombre;
                    valor.Codigo = item.Codigo;
                    lista.Add(valor);
                }
                context.TipoGasto.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedSituacionFondo(DataContext context)
        {
            SituacionFondo valor = null;
            List<SituacionFondo> lista = new List<SituacionFondo>();

            if (!context.SituacionFondo.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_SituacionFondo.json");
                var items = JsonConvert.DeserializeObject<List<SituacionFondo>>(data);
                foreach (var item in items)
                {
                    valor = new SituacionFondo();
                    valor.Nombre = item.Nombre;
                    valor.Codigo = item.Codigo;
                    lista.Add(valor);
                }
                context.SituacionFondo.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedFuenteFinanciacion(DataContext context)
        {
            FuenteFinanciacion valor = null;
            List<FuenteFinanciacion> lista = new List<FuenteFinanciacion>();

            if (!context.FuenteFinanciacion.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_FuenteFinanciacion.json");
                var items = JsonConvert.DeserializeObject<List<FuenteFinanciacion>>(data);
                foreach (var item in items)
                {
                    valor = new FuenteFinanciacion();
                    valor.Nombre = item.Nombre;
                    valor.Codigo = item.Codigo;
                    lista.Add(valor);
                }
                context.FuenteFinanciacion.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedActividadEconomica(DataContext context)
        {
            ActividadEconomica actividad = null;
            List<ActividadEconomica> lista = new List<ActividadEconomica>();

            if (!context.ActividadEconomica.Any())
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

        private static void SeedCriterioCalculoReteFuente(DataContext context)
        {
            if (!context.CriterioCalculoReteFuente.Any())
            {
                CriterioCalculoReteFuente nuevoItem = null;
                List<CriterioCalculoReteFuente> lista = new List<CriterioCalculoReteFuente>();

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

        private static void SeedTerceroDeducciones(DataContext context)
        {
            if (!context.TerceroDeducciones.Any())
            {
                TerceroDeduccion nuevoItem = null;
                List<TerceroDeduccion> lista = new List<TerceroDeduccion>();

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

        private static void SeedParametroLiquidacionTercero(DataContext context)
        {
            if (!context.ParametroLiquidacionTercero.Any())
            {
                ParametroLiquidacionTercero nuevoItem = null;
                List<ParametroLiquidacionTercero> lista = new List<ParametroLiquidacionTercero>();
                DateTime fecha;

                var data = File.ReadAllText("Data/SeedFiles/_ParametroLiquidacionTercero.json");
                var items = JsonConvert.DeserializeObject<List<ParametroLiquidacionTerceroDto>>(data);
                foreach (var item in items)
                {
                    var terceroBD = obtenerTercero(context, item.TipoIdentificacion, item.IdentificacionTercero);
                    if (terceroBD != null)
                    {
                        nuevoItem = new ParametroLiquidacionTercero();
                        nuevoItem.TerceroId = terceroBD.TerceroId;
                        nuevoItem.Afc = item.Afc;
                        nuevoItem.AportePension = item.AportePension;
                        nuevoItem.AporteSalud = item.AporteSalud;
                        nuevoItem.BaseAporteSalud = item.BaseAporteSalud;
                        nuevoItem.ConvenioFontic = item.ConvenioFontic;
                        nuevoItem.Credito = item.Credito;
                        nuevoItem.Debito = item.Debito;
                        nuevoItem.Dependiente = item.Dependiente;

                        if (!string.IsNullOrEmpty(item.FechaFinalDescuentoInteresVivienda))
                        {
                            if (DateTime.TryParse(item.FechaFinalDescuentoInteresVivienda, out fecha))
                                nuevoItem.FechaFinalDescuentoInteresVivienda = fecha;
                        }

                        if (!string.IsNullOrEmpty(item.FechaInicioDescuentoInteresVivienda))
                            if (DateTime.TryParse(item.FechaInicioDescuentoInteresVivienda, out fecha))
                                nuevoItem.FechaInicioDescuentoInteresVivienda = fecha;

                        nuevoItem.FondoSolidaridad = item.FondoSolidaridad;
                        nuevoItem.HonorarioSinIva = item.HonorarioSinIva;
                        nuevoItem.InteresVivienda = item.InteresVivienda;
                        nuevoItem.MedicinaPrepagada = item.MedicinaPrepagada;
                        nuevoItem.ModalidadContrato = item.ModalidadContrato;
                        nuevoItem.NumeroCuenta = item.NumeroCuenta;
                        nuevoItem.PensionVoluntaria = item.PensionVoluntaria;
                        nuevoItem.RiesgoLaboral = item.RiesgoLaboral;
                        nuevoItem.TarifaIva = item.TarifaIva;
                        nuevoItem.TipoCuenta = item.TipoCuenta;
                        nuevoItem.TipoCuentaPorPagar = item.TipoCuentaPorPagar;
                        nuevoItem.TipoDocumentoSoporte = item.TipoDocumentoSoporte;
                        nuevoItem.TipoIva = item.TipoIva;
                        nuevoItem.TipoPago = item.TipoPago;
                        lista.Add(nuevoItem);
                    }
                }
                context.ParametroLiquidacionTercero.AddRange(lista);
                context.SaveChanges();
            }
        }

        private static void SeedDeduccion(DataContext context)
        {
            Deduccion nuevoItem = null;
            List<Deduccion> lista = new List<Deduccion>();

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

        private static void SeedParametroGeneral(DataContext context)
        {
            ParametroGeneral nuevoItem = null;
            List<ParametroGeneral> lista = new List<ParametroGeneral>();

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
                    lista.Add(nuevoItem);
                }
            }
            context.ParametroGeneral.AddRange(lista);
            context.SaveChanges();
        }

        private static void SeedTipoBaseDeduccion(DataContext context)
        {
            TipoBaseDeduccion nuevoItem = null;
            //if (!context.TipoBaseDeduccion.Any())
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

        private static void SeedUsuario(DataContext context)
        {
            if (!context.Usuario.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_UsuarioSeed.json");
                var users = JsonConvert.DeserializeObject<List<Usuario>>(data);
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

                    user.Username = user.Username.ToLower();
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    context.Usuario.Add(user);
                }
                context.SaveChanges();
            }
        }
        private static void SeedRubroPresupuestal(DataContext context)
        {
            RubroPresupuestal rubro = null;
            RubroPresupuestal rubroPapa = null;

            //if (!context.RubroPresupuestal.Any())
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
        private static void SeedArea(DataContext context)
        {
            if (!context.Area.Any())
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
        private static void SeedCargo(DataContext context)
        {
            if (!context.Cargo.Any())
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
        private static void SeedTipoOperacion(DataContext context)
        {
            if (!context.TipoOperacion.Any())
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
        private static void SeedTipoDetalleModificacion(DataContext context)
        {
            if (!context.TipoDetalleModificacion.Any())
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

        private static void SeedTercero(DataContext context)
        {
            if (!context.Tercero.Any())
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

        //La tabla estado puede tener nueva información
        private static void SeedEstado(DataContext context)
        {
            Estado nuevoEstado = null;
            List<Estado> lista = new List<Estado>();

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

        private static void SeedActividadGeneral(DataContext context)
        {
            if (!context.ActividadGeneral.Any())
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

        private static void SeedActividadEspecifica(DataContext context)
        {
            var listaActEspecifica = new List<ActividadEspecifica>();
            ActividadEspecifica actividad = null;

            if (!context.ActividadEspecifica.Any())
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

        private static void SeedDependencia(DataContext context)
        {
            var listaDependencia = new List<Dependencia>();
            Dependencia dependencia = null;

            if (!context.Dependencia.Any())
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

        private static void SeedUsoPresupuestal(DataContext context)
        {
            var lista = new List<UsoPresupuestal>();
            UsoPresupuestal uso = null;

            if (!context.UsoPresupuestal.Any())
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

        private static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any())
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

        //La tabla perfil puede tener nueva información
        private static void SeedPerfil(DataContext context)
        {
            Perfil nuevo = null;
            List<Perfil> lista = new List<Perfil>();

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

        //La tabla transaccion puede tener nueva información
        private static void SeedTransaccion(DataContext context)
        {
            Transaccion tran = null;
            Transaccion tranPapa = null;

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

        //La tabla perfil puede tener nueva información
        private static void SeedPerfilTransaccion(DataContext context)
        {
            PerfilTransaccion nuevo = null;
            List<PerfilTransaccion> lista = new List<PerfilTransaccion>();

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


        private static void SeedUsuarioPerfil(DataContext context)
        {
            UsuarioPerfil nuevo = null;
            List<UsuarioPerfil> lista = new List<UsuarioPerfil>();

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
            return context.ActividadGeneral.Where(x => x.Nombre == nombre).FirstOrDefault();
        }

        private static RubroPresupuestal obtenerRubroPresupuestal(DataContext context, string Identificacion)
        {
            return context.RubroPresupuestal.Where(x => x.Identificacion == Identificacion).FirstOrDefault();
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
    }
}