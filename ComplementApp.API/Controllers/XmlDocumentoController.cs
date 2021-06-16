using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ComplementApp.API.Dtos;
using System.Xml.Serialization;
using Compromiso = ComplementApp.API.Dtos.Serializacion.Compromiso;
using Obligacion = ComplementApp.API.Dtos.Serializacion.Obligacion;
using OrdenPago = ComplementApp.API.Dtos.Serializacion.OrdenPago;
using ComplementApp.API.Models;
using ComplementApp.API.Interfaces;
using Cdp = ComplementApp.API.Dtos.Serializacion.CDP;

namespace ComplementApp.API.Controllers
{
   
    public class XmlDocumentoController : BaseApiController
    {
        #region Propiedades
        private readonly IDocumentoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        private IConfiguration _configuration { get; }

        #endregion Propiedades

        public XmlDocumentoController(IUnitOfWork unitOfWork, IDocumentoRepository repo,
                            IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = repo;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("upload")]
        public IActionResult CargarInformacionXML()
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files[0];

                    if (file == null || file.Length <= 0)
                        return BadRequest("El archivo se encuentra vacío");

                    if (!Path.GetExtension(file.FileName).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                        return BadRequest("El archivo no es soportado, el archivo debe tener la extensión: xml");

                    if (file.FileName.Contains("CDP"))
                    {
                        #region Registrar CDP

                        EliminarInformacionCDPXInstancia((int)TipoDocumento.Cdp);

                        Cdp.tipoCDP listaCDP = LeerInfomacionDelArchivoCDP(file);

                        List<CDPDto> listaDetalle = obtenerListaCDP(listaCDP);

                        var EsCabeceraCorrecto = _repo.InsertaCabeceraCDPConTercero(listaDetalle);

                        if (!EsCabeceraCorrecto)
                            throw new ArgumentException("No se pudo registrar los CDP");

                        #endregion Registrar CDP
                    }
                    else if (file.FileName.Contains("COMPROMISO"))
                    {
                        #region Registrar Compromiso

                        EliminarInformacionCDPXInstancia((int)TipoDocumento.Compromiso);

                        Compromiso.tipoCompromisoPptal lista = LeerInfomacionDelArchivoCompromiso(file);

                        List<CDPDto> listaDetalle = obtenerListaCompromiso(lista);

                        var EsCabeceraCorrecto = _repo.InsertaCabeceraCDPConTercero(listaDetalle);

                        if (!EsCabeceraCorrecto)
                            throw new ArgumentException("No se pudo registrar los Compromisos");

                        #endregion Registrar Compromiso
                    }
                    else if (file.FileName.Contains("OBLIGACION"))
                    {
                        #region Registrar Obligacion

                        EliminarInformacionCDPXInstancia((int)TipoDocumento.Obligacion);

                        Obligacion.tipoObligacionPptal lista = LeerInfomacionDelArchivoObligacion(file);

                        List<CDPDto> listaDetalle = obtenerListaObligacion(lista);

                        var EsCabeceraCorrecto = _repo.InsertaCabeceraCDPConTercero(listaDetalle);

                        if (!EsCabeceraCorrecto)
                            throw new ArgumentException("No se pudo registrar las obligaciones");

                        #endregion Registrar Obligacion
                    }
                    else if (file.FileName.Contains("PAGO"))
                    {
                        #region Registrar Obligacion

                        EliminarInformacionCDPXInstancia((int)TipoDocumento.OrdenPago);

                        OrdenPago.tipoPagoTercero lista = LeerInfomacionDelArchivoOrdenPago(file);

                        List<CDPDto> listaDetalle = obtenerListaOrdenPago(lista);

                        var EsCabeceraCorrecto = _repo.InsertaCabeceraCDPConTercero(listaDetalle);

                        if (!EsCabeceraCorrecto)
                            throw new ArgumentException("No se pudo registrar las ordenes de pago");

                        #endregion Registrar Obligacion
                    }

                }
                else
                {
                    return BadRequest("El archivo no pudo ser enviado al servidor web");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }

