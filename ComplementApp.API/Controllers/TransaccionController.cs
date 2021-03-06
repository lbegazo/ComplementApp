using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Data;
using ComplementApp.API.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    public class TransaccionController : BaseApiController
    {
        private readonly DataContext _dataContext;

        private readonly ITransaccionRepository _transaccionRepository;

        public TransaccionController(ITransaccionRepository transaccionRepository, DataContext dataContext)
        {
            _transaccionRepository = transaccionRepository;
            _dataContext = dataContext;
        }

        [Route("[action]/{idUsuarioLogueado}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaTransaccionXUsuario(int idUsuarioLogueado)
        {
            var transacciones = await _transaccionRepository.ObtenerListaTransaccionXUsuario(idUsuarioLogueado);
            return Ok(transacciones);
        }

        [Route("[action]/{codigoTransaccion}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTransaccionXCodigo(string codigoTransaccion)
        {
            var transacciones = await _transaccionRepository.ObtenerTransaccionXCodigo(codigoTransaccion);
            return Ok(transacciones);
        }

        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> ActualizarPerfil([FromQuery(Name = "perfilId")] int perfilId,
                                                            [FromQuery(Name = "listaTransaccionId")] string listaTransaccionId)
        {
            List<int> listIds = new List<int>();
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (listaTransaccionId.Length > 0)
                {
                    listIds = listaTransaccionId.Split(',').Select(int.Parse).ToList();
                }

                _transaccionRepository.EliminarTransaccionesXPerfil(perfilId);
                _dataContext.SaveChanges();

                _transaccionRepository.InsertarTransaccionesXPerfil(perfilId, listIds);
                _dataContext.SaveChanges();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(perfilId);
        }

    }
}