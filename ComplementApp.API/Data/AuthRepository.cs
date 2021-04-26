using System.Text;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Interfaces;
using System.Security.Cryptography;
using System.IO;
using System;

namespace ComplementApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private const string parametroFecha = "fechasistema";
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeneralInterface _generalInterface;


        public AuthRepository(DataContext context, IUnitOfWork unitOfWork, IGeneralInterface generalInterface)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _generalInterface = generalInterface;

        }
        public async Task<Usuario> Login(string username, string password)
        {
            var user = await _context.Usuario.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task<bool> ValidateDate()
        {
            DateTime? fechaRegistrada = null;
            var parameter = await _context.ParametroSistema.FirstOrDefaultAsync(x => x.Nombre.ToLower() == parametroFecha.ToLower());
            var fechaSistema = _generalInterface.ObtenerFechaHoraActual();

            if (parameter == null)
                return false;

            var valor = Decrypt(parameter.Valor);

            if (string.IsNullOrEmpty(valor))
                return false;

            fechaRegistrada = Convert.ToDateTime(valor);
            fechaRegistrada = fechaRegistrada.Value.Date.AddDays(1);

            if (fechaSistema >= fechaRegistrada)
                return false;

            return true;
        }



        public async Task<Usuario> Register(Usuario user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Usuario.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Usuario.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }

        private static string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}