        private Cdp.tipoCDP LeerInfomacionDelArchivoCDP(IFormFile file)
        {
            DataTable dtDetalle = new DataTable();
            Cdp.tipoCDP tipoCdp = null;
            string contenido = string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(Cdp.tipoCDP));
            try
            {
                contenido = ReadFormFile(file);
                using (StringReader sr = new StringReader(contenido))
                {
                    tipoCdp = (Cdp.tipoCDP)serializer.Deserialize(sr);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tipoCdp;
        }

        private Compromiso.tipoCompromisoPptal LeerInfomacionDelArchivoCompromiso(IFormFile file)
        {
            DataTable dtDetalle = new DataTable();
            Compromiso.tipoCompromisoPptal tipoCdp = null;
            string contenido = string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(Compromiso.tipoCompromisoPptal));
            try
            {
                contenido = ReadFormFile(file);
                using (StringReader sr = new StringReader(contenido))
                {
                    tipoCdp = (Compromiso.tipoCompromisoPptal)serializer.Deserialize(sr);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tipoCdp;
        }

        private Obligacion.tipoObligacionPptal LeerInfomacionDelArchivoObligacion(IFormFile file)
        {
            DataTable dtDetalle = new DataTable();
            Obligacion.tipoObligacionPptal tipoCdp = null;
            string contenido = string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(Obligacion.tipoObligacionPptal));
            try
            {
                contenido = ReadFormFile(file);
                using (StringReader sr = new StringReader(contenido))
                {
                    tipoCdp = (Obligacion.tipoObligacionPptal)serializer.Deserialize(sr);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tipoCdp;
        }

        private OrdenPago.tipoPagoTercero LeerInfomacionDelArchivoOrdenPago(IFormFile file)
        {
            DataTable dtDetalle = new DataTable();
            OrdenPago.tipoPagoTercero tipoCdp = null;
            string contenido = string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(OrdenPago.tipoPagoTercero));
            try
            {
                contenido = ReadFormFile(file);
                using (StringReader sr = new StringReader(contenido))
                {
                    tipoCdp = (OrdenPago.tipoPagoTercero)serializer.Deserialize(sr);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tipoCdp;
        }

        private static string ReadFormFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private bool EliminarInformacionCDPXInstancia(int instancia)
        {
            var resultado = false;
            resultado = this._repo.EliminarCabeceraCDPXInstancia(instancia);

            return resultado;
        }

        private List<CDPDto> obtenerListaCDP(Cdp.tipoCDP dtDetalle)
        {
            CDPDto documento = null;
            List<CDPDto> listaDocumento = new List<CDPDto>();

            foreach (var item in dtDetalle.listaItemCDP)
            {
                foreach (var itemAfectacion in item.listaItemAfectacion)
                {
                    documento = new CDPDto();
                    documento.Cdp = item.codCDP;
                    documento.Fecha = item.fechaRegistro;
                    documento.Instancia = (int)TipoDocumento.Cdp;
                    documento.Detalle1 = item.estado;
                    documento.Detalle4 = item.objeto;

                    documento.ValorInicial = itemAfectacion.valorInicial;
                    documento.ValorTotal = itemAfectacion.valorActual;
                    documento.SaldoActual = itemAfectacion.saldoComp;
                    documento.Operacion = itemAfectacion.valorOperacion;
                    documento.Detalle2 = itemAfectacion.codDepAfectacion;
                    documento.Detalle3 = itemAfectacion.nomDepAfectacion;
                    documento.IdentificacionRubro = itemAfectacion.codPosicionGasto;
                }
                listaDocumento.Add(documento);
            }
            return listaDocumento;
        }

        private List<CDPDto> obtenerListaCompromiso(Compromiso.tipoCompromisoPptal dtDetalle)
        {
            int tipoDocumentoId = 0;
            CDPDto documento = null;
            List<CDPDto> listaDocumento = new List<CDPDto>();

            foreach (var item in dtDetalle.listaItemCompromiso)
            {
                foreach (var itemAfectacion in item.listaItemAfectacion)
                {
                    documento = new CDPDto();
                    documento.Cdp = item.codCDP;
                    documento.Crp = item.codCompromiso;
                    documento.Fecha = item.fechaRegistro;
                    documento.Instancia = (int)TipoDocumento.Compromiso;
                    documento.Detalle1 = item.estado;
                    documento.Detalle4 = item.objeto;

                    if (item.terceroBeneficiario != null)
                    {
                        if (Int32.TryParse(item.terceroBeneficiario.codTipoDocumento, out tipoDocumentoId))
                        {
                            documento.TipoIdentificacionTercero = tipoDocumentoId;
                        }
                        documento.NumeroIdentificacionTercero = item.terceroBeneficiario.numDocIdentidad;
                        documento.NombreTercero = item.terceroBeneficiario.nomTercero;
                    }

                    if (item.datosAdmin != null)
                    {
                        documento.Detalle6 = item.datosAdmin.numDocSoporte;
                        documento.Detalle7 = item.datosAdmin.nomTipoDocSoporte;
                    }

                    documento.ValorInicial = itemAfectacion.valorInicial;
                    documento.ValorTotal = itemAfectacion.valorActual;
                    documento.SaldoActual = itemAfectacion.saldoUtilizar;
                    documento.Operacion = itemAfectacion.valorOperaciones;
                    documento.Detalle2 = itemAfectacion.codDepAfectacion;
                    documento.Detalle3 = itemAfectacion.nomDepAfectacion;
                    documento.IdentificacionRubro = itemAfectacion.codPosicionGasto;
                    documento.NombreRubro = itemAfectacion.nomPosicionGasto;

                    listaDocumento.Add(documento);
                }
            }
            return listaDocumento;
        }

        private List<CDPDto> obtenerListaObligacion(Obligacion.tipoObligacionPptal dtDetalle)
        {
            int tipoDocumentoId = 0;
            CDPDto documento = null;
            List<CDPDto> listaDocumento = new List<CDPDto>();

            foreach (var item in dtDetalle.listaItemObligacion)
            {
                foreach (var itemAfectacion in item.listaItemAfectacion)
                {
                    documento = new CDPDto();
                    documento.Obligacion = item.codObligacion;
                    documento.Fecha = item.fechaRegistro;
                    documento.Instancia = (int)TipoDocumento.Obligacion;
                    documento.Detalle1 = item.estado;


                    if (item.docRelacion != null)
                    {
                        documento.Cdp = item.docRelacion.codCDP;
                        documento.Crp = item.docRelacion.codCompromiso;
                        documento.OrdenPago = item.docRelacion.codOrdenPago.HasValue ? item.docRelacion.codOrdenPago.Value : 0;
                    }

                    if (item.datosAdminObligacion != null)
                    {
                        documento.Detalle4 = item.datosAdminObligacion.notas;
                        documento.Detalle6 = item.datosAdminObligacion.numDocSoporte;
                        documento.Detalle7 = item.datosAdminObligacion.nomTipoDocSoporte;
                    }

                    if (item.terceroBeneficiario != null)
                    {
                        if (Int32.TryParse(item.terceroBeneficiario.codTipoDocumento, out tipoDocumentoId))
                        {
                            documento.TipoIdentificacionTercero = tipoDocumentoId;
                        }
                        documento.NumeroIdentificacionTercero = item.terceroBeneficiario.numDocIdentidad;
                        documento.NombreTercero = item.terceroBeneficiario.nomTercero;
                    }

                    documento.ValorInicial = itemAfectacion.valorInicial;
                    documento.ValorTotal = itemAfectacion.valorActual;
                    documento.SaldoActual = itemAfectacion.saldoUtilizar;
                    documento.Operacion = itemAfectacion.valorOperaciones;
                    documento.Detalle2 = itemAfectacion.codDepAfectacion;
                    documento.Detalle3 = itemAfectacion.nomDepAfectacion;
                    documento.IdentificacionRubro = itemAfectacion.codPosicionGasto;
                    listaDocumento.Add(documento);
                }
            }
            return listaDocumento;
        }

        private List<CDPDto> obtenerListaOrdenPago(OrdenPago.tipoPagoTercero dtDetalle)
        {
            CDPDto documento = null;
            List<CDPDto> listaDocumento = new List<CDPDto>();

            foreach (var item in dtDetalle.listaItemPagoTercero)
            {
                documento = new CDPDto();
                documento.OrdenPago = item.codOrdenPago;
                documento.Fecha = item.fechaPago;
                documento.Instancia = (int)TipoDocumento.OrdenPago;

                if (item.terceroBeneficiarioPago != null)
                {
                    documento.NumeroIdentificacionTercero = item.terceroBeneficiarioPago.numDocIdentidad;
                }
                // documento.Detalle1 = item.estado;
                // documento.Detalle4 = item.objeto;

                // foreach (var itemAfectacion in item.)
                // {
                //     documento.ValorInicial = itemAfectacion.valorInicial;
                //     documento.ValorTotal = itemAfectacion.valorActual;
                //     documento.SaldoActual = itemAfectacion.saldoComp;
                //     documento.Operacion = itemAfectacion.valorOperacion;
                //     documento.Detalle2 = itemAfectacion.codDepAfectacion;
                //     documento.Detalle3 = itemAfectacion.nomDepAfectacion;
                //     documento.IdentificacionRubro = itemAfectacion.codPosicionGasto;
                // }
                listaDocumento.Add(documento);
            }
            return listaDocumento;
        }

    }
}