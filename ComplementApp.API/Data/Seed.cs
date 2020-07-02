using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ComplementApp.API.Models;
using Newtonsoft.Json;

namespace ComplementApp.API.Data
{
    public class Seed
    {
        public static void SeedUsuario(DataContext context)
        {
            if (!context.Usuario.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_UsuarioSeed.json");
                var users = JsonConvert.DeserializeObject<List<Usuario>>(data);
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("veritas", out passwordHash, out passwordSalt);

                    user.Username = user.Username.ToLower();
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    context.Usuario.Add(user);
                }
                context.SaveChanges();
            }
        }
        public static void SeedRubroPresupuestal(DataContext context)
        {
            if (!context.RubroPresupuestal.Any())
            {
                var data = File.ReadAllText("Data/SeedFiles/_RubroPresupuestalSeed.json");
                var items = JsonConvert.DeserializeObject<List<RubroPresupuestal>>(data);
                foreach (var item in items)
                {
                    context.RubroPresupuestal.Add(item);
                }
                context.SaveChanges();
            }
        }
        public static void SeedArea(DataContext context)
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
        public static void SeedCargo(DataContext context)
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
        public static void SeedTipoOperacion(DataContext context)
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

        public static void SeedUsers(DataContext context)
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

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

    }
}