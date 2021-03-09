using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace ComplementApp.API.Services
{
    public class ProcesoCreacionArchivoExcel : ControllerBase, IProcesoCreacionArchivoExcel
    {
        public DataTable ObtenerTablaDeListaRadicado(List<RadicadoDto> lista)
        {
            int consecutivo = 1;
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("FECHA_RAD", typeof(string)));
            dt.Columns.Add(new DataColumn("ESTADO", typeof(string)));
            dt.Columns.Add(new DataColumn("CRP", typeof(string)));
            dt.Columns.Add(new DataColumn("NIT", typeof(string)));
            dt.Columns.Add(new DataColumn("NOMBRE_TERCERO", typeof(string)));
            dt.Columns.Add(new DataColumn("NUM_RAD_PROV", typeof(string)));
            dt.Columns.Add(new DataColumn("NUM_RAD_SUP", typeof(string)));
            dt.Columns.Add(new DataColumn("TOTAL_A_PAGAR", typeof(decimal)));
            dt.Columns.Add(new DataColumn("NUM_OBLI", typeof(string)));
            dt.Columns.Add(new DataColumn("NUM_OP", typeof(string)));
            dt.Columns.Add(new DataColumn("FECHA_PAGO", typeof(string)));
            dt.Columns.Add(new DataColumn("DETALLE", typeof(string)));

            foreach (var item in lista)
            {
                dr = dt.NewRow();
                dr["ID"] = consecutivo;
                dr["FECHA_RAD"] = item.FechaRadicadoSupervisorDescripcion;
                dr["ESTADO"] = item.Estado;
                dr["CRP"] = item.Crp;
                dr["NIT"] = item.NIT;
                dr["NOMBRE_TERCERO"] = item.NombreTercero;
                dr["NUM_RAD_PROV"] = item.NumeroRadicadoProveedor;
                dr["NUM_RAD_SUP"] = item.NumeroRadicadoSupervisor;
                dr["TOTAL_A_PAGAR"] = item.ValorAPagar;
                dr["NUM_OBLI"] = item.Obligacion;
                dr["NUM_OP"] = item.OrdenPago;
                dr["FECHA_PAGO"] = item.FechaOrdenPagoDescripcion;
                dr["DETALLE"] = item.TextoComprobanteContable;
                dt.Rows.Add(dr);
                consecutivo++;
            }
            return dt;
        }

        public DataTable ObtenerTablaDeListaTercero(List<TerceroDto> lista)
        {
            int consecutivo = 1;
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("TIPO_IDENTIFICACION", typeof(string)));
            dt.Columns.Add(new DataColumn("IDENTIFICACION", typeof(string)));
            dt.Columns.Add(new DataColumn("NOMBRE_DEL_TERCERO", typeof(string)));
            dt.Columns.Add(new DataColumn("DECLARA_RENTA", typeof(string)));
            dt.Columns.Add(new DataColumn("DIRECCION", typeof(string)));
            dt.Columns.Add(new DataColumn("TELEFONO", typeof(string)));
            dt.Columns.Add(new DataColumn("REGIMEN_TRIBUTARIO", typeof(string)));

            foreach (var item in lista)
            {
                dr = dt.NewRow();
                dr["ID"] = consecutivo;
                dr["TIPO_IDENTIFICACION"] = item.TipoDocumentoIdentidad;
                dr["IDENTIFICACION"] = item.NumeroIdentificacion;
                dr["NOMBRE_DEL_TERCERO"] = item.Nombre;
                dr["DECLARA_RENTA"] = item.DeclaranteRentaDescripcion;
                dr["DIRECCION"] = item.Direccion;
                dr["TELEFONO"] = item.Telefono;
                dr["REGIMEN_TRIBUTARIO"] = item.RegimenTributario;
                dt.Rows.Add(dr);
                consecutivo++;
            }
            return dt;
        }

        public DataTable ObtenerTablaDeListaPlanPago(List<PlanPagoDto> lista)
        {
            int consecutivo = 1;
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("CDP", typeof(int)));
            dt.Columns.Add(new DataColumn("CRP", typeof(int)));
            dt.Columns.Add(new DataColumn("AÑO", typeof(int)));
            dt.Columns.Add(new DataColumn("MES", typeof(int)));
            dt.Columns.Add(new DataColumn("IDENTIFICACION", typeof(string)));
            dt.Columns.Add(new DataColumn("NOMBRE_CONTRATISTA/PROVEEDOR", typeof(string)));
            dt.Columns.Add(new DataColumn("OBJETO_COMPROMISO", typeof(string)));
            dt.Columns.Add(new DataColumn("VALOR_PAGAR", typeof(decimal)));

            foreach (var item in lista)
            {
                dr = dt.NewRow();
                dr["ID"] = consecutivo;
                dr["CDP"] = item.Cdp;
                dr["CRP"] = item.Crp;
                dr["AÑO"] = item.AnioPago;
                dr["MES"] = item.MesPago;
                dr["IDENTIFICACION"] = item.IdentificacionTercero;
                dr["NOMBRE_CONTRATISTA/PROVEEDOR"] = item.NombreTercero;
                dr["OBJETO_COMPROMISO"] = item.DetallePlanPago;
                dr["VALOR_PAGAR"] = item.ValorAPagar;
                dt.Rows.Add(dr);
                consecutivo++;
            }
            return dt;
        }

        public DataTable ObtenerTablaDeListaClavePresupuestalContable(List<ClavePresupuestalContableDto> lista)
        {
            int consecutivo = 1;
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("CRP", typeof(long)));
            dt.Columns.Add(new DataColumn("NUM_IDENT", typeof(string)));
            dt.Columns.Add(new DataColumn("NOMBRE_TERCERO", typeof(string)));
            dt.Columns.Add(new DataColumn("DETALLE4", typeof(string)));
            dt.Columns.Add(new DataColumn("RUBRO_PTAL", typeof(string)));
            dt.Columns.Add(new DataColumn("USO_PTAL", typeof(string)));
            dt.Columns.Add(new DataColumn("NOMBRE_USO_PTAL", typeof(string)));
            dt.Columns.Add(new DataColumn("ATRIBUTO_CNT", typeof(string)));
            dt.Columns.Add(new DataColumn("TIPO_GASTO", typeof(string)));
            dt.Columns.Add(new DataColumn("CTA_CNT", typeof(string)));
            dt.Columns.Add(new DataColumn("NOMBRE_CTA_CNT", typeof(string)));
            dt.Columns.Add(new DataColumn("TIPO_OPERACION", typeof(int)));
            dt.Columns.Add(new DataColumn("USO_CONTABLE", typeof(int)));

            foreach (var item in lista)
            {
                dr = dt.NewRow();
                dr["ID"] = consecutivo;
                dr["CRP"] = item.Crp;
                dr["NUM_IDENT"] = item.Tercero.Codigo;
                dr["NOMBRE_TERCERO"] = item.Tercero.Nombre;
                dr["DETALLE4"] = item.Detalle4;
                dr["RUBRO_PTAL"] = item.RubroPresupuestal.Codigo;
                dr["USO_PTAL"] = item.UsoPresupuestal.Codigo;
                dr["NOMBRE_USO_PTAL"] = item.UsoPresupuestal.Nombre;
                dr["ATRIBUTO_CNT"] = item.RelacionContableDto.AtributoContable.Nombre;
                dr["TIPO_GASTO"] = item.RelacionContableDto.TipoGasto.Codigo;
                dr["CTA_CNT"] = item.CuentaContable.Codigo;
                dr["NOMBRE_CTA_CNT"] = item.CuentaContable.Nombre;
                dr["TIPO_OPERACION"] = item.RelacionContableDto.TipoOperacion.HasValue ? item.RelacionContableDto.TipoOperacion.Value : 0;
                dr["USO_CONTABLE"] = item.RelacionContableDto.UsoContable.HasValue ? item.RelacionContableDto.UsoContable.Value : 0;
                dt.Rows.Add(dr);
                consecutivo++;
            }
            return dt;
        }

        public DataTable ObtenerTablaDeListaContrato(List<ContratoDto> lista)
        {
            int consecutivo = 1;
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("CONTRATO", typeof(long)));
            dt.Columns.Add(new DataColumn("COMPROMISO", typeof(long)));
            dt.Columns.Add(new DataColumn("MODALIDAD_CONTRATO", typeof(string)));
            dt.Columns.Add(new DataColumn("FECHA_REGISTRO", typeof(string)));
            dt.Columns.Add(new DataColumn("FECHA_ACTA_INICIO", typeof(string)));
            dt.Columns.Add(new DataColumn("FECHA_FINAL", typeof(string)));
            dt.Columns.Add(new DataColumn("FECHA_APROBAR_POLIZA", typeof(string)));
            dt.Columns.Add(new DataColumn("SUPERVISOR_1", typeof(string)));
            dt.Columns.Add(new DataColumn("SUPERVISOR_2", typeof(string)));

            foreach (var item in lista)
            {
                dr = dt.NewRow();
                dr["ID"] = consecutivo;
                dr["CONTRATO"] = item.Crp;
                dr["COMPROMISO"] = item.Crp;
                dr["MODALIDAD_CONTRATO"] = item.TipoContrato.Nombre;
                dr["FECHA_REGISTRO"] = item.FechaRegistro.ToString("yyyy-MM-dd");
                dr["FECHA_ACTA_INICIO"] = item.FechaInicio.ToString("yyyy-MM-dd");
                dr["FECHA_FINAL"] = item.FechaFinal.ToString("yyyy-MM-dd");
                dr["FECHA_APROBAR_POLIZA"] = item.FechaExpedicionPoliza.ToString("yyyy-MM-dd");
                dr["SUPERVISOR_1"] = item.Supervisor1.NombreCompleto;
                dr["SUPERVISOR_2"] = item.Supervisor2.NombreCompleto;
                dt.Rows.Add(dr);
                consecutivo++;
            }
            return dt;
        }

        public FileStreamResult ExportExcel(HttpResponse response, DataTable dt, string nombreArchivo)
        {
            var memoryStream = new MemoryStream();

            using (var package = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SIGPAA");
                int currentRowNo = 1;
                int totalRows = dt.Rows.Count;
                int k = 0;
                foreach (DataColumn column in dt.Columns)
                {
                    worksheet.Cells[currentRowNo, k + 1].Value = column.ColumnName;
                    k++;
                }
                currentRowNo++;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        worksheet.Cells[currentRowNo, j + 1].Value = Convert.ToString(dt.Rows[i][j]);
                    }
                    currentRowNo++;
                }

                int columnCount = dt.Columns.Count;
                for (int i = 1; i <= columnCount; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                worksheet.Row(1).Height = 55;
                worksheet.Row(1).Style.Font.Bold = true;

                package.Save();
            }

            memoryStream.Position = 0;
            var contentType = "application/octet-stream";
            var fileName = nombreArchivo;
            response.AddFileName(nombreArchivo);
            return File(memoryStream, contentType, fileName);
        }
    }
}