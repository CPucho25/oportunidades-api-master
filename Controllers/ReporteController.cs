using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
//using BF.DTO;
using BF.Models;
using BF.Objeto;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;
using BF.DTO;
using Microsoft.AspNetCore.Identity.UI.Services;
using OfficeOpenXml.Style;
using System.IO;

namespace BF.Controllers
{
    [Produces("application/json")]
    [Route("api/reporte")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private readonly Models.Opotunidades.db_oportunidades _context;
        private readonly IMapper _mapper;
        public ReporteController(Models.Opotunidades.db_oportunidades context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("listarSocxServ")]
        public RespondSearchObject<List<VWListarSocxServ>> buscarServicio(DTOOportunidad dto)
        {
            try
            {
                var listSocServ = new List<VWListarSocxServ>();
                switch (dto.IdRol)
                {
                    //Adminitrador
                    case 1:
                        listSocServ = _context.VWListarSocxServ.Where(item => (dto.IdPeriodo == 0 ? item.IdPeriodo == 1 : item.IdPeriodo == dto.IdPeriodo)
                                                                            && item.FlgActivo == 1
                                                                        )
                        .Select(item => new VWListarSocxServ
                        {
                            Id = item.IdServicio,
                            Descripcion = item.Descripcion,
                            FlgActivo = item.FlgActivo
                        }).OrderBy(item => item.Descripcion).ToList();
                        break;
                    //Socio
                    case 2:
                        listSocServ = _context.VWListarSocxServ.Where(item => item.IdSocio == dto.IdSocio
                                                                            && (dto.IdPeriodo == 0 ? item.IdPeriodo == 1 : item.IdPeriodo == dto.IdPeriodo)
                                                                            && item.FlgActivo == 1
                                                            )
                        .Select(item => new VWListarSocxServ
                        {
                            Id = item.IdServicio,
                            Descripcion = item.Descripcion,
                            FlgActivo = item.FlgActivo
                        }).OrderBy(item => item.Descripcion).ToList();
                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<VWListarSocxServ>();
                        listSocServ = emptyList;
                        break;
                }

                if (listSocServ.Count > 0)
                {
                    return new RespondSearchObject<List<VWListarSocxServ>>()
                    {
                        Objeto = listSocServ,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                }
                else
                {
                    return new RespondSearchObject<List<VWListarSocxServ>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
                }
            }
            catch (Exception ex) 
            {
                return new RespondSearchObject<List<VWListarSocxServ>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
            
        }

        #region Reporte General
        //euscuvil 13-10-2022: Se agregó un nuevo parametro en FormatoReporteGeneral el de 'area' y se agrego casos cuando el area es PT 
        public MemoryStream FormatoReporteGeneral(List<VWReporteGeneral> dto, Int32 rol, Int32 area)
        {
            MemoryStream memStream;

            //Estilos
            var colorHeader = System.Drawing.ColorTranslator.FromHtml("#EBDA00");

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                //name Sheet
                var sheetName = "Reporte";
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                //Font Size
                worksheet.Cells.Style.Font.Name = "EYInterstate Light";
                worksheet.Cells.Style.Font.Size = 10;
                int row = 2;
                switch (rol)
                {   //euscuvil 13-10-2022
                    //Administrador
                    case 1:
                        if (area == 1)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 11;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 20;
                            worksheet.Column(4).Width = 16;
                            worksheet.Column(5).Width = 38;
                            worksheet.Column(6).Width = 35;
                            worksheet.Column(7).Width = 25;
                            worksheet.Column(8).Width = 15;
                            worksheet.Column(9).Width = 30;
                            worksheet.Column(10).Width = 32;
                            worksheet.Column(11).Width = 15;
                            worksheet.Column(12).Width = 15;
                            worksheet.Column(13).Width = 12;
                            worksheet.Column(14).Width = 12;
                            worksheet.Column(15).Width = 12;
                            worksheet.Column(16).Width = 18;
                            worksheet.Column(17).Width = 20;
                            worksheet.Column(18).Width = 16;
                            worksheet.Column(19).Width = 15;
                            worksheet.Column(20).Width = 24;
                            worksheet.Column(21).Width = 24;
                            worksheet.Column(22).Width = 24;
                            worksheet.Column(23).Width = 26;
                            worksheet.Column(24).Width = 16;
                            worksheet.Column(25).Width = 20;
                            worksheet.Column(26).Width = 20;
                            worksheet.Column(27).Width = 28;
                            worksheet.Column(28).Width = 34;
                            worksheet.Column(29).Width = 22;

                            worksheet.Column(30).Width = 22;
                            worksheet.Column(31).Width = 22;
                            worksheet.Column(32).Width = 22;
                            worksheet.Column(33).Width = 22;
                            worksheet.Column(34).Width = 22;
                            worksheet.Column(35).Width = 22;
                            worksheet.Column(36).Width = 22;
                            worksheet.Column(37).Width = 22;

                            worksheet.Column(38).Width = 32;

                            worksheet.Cells["A1:AL1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:AL1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:AL1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:AL1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:AL1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "Gerente";
                            worksheet.Cells[1, 4].Value = "RUC";
                            worksheet.Cells[1, 5].Value = "Razón Social";
                            worksheet.Cells[1, 6].Value = "Servicio";
                            worksheet.Cells[1, 7].Value = "Subservicio";
                            worksheet.Cells[1, 8].Value = "Condición";
                            worksheet.Cells[1, 9].Value = "Grupo Económico";
                            worksheet.Cells[1, 10].Value = "Sector";
                            worksheet.Cells[1, 11].Value = "Estado";
                            worksheet.Cells[1, 12].Value = "Tipo Fee";
                            worksheet.Cells[1, 13].Value = "Moneda";
                            worksheet.Cells[1, 14].Value = "Fee";
                            worksheet.Cells[1, 15].Value = "ITAN";
                            worksheet.Cells[1, 16].Value = "Tarifa Horaria";
                            worksheet.Cells[1, 17].Value = "Cantidad de Horas";
                            worksheet.Cells[1, 18].Value = "Total Soles";
                            worksheet.Cells[1, 19].Value = "Fee Sublínea";
                            worksheet.Cells[1, 20].Value = "Tarifa Horaria Sublínea";
                            worksheet.Cells[1, 21].Value = "Cantidad Hrs. Sublínea";
                            worksheet.Cells[1, 22].Value = "Total Soles Sublínea";
                            worksheet.Cells[1, 23].Value = "Cláusula de Gastos";
                            worksheet.Cells[1, 24].Value = "Gastos Fijos";
                            worksheet.Cells[1, 25].Value = "Detalle de Gastos";
                            worksheet.Cells[1, 26].Value = "Engagement 1";
                            worksheet.Cells[1, 27].Value = "Engagement 2";
                            worksheet.Cells[1, 28].Value = "Competencia RDJ";
                            worksheet.Cells[1, 29].Value = "Quién Ganó?";

                            worksheet.Cells[1, 30].Value = "EAF";
                            worksheet.Cells[1, 31].Value = "Margen";
                            worksheet.Cells[1, 32].Value = "Total de Horas";
                            worksheet.Cells[1, 33].Value = "ID Mercury";
                            worksheet.Cells[1, 34].Value = "Propuesta Firmada";
                            worksheet.Cells[1, 35].Value = "Numero Pace";
                            worksheet.Cells[1, 36].Value = "Ruta Gear";
                            worksheet.Cells[1, 37].Value = "Ruta Workspace";

                            worksheet.Cells[1, 38].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.NomGerente;
                                worksheet.Cells[row, 4].Value = data.Ruc;
                                worksheet.Cells[row, 5].Value = data.RazonSocial;
                                worksheet.Cells[row, 6].Value = data.DetServicio;
                                worksheet.Cells[row, 7].Value = data.DetSubservicio;
                                worksheet.Cells[row, 8].Value = data.DetCondicion;
                                worksheet.Cells[row, 9].Value = data.DetGrpeco;
                                worksheet.Cells[row, 10].Value = data.DetSector;
                                worksheet.Cells[row, 11].Value = data.DetEstado;
                                worksheet.Cells[row, 12].Value = data.DetFee;
                                worksheet.Cells[row, 13].Value = data.DetMoneda;
                                worksheet.Cells[row, 14].Value = data.Fee;
                                worksheet.Cells[row, 14].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 15].Value = data.Itan;
                                worksheet.Cells[row, 15].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 16].Value = data.TarifHoraria;
                                worksheet.Cells[row, 16].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 17].Value = data.CantHoras;
                                worksheet.Cells[row, 18].Value = data.TotalMonto;
                                worksheet.Cells[row, 18].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 19].Value = data.FeeSublinea;
                                worksheet.Cells[row, 19].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 20].Value = data.TarifHorariaSublinea;
                                worksheet.Cells[row, 20].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 21].Value = data.CantHorasSublinea;
                                worksheet.Cells[row, 22].Value = data.TotalSublinea;
                                worksheet.Cells[row, 22].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 23].Value = data.DetGastos;
                                worksheet.Cells[row, 24].Value = data.GastosFijos;
                                worksheet.Cells[row, 25].Value = data.GastosDetalle;
                                worksheet.Cells[row, 26].Value = data.Engagement1;
                                worksheet.Cells[row, 27].Value = data.Engagement2;
                                worksheet.Cells[row, 28].Value = data.CompetenciaRdj;
                                worksheet.Cells[row, 29].Value = data.QuienGano;
                                worksheet.Cells[row, 30].Value = data.Eaf;
                                worksheet.Cells[row, 31].Value = data.Margen;
                                worksheet.Cells[row, 32].Value = data.TotalHoras;
                                worksheet.Cells[row, 33].Value = data.IdOportunidad2;
                                worksheet.Cells[row, 34].Value = data.PropuestaFirmada;
                                worksheet.Cells[row, 35].Value = data.NumeroPace;
                                worksheet.Cells[row, 36].Value = data.RutaGear;
                                worksheet.Cells[row, 37].Value = data.RutaWorkSpace;

                                worksheet.Cells[1, 38].Value = data.Comentarios;
                                row++;
                            });
                            #endregion
                        }

                        else if (area == 2)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 11;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 20;
                            worksheet.Column(4).Width = 16;
                            worksheet.Column(5).Width = 38;
                            worksheet.Column(6).Width = 35;

                            worksheet.Column(7).Width = 15;
                            worksheet.Column(8).Width = 30;

                            worksheet.Column(9).Width = 15;

                            worksheet.Column(10).Width = 12;
                            worksheet.Column(11).Width = 12;


                            worksheet.Column(12).Width = 16;


                            worksheet.Column(13).Width = 20;

                            worksheet.Column(14).Width = 32;

                            worksheet.Cells["A1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:N1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "Gerente";
                            worksheet.Cells[1, 4].Value = "RUC";
                            worksheet.Cells[1, 5].Value = "Razón Social";
                            worksheet.Cells[1, 6].Value = "Servicio";

                            worksheet.Cells[1, 7].Value = "Condición";
                            worksheet.Cells[1, 8].Value = "Grupo Económico";

                            worksheet.Cells[1, 9].Value = "Estado";

                            worksheet.Cells[1, 10].Value = "Moneda";
                            worksheet.Cells[1, 11].Value = "Fee";

                            worksheet.Cells[1, 12].Value = "Total Soles";

                            worksheet.Cells[1, 13].Value = "Engagement 1";

                            worksheet.Cells[1, 14].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.NomGerente;
                                worksheet.Cells[row, 4].Value = data.Ruc;
                                worksheet.Cells[row, 5].Value = data.RazonSocial;
                                worksheet.Cells[row, 6].Value = data.DetServicio;

                                worksheet.Cells[row, 7].Value = data.DetCondicion;
                                worksheet.Cells[row, 8].Value = data.DetGrpeco;

                                worksheet.Cells[row, 9].Value = data.DetEstado;

                                worksheet.Cells[row, 10].Value = data.DetMoneda;
                                worksheet.Cells[row, 11].Value = data.Fee;
                                worksheet.Cells[row, 11].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";


                                worksheet.Cells[row, 12].Value = data.TotalMonto;
                                worksheet.Cells[row, 12].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";

                                worksheet.Cells[row, 13].Value = data.Engagement1;

                                worksheet.Cells[row, 14].Value = data.Comentarios;

                                row++;
                            });
                            #endregion
                        }

                        break;

                    //Socio
                    case 2:
                        if (area == 1)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 22;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 16;
                            worksheet.Column(4).Width = 38;
                            worksheet.Column(5).Width = 35;
                            worksheet.Column(6).Width = 25;
                            worksheet.Column(7).Width = 15;
                            worksheet.Column(8).Width = 30;
                            worksheet.Column(9).Width = 32;
                            worksheet.Column(10).Width = 15;
                            worksheet.Column(11).Width = 15;
                            worksheet.Column(12).Width = 12;
                            worksheet.Column(13).Width = 12;
                            worksheet.Column(14).Width = 12;
                            worksheet.Column(15).Width = 18;
                            worksheet.Column(16).Width = 20;
                            worksheet.Column(17).Width = 16;
                            worksheet.Column(18).Width = 15;
                            worksheet.Column(19).Width = 24;
                            worksheet.Column(20).Width = 24;
                            worksheet.Column(21).Width = 24;
                            worksheet.Column(22).Width = 26;
                            worksheet.Column(23).Width = 16;
                            worksheet.Column(24).Width = 20;
                            worksheet.Column(25).Width = 20;
                            worksheet.Column(26).Width = 28;
                            worksheet.Column(27).Width = 34;
                            worksheet.Column(28).Width = 22;
                            worksheet.Column(29).Width = 22;
                            worksheet.Column(30).Width = 22;
                            worksheet.Column(31).Width = 22;
                            worksheet.Column(32).Width = 22;
                            worksheet.Column(33).Width = 22;
                            worksheet.Column(34).Width = 22;
                            worksheet.Column(35).Width = 22;
                            worksheet.Column(36).Width = 22;

                            worksheet.Column(37).Width = 32;
                            worksheet.Cells["A1:AK1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:AK1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:AK1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:AK1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:AK1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Gerente";
                            worksheet.Cells[1, 3].Value = "RUC";
                            worksheet.Cells[1, 4].Value = "Razón Social";
                            worksheet.Cells[1, 5].Value = "Servicio";
                            worksheet.Cells[1, 6].Value = "Subservicio";
                            worksheet.Cells[1, 7].Value = "Condición";
                            worksheet.Cells[1, 8].Value = "Grupo Económico";
                            worksheet.Cells[1, 9].Value = "Sector";
                            worksheet.Cells[1, 10].Value = "Estado";
                            worksheet.Cells[1, 11].Value = "Tipo Fee";
                            worksheet.Cells[1, 12].Value = "Moneda";
                            worksheet.Cells[1, 13].Value = "Fee";
                            worksheet.Cells[1, 14].Value = "ITAN";
                            worksheet.Cells[1, 15].Value = "Tarifa Horaria";
                            worksheet.Cells[1, 16].Value = "Cantidad de Horas";
                            worksheet.Cells[1, 17].Value = "Total Soles";
                            worksheet.Cells[1, 18].Value = "Fee Sublínea";
                            worksheet.Cells[1, 19].Value = "Tarifa Horaria Sublínea";
                            worksheet.Cells[1, 20].Value = "Cantidad Hrs. Sublínea";
                            worksheet.Cells[1, 21].Value = "Total Soles Sublínea";
                            worksheet.Cells[1, 22].Value = "Cláusula de Gastos";
                            worksheet.Cells[1, 23].Value = "Gastos Fijos";
                            worksheet.Cells[1, 24].Value = "Detalle de Gastos";
                            worksheet.Cells[1, 25].Value = "Engagement 1";
                            worksheet.Cells[1, 26].Value = "Engagement 2";
                            worksheet.Cells[1, 27].Value = "Competencia RDJ";
                            worksheet.Cells[1, 28].Value = "Quién Ganó?";
                            worksheet.Cells[1, 29].Value = "EAF";
                            worksheet.Cells[1, 30].Value = "Margen";
                            worksheet.Cells[1, 31].Value = "Total de Horas";
                            worksheet.Cells[1, 32].Value = "ID Mercury";
                            worksheet.Cells[1, 33].Value = "Propuesta Firmada";
                            worksheet.Cells[1, 34].Value = "Numero Pace";
                            worksheet.Cells[1, 35].Value = "Ruta Gear";
                            worksheet.Cells[1, 36].Value = "Ruta Workspace";
                            worksheet.Cells[1, 37].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomGerente;
                                worksheet.Cells[row, 3].Value = data.Ruc;
                                worksheet.Cells[row, 4].Value = data.RazonSocial;
                                worksheet.Cells[row, 5].Value = data.DetServicio;
                                worksheet.Cells[row, 6].Value = data.DetSubservicio;
                                worksheet.Cells[row, 7].Value = data.DetCondicion;
                                worksheet.Cells[row, 8].Value = data.DetGrpeco;
                                worksheet.Cells[row, 9].Value = data.DetSector;
                                worksheet.Cells[row, 10].Value = data.DetEstado;
                                worksheet.Cells[row, 11].Value = data.DetFee;
                                worksheet.Cells[row, 12].Value = data.DetMoneda;
                                worksheet.Cells[row, 13].Value = data.Fee;
                                worksheet.Cells[row, 13].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 14].Value = data.Itan;
                                worksheet.Cells[row, 14].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 15].Value = data.TarifHoraria;
                                worksheet.Cells[row, 15].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 16].Value = data.CantHoras;
                                worksheet.Cells[row, 17].Value = data.TotalMonto;
                                worksheet.Cells[row, 17].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 18].Value = data.FeeSublinea;
                                worksheet.Cells[row, 18].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 19].Value = data.TarifHorariaSublinea;
                                worksheet.Cells[row, 19].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 20].Value = data.CantHorasSublinea;
                                worksheet.Cells[row, 21].Value = data.TotalSublinea;
                                worksheet.Cells[row, 21].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 22].Value = data.DetGastos;
                                worksheet.Cells[row, 23].Value = data.GastosFijos;
                                worksheet.Cells[row, 24].Value = data.GastosDetalle;
                                worksheet.Cells[row, 25].Value = data.Engagement1;
                                worksheet.Cells[row, 26].Value = data.Engagement2;
                                worksheet.Cells[row, 27].Value = data.CompetenciaRdj;
                                worksheet.Cells[row, 28].Value = data.QuienGano;
                                worksheet.Cells[row, 29].Value = data.Eaf;
                                worksheet.Cells[row, 30].Value = data.Margen;
                                worksheet.Cells[row, 31].Value = data.TotalHoras;
                                worksheet.Cells[row, 32].Value = data.IdOportunidad2;
                                worksheet.Cells[row, 33].Value = data.PropuestaFirmada;
                                worksheet.Cells[row, 34].Value = data.NumeroPace;
                                worksheet.Cells[row, 35].Value = data.RutaGear;
                                worksheet.Cells[row, 36].Value = data.RutaWorkSpace;

                                worksheet.Cells[1, 37].Value = data.Comentarios;
                                row++;
                            });
                            #endregion
                        }

                        else if (area == 2)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 22;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 16;
                            worksheet.Column(4).Width = 38;
                            worksheet.Column(5).Width = 35;

                            worksheet.Column(6).Width = 15;
                            worksheet.Column(7).Width = 30;

                            worksheet.Column(8).Width = 15;

                            worksheet.Column(9).Width = 12;
                            worksheet.Column(10).Width = 12;


                            worksheet.Column(11).Width = 16;

                            worksheet.Column(12).Width = 20;

                            worksheet.Column(13).Width = 32;

                            worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:M1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:M1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:M1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:M1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Gerente";
                            worksheet.Cells[1, 3].Value = "RUC";
                            worksheet.Cells[1, 4].Value = "Razón Social";
                            worksheet.Cells[1, 5].Value = "Servicio";

                            worksheet.Cells[1, 6].Value = "Condición";
                            worksheet.Cells[1, 7].Value = "Grupo Económico";

                            worksheet.Cells[1, 8].Value = "Estado";

                            worksheet.Cells[1, 9].Value = "Moneda";
                            worksheet.Cells[1, 10].Value = "Fee";

                            worksheet.Cells[1, 11].Value = "Total Soles";

                            worksheet.Cells[1, 12].Value = "Engagement 1";

                            worksheet.Cells[1, 13].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomGerente;
                                worksheet.Cells[row, 3].Value = data.Ruc;
                                worksheet.Cells[row, 4].Value = data.RazonSocial;
                                worksheet.Cells[row, 5].Value = data.DetServicio;

                                worksheet.Cells[row, 6].Value = data.DetCondicion;
                                worksheet.Cells[row, 7].Value = data.DetGrpeco;

                                worksheet.Cells[row, 8].Value = data.DetEstado;

                                worksheet.Cells[row, 9].Value = data.DetMoneda;
                                worksheet.Cells[row, 10].Value = data.Fee;
                                worksheet.Cells[row, 10].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";

                                worksheet.Cells[row, 11].Value = data.TotalMonto;
                                worksheet.Cells[row, 11].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";

                                worksheet.Cells[row, 12].Value = data.Engagement1;

                                worksheet.Cells[row, 13].Value = data.Comentarios;

                                row++;
                            });
                            #endregion
                        }
                        break;


                    //Gerente
                    case 3:
                        if (area == 1)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 22;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 16;
                            worksheet.Column(4).Width = 38;
                            worksheet.Column(5).Width = 35;
                            worksheet.Column(6).Width = 25;
                            worksheet.Column(7).Width = 15;
                            worksheet.Column(8).Width = 30;
                            worksheet.Column(9).Width = 32;
                            worksheet.Column(10).Width = 15;
                            worksheet.Column(11).Width = 15;
                            worksheet.Column(12).Width = 12;
                            worksheet.Column(13).Width = 12;
                            worksheet.Column(14).Width = 12;
                            worksheet.Column(15).Width = 18;
                            worksheet.Column(16).Width = 20;
                            worksheet.Column(17).Width = 16;
                            worksheet.Column(18).Width = 15;
                            worksheet.Column(19).Width = 24;
                            worksheet.Column(20).Width = 24;
                            worksheet.Column(21).Width = 24;
                            worksheet.Column(22).Width = 26;
                            worksheet.Column(23).Width = 16;
                            worksheet.Column(24).Width = 20;
                            worksheet.Column(25).Width = 20;
                            worksheet.Column(26).Width = 28;
                            worksheet.Column(27).Width = 34;
                            worksheet.Column(28).Width = 22;
                            worksheet.Column(29).Width = 32;

                            worksheet.Cells["A1:AC1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:AC1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:AC1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:AC1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:AC1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "RUC";
                            worksheet.Cells[1, 4].Value = "Razón Social";
                            worksheet.Cells[1, 5].Value = "Servicio";
                            worksheet.Cells[1, 6].Value = "Subservicio";
                            worksheet.Cells[1, 7].Value = "Condición";
                            worksheet.Cells[1, 8].Value = "Grupo Económico";
                            worksheet.Cells[1, 9].Value = "Sector";
                            worksheet.Cells[1, 10].Value = "Estado";
                            worksheet.Cells[1, 11].Value = "Tipo Fee";
                            worksheet.Cells[1, 12].Value = "Moneda";
                            worksheet.Cells[1, 13].Value = "Fee";
                            worksheet.Cells[1, 14].Value = "ITAN";
                            worksheet.Cells[1, 15].Value = "Tarifa Horaria";
                            worksheet.Cells[1, 16].Value = "Cantidad de Horas";
                            worksheet.Cells[1, 17].Value = "Total Soles";
                            worksheet.Cells[1, 18].Value = "Fee Sublínea";
                            worksheet.Cells[1, 19].Value = "Tarifa Horaria Sublínea";
                            worksheet.Cells[1, 20].Value = "Cantidad Hrs. Sublínea";
                            worksheet.Cells[1, 21].Value = "Total Soles Sublínea";
                            worksheet.Cells[1, 22].Value = "Cláusula de Gastos";
                            worksheet.Cells[1, 23].Value = "Gastos Fijos";
                            worksheet.Cells[1, 24].Value = "Detalle de Gastos";
                            worksheet.Cells[1, 25].Value = "Engagement 1";
                            worksheet.Cells[1, 26].Value = "Engagement 2";
                            worksheet.Cells[1, 27].Value = "Competencia RDJ";
                            worksheet.Cells[1, 28].Value = "Quién Ganó?";
                            worksheet.Cells[1, 29].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.Ruc;
                                worksheet.Cells[row, 4].Value = data.RazonSocial;
                                worksheet.Cells[row, 5].Value = data.DetServicio;
                                worksheet.Cells[row, 6].Value = data.DetSubservicio;
                                worksheet.Cells[row, 7].Value = data.DetCondicion;
                                worksheet.Cells[row, 8].Value = data.DetGrpeco;
                                worksheet.Cells[row, 9].Value = data.DetSector;
                                worksheet.Cells[row, 10].Value = data.DetEstado;
                                worksheet.Cells[row, 11].Value = data.DetFee;
                                worksheet.Cells[row, 12].Value = data.DetMoneda;
                                worksheet.Cells[row, 13].Value = data.Fee;
                                worksheet.Cells[row, 13].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 14].Value = data.Itan;
                                worksheet.Cells[row, 14].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 15].Value = data.TarifHoraria;
                                worksheet.Cells[row, 15].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 16].Value = data.CantHoras;
                                worksheet.Cells[row, 17].Value = data.TotalMonto;
                                worksheet.Cells[row, 17].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 18].Value = data.FeeSublinea;
                                worksheet.Cells[row, 18].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 19].Value = data.TarifHorariaSublinea;
                                worksheet.Cells[row, 19].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 20].Value = data.CantHorasSublinea;
                                worksheet.Cells[row, 21].Value = data.TotalSublinea;
                                worksheet.Cells[row, 21].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 22].Value = data.DetGastos;
                                worksheet.Cells[row, 23].Value = data.GastosFijos;
                                worksheet.Cells[row, 24].Value = data.GastosDetalle;
                                worksheet.Cells[row, 25].Value = data.Engagement1;
                                worksheet.Cells[row, 26].Value = data.Engagement2;
                                worksheet.Cells[row, 27].Value = data.CompetenciaRdj;
                                worksheet.Cells[row, 28].Value = data.QuienGano;
                                worksheet.Cells[row, 29].Value = data.Comentarios;

                                row++;
                            });
                            #endregion
                        }

                        else if (area == 2)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 22;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 16;
                            worksheet.Column(4).Width = 38;
                            worksheet.Column(5).Width = 35;

                            worksheet.Column(6).Width = 15;
                            worksheet.Column(7).Width = 30;

                            worksheet.Column(8).Width = 15;

                            worksheet.Column(9).Width = 12;
                            worksheet.Column(10).Width = 12;

                            worksheet.Column(11).Width = 16;

                            worksheet.Column(12).Width = 20;

                            worksheet.Column(13).Width = 32;

                            worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:M1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:M1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:M1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:M1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "RUC";
                            worksheet.Cells[1, 4].Value = "Razón Social";
                            worksheet.Cells[1, 5].Value = "Servicio";

                            worksheet.Cells[1, 6].Value = "Condición";
                            worksheet.Cells[1, 7].Value = "Grupo Económico";

                            worksheet.Cells[1, 8].Value = "Estado";

                            worksheet.Cells[1, 9].Value = "Moneda";
                            worksheet.Cells[1, 10].Value = "Fee";

                            worksheet.Cells[1, 11].Value = "Total Soles";

                            worksheet.Cells[1, 12].Value = "Engagement 1";

                            worksheet.Cells[1, 13].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.Ruc;
                                worksheet.Cells[row, 4].Value = data.RazonSocial;
                                worksheet.Cells[row, 5].Value = data.DetServicio;

                                worksheet.Cells[row, 6].Value = data.DetCondicion;
                                worksheet.Cells[row, 7].Value = data.DetGrpeco;

                                worksheet.Cells[row, 8].Value = data.DetEstado;

                                worksheet.Cells[row, 9].Value = data.DetMoneda;
                                worksheet.Cells[row, 10].Value = data.Fee;
                                worksheet.Cells[row, 10].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";

                                worksheet.Cells[row, 11].Value = data.TotalMonto;
                                worksheet.Cells[row, 11].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";

                                worksheet.Cells[row, 12].Value = data.Engagement1;
                                worksheet.Cells[row, 13].Value = data.Comentarios;

                                row++;
                            });
                            #endregion
                        }

                        break;

                    //Socio Lider
                    case 5:
                        if (area == 1)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 11;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 20;
                            worksheet.Column(4).Width = 16;
                            worksheet.Column(5).Width = 38;
                            worksheet.Column(6).Width = 35;
                            worksheet.Column(7).Width = 25;
                            worksheet.Column(8).Width = 15;
                            worksheet.Column(9).Width = 30;
                            worksheet.Column(10).Width = 32;
                            worksheet.Column(11).Width = 15;
                            worksheet.Column(12).Width = 15;
                            worksheet.Column(13).Width = 12;
                            worksheet.Column(14).Width = 12;
                            worksheet.Column(15).Width = 12;
                            worksheet.Column(16).Width = 18;
                            worksheet.Column(17).Width = 20;
                            worksheet.Column(18).Width = 16;
                            worksheet.Column(19).Width = 15;
                            worksheet.Column(20).Width = 24;
                            worksheet.Column(21).Width = 24;
                            worksheet.Column(22).Width = 24;
                            worksheet.Column(23).Width = 26;
                            worksheet.Column(24).Width = 16;
                            worksheet.Column(25).Width = 20;
                            worksheet.Column(26).Width = 20;
                            worksheet.Column(27).Width = 28;
                            worksheet.Column(28).Width = 34;
                            worksheet.Column(29).Width = 22;
                            worksheet.Column(30).Width = 22;
                            worksheet.Column(31).Width = 22;
                            worksheet.Column(32).Width = 22;
                            worksheet.Column(33).Width = 22;
                            worksheet.Column(34).Width = 22;
                            worksheet.Column(35).Width = 22;
                            worksheet.Column(36).Width = 22;
                            worksheet.Column(37).Width = 22;

                            worksheet.Column(38).Width = 32;
                            worksheet.Cells["A1:AL1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:AL1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:AL1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:AL1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:AL1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "Gerente";
                            worksheet.Cells[1, 4].Value = "RUC";
                            worksheet.Cells[1, 5].Value = "Razón Social";
                            worksheet.Cells[1, 6].Value = "Servicio";
                            worksheet.Cells[1, 7].Value = "Subservicio";
                            worksheet.Cells[1, 8].Value = "Condición";
                            worksheet.Cells[1, 9].Value = "Grupo Económico";
                            worksheet.Cells[1, 10].Value = "Sector";
                            worksheet.Cells[1, 11].Value = "Estado";
                            worksheet.Cells[1, 12].Value = "Tipo Fee";
                            worksheet.Cells[1, 13].Value = "Moneda";
                            worksheet.Cells[1, 14].Value = "Fee";
                            worksheet.Cells[1, 15].Value = "ITAN";
                            worksheet.Cells[1, 16].Value = "Tarifa Horaria";
                            worksheet.Cells[1, 17].Value = "Cantidad de Horas";
                            worksheet.Cells[1, 18].Value = "Total Soles";
                            worksheet.Cells[1, 19].Value = "Fee Sublínea";
                            worksheet.Cells[1, 20].Value = "Tarifa Horaria Sublínea";
                            worksheet.Cells[1, 21].Value = "Cantidad Hrs. Sublínea";
                            worksheet.Cells[1, 22].Value = "Total Soles Sublínea";
                            worksheet.Cells[1, 23].Value = "Cláusula de Gastos";
                            worksheet.Cells[1, 24].Value = "Gastos Fijos";
                            worksheet.Cells[1, 25].Value = "Detalle de Gastos";
                            worksheet.Cells[1, 26].Value = "Engagement 1";
                            worksheet.Cells[1, 27].Value = "Engagement 2";
                            worksheet.Cells[1, 28].Value = "Competencia RDJ";
                            worksheet.Cells[1, 29].Value = "Quién Ganó?";
                            worksheet.Cells[1, 30].Value = "EAF";
                            worksheet.Cells[1, 31].Value = "Margen";
                            worksheet.Cells[1, 32].Value = "Total de Horas";
                            worksheet.Cells[1, 33].Value = "ID Mercury";
                            worksheet.Cells[1, 34].Value = "Propuesta Firmada";
                            worksheet.Cells[1, 35].Value = "Numero Pace";
                            worksheet.Cells[1, 36].Value = "Ruta Gear";
                            worksheet.Cells[1, 37].Value = "Ruta Workspace";

                            worksheet.Cells[1, 38].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.NomGerente;
                                worksheet.Cells[row, 4].Value = data.Ruc;
                                worksheet.Cells[row, 5].Value = data.RazonSocial;
                                worksheet.Cells[row, 6].Value = data.DetServicio;
                                worksheet.Cells[row, 7].Value = data.DetSubservicio;
                                worksheet.Cells[row, 8].Value = data.DetCondicion;
                                worksheet.Cells[row, 9].Value = data.DetGrpeco;
                                worksheet.Cells[row, 10].Value = data.DetSector;
                                worksheet.Cells[row, 11].Value = data.DetEstado;
                                worksheet.Cells[row, 12].Value = data.DetFee;
                                worksheet.Cells[row, 13].Value = data.DetMoneda;
                                worksheet.Cells[row, 14].Value = data.Fee;
                                worksheet.Cells[row, 14].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 15].Value = data.Itan;
                                worksheet.Cells[row, 15].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 16].Value = data.TarifHoraria;
                                worksheet.Cells[row, 16].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 17].Value = data.CantHoras;
                                worksheet.Cells[row, 18].Value = data.TotalMonto;
                                worksheet.Cells[row, 18].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 19].Value = data.FeeSublinea;
                                worksheet.Cells[row, 19].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 20].Value = data.TarifHorariaSublinea;
                                worksheet.Cells[row, 20].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 21].Value = data.CantHorasSublinea;
                                worksheet.Cells[row, 22].Value = data.TotalSublinea;
                                worksheet.Cells[row, 22].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 23].Value = data.DetGastos;
                                worksheet.Cells[row, 24].Value = data.GastosFijos;
                                worksheet.Cells[row, 25].Value = data.GastosDetalle;
                                worksheet.Cells[row, 26].Value = data.Engagement1;
                                worksheet.Cells[row, 27].Value = data.Engagement2;
                                worksheet.Cells[row, 28].Value = data.CompetenciaRdj;
                                worksheet.Cells[row, 29].Value = data.QuienGano;
                                worksheet.Cells[row, 30].Value = data.Eaf;
                                worksheet.Cells[row, 31].Value = data.Margen;
                                worksheet.Cells[row, 32].Value = data.TotalHoras;
                                worksheet.Cells[row, 33].Value = data.IdOportunidad2;
                                worksheet.Cells[row, 34].Value = data.PropuestaFirmada;
                                worksheet.Cells[row, 35].Value = data.NumeroPace;
                                worksheet.Cells[row, 36].Value = data.RutaGear;
                                worksheet.Cells[row, 37].Value = data.RutaWorkSpace;

                                worksheet.Cells[1, 38].Value = data.Comentarios;
                                row++;
                            });
                            #endregion
                        }

                        else if (area == 2)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 11;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 20;
                            worksheet.Column(4).Width = 16;
                            worksheet.Column(5).Width = 38;
                            worksheet.Column(6).Width = 35;

                            worksheet.Column(7).Width = 15;
                            worksheet.Column(8).Width = 30;

                            worksheet.Column(9).Width = 15;

                            worksheet.Column(10).Width = 12;
                            worksheet.Column(11).Width = 12;


                            worksheet.Column(12).Width = 16;


                            worksheet.Column(13).Width = 20;

                            worksheet.Column(14).Width = 32;

                            worksheet.Cells["A1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:N1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "Gerente";
                            worksheet.Cells[1, 4].Value = "RUC";
                            worksheet.Cells[1, 5].Value = "Razón Social";
                            worksheet.Cells[1, 6].Value = "Servicio";

                            worksheet.Cells[1, 7].Value = "Condición";
                            worksheet.Cells[1, 8].Value = "Grupo Económico";

                            worksheet.Cells[1, 9].Value = "Estado";

                            worksheet.Cells[1, 10].Value = "Moneda";
                            worksheet.Cells[1, 11].Value = "Fee";

                            worksheet.Cells[1, 12].Value = "Total Soles";

                            worksheet.Cells[1, 13].Value = "Engagement 1";

                            worksheet.Cells[1, 14].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.NomGerente;
                                worksheet.Cells[row, 4].Value = data.Ruc;
                                worksheet.Cells[row, 5].Value = data.RazonSocial;
                                worksheet.Cells[row, 6].Value = data.DetServicio;

                                worksheet.Cells[row, 7].Value = data.DetCondicion;
                                worksheet.Cells[row, 8].Value = data.DetGrpeco;

                                worksheet.Cells[row, 9].Value = data.DetEstado;

                                worksheet.Cells[row, 10].Value = data.DetMoneda;
                                worksheet.Cells[row, 11].Value = data.Fee;
                                worksheet.Cells[row, 11].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";


                                worksheet.Cells[row, 12].Value = data.TotalMonto;
                                worksheet.Cells[row, 12].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";

                                worksheet.Cells[row, 13].Value = data.Engagement1;

                                worksheet.Cells[row, 14].Value = data.Comentarios;

                                row++;
                            });
                            #endregion
                        }

                        break;


                    //SemiAdministrador
                    case 6:
                        if (area == 1)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 11;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 20;
                            worksheet.Column(4).Width = 16;
                            worksheet.Column(5).Width = 38;
                            worksheet.Column(6).Width = 35;
                            worksheet.Column(7).Width = 25;
                            worksheet.Column(8).Width = 15;
                            worksheet.Column(9).Width = 30;
                            worksheet.Column(10).Width = 32;
                            worksheet.Column(11).Width = 15;
                            worksheet.Column(12).Width = 15;
                            worksheet.Column(13).Width = 12;
                            worksheet.Column(14).Width = 12;
                            worksheet.Column(15).Width = 12;
                            worksheet.Column(16).Width = 18;
                            worksheet.Column(17).Width = 20;
                            worksheet.Column(18).Width = 16;
                            worksheet.Column(19).Width = 15;
                            worksheet.Column(20).Width = 24;
                            worksheet.Column(21).Width = 24;
                            worksheet.Column(22).Width = 24;
                            worksheet.Column(23).Width = 26;
                            worksheet.Column(24).Width = 16;
                            worksheet.Column(25).Width = 20;
                            worksheet.Column(26).Width = 20;
                            worksheet.Column(27).Width = 28;
                            worksheet.Column(28).Width = 34;
                            worksheet.Column(29).Width = 22;
                            worksheet.Column(30).Width = 22;
                            worksheet.Column(31).Width = 22;
                            worksheet.Column(32).Width = 22;
                            worksheet.Column(33).Width = 22;
                            worksheet.Column(34).Width = 22;
                            worksheet.Column(35).Width = 22;
                            worksheet.Column(36).Width = 22;
                            worksheet.Column(37).Width = 22;

                            worksheet.Column(38).Width = 32;
                            worksheet.Cells["A1:AL1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:AL1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:AL1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:AL1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:AL1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "Gerente";
                            worksheet.Cells[1, 4].Value = "RUC";
                            worksheet.Cells[1, 5].Value = "Razón Social";
                            worksheet.Cells[1, 6].Value = "Servicio";
                            worksheet.Cells[1, 7].Value = "Subservicio";
                            worksheet.Cells[1, 8].Value = "Condición";
                            worksheet.Cells[1, 9].Value = "Grupo Económico";
                            worksheet.Cells[1, 10].Value = "Sector";
                            worksheet.Cells[1, 11].Value = "Estado";
                            worksheet.Cells[1, 12].Value = "Tipo Fee";
                            worksheet.Cells[1, 13].Value = "Moneda";
                            worksheet.Cells[1, 14].Value = "Fee";
                            worksheet.Cells[1, 15].Value = "ITAN";
                            worksheet.Cells[1, 16].Value = "Tarifa Horaria";
                            worksheet.Cells[1, 17].Value = "Cantidad de Horas";
                            worksheet.Cells[1, 18].Value = "Total Soles";
                            worksheet.Cells[1, 19].Value = "Fee Sublínea";
                            worksheet.Cells[1, 20].Value = "Tarifa Horaria Sublínea";
                            worksheet.Cells[1, 21].Value = "Cantidad Hrs. Sublínea";
                            worksheet.Cells[1, 22].Value = "Total Soles Sublínea";
                            worksheet.Cells[1, 23].Value = "Cláusula de Gastos";
                            worksheet.Cells[1, 24].Value = "Gastos Fijos";
                            worksheet.Cells[1, 25].Value = "Detalle de Gastos";
                            worksheet.Cells[1, 26].Value = "Engagement 1";
                            worksheet.Cells[1, 27].Value = "Engagement 2";
                            worksheet.Cells[1, 28].Value = "Competencia RDJ";
                            worksheet.Cells[1, 29].Value = "Quién Ganó?";
                            worksheet.Cells[1, 30].Value = "EAF";
                            worksheet.Cells[1, 31].Value = "Margen";
                            worksheet.Cells[1, 32].Value = "Total de Horas";
                            worksheet.Cells[1, 33].Value = "ID Mercury";
                            worksheet.Cells[1, 34].Value = "Propuesta Firmada";
                            worksheet.Cells[1, 35].Value = "Numero Pace";
                            worksheet.Cells[1, 36].Value = "Ruta Gear";
                            worksheet.Cells[1, 37].Value = "Ruta Workspace";
                            worksheet.Cells[1, 38].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.NomGerente;
                                worksheet.Cells[row, 4].Value = data.Ruc;
                                worksheet.Cells[row, 5].Value = data.RazonSocial;
                                worksheet.Cells[row, 6].Value = data.DetServicio;
                                worksheet.Cells[row, 7].Value = data.DetSubservicio;
                                worksheet.Cells[row, 8].Value = data.DetCondicion;
                                worksheet.Cells[row, 9].Value = data.DetGrpeco;
                                worksheet.Cells[row, 10].Value = data.DetSector;
                                worksheet.Cells[row, 11].Value = data.DetEstado;
                                worksheet.Cells[row, 12].Value = data.DetFee;
                                worksheet.Cells[row, 13].Value = data.DetMoneda;
                                worksheet.Cells[row, 14].Value = data.Fee;
                                worksheet.Cells[row, 14].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 15].Value = data.Itan;
                                worksheet.Cells[row, 15].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 16].Value = data.TarifHoraria;
                                worksheet.Cells[row, 16].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 17].Value = data.CantHoras;
                                worksheet.Cells[row, 18].Value = data.TotalMonto;
                                worksheet.Cells[row, 18].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 19].Value = data.FeeSublinea;
                                worksheet.Cells[row, 19].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 20].Value = data.TarifHorariaSublinea;
                                worksheet.Cells[row, 20].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 21].Value = data.CantHorasSublinea;
                                worksheet.Cells[row, 22].Value = data.TotalSublinea;
                                worksheet.Cells[row, 22].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                                worksheet.Cells[row, 23].Value = data.DetGastos;
                                worksheet.Cells[row, 24].Value = data.GastosFijos;
                                worksheet.Cells[row, 25].Value = data.GastosDetalle;
                                worksheet.Cells[row, 26].Value = data.Engagement1;
                                worksheet.Cells[row, 27].Value = data.Engagement2;
                                worksheet.Cells[row, 28].Value = data.CompetenciaRdj;
                                worksheet.Cells[row, 29].Value = data.QuienGano;
                                worksheet.Cells[row, 30].Value = data.Eaf;
                                worksheet.Cells[row, 31].Value = data.Margen;
                                worksheet.Cells[row, 32].Value = data.TotalHoras;
                                worksheet.Cells[row, 33].Value = data.IdOportunidad2;
                                worksheet.Cells[row, 34].Value = data.PropuestaFirmada;
                                worksheet.Cells[row, 35].Value = data.NumeroPace;
                                worksheet.Cells[row, 36].Value = data.RutaGear;
                                worksheet.Cells[row, 37].Value = data.RutaWorkSpace;

                                worksheet.Cells[1, 38].Value = data.Comentarios;
                                row++;
                            });
                            #endregion
                        }

                        else if (area == 2)
                        {
                            //Formato de columnas
                            #region Formato columnas
                            worksheet.Column(1).Width = 11;
                            worksheet.Column(2).Width = 20;
                            worksheet.Column(3).Width = 20;
                            worksheet.Column(4).Width = 16;
                            worksheet.Column(5).Width = 38;
                            worksheet.Column(6).Width = 35;

                            worksheet.Column(7).Width = 15;
                            worksheet.Column(8).Width = 30;

                            worksheet.Column(9).Width = 15;

                            worksheet.Column(10).Width = 12;
                            worksheet.Column(11).Width = 12;


                            worksheet.Column(12).Width = 16;


                            worksheet.Column(13).Width = 20;

                            worksheet.Column(14).Width = 32;

                            worksheet.Cells["A1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:N1"].Style.Font.Size = 12;
                            worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                            worksheet.View.FreezePanes(2, 1);
                            #endregion

                            //Header
                            #region Header
                            worksheet.Cells[1, 1].Value = "Periodo";
                            worksheet.Cells[1, 2].Value = "Socio";
                            worksheet.Cells[1, 3].Value = "Gerente";
                            worksheet.Cells[1, 4].Value = "RUC";
                            worksheet.Cells[1, 5].Value = "Razón Social";
                            worksheet.Cells[1, 6].Value = "Servicio";

                            worksheet.Cells[1, 7].Value = "Condición";
                            worksheet.Cells[1, 8].Value = "Grupo Económico";

                            worksheet.Cells[1, 9].Value = "Estado";

                            worksheet.Cells[1, 10].Value = "Moneda";
                            worksheet.Cells[1, 11].Value = "Fee";

                            worksheet.Cells[1, 12].Value = "Total Soles";

                            worksheet.Cells[1, 13].Value = "Engagement 1";

                            worksheet.Cells[1, 14].Value = "Comentarios";
                            #endregion

                            //Body
                            #region Body
                            row = 2;
                            dto.ForEach(data =>
                            {
                                worksheet.Cells[row, 1].Value = data.DetPeriodo;
                                worksheet.Cells[row, 2].Value = data.NomSocio;
                                worksheet.Cells[row, 3].Value = data.NomGerente;
                                worksheet.Cells[row, 4].Value = data.Ruc;
                                worksheet.Cells[row, 5].Value = data.RazonSocial;
                                worksheet.Cells[row, 6].Value = data.DetServicio;

                                worksheet.Cells[row, 7].Value = data.DetCondicion;
                                worksheet.Cells[row, 8].Value = data.DetGrpeco;

                                worksheet.Cells[row, 9].Value = data.DetEstado;

                                worksheet.Cells[row, 10].Value = data.DetMoneda;
                                worksheet.Cells[row, 11].Value = data.Fee;
                                worksheet.Cells[row, 11].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";


                                worksheet.Cells[row, 12].Value = data.TotalMonto;
                                worksheet.Cells[row, 12].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";

                                worksheet.Cells[row, 13].Value = data.Engagement1;

                                worksheet.Cells[row, 14].Value = data.Comentarios;

                                row++;
                            });
                            #endregion
                        }

                        break;

                    default:
                        break;
                }

                memStream = new MemoryStream(package.GetAsByteArray());
            }

            return memStream;
        }

        [HttpPost]
        [Route("reporteGeneral")]
        public async Task<String> generarRerpoteGeneral(DTOOportunidad dto)
        {
            try
            {
                //Ruta física - virtual Directory
                //Local
                //var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base de Oportunidades\Reporte\";
                //Prod
                var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base_Oportunidades\Template\";

                //Nombre del archivo
                var dateString = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var hourString = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                var filename = "Cartera_Cliente_" + dateString + "_" + hourString + ".xlsx";

                //Validamos si existe el archivo en la ruta virtual, de ser así, se procede a eliminarlo
                if (System.IO.File.Exists(rutaVirtualDirectory + filename))
                    System.IO.File.Delete(rutaVirtualDirectory + filename);

                //Creamos reporte
                //Obtenemos los registros de la BD
                var listReporte = new List<VWReporteGeneral>();
                #region data
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listReporte = await _context.VWReporteGeneral.Where(r => r.FlgActivo == 1
                                                                                    && (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.IdSocio == 0 ? 1 == 1 : r.IdSocio == dto.IdSocio)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : r.IdEmpresa == dto.IdEmpresa)
                                                                                    && (dto.IdServicio == 0 ? 1 == 1 : r.IdServicio == dto.IdServicio)
                                                                                    && (dto.IdSubservicio == 0 ? 1 == 1 : r.IdSubservicio == dto.IdSubservicio)
                                                                                    && (dto.IdEstado == 0 ? 1 == 1 : r.IdEstado == dto.IdEstado)
                                                                                    && (dto.IdCondicion == 0 ? 1 == 1 : r.IdCondicion == dto.IdCondicion)
                                                                                    && r.IdArea == dto.IdArea
                                                                                )
                            .Select(r => new VWReporteGeneral
                            {
                                DetPeriodo = r.DetPeriodo,
                                NomSocio = r.NomSocio,
                                NomGerente = r.NomGerente,
                                Ruc = r.Ruc,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                DetSubservicio = r.DetSubservicio,
                                DetCondicion = r.DetCondicion,
                                DetGrpeco = r.DetGrpeco,
                                DetSector = r.DetSector,
                                DetEstado = r.DetEstado,
                                DetFee = r.DetFee,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                Itan = r.Itan,
                                TarifHoraria = r.TarifHoraria,
                                CantHoras = r.CantHoras,
                                TotalMonto = r.TotalMonto,
                                FeeSublinea = r.FeeSublinea,
                                TarifHorariaSublinea = r.TarifHorariaSublinea,
                                CantHorasSublinea = r.CantHorasSublinea,
                                TotalSublinea = r.TotalSublinea,
                                DetGastos = r.DetGastos,
                                GastosFijos = r.GastosFijos,
                                GastosDetalle = r.GastosDetalle,
                                Engagement1 = r.Engagement1,
                                Engagement2 = r.Engagement2,
                                CompetenciaRdj = r.CompetenciaRdj,
                                QuienGano = r.QuienGano,
                                Comentarios = r.Comentarios,
                                Eaf = r.Eaf,
                                Margen = r.Margen,
                                TotalHoras = r.TotalHoras,
                                IdOportunidad2 = r.IdOportunidad2,
                                PropuestaFirmada = r.PropuestaFirmada,
                                NumeroPace = r.NumeroPace,
                                RutaGear = r.RutaGear,
                                RutaWorkSpace = r.RutaWorkSpace


        }).OrderBy(r => r.DetPeriodo).ThenBy(r => r.NomSocio).ThenBy(r => r.NomGerente)
                                .ThenBy(r => r.Ruc).ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();
                        break;
                    //Socio
                    case 2:
                        listReporte = await _context.VWReporteGeneral.Where(r => r.FlgActivo == 1 && r.IdSocio == dto.IdSocio
                                                                                    && (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : r.IdEmpresa == dto.IdEmpresa)
                                                                                    && (dto.IdServicio == 0 ? 1 == 1 : r.IdServicio == dto.IdServicio)
                                                                                    && (dto.IdSubservicio == 0 ? 1 == 1 : r.IdSubservicio == dto.IdSubservicio)
                                                                                    && (dto.IdEstado == 0 ? 1 == 1 : r.IdEstado == dto.IdEstado)
                                                                                    && (dto.IdCondicion == 0 ? 1 == 1 : r.IdCondicion == dto.IdCondicion)
                                                                                    && r.IdArea == dto.IdArea


                                                                                )
                            .Select(r => new VWReporteGeneral
                            {
                                DetPeriodo = r.DetPeriodo,
                            //NomSocio = r.NomSocio,
                            NomGerente = r.NomGerente,
                                Ruc = r.Ruc,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                DetSubservicio = r.DetSubservicio,
                                DetCondicion = r.DetCondicion,
                                DetGrpeco = r.DetGrpeco,
                                DetSector = r.DetSector,
                                DetEstado = r.DetEstado,
                                DetFee = r.DetFee,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                Itan = r.Itan,
                                TarifHoraria = r.TarifHoraria,
                                CantHoras = r.CantHoras,
                                TotalMonto = r.TotalMonto,
                                FeeSublinea = r.FeeSublinea,
                                TarifHorariaSublinea = r.TarifHorariaSublinea,
                                CantHorasSublinea = r.CantHorasSublinea,
                                TotalSublinea = r.TotalSublinea,
                                DetGastos = r.DetGastos,
                                GastosFijos = r.GastosFijos,
                                GastosDetalle = r.GastosDetalle,
                                Engagement1 = r.Engagement1,
                                Engagement2 = r.Engagement2,
                                CompetenciaRdj = r.CompetenciaRdj,
                                QuienGano = r.QuienGano,
                                Comentarios = r.Comentarios,
                                Eaf = r.Eaf,
                                Margen = r.Margen,
                                TotalHoras = r.TotalHoras,
                                IdOportunidad2 = r.IdOportunidad2,
                                PropuestaFirmada = r.PropuestaFirmada,
                                NumeroPace = r.NumeroPace,
                                RutaGear = r.RutaGear,
                                RutaWorkSpace = r.RutaWorkSpace
                            }).OrderBy(r => r.DetPeriodo).ThenBy(r => r.NomGerente).ThenBy(r => r.Ruc)
                                .ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();
                        break;
                    //Gerente
                    case 3:
                        listReporte = await _context.VWReporteGeneral.Where(r => r.FlgActivo == 1 && r.IdGerente == dto.IdGerente
                                                                                    && (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.IdSocio == 0 ? 1 == 1 : r.IdSocio == dto.IdSocio)
                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : r.IdEmpresa == dto.IdEmpresa)
                                                                                    && (dto.IdServicio == 0 ? 1 == 1 : r.IdServicio == dto.IdServicio)
                                                                                    && (dto.IdSubservicio == 0 ? 1 == 1 : r.IdSubservicio == dto.IdSubservicio)
                                                                                    && (dto.IdEstado == 0 ? 1 == 1 : r.IdEstado == dto.IdEstado)
                                                                                    && (dto.IdCondicion == 0 ? 1 == 1 : r.IdCondicion == dto.IdCondicion)
                                                                                    && r.IdArea == dto.IdArea
                                                                                )
                            .Select(r => new VWReporteGeneral
                            {
                                DetPeriodo = r.DetPeriodo,
                                NomSocio = r.NomSocio,
                            //NomGerente = r.NomGerente,
                            Ruc = r.Ruc,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                DetSubservicio = r.DetSubservicio,
                                DetCondicion = r.DetCondicion,
                                DetGrpeco = r.DetGrpeco,
                                DetSector = r.DetSector,
                                DetEstado = r.DetEstado,
                                DetFee = r.DetFee,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                Itan = r.Itan,
                                TarifHoraria = r.TarifHoraria,
                                CantHoras = r.CantHoras,
                                TotalMonto = r.TotalMonto,
                                FeeSublinea = r.FeeSublinea,
                                TarifHorariaSublinea = r.TarifHorariaSublinea,
                                CantHorasSublinea = r.CantHorasSublinea,
                                TotalSublinea = r.TotalSublinea,
                                DetGastos = r.DetGastos,
                                GastosFijos = r.GastosFijos,
                                GastosDetalle = r.GastosDetalle,
                                Engagement1 = r.Engagement1,
                                Engagement2 = r.Engagement2,
                                CompetenciaRdj = r.CompetenciaRdj,
                                QuienGano = r.QuienGano,
                                Comentarios = r.Comentarios,
                                Eaf = r.Eaf,
                                Margen = r.Margen,
                                TotalHoras = r.TotalHoras,
                                IdOportunidad2 = r.IdOportunidad2,
                                PropuestaFirmada = r.PropuestaFirmada,
                                NumeroPace = r.NumeroPace,
                                RutaGear = r.RutaGear,
                                RutaWorkSpace = r.RutaWorkSpace
                            }).OrderBy(r => r.DetPeriodo).ThenBy(r => r.NomSocio).ThenBy(r => r.Ruc)
                                .ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();
                        break;
                    //Socio Lider
                    case 5:
                        listReporte = await _context.VWReporteGeneral.Where(r => r.FlgActivo == 1
                                                                                    && (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.IdSocio == 0 ? 1 == 1 : r.IdSocio == dto.IdSocio)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : r.IdEmpresa == dto.IdEmpresa)
                                                                                    && (dto.IdServicio == 0 ? 1 == 1 : r.IdServicio == dto.IdServicio)
                                                                                    && (dto.IdSubservicio == 0 ? 1 == 1 : r.IdSubservicio == dto.IdSubservicio)
                                                                                    && (dto.IdEstado == 0 ? 1 == 1 : r.IdEstado == dto.IdEstado)
                                                                                    && (dto.IdCondicion == 0 ? 1 == 1 : r.IdCondicion == dto.IdCondicion)
                                                                                    && r.IdArea == dto.IdArea
                                                                                )
                            .Select(r => new VWReporteGeneral
                            {
                                DetPeriodo = r.DetPeriodo,
                                NomSocio = r.NomSocio,
                                NomGerente = r.NomGerente,
                                Ruc = r.Ruc,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                DetSubservicio = r.DetSubservicio,
                                DetCondicion = r.DetCondicion,
                                DetGrpeco = r.DetGrpeco,
                                DetSector = r.DetSector,
                                DetEstado = r.DetEstado,
                                DetFee = r.DetFee,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                Itan = r.Itan,
                                TarifHoraria = r.TarifHoraria,
                                CantHoras = r.CantHoras,
                                TotalMonto = r.TotalMonto,
                                FeeSublinea = r.FeeSublinea,
                                TarifHorariaSublinea = r.TarifHorariaSublinea,
                                CantHorasSublinea = r.CantHorasSublinea,
                                TotalSublinea = r.TotalSublinea,
                                DetGastos = r.DetGastos,
                                GastosFijos = r.GastosFijos,
                                GastosDetalle = r.GastosDetalle,
                                Engagement1 = r.Engagement1,
                                Engagement2 = r.Engagement2,
                                CompetenciaRdj = r.CompetenciaRdj,
                                QuienGano = r.QuienGano,
                                Comentarios = r.Comentarios,
                                Eaf = r.Eaf,
                                Margen = r.Margen,
                                TotalHoras = r.TotalHoras,
                                IdOportunidad2 = r.IdOportunidad2,
                                PropuestaFirmada = r.PropuestaFirmada,
                                NumeroPace = r.NumeroPace,
                                RutaGear = r.RutaGear,
                                RutaWorkSpace = r.RutaWorkSpace
                            }).OrderBy(r => r.DetPeriodo).ThenBy(r => r.NomSocio).ThenBy(r => r.NomGerente)
                                .ThenBy(r => r.Ruc).ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listReporte = await _context.VWReporteGeneral.Where(r => r.FlgActivo == 1
                                                                                    && (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.IdSocio == 0 ? 1 == 1 : r.IdSocio == dto.IdSocio)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : r.IdEmpresa == dto.IdEmpresa)
                                                                                    && (dto.IdServicio == 0 ? 1 == 1 : r.IdServicio == dto.IdServicio)
                                                                                    && (dto.IdSubservicio == 0 ? 1 == 1 : r.IdSubservicio == dto.IdSubservicio)
                                                                                    && (dto.IdEstado == 0 ? 1 == 1 : r.IdEstado == dto.IdEstado)
                                                                                    && (dto.IdCondicion == 0 ? 1 == 1 : r.IdCondicion == dto.IdCondicion)
                                                                                    && r.IdArea == dto.IdArea
                                                                                )
                            .Select(r => new VWReporteGeneral
                            {
                                DetPeriodo = r.DetPeriodo,
                                NomSocio = r.NomSocio,
                                NomGerente = r.NomGerente,
                                Ruc = r.Ruc,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                DetSubservicio = r.DetSubservicio,
                                DetCondicion = r.DetCondicion,
                                DetGrpeco = r.DetGrpeco,
                                DetSector = r.DetSector,
                                DetEstado = r.DetEstado,
                                DetFee = r.DetFee,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                Itan = r.Itan,
                                TarifHoraria = r.TarifHoraria,
                                CantHoras = r.CantHoras,
                                TotalMonto = r.TotalMonto,
                                FeeSublinea = r.FeeSublinea,
                                TarifHorariaSublinea = r.TarifHorariaSublinea,
                                CantHorasSublinea = r.CantHorasSublinea,
                                TotalSublinea = r.TotalSublinea,
                                DetGastos = r.DetGastos,
                                GastosFijos = r.GastosFijos,
                                GastosDetalle = r.GastosDetalle,
                                Engagement1 = r.Engagement1,
                                Engagement2 = r.Engagement2,
                                CompetenciaRdj = r.CompetenciaRdj,
                                QuienGano = r.QuienGano,
                                Comentarios = r.Comentarios,
                                Eaf = r.Eaf,
                                Margen = r.Margen,
                                TotalHoras = r.TotalHoras,
                                IdOportunidad2 = r.IdOportunidad2,
                                PropuestaFirmada = r.PropuestaFirmada,
                                NumeroPace = r.NumeroPace,
                                RutaGear = r.RutaGear,
                                RutaWorkSpace = r.RutaWorkSpace
                            }).OrderBy(r => r.DetPeriodo).ThenBy(r => r.NomSocio).ThenBy(r => r.NomGerente)
                                .ThenBy(r => r.Ruc).ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();
                        break;
                    default:
                        break;
                }
                #endregion

                // Enviamos la data para la generación del reporte
                var memStream = FormatoReporteGeneral(listReporte, dto.IdRol, dto.IdArea);//euscuvil 13-10-2022: Se agrego el area.

                using (FileStream file = new FileStream(rutaVirtualDirectory + filename, FileMode.Create, System.IO.FileAccess.Write))
                {
                    byte[] bytes = new byte[memStream.Length];
                    memStream.Read(bytes, 0, (int)memStream.Length);
                    file.Write(bytes, 0, bytes.Length);
                    memStream.Close();
                }

                //Devolvemos el nombre del archivo para descargarlo mediante el FrontEnd
                return filename;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }            
        }

        #endregion

        #region Reporte Facturacion
        //Formato para reporte de Facturacion
        public MemoryStream FormatoReporteFacturacion(List<VWListarMantProgramacionFacturacion> dto, Int32 rol)
        {
            MemoryStream memStream;

            //Estilos
            var colorHeader = System.Drawing.ColorTranslator.FromHtml("#EBDA00");

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                //name Sheet
                var sheetName = "Reporte";
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                //Font Size
                worksheet.Cells.Style.Font.Name = "EYInterstate Light";
                worksheet.Cells.Style.Font.Size = 10;
                int row = 2;

                //Formato de columnas
                #region Formato columnas
                worksheet.Column(1).Width = 11;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 16;
                worksheet.Column(5).Width = 38;
                worksheet.Column(6).Width = 35;
                worksheet.Column(7).Width = 25;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 30;
                worksheet.Column(10).Width = 32;
                worksheet.Column(11).Width = 15;

                worksheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:K1"].Style.Font.Size = 12;
                worksheet.Cells["A1:K1"].Style.Font.Bold = true;
                worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                worksheet.View.FreezePanes(2, 1);
                #endregion

                //Header
                #region Header
                worksheet.Cells[1, 1].Value = "Oportunidad";
                worksheet.Cells[1, 2].Value = "Socio";
                worksheet.Cells[1, 3].Value = "Gerente";
                worksheet.Cells[1, 4].Value = "Razón Social Op";
                worksheet.Cells[1, 5].Value = "Servicio";
                worksheet.Cells[1, 6].Value = "Engagement";
                worksheet.Cells[1, 7].Value = "Moneda";
                worksheet.Cells[1, 8].Value = "Fee";
                worksheet.Cells[1, 9].Value = "Importe Programado";
                worksheet.Cells[1, 10].Value = "Facturado";
                worksheet.Cells[1, 11].Value = "Saldo a facturar";
                #endregion

                //Body
                #region Body
                row = 2;
                dto.ForEach(data =>
                {
                    worksheet.Cells[row, 1].Value = data.id_oportunidad;
                    worksheet.Cells[row, 2].Value = data.nom_socio;
                    worksheet.Cells[row, 3].Value = data.nom_gerente;
                    worksheet.Cells[row, 4].Value = data.RazonSocial;
                    worksheet.Cells[row, 5].Value = data.DetServicio;
                    worksheet.Cells[row, 6].Value = data.engagement;
                    worksheet.Cells[row, 7].Value = data.DetMoneda;
                    worksheet.Cells[row, 8].Value = data.Fee;
                    worksheet.Cells[row, 9].Value = data.ImporteProgramado;
                    worksheet.Cells[row, 10].Value = data.Facturado;
                    worksheet.Cells[row, 11].Value = data.SaldoFacturar;                   

                    row++;
                });
                #endregion                

                memStream = new MemoryStream(package.GetAsByteArray());
            }

            return memStream;
        }


        //Llamada para el reporte facturacion
        [HttpPost]
        [Route("reporteMantFacturacion")]
        public async Task<String> generarReporteMantFacturacion(DTOMantPrograFacturacion dto)
        {
            try
            {
                //Ruta física - virtual Directory
                //Local
                //var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base de Oportunidades\Reporte\";
                //Prod
                var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base_Oportunidades\Template\";

                //Nombre del archivo
                var dateString = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var hourString = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                var filename = "Facturacion_Oportunidades_" + dateString + "_" + hourString + ".xlsx";

                //Validamos si existe el archivo en la ruta virtual, de ser así, se procede a eliminarlo
                if (System.IO.File.Exists(rutaVirtualDirectory + filename))
                    System.IO.File.Delete(rutaVirtualDirectory + filename);

                //Creamos reporte
                //Obtenemos los registros de la BD
                var listReporte = new List<VWListarMantProgramacionFacturacion>();
                listReporte = await _context.VWListarMantProgramacionFacturacion.Where(r => r.FlgActivo == 1
                                                                                    //&& (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.id_oportunidad == 0 ? 1 == 1 : r.id_oportunidad == dto.id_oportunidad)
                                                                                    && (dto.engagement == null || dto.engagement == "" ? 1 == 1 : r.engagement == dto.engagement)
                                                                                    && (dto.IdFacturado == 0 ? 1 == 1 : r.IdFacturado == dto.IdFacturado)

                                                                                    && (dto.IdSocio == 0 ? 1 == 1 : r.IdSocio == dto.IdSocio)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                )
                            .Select(r => new VWListarMantProgramacionFacturacion
                            {
                                //DetPeriodo = r.DetPeriodo,
                                id_oportunidad = r.id_oportunidad,                                
                                nom_socio = r.nom_socio,
                                nom_gerente = r.nom_gerente,
                                RazonSocial=r.RazonSocial,
                                DetServicio=r.DetServicio,
                                engagement=r.engagement,
                                DetMoneda=r.DetMoneda,
                                Fee=r.Fee,
                                ImporteProgramado=r.ImporteProgramado,
                                Facturado=r.Facturado,
                                SaldoFacturar=r.SaldoFacturar,
                                
                            }).OrderBy(r => r.Id).ThenBy(r => r.nom_socio).ThenBy(r => r.nom_gerente)
                                .ThenBy(r => r.Ruc).ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();

                // Enviamos la data para la generación del reporte
                var memStream = FormatoReporteFacturacion(listReporte, dto.IdRol);//euscuvil 13-10-2022: Se agrego el area.

                using (FileStream file = new FileStream(rutaVirtualDirectory + filename, FileMode.Create, System.IO.FileAccess.Write))
                {
                    byte[] bytes = new byte[memStream.Length];
                    memStream.Read(bytes, 0, (int)memStream.Length);
                    file.Write(bytes, 0, bytes.Length);
                    memStream.Close();
                }

                //Devolvemos el nombre del archivo para descargarlo mediante el FrontEnd
                return filename;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion

        #region Reporte Facturacion Filtrado
        //Formato para reporte de Facturacion
        public MemoryStream FormatoReporteFacturacionFiltra(List<VWListarReporteFacturacion> dto, Int32 rol)
        {
            MemoryStream memStream;

            //Estilos
            var colorHeader = System.Drawing.ColorTranslator.FromHtml("#EBDA00");

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                //name Sheet
                var sheetName = "Reporte";
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                //Font Size
                worksheet.Cells.Style.Font.Name = "EYInterstate Light";
                worksheet.Cells.Style.Font.Size = 10;
                int row = 2;

                //Formato de columnas
                #region Formato columnas
                worksheet.Column(1).Width = 11;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 38;
                worksheet.Column(5).Width = 38;
                worksheet.Column(6).Width = 35;
                worksheet.Column(7).Width = 25;
                worksheet.Column(8).Width = 19;
                worksheet.Column(9).Width = 19;
                worksheet.Column(10).Width = 19;//Fecha darle tambien un formato de 'Fecha'
                worksheet.Column(10).Style.Numberformat.Format = "dd/MM/yyyy";
                worksheet.Column(11).Width = 15;
                worksheet.Column(12).Width = 18;
                //worksheet.Column(11).Width = 15;

                worksheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:L1"].Style.Font.Size = 12;
                worksheet.Cells["A1:L1"].Style.Font.Bold = true;
                worksheet.Cells["A1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:L1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                worksheet.View.FreezePanes(2, 1);
                #endregion

                //Header
                #region Header
                worksheet.Cells[1, 1].Value = "Oportunidad";
                worksheet.Cells[1, 2].Value = "Socio";
                worksheet.Cells[1, 3].Value = "Gerente";
                worksheet.Cells[1, 4].Value = "Razón Social Op";
                worksheet.Cells[1, 5].Value = "Razón Social Fa";
                worksheet.Cells[1, 6].Value = "Servicio";
                worksheet.Cells[1, 7].Value = "Engagement";
                worksheet.Cells[1, 8].Value = "Referencia"; 
                worksheet.Cells[1, 9].Value = "Fact. Consolidada";
                worksheet.Cells[1, 10].Value = "Fecha Estimada";
                worksheet.Cells[1, 11].Value = "Moneda";
                worksheet.Cells[1, 12].Value = "Importe Programado";
                //worksheet.Cells[1, 11].Value = "Saldo a facturar";
                #endregion

                //Body
                #region Body
                row = 2;
                dto.ForEach(data =>
                {

                    worksheet.Cells[row, 1].Value = data.id_oportunidad;
                    worksheet.Cells[row, 2].Value = data.nom_socio;
                    worksheet.Cells[row, 3].Value = data.nom_gerente;
                    worksheet.Cells[row, 4].Value = data.RazonSocial;
                    worksheet.Cells[row, 5].Value = data.RazonSocial;
                    worksheet.Cells[row, 6].Value = data.DetServicio;
                    worksheet.Cells[row, 7].Value = data.engagement;
                    worksheet.Cells[row, 8].Value = data.referencia;
                    worksheet.Cells[row, 9].Value = data.FactConsoli == 0 || data.FactConsoli == null ? "No" : "Si";
                    worksheet.Cells[row, 10].Value = data.fechaEstimada;
                    worksheet.Cells[row, 11].Value = data.DetMoneda;
                    worksheet.Cells[row, 12].Value = data.importeFactura;
                    //worksheet.Cells[row, 11].Value = data.SaldoFacturar;                   

                    row++;
                });
                #endregion                

                memStream = new MemoryStream(package.GetAsByteArray());
            }

            return memStream;
        }


        //Llamada para el reporte facturacion
        [HttpPost]
        [Route("reporteFacturacion")]
        public async Task<String> generarReporteFacturacion(DTOMantPrograFacturacion dto)
        {
            try
            {
                //Ruta física - virtual Directory
                //Local
                //var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base de Oportunidades\Reporte\";
                //Prod
                var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base_Oportunidades\Template\";

                //Nombre del archivo
                var dateString = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var hourString = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                var filename = "Reporte_Facturacion_Oportunidades_" + dateString + "_" + hourString + ".xlsx";

                //Validamos si existe el archivo en la ruta virtual, de ser así, se procede a eliminarlo
                if (System.IO.File.Exists(rutaVirtualDirectory + filename))
                    System.IO.File.Delete(rutaVirtualDirectory + filename);

                /*
                            1: Pendiente 2: Semana en curso 3: Próxima semana
                        Se debe obtener la fecha actual y dependienco del parametro de IdSemana se halla la 
                        fechaInicio y fechaFin
                         */
                DateTime fechaInicio, fechaFin;

                // Obtener la fecha actual y el día de la semana actual
                DateTime fechaActual = DateTime.Now;
                DayOfWeek diaSemanaActual = fechaActual.DayOfWeek;

                // Obtener el lunes de la semana pasada
                DateTime lunesSemanaPasada = fechaActual.AddDays(-(int)diaSemanaActual - 6);

                // Obtener el lunes de la semana actual
                DateTime lunesSemanaActual = fechaActual.AddDays(-(int)diaSemanaActual + 1);

                // Obtener el lunes de la semana próxima
                DateTime lunesSemanaProxima = fechaActual.AddDays(-(int)diaSemanaActual + 8);

                // Obtener el domingo de la semana pasada
                DateTime domingoSemanaPasada = lunesSemanaPasada.AddDays(6);

                // Obtener el domingo de la semana actual
                DateTime domingoSemanaActual = lunesSemanaActual.AddDays(6);

                // Obtener el domingo de la semana próxima
                DateTime domingoSemanaProxima = lunesSemanaProxima.AddDays(6);


                if (dto.IdSemana == 1)
                {
                    fechaInicio = lunesSemanaPasada;
                    fechaFin = domingoSemanaPasada;
                }
                else if (dto.IdSemana == 2)
                {
                    fechaInicio = lunesSemanaActual;
                    fechaFin = domingoSemanaActual;
                }
                else if (dto.IdSemana == 3)
                {
                    fechaInicio = lunesSemanaProxima;
                    fechaFin = domingoSemanaProxima;
                }
                else
                {
                    // En caso de que IdSemana no sea 1, 2 o 3, puedes establecer un valor por defecto o manejar el escenario según tus necesidades.
                    fechaInicio = DateTime.MinValue;
                    fechaFin = DateTime.MinValue;
                }

                // Utiliza las variables fechaInicio y fechaFin según sea necesario en tu código posterior.
                // Establecer las horas, minutos y segundos en 0
                fechaInicio = fechaInicio.Date;
                fechaFin = fechaFin.Date;

                //Creamos reporte
                //Obtenemos los registros de la BD
                var listReporte = new List<VWListarReporteFacturacion>();
                listReporte = await _context.VWListarReporteFacturacion.Where(r => r.FlgActivo == 1
                                                                                    //&& (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.id_oportunidad == 0 ? 1 == 1 : r.id_oportunidad == dto.id_oportunidad)
                                                                                    && (dto.engagement == null || dto.engagement == "" ? 1 == 1 : r.engagement == dto.engagement)
                                                                                    //&& (dto.IdFacturado == 0 ? 1 == 1 : r.IdFacturado == dto.IdFacturado)
                                                                                    && (r.fechaEstimada >= fechaInicio && r.fechaEstimada <= fechaFin)
                                                                                    && (dto.IdSocio == 0 ? 1 == 1 : r.IdSocio == dto.IdSocio)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                )
                            .Select(r => new VWListarReporteFacturacion
                            {
                                //DetPeriodo = r.DetPeriodo,
                                id_oportunidad = r.id_oportunidad,
                                nom_socio = r.nom_socio,
                                nom_gerente = r.nom_gerente,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                engagement = r.engagement,
                                fechaEstimada = r.fechaEstimada,
                                importeFactura = r.importeFactura,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                ImporteProgramado = r.ImporteProgramado,
                                Facturado = r.Facturado,
                                SaldoFacturar = r.SaldoFacturar,
                                FactConsoli = r.FactConsoli,
                                referencia= r.referencia,
                            }).OrderBy(r => r.fechaEstimada).ThenBy(r => r.nom_socio).ThenBy(r => r.nom_gerente)
                                .ThenBy(r => r.Ruc).ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();

                // Enviamos la data para la generación del reporte
                var memStream = FormatoReporteFacturacionFiltra(listReporte, dto.IdRol);//euscuvil 13-10-2022: Se agrego el area.

                using (FileStream file = new FileStream(rutaVirtualDirectory + filename, FileMode.Create, System.IO.FileAccess.Write))
                {
                    byte[] bytes = new byte[memStream.Length];
                    memStream.Read(bytes, 0, (int)memStream.Length);
                    file.Write(bytes, 0, bytes.Length);
                    memStream.Close();
                }

                //Devolvemos el nombre del archivo para descargarlo mediante el FrontEnd
                return filename;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion


        #region Reporte Cuentas
        public MemoryStream FormatoReporteCuentas(List<VWReporteCuentas> dto, Int32 rol)
        {
            MemoryStream memStream;

            //Estilos
            var colorHeader = System.Drawing.ColorTranslator.FromHtml("#EBDA00");

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                //name Sheet
                var sheetName = "Reporte";
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                //Font Size
                worksheet.Cells.Style.Font.Name = "EYInterstate Light";
                worksheet.Cells.Style.Font.Size = 10;

                switch (rol)
                {
                    //Administrador
                    case 1:
                        //Formato de columnas
                        #region Formato columnas
                        worksheet.Column(1).Width = 11;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 20;
                        worksheet.Column(4).Width = 16;
                        worksheet.Column(5).Width = 38;
                        worksheet.Column(6).Width = 35;
                        worksheet.Column(7).Width = 25;
                        worksheet.Column(8).Width = 15;
                        worksheet.Column(9).Width = 30;
                        worksheet.Column(10).Width = 32;
                        worksheet.Column(11).Width = 15;
                        worksheet.Column(12).Width = 15;
                        worksheet.Column(13).Width = 12;
                        worksheet.Column(14).Width = 12;
                        worksheet.Column(15).Width = 12;
                        worksheet.Column(16).Width = 18;
                        worksheet.Column(17).Width = 20;
                        worksheet.Column(18).Width = 16;
                        worksheet.Column(19).Width = 15;
                        worksheet.Column(20).Width = 24;
                        worksheet.Column(21).Width = 24;
                        worksheet.Column(22).Width = 24;
                        worksheet.Column(23).Width = 26;
                        worksheet.Column(24).Width = 16;
                        worksheet.Column(25).Width = 20;
                        worksheet.Column(26).Width = 20;
                        worksheet.Column(27).Width = 28;
                        worksheet.Column(28).Width = 34;
                        worksheet.Column(29).Width = 22;
                        worksheet.Column(30).Width = 32;

                        worksheet.Cells["A1:AD1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A1:AD1"].Style.Font.Size = 12;
                        worksheet.Cells["A1:AD1"].Style.Font.Bold = true;
                        worksheet.Cells["A1:AD1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A1:AD1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                        worksheet.View.FreezePanes(2, 1);
                        #endregion

                        //Header
                        #region Header
                        worksheet.Cells[1, 1].Value = "Periodo";
                        worksheet.Cells[1, 2].Value = "Socio";
                        worksheet.Cells[1, 3].Value = "Gerente";
                        worksheet.Cells[1, 4].Value = "RUC";
                        worksheet.Cells[1, 5].Value = "Razón Social";
                        worksheet.Cells[1, 6].Value = "Servicio";
                        worksheet.Cells[1, 7].Value = "Subservicio";
                        worksheet.Cells[1, 8].Value = "Condición";
                        worksheet.Cells[1, 9].Value = "Grupo Económico";
                        worksheet.Cells[1, 10].Value = "Sector";
                        worksheet.Cells[1, 11].Value = "Estado";
                        worksheet.Cells[1, 12].Value = "Tipo Fee";
                        worksheet.Cells[1, 13].Value = "Moneda";
                        worksheet.Cells[1, 14].Value = "Fee";
                        worksheet.Cells[1, 15].Value = "ITAN";
                        worksheet.Cells[1, 16].Value = "Tarifa Horaria";
                        worksheet.Cells[1, 17].Value = "Cantidad de Horas";
                        worksheet.Cells[1, 18].Value = "Total Soles";
                        worksheet.Cells[1, 19].Value = "Fee Sublínea";
                        worksheet.Cells[1, 20].Value = "Tarifa Horaria Sublínea";
                        worksheet.Cells[1, 21].Value = "Cantidad Hrs. Sublínea";
                        worksheet.Cells[1, 22].Value = "Total Soles Sublínea";
                        worksheet.Cells[1, 23].Value = "Cláusula de Gastos";
                        worksheet.Cells[1, 24].Value = "Gastos Fijos";
                        worksheet.Cells[1, 25].Value = "Detalle de Gastos";
                        worksheet.Cells[1, 26].Value = "Engagement 1";
                        worksheet.Cells[1, 27].Value = "Engagement 2";
                        worksheet.Cells[1, 28].Value = "Competencia RDJ";
                        worksheet.Cells[1, 29].Value = "Quién Ganó?";
                        worksheet.Cells[1, 30].Value = "Comentarios";
                        #endregion

                        //Body
                        #region Body
                        int row = 2;
                        dto.ForEach(data =>
                        {
                            worksheet.Cells[row, 1].Value = data.DetPeriodo;
                            worksheet.Cells[row, 2].Value = data.NomSocio;
                            worksheet.Cells[row, 3].Value = data.NomGerente;
                            worksheet.Cells[row, 4].Value = data.Ruc;
                            worksheet.Cells[row, 5].Value = data.RazonSocial;
                            worksheet.Cells[row, 6].Value = data.DetServicio;
                            worksheet.Cells[row, 7].Value = data.DetSubservicio;
                            worksheet.Cells[row, 8].Value = data.DetCondicion;
                            worksheet.Cells[row, 9].Value = data.DetGrpeco;
                            worksheet.Cells[row, 10].Value = data.DetSector;
                            worksheet.Cells[row, 11].Value = data.DetEstado;
                            worksheet.Cells[row, 12].Value = data.DetFee;
                            worksheet.Cells[row, 13].Value = data.DetMoneda;
                            worksheet.Cells[row, 14].Value = data.Fee;
                            worksheet.Cells[row, 14].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 15].Value = data.Itan;
                            worksheet.Cells[row, 15].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 16].Value = data.TarifHoraria;
                            worksheet.Cells[row, 16].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 17].Value = data.CantHoras;
                            worksheet.Cells[row, 18].Value = data.TotalMonto;
                            worksheet.Cells[row, 18].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 19].Value = data.FeeSublinea;
                            worksheet.Cells[row, 19].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 20].Value = data.TarifHorariaSublinea;
                            worksheet.Cells[row, 20].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 21].Value = data.CantHorasSublinea;
                            worksheet.Cells[row, 22].Value = data.TotalSublinea;
                            worksheet.Cells[row, 22].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 23].Value = data.DetGastos;
                            worksheet.Cells[row, 24].Value = data.GastosFijos;
                            worksheet.Cells[row, 25].Value = data.GastosDetalle;
                            worksheet.Cells[row, 26].Value = data.Engagement1;
                            worksheet.Cells[row, 27].Value = data.Engagement2;
                            worksheet.Cells[row, 28].Value = data.CompetenciaRdj;
                            worksheet.Cells[row, 29].Value = data.QuienGano;
                            worksheet.Cells[row, 30].Value = data.Comentarios;

                            row++;
                        });
                        #endregion
                        break;
                    //Socio
                    case 2:
                        //Formato de columnas
                        #region Formato columnas
                        worksheet.Column(1).Width = 22;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 20;
                        worksheet.Column(4).Width = 16;
                        worksheet.Column(5).Width = 38;
                        worksheet.Column(6).Width = 35;
                        worksheet.Column(7).Width = 25;
                        worksheet.Column(8).Width = 15;
                        worksheet.Column(9).Width = 30;
                        worksheet.Column(10).Width = 32;
                        worksheet.Column(11).Width = 15;
                        worksheet.Column(12).Width = 15;
                        worksheet.Column(13).Width = 12;
                        worksheet.Column(14).Width = 12;
                        worksheet.Column(15).Width = 12;
                        worksheet.Column(16).Width = 18;
                        worksheet.Column(17).Width = 20;
                        worksheet.Column(18).Width = 16;
                        worksheet.Column(19).Width = 15;
                        worksheet.Column(20).Width = 24;
                        worksheet.Column(21).Width = 24;
                        worksheet.Column(22).Width = 24;
                        worksheet.Column(23).Width = 26;
                        worksheet.Column(24).Width = 16;
                        worksheet.Column(25).Width = 20;
                        worksheet.Column(26).Width = 20;
                        worksheet.Column(27).Width = 28;
                        worksheet.Column(28).Width = 34;
                        worksheet.Column(29).Width = 22;
                        worksheet.Column(30).Width = 32;

                        worksheet.Cells["A1:AD1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A1:AD1"].Style.Font.Size = 12;
                        worksheet.Cells["A1:AD1"].Style.Font.Bold = true;
                        worksheet.Cells["A1:AD1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A1:AD1"].Style.Fill.BackgroundColor.SetColor(colorHeader);
                        worksheet.View.FreezePanes(2, 1);
                        #endregion

                        //Header
                        #region Header
                        worksheet.Cells[1, 1].Value = "Periodo";
                        worksheet.Cells[1, 2].Value = "Socio";
                        worksheet.Cells[1, 3].Value = "Gerente";
                        worksheet.Cells[1, 4].Value = "RUC";
                        worksheet.Cells[1, 5].Value = "Razón Social";
                        worksheet.Cells[1, 6].Value = "Servicio";
                        worksheet.Cells[1, 7].Value = "Subservicio";
                        worksheet.Cells[1, 8].Value = "Condición";
                        worksheet.Cells[1, 9].Value = "Grupo Económico";
                        worksheet.Cells[1, 10].Value = "Sector";
                        worksheet.Cells[1, 11].Value = "Estado";
                        worksheet.Cells[1, 12].Value = "Tipo Fee";
                        worksheet.Cells[1, 13].Value = "Moneda";
                        worksheet.Cells[1, 14].Value = "Fee";
                        worksheet.Cells[1, 15].Value = "ITAN";
                        worksheet.Cells[1, 16].Value = "Tarifa Horaria";
                        worksheet.Cells[1, 17].Value = "Cantidad de Horas";
                        worksheet.Cells[1, 18].Value = "Total Soles";
                        worksheet.Cells[1, 19].Value = "Fee Sublínea";
                        worksheet.Cells[1, 20].Value = "Tarifa Horaria Sublínea";
                        worksheet.Cells[1, 21].Value = "Cantidad Hrs. Sublínea";
                        worksheet.Cells[1, 22].Value = "Total Soles Sublínea";
                        worksheet.Cells[1, 23].Value = "Cláusula de Gastos";
                        worksheet.Cells[1, 24].Value = "Gastos Fijos";
                        worksheet.Cells[1, 25].Value = "Detalle de Gastos";
                        worksheet.Cells[1, 26].Value = "Engagement 1";
                        worksheet.Cells[1, 27].Value = "Engagement 2";
                        worksheet.Cells[1, 28].Value = "Competencia RDJ";
                        worksheet.Cells[1, 29].Value = "Quién Ganó?";
                        worksheet.Cells[1, 30].Value = "Comentarios";
                        #endregion

                        //Body
                        #region Body
                        row = 2;
                        dto.ForEach(data =>
                        {
                            worksheet.Cells[row, 1].Value = data.DetPeriodo;
                            worksheet.Cells[row, 2].Value = data.NomSocio2;
                            worksheet.Cells[row, 3].Value = data.NomGerente;
                            worksheet.Cells[row, 4].Value = data.Ruc;
                            worksheet.Cells[row, 5].Value = data.RazonSocial;
                            worksheet.Cells[row, 6].Value = data.DetServicio;
                            worksheet.Cells[row, 7].Value = data.DetSubservicio;
                            worksheet.Cells[row, 8].Value = data.DetCondicion;
                            worksheet.Cells[row, 9].Value = data.DetGrpeco;
                            worksheet.Cells[row, 10].Value = data.DetSector;
                            worksheet.Cells[row, 11].Value = data.DetEstado;
                            worksheet.Cells[row, 12].Value = data.DetFee;
                            worksheet.Cells[row, 13].Value = data.DetMoneda;
                            worksheet.Cells[row, 14].Value = data.Fee;
                            worksheet.Cells[row, 14].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 15].Value = data.Itan;
                            worksheet.Cells[row, 15].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 16].Value = data.TarifHoraria;
                            worksheet.Cells[row, 16].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 17].Value = data.CantHoras;
                            worksheet.Cells[row, 18].Value = data.TotalMonto;
                            worksheet.Cells[row, 18].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 19].Value = data.FeeSublinea;
                            worksheet.Cells[row, 19].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 20].Value = data.TarifHorariaSublinea;
                            worksheet.Cells[row, 20].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 21].Value = data.CantHorasSublinea;
                            worksheet.Cells[row, 22].Value = data.TotalSublinea;
                            worksheet.Cells[row, 22].Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
                            worksheet.Cells[row, 23].Value = data.DetGastos;
                            worksheet.Cells[row, 24].Value = data.GastosFijos;
                            worksheet.Cells[row, 25].Value = data.GastosDetalle;
                            worksheet.Cells[row, 26].Value = data.Engagement1;
                            worksheet.Cells[row, 27].Value = data.Engagement2;
                            worksheet.Cells[row, 28].Value = data.CompetenciaRdj;
                            worksheet.Cells[row, 29].Value = data.QuienGano;
                            worksheet.Cells[row, 30].Value = data.Comentarios;

                            row++;
                        });
                        #endregion
                        break;
                    default:
                        break;
                }

                memStream = new MemoryStream(package.GetAsByteArray());
            }

            return memStream;
        }

        [HttpPost]
        [Route("reporteCuentas")]
        public async Task<String> reporteCuentas(DTOOportunidad dto)
        {
            try
            {
                //Ruta física - virtual Directory
                //Local
                //var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base de Oportunidades\Reporte\";
                //Prod
                var rutaVirtualDirectory = @"C:\Digital Tax\Proyectos\Base_Oportunidades\Template\";

                //Nombre del archivo
                var dateString = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var hourString = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                var filename = "Reporte_Cuentas_" + dateString + "_" + hourString + ".xlsx";

                //Validamos si existe el archivo en la ruta virtual, de ser así, se procede a eliminarlo
                if (System.IO.File.Exists(rutaVirtualDirectory + filename))
                    System.IO.File.Delete(rutaVirtualDirectory + filename);

                //Creamos reporte
                //Obtenemos los registros de la BD
                var listReporte = new List<VWReporteCuentas>();
                #region data
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listReporte = await _context.VWReporteGeneral.Where(r => r.FlgActivo == 1
                                                                                    && (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.IdSocio == 0 ? 1 == 1 : r.IdSocio == dto.IdSocio)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : r.IdEmpresa == dto.IdEmpresa)
                                                                                    && (dto.IdServicio == 0 ? 1 == 1 : r.IdServicio == dto.IdServicio)
                                                                                    && (dto.IdSubservicio == 0 ? 1 == 1 : r.IdSubservicio == dto.IdSubservicio)
                                                                                    && (dto.IdEstado == 0 ? 1 == 1 : r.IdEstado == dto.IdEstado)
                                                                                    && r.IdArea==dto.IdArea
                                                                                )
                            .Select(r => new VWReporteCuentas
                            {
                                DetPeriodo = r.DetPeriodo,
                                NomSocio = r.NomSocio,
                                NomGerente = r.NomGerente,
                                Ruc = r.Ruc,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                DetSubservicio = r.DetSubservicio,
                                DetCondicion = r.DetCondicion,
                                DetGrpeco = r.DetGrpeco,
                                DetSector = r.DetSector,
                                DetEstado = r.DetEstado,
                                DetFee = r.DetFee,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                Itan = r.Itan,
                                TarifHoraria = r.TarifHoraria,
                                CantHoras = r.CantHoras,
                                TotalMonto = r.TotalMonto,
                                FeeSublinea = r.FeeSublinea,
                                TarifHorariaSublinea = r.TarifHorariaSublinea,
                                CantHorasSublinea = r.CantHorasSublinea,
                                TotalSublinea = r.TotalSublinea,
                                DetGastos = r.DetGastos,
                                GastosFijos = r.GastosFijos,
                                GastosDetalle = r.GastosDetalle,
                                Engagement1 = r.Engagement1,
                                Engagement2 = r.Engagement2,
                                CompetenciaRdj = r.CompetenciaRdj,
                                QuienGano = r.QuienGano,
                                Comentarios = r.Comentarios
                            }).OrderBy(r => r.DetPeriodo).ThenBy(r => r.NomSocio).ThenBy(r => r.NomGerente)
                                .ThenBy(r => r.Ruc).ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();
                        break;
                    //Socio
                    case 2:
                        listReporte = await _context.VWReporteCuentas.Where(r => r.FlgActivo == 1 && r.IdSocio == dto.IdSocio
                                                                                    && (dto.IdPeriodo == 0 ? 1 == 1 : r.IdPeriodo == dto.IdPeriodo)
                                                                                    && (dto.IdGerente == 0 ? 1 == 1 : r.IdGerente == dto.IdGerente)
                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : r.IdEmpresa == dto.IdEmpresa)
                                                                                    && (dto.IdServicio == 0 ? 1 == 1 : r.IdServicio == dto.IdServicio)
                                                                                    && (dto.IdSubservicio == 0 ? 1 == 1 : r.IdSubservicio == dto.IdSubservicio)
                                                                                    && (dto.IdEstado == 0 ? 1 == 1 : r.IdEstado == dto.IdEstado)
                                                                                    && r.IdArea == dto.IdArea
                                                                                )
                            .Select(r => new VWReporteCuentas
                            {
                                DetPeriodo = r.DetPeriodo,
                                //NomSocio = r.NomSocio,
                                NomSocio2 = r.NomSocio2,
                                NomGerente = r.NomGerente,
                                Ruc = r.Ruc,
                                RazonSocial = r.RazonSocial,
                                DetServicio = r.DetServicio,
                                DetSubservicio = r.DetSubservicio,
                                DetCondicion = r.DetCondicion,
                                DetGrpeco = r.DetGrpeco,
                                DetSector = r.DetSector,
                                DetEstado = r.DetEstado,
                                DetFee = r.DetFee,
                                DetMoneda = r.DetMoneda,
                                Fee = r.Fee,
                                Itan = r.Itan,
                                TarifHoraria = r.TarifHoraria,
                                CantHoras = r.CantHoras,
                                TotalMonto = r.TotalMonto,
                                FeeSublinea = r.FeeSublinea,
                                TarifHorariaSublinea = r.TarifHorariaSublinea,
                                CantHorasSublinea = r.CantHorasSublinea,
                                TotalSublinea = r.TotalSublinea,
                                DetGastos = r.DetGastos,
                                GastosFijos = r.GastosFijos,
                                GastosDetalle = r.GastosDetalle,
                                Engagement1 = r.Engagement1,
                                Engagement2 = r.Engagement2,
                                CompetenciaRdj = r.CompetenciaRdj,
                                QuienGano = r.QuienGano,
                                Comentarios = r.Comentarios
                            }).OrderBy(r => r.DetPeriodo).ThenBy(r => r.NomGerente).ThenBy(r => r.Ruc)
                                .ThenBy(r => r.RazonSocial).ThenBy(r => r.DetServicio).ToListAsync();
                        break;
                    default:
                        break;
                }
                #endregion

                // Enviamos la data para la generación del reporte
                var memStream = FormatoReporteCuentas(listReporte, dto.IdRol);

                using (FileStream file = new FileStream(rutaVirtualDirectory + filename, FileMode.Create, System.IO.FileAccess.Write))
                {
                    byte[] bytes = new byte[memStream.Length];
                    memStream.Read(bytes, 0, (int)memStream.Length);
                    file.Write(bytes, 0, bytes.Length);
                    memStream.Close();
                }

                //Devolvemos el nombre del archivo para descargarlo mediante el FrontEnd
                return filename;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion

        [HttpPost]
        [Route("auditoriaReporte")]
        public RespondSearchObject<DTO.DTOAuditoria> grabarAuditoria(DTO.DTOAuditoria dto)
        {
            var auditoria = new Models.OpLReporte
            {
                TipoReporte = dto.TipoReporte,
                UsuCreacion = dto.UsuCreacion,
                FecCreacion = dto.FecCreacion
            };
            try
            {
                _context.Add(auditoria);
                _context.SaveChanges();

                return new RespondSearchObject<DTO.DTOAuditoria>()
                {
                    Objeto = dto,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTO.DTOAuditoria>()
                {
                    Objeto = null,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }
    }
}
