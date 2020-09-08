using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                var estado = obtenerEstado(context, item.Nombre);
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
                    if (string.IsNullOrEmpty(item.PadreMenu))
                    {
                        tran.PadreTransaccionId = 0;
                    }
                    else
                    {
                        tranPapa = obtenerTransaccion(context, item.PadreMenu);
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

        private static Area obtenerArea(DataContext context, string nombre)
        {
            return context.Area.Where(x => x.Nombre.ToLower() == nombre.ToLower()).FirstOrDefault();
        }

        private static Estado obtenerEstado(DataContext context, string nombre)
        {
            return context.Estado.Where(x => x.Nombre.ToLower() == nombre.ToLower()).FirstOrDefault();
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

    }
}