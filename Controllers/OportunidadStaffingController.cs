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
using BF.DTO;
using BF.Objeto.Password;
using BF.Objeto.Sesion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace BF.Controllers
{
    [Produces("application/json")]
    [Route("api/staffing")]
    [ApiController]
    public class OportunidadStaffingController : ControllerBase
    {
        private readonly Models.Opotunidades.db_oportunidades _contextOportunidad;
        private readonly Models.Staffing.db_staffing _contextStaffing;
        private readonly IMapper _mapper;
        public OportunidadStaffingController(Models.Opotunidades.db_oportunidades contextOportunidad, Models.Staffing.db_staffing contextStaffing,   IMapper mapper)
        {
            _contextOportunidad = contextOportunidad;
            _contextStaffing = contextStaffing;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("horasSolicitadasOportunidad")]
        public async Task<ActionResult<RespondSearchObject<List<DTOHorasSolcitadasRank>>>> horasSolicitadasOportunidad(DTO.DTOOportunidad dto)
        {
            var solicitud = _contextStaffing.StMSolicitudStaffing.Where(o => o.FlgActivo == 1 && o.IdOportunidad == dto.IdSolicitudStaffing).FirstOrDefault();
            if (solicitud != null)
            { 
                var listHoras = await _contextStaffing.StTSolicitudHrsStaffing.Where(l => l.FlgActivo == 1 && solicitud.Id == l.IdSolicitudStaffing).Select(l => new DTOHorasSolcitadasRank { idRank = (int)l.IdRol, horas = (double)l.HorasSolicitud, fecha = (DateTime)l.FechaSolicitud }).ToListAsync();
            try
            {
                return new RespondSearchObject<List<DTOHorasSolcitadasRank>>()
                {
                    Objeto = listHoras,
                    Mensaje = "Se encontro registros",
                    Flag = true
                };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<DTOHorasSolcitadasRank>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
            }
            else
            {
                return new RespondSearchObject<List<DTOHorasSolcitadasRank>>()
                {
                    Objeto = null,
                    Mensaje = "",
                    Flag = false
                };
            }
        }
        [HttpPost]
        [Route("listarOportunidadJob")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarOportunidad>>>> listarOportunidad(DTO.DTOOportunidadStaffing dto)
        {
            var listOportunidad = new List<VWListarOportunidad>();
            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidad = await _contextOportunidad.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && dto.Engagement1 == o.Engagement1
                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdSubservicio = o.IdSubservicio,
                                DetSubservicio = o.DetSubservicio,
                                IdCondicion = o.IdCondicion,
                                IdGrpeco = o.IdGrpeco,
                                IdSector = o.IdSector,
                                IdEstado = o.IdEstado,
                                IdFee = o.IdFee,
                                IdMoneda = o.IdMoneda,
                                Fee = o.Fee,
                                Itan = o.Itan,
                                TarifHoraria = o.TarifHoraria,
                                CantHoras = o.CantHoras,
                                TotalMonto = o.TotalMonto,
                                FeeSublinea = o.FeeSublinea,
                                TarifHorariaSublinea = o.TarifHorariaSublinea,
                                CantHorasSublinea = o.CantHorasSublinea,
                                TotalSublinea = o.TotalSublinea,
                                IdGastos = o.IdGastos,
                                GastosFijos = o.GastosFijos,
                                GastosDetalle = o.GastosDetalle,
                                Engagement1 = o.Engagement1,
                                Engagement2 = o.Engagement2,
                                CompetenciaRdj = o.CompetenciaRdj,
                                QuienGano = o.QuienGano,
                                Comentarios = o.Comentarios,
                                IdRol = o.IdRol,
                                IdArea = o.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                Ner = o.Ner,
                                PsmFlag = o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio
                    case 2:
                        listOportunidad = await _contextOportunidad.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                && dto.Engagement1 == o.Engagement1
                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdSubservicio = o.IdSubservicio,
                                DetSubservicio = o.DetSubservicio,
                                IdCondicion = o.IdCondicion,
                                IdGrpeco = o.IdGrpeco,
                                IdSector = o.IdSector,
                                IdEstado = o.IdEstado,
                                IdFee = o.IdFee,
                                IdMoneda = o.IdMoneda,
                                Fee = o.Fee,
                                Itan = o.Itan,
                                TarifHoraria = o.TarifHoraria,
                                CantHoras = o.CantHoras,
                                TotalMonto = o.TotalMonto,
                                FeeSublinea = o.FeeSublinea,
                                TarifHorariaSublinea = o.TarifHorariaSublinea,
                                CantHorasSublinea = o.CantHorasSublinea,
                                TotalSublinea = o.TotalSublinea,
                                IdGastos = o.IdGastos,
                                GastosFijos = o.GastosFijos,
                                GastosDetalle = o.GastosDetalle,
                                Engagement1 = o.Engagement1,
                                Engagement2 = o.Engagement2,
                                CompetenciaRdj = o.CompetenciaRdj,
                                QuienGano = o.QuienGano,
                                Comentarios = o.Comentarios,
                                IdRol = o.IdRol,
                                IdArea = o.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                Ner = o.Ner,
                                PsmFlag = o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Gerente
                    case 3:
                        listOportunidad = await _contextOportunidad.VWListarOportunidad.Where(o => o.FlgActivo == 1 
                                                                                         && dto.Engagement1==o.Engagement1
                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdSubservicio = o.IdSubservicio,
                                DetSubservicio = o.DetSubservicio,
                                IdCondicion = o.IdCondicion,
                                IdGrpeco = o.IdGrpeco,
                                IdSector = o.IdSector,
                                IdEstado = o.IdEstado,
                                IdFee = o.IdFee,
                                IdMoneda = o.IdMoneda,
                                Fee = o.Fee,
                                Itan = o.Itan,
                                TarifHoraria = o.TarifHoraria,
                                CantHoras = o.CantHoras,
                                TotalMonto = o.TotalMonto,
                                FeeSublinea = o.FeeSublinea,
                                TarifHorariaSublinea = o.TarifHorariaSublinea,
                                CantHorasSublinea = o.CantHorasSublinea,
                                TotalSublinea = o.TotalSublinea,
                                IdGastos = o.IdGastos,
                                GastosFijos = o.GastosFijos,
                                GastosDetalle = o.GastosDetalle,
                                Engagement1 = o.Engagement1,
                                Engagement2 = o.Engagement2,
                                CompetenciaRdj = o.CompetenciaRdj,
                                QuienGano = o.QuienGano,
                                Comentarios = o.Comentarios,
                                IdRol = o.IdRol,
                                IdArea = o.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                Ner = o.Ner,
                                PsmFlag = o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //EA
                    case 4:
                        listOportunidad = await _contextOportunidad.VWOportunidadEA.Where(o => o.FlgActivo == 1 && dto.Engagement1 == o.Engagement1
                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdSubservicio = o.IdSubservicio,
                                DetSubservicio = o.DetSubservicio,
                                IdCondicion = o.IdCondicion,
                                IdGrpeco = o.IdGrpeco,
                                IdSector = o.IdSector,
                                IdEstado = o.IdEstado,
                                IdFee = o.IdFee,
                                IdMoneda = o.IdMoneda,
                                Fee = o.Fee,
                                Itan = o.Itan,
                                TarifHoraria = o.TarifHoraria,
                                CantHoras = o.CantHoras,
                                TotalMonto = o.TotalMonto,
                                FeeSublinea = o.FeeSublinea,
                                TarifHorariaSublinea = o.TarifHorariaSublinea,
                                CantHorasSublinea = o.CantHorasSublinea,
                                TotalSublinea = o.TotalSublinea,
                                IdGastos = o.IdGastos,
                                GastosFijos = o.GastosFijos,
                                GastosDetalle = o.GastosDetalle,
                                Engagement1 = o.Engagement1,
                                Engagement2 = o.Engagement2,
                                CompetenciaRdj = o.CompetenciaRdj,
                                QuienGano = o.QuienGano,
                                Comentarios = o.Comentarios,
                                IdRol = o.IdRol,
                                IdArea = o.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                Ner = o.Ner,
                                PsmFlag = o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,


                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio Lider
                    case 5:
                        listOportunidad = await _contextOportunidad.VWListarOportunidad.Where(o => o.FlgActivo == 1
                        && dto.Engagement1 == o.Engagement1

                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdSubservicio = o.IdSubservicio,
                                DetSubservicio = o.DetSubservicio,
                                IdCondicion = o.IdCondicion,
                                IdGrpeco = o.IdGrpeco,
                                IdSector = o.IdSector,
                                IdEstado = o.IdEstado,
                                IdFee = o.IdFee,
                                IdMoneda = o.IdMoneda,
                                Fee = o.Fee,
                                Itan = o.Itan,
                                TarifHoraria = o.TarifHoraria,
                                CantHoras = o.CantHoras,
                                TotalMonto = o.TotalMonto,
                                FeeSublinea = o.FeeSublinea,
                                TarifHorariaSublinea = o.TarifHorariaSublinea,
                                CantHorasSublinea = o.CantHorasSublinea,
                                TotalSublinea = o.TotalSublinea,
                                IdGastos = o.IdGastos,
                                GastosFijos = o.GastosFijos,
                                GastosDetalle = o.GastosDetalle,
                                Engagement1 = o.Engagement1,
                                Engagement2 = o.Engagement2,
                                CompetenciaRdj = o.CompetenciaRdj,
                                QuienGano = o.QuienGano,
                                Comentarios = o.Comentarios,
                                IdRol = o.IdRol,
                                IdArea = o.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                Ner = o.Ner,
                                PsmFlag = o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listOportunidad = await _contextOportunidad.VWListarOportunidad.Where(o => o.FlgActivo == 1 && dto.Engagement1 == o.Engagement1

                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdSubservicio = o.IdSubservicio,
                                DetSubservicio = o.DetSubservicio,
                                IdCondicion = o.IdCondicion,
                                IdGrpeco = o.IdGrpeco,
                                IdSector = o.IdSector,
                                IdEstado = o.IdEstado,
                                IdFee = o.IdFee,
                                IdMoneda = o.IdMoneda,
                                Fee = o.Fee,
                                Itan = o.Itan,
                                TarifHoraria = o.TarifHoraria,
                                CantHoras = o.CantHoras,
                                TotalMonto = o.TotalMonto,
                                FeeSublinea = o.FeeSublinea,
                                TarifHorariaSublinea = o.TarifHorariaSublinea,
                                CantHorasSublinea = o.CantHorasSublinea,
                                TotalSublinea = o.TotalSublinea,
                                IdGastos = o.IdGastos,
                                GastosFijos = o.GastosFijos,
                                GastosDetalle = o.GastosDetalle,
                                Engagement1 = o.Engagement1,
                                Engagement2 = o.Engagement2,
                                CompetenciaRdj = o.CompetenciaRdj,
                                QuienGano = o.QuienGano,
                                Comentarios = o.Comentarios,
                                IdRol = o.IdRol,
                                IdArea = o.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                Ner = o.Ner,
                                PsmFlag = o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<VWListarOportunidad>();
                        listOportunidad = emptyList;
                        break;
                }


                if (listOportunidad.Count > 0)
                    return new RespondSearchObject<List<VWListarOportunidad>>()
                    {
                        Objeto = listOportunidad,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else

                    return new RespondSearchObject<List<VWListarOportunidad>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarOportunidad>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("listarTarifaxRank")]
        public async Task<ActionResult<RespondSearchObject<List<DTORankTarifa>>>> listarTarifaxRank(DTO.DTOOportunidad dto)
        {
            var listTarifa = await _contextStaffing.StMRolPersonal.Where(l => l.FlgActivo == 1 && l.IdArea==dto.IdArea).Select(l => new DTORankTarifa { Id = l.Id, Descripcion = l.Descripcion, Tarifa=(float)l.Tarifa }).ToListAsync();
            try {
                return new RespondSearchObject<List<DTORankTarifa>>()
                {
                    Objeto = listTarifa,
                    Mensaje = "Se encontro registros",
                    Flag = true
                };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<DTORankTarifa>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("buscarERPActualxJob")]
        public RespondSearchObject<List<DTO.DTOLista>> buscarERPActual(DTO.DTOOportunidad dto)
        {
            var erpActual = _contextStaffing.StMFinanzas.Where(l => l.FlgActivo == 1 && l.Job == dto.Engagement1).Select(l => new DTOLista() { Id = l.Id, Descripcion = l.ErpActual.ToString() }).ToList();
            try
            {
                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = erpActual,
                    Mensaje = "Se encontro registros",
                    Flag = true
                };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarSolicitudStaffing")]
        public RespondSearchObject<List<DTO.DTOLista>> grabarSolicitudStaffing(DTOGrabarSolicitudStaffing dto) {

            //CHECK IF EXIST
            //Ubicamos el registro a editar
            var entitySolicitud = _contextStaffing.StMSolicitudStaffing.FirstOrDefault(item => item.IdOportunidad == dto.oportunidad.IdOportunidad);
            if (entitySolicitud == null) {
                
                // SAVE NEW

                //DTOGrabarSolicitudStaffing dto
                var entity = new Models.Staffing.StMSolicitudStaffing
                {
                    IdOportunidad = dto.oportunidad.IdOportunidad,
                    HrsAcumuladoLider = dto.oportunidad.HrsAcumuladoLider,
                    HrsAcumuladoSocio = dto.oportunidad.HrsAcumuladoSocio,
                    HrsAcumuladoSeniorManager = dto.oportunidad.HrsAcumuladoSeniorManager,
                    HrsAcumuladoManager = dto.oportunidad.HrsAcumuladoManager,
                    HrsAcumuladoSenior = dto.oportunidad.HrsAcumuladoSenior,
                    HrsAcumuladoStaff = dto.oportunidad.HrsAcumuladoStaff,
                    HrsAcumuladoTrainee = dto.oportunidad.HrsAcumuladoTrainee,

                    TnrProyectado = dto.oportunidad.TnrProyectado,
                    TnrYtd = dto.oportunidad.TnrYtd,
                    DesviacionPresupuesto = dto.oportunidad.DesviacionPresupuesto,

                    TnrProyectadoPercent = dto.oportunidad.TnrProyectadoPercent,
                    TnrYtdPercent = dto.oportunidad.TnrYtdPercent,



                    DesvHorasLider = dto.oportunidad.DesvHorasLider,
                    DesvHorasSocio = dto.oportunidad.DesvHorasSocio,
                    DesvHorasSeniorManager = dto.oportunidad.DesvHorasSeniorManager,
                    DesvHorasManager = dto.oportunidad.DesvHorasManager,
                    DesvHorasSenior = dto.oportunidad.DesvHorasSenior,
                    DesvHorasStaff = dto.oportunidad.DesvHorasStaff,
                    DesvHorasTrainee = dto.oportunidad.DesvHorasTrainee,

                    FlgAprobado = 0,
                    FlgActivo = 1,
                    FecCreacion = DateTime.Now,
                    UsuCreacion = dto.oportunidad.UsuCreacion,
                };
                try
                {
                    _contextStaffing.Add(entity);
                    _contextStaffing.SaveChanges();

                    var idSolicitud = _contextStaffing.StMSolicitudStaffing.Where(n => n.IdOportunidad == dto.oportunidad.IdOportunidad && n.FlgActivo == 1).Select(n => n.Id).FirstOrDefault();
                    foreach (var rank in dto.horasSolcitadasRanks)
                    {
                        var horasRank = new Models.Staffing.StTSolicitudHrsStaffing
                        {
                            IdSolicitudStaffing = idSolicitud,
                            IdRol = rank.idRank,
                            HorasSolicitud = rank.horas,
                            FechaSolicitud = rank.fecha,
                            UsuCreacion = dto.oportunidad.UsuCreacion,
                            FecCreacion = DateTime.Now,
                            FlgActivo=1
                            
                        };
                        _contextStaffing.Add(horasRank);
                        _contextStaffing.SaveChanges();
                    }

                    return new RespondSearchObject<List<DTOLista>>()
                    {
                        Objeto = null,
                        Mensaje = "Se encontro registros",
                        Flag = true
                    };
                }
                catch (Exception ex)
                {
                    return new RespondSearchObject<List<DTOLista>>()
                    {
                        Objeto = null,
                        Mensaje = ex.ToString(),
                        Flag = false
                    };
                }

            }
            else {
                // EDIT
                try {
                    //Registro auditoría 
                    var auditoria = new Models.Staffing.StLSolicitudAuditoria
                    {
                        IdOportunidad = entitySolicitud.IdOportunidad,
                        HrsAcumuladoLider = entitySolicitud.HrsAcumuladoLider,
                        HrsAcumuladoSocio = entitySolicitud.HrsAcumuladoSocio,
                        HrsAcumuladoSeniorManager = entitySolicitud.HrsAcumuladoSeniorManager,
                        HrsAcumuladoManager = entitySolicitud.HrsAcumuladoManager,
                        HrsAcumuladoSenior = entitySolicitud.HrsAcumuladoSenior,
                        HrsAcumuladoStaff = entitySolicitud.HrsAcumuladoStaff,
                        HrsAcumuladoTrainee = entitySolicitud.HrsAcumuladoTrainee,

                        TnrProyectado = entitySolicitud.TnrProyectado,
                        TnrYtd = entitySolicitud.TnrYtd,
                        DesviacionPresupuesto = entitySolicitud.DesviacionPresupuesto,

                        TnrProyectadoPercent = entitySolicitud.TnrProyectadoPercent,
                        TnrYtdPercent = entitySolicitud.TnrYtdPercent,

                        DesvHorasLider = entitySolicitud.DesvHorasLider,
                        DesvHorasSocio = entitySolicitud.DesvHorasSocio,
                        DesvHorasSeniorManager = entitySolicitud.DesvHorasSeniorManager,
                        DesvHorasManager = entitySolicitud.DesvHorasManager,
                        DesvHorasSenior = entitySolicitud.DesvHorasSenior,
                        DesvHorasStaff = entitySolicitud.DesvHorasStaff,
                        DesvHorasTrainee = entitySolicitud.DesvHorasTrainee,

                        UsuCreacion = dto.oportunidad.UsuModificacion,
                        FecCreacion = DateTime.Now,
                        FlgActivo = entitySolicitud.FlgActivo,

                    };

                    _contextStaffing.Add(auditoria);
                    _contextStaffing.SaveChanges();

                    //Editamos registro de staffing
                    #region staffing
                    entitySolicitud.DesvHorasLider = dto.oportunidad.DesvHorasLider;
                    entitySolicitud.DesvHorasSocio = dto.oportunidad.DesvHorasSocio;
                    entitySolicitud.DesvHorasSeniorManager = dto.oportunidad.DesvHorasSeniorManager;
                    entitySolicitud.DesvHorasManager = dto.oportunidad.DesvHorasManager;
                    entitySolicitud.DesvHorasSenior = dto.oportunidad.DesvHorasSenior;
                    entitySolicitud.DesvHorasStaff = dto.oportunidad.DesvHorasStaff;
                    entitySolicitud.DesvHorasTrainee = dto.oportunidad.DesvHorasTrainee;
                    entitySolicitud.DesviacionPresupuesto = dto.oportunidad.DesviacionPresupuesto;

                    entitySolicitud.FlgActivo = 1;

                    entitySolicitud.HrsAcumuladoLider = dto.oportunidad.HrsAcumuladoLider;
                    entitySolicitud.HrsAcumuladoSocio = dto.oportunidad.HrsAcumuladoSocio;
                    entitySolicitud.HrsAcumuladoSeniorManager = dto.oportunidad.HrsAcumuladoSeniorManager;
                    entitySolicitud.HrsAcumuladoManager = dto.oportunidad.HrsAcumuladoManager;
                    entitySolicitud.HrsAcumuladoSenior = dto.oportunidad.HrsAcumuladoSenior;
                    entitySolicitud.HrsAcumuladoStaff = dto.oportunidad.HrsAcumuladoStaff;
                    entitySolicitud.HrsAcumuladoTrainee = dto.oportunidad.HrsAcumuladoTrainee;

                    entitySolicitud.TnrProyectado = dto.oportunidad.TnrProyectado;
                    entitySolicitud.TnrProyectadoPercent = dto.oportunidad.TnrProyectadoPercent;
                    entitySolicitud.TnrYtd = dto.oportunidad.TnrYtd;
                    entitySolicitud.TnrYtdPercent = dto.oportunidad.TnrYtdPercent;

                    entitySolicitud.FecModificacion = DateTime.Now;
                    entitySolicitud.UsuModificacion = dto.oportunidad.UsuModificacion;

                    _contextStaffing.Update(entitySolicitud);
                    _contextStaffing.SaveChanges();
                    #endregion


                    #region horasStaffinxRank
                    var idSolicitud = _contextStaffing.StMSolicitudStaffing.Where(n => n.IdOportunidad == dto.oportunidad.IdOportunidad && n.FlgActivo == 1).Select(n => n.Id).FirstOrDefault();

                    var horasStaffing = _contextStaffing.StTSolicitudHrsStaffing.Where(l => l.IdSolicitudStaffing == idSolicitud).ToList();

                    //Si se encuentran horas previas registradas editar, sino ingresar los registros
                    if (horasStaffing.Count > 0)
                    {
                        //encontrar la fecha minima y maxima ingresada por el usuario(1)
                        //verificar si hay registros anteriores en ese rango de fecha(2)
                            //Todas las fechas previamente registras mayor o igual a la fechaminUsu sera actualizadas, actualizar su flgactivo=0(3)
                        //Registrar las fechas ingresada por el usuario.(4)

                        //(1)
                        var fechaminUsu = horasStaffing[0].FechaSolicitud;
                        var fechamaxUsu = fechaminUsu;
                        foreach (var horasSolcitadasRank in dto.horasSolcitadasRanks)
                        {
                            if (horasSolcitadasRank.fecha < fechaminUsu)
                            {
                                fechaminUsu = horasSolcitadasRank.fecha;
                            }
                            if (horasSolcitadasRank.fecha > fechamaxUsu)
                            {
                                fechamaxUsu = horasSolcitadasRank.fecha;
                            }
                            //(4)
                            var horasRank = new Models.Staffing.StTSolicitudHrsStaffing
                            {
                                IdSolicitudStaffing = idSolicitud,
                                IdRol = horasSolcitadasRank.idRank,
                                HorasSolicitud = horasSolcitadasRank.horas,
                                FechaSolicitud = horasSolcitadasRank.fecha,
                                FlgActivo = 1,
                                UsuCreacion = dto.oportunidad.UsuCreacion,
                                FecCreacion = DateTime.Now
                            };
                            _contextStaffing.Add(horasRank);
                            _contextStaffing.SaveChanges();
                        }
                        //(2)
                        foreach (var horasprev in horasStaffing)
                        {
                            //(3)
                            if (horasprev.FechaSolicitud >= fechaminUsu && horasprev.FlgActivo==1)
                            {                                
                                horasprev.UsuModificacion = dto.oportunidad.UsuModificacion;
                                horasprev.FecModificacion = DateTime.Now;
                                horasprev.FlgActivo = 0;
                                _contextStaffing.Update(horasprev);
                                _contextStaffing.SaveChanges();
                            }
                        }
                        /*
                        //obtener la fecha maxima y minima registrada previamente
                        var fechamin = horasStaffing[0].FechaSolicitud;
                        var fechamax = fechamin;
                        foreach (var rank in horasStaffing) {
                            if (rank.FechaSolicitud < fechamin)
                            {
                                fechamin = rank.FechaSolicitud;
                            }
                            if (rank.FechaSolicitud > fechamax)
                            {
                                fechamax = rank.FechaSolicitud;
                            }
                        }
                        //Si la fecha ingresada por el usuario es menor a la fechamin->Registrar
                        //Si la fecha ingresada por el usuario es mayor a la fechamax->Registrar
                        //Si la fecha ingresada por el usuario se encuentra entre fechamin y fechamax->Actualizar los registros segun lo ingresado por el usuario
                        foreach (var horasSolcitadasRank in dto.horasSolcitadasRanks)
                        {
                            if (horasSolcitadasRank.fecha > fechamax || horasSolcitadasRank.fecha < fechamin)
                            {
                                var horasRank = new Models.Staffing.StTSolicitudHrsStaffing
                                {
                                    IdSolicitudStaffing = idSolicitud,
                                    IdRol = horasSolcitadasRank.idRank,
                                    HorasSolicitud = horasSolcitadasRank.horas,
                                    FechaSolicitud = horasSolcitadasRank.fecha,
                                    UsuCreacion = dto.oportunidad.UsuCreacion,
                                    FecCreacion = DateTime.Now
                                };
                                _contextStaffing.Add(horasRank);
                                _contextStaffing.SaveChanges();
                            }
                            else
                            {
                                //(A)si existe un valor previo en la fecha y rol ingresado por el usuario->Actualizar Valor
                                //(B)si existe un valor previo en la fecha y el usuario no ha ingresado valor en ese Rol->Cambiar FlgActivo
                                foreach (var horasprevias in horasStaffing)
                                {
                                    //buscar la fecha y rol ingresado por el usuario, ver (A)
                                    if (horasprevias.FechaSolicitud == horasSolcitadasRank.fecha && horasprevias.IdRol == horasSolcitadasRank.idRank)
                                    {
                                        horasprevias.HorasSolicitud = horasSolcitadasRank.horas;
                                        _contextStaffing.Update(horasprevias);
                                        _contextStaffing.SaveChanges();

                                    }
                                    else
                                    {
                                        bool prevRegistrado = false;
                                        foreach (var horasSolcitadasRankChecking in dto.horasSolcitadasRanks)
                                        {
                                            //Para las horas previas registradas, que el usuario decidio quitar (B)
                                            if(horasSolcitadasRankChecking.fecha==horasprevias.FechaSolicitud&& horasSolcitadasRankChecking.idRank == horasprevias.IdRol)
                                            {
                                                prevRegistrado = true;
                                                break;
                                            }
                                        }
                                        if (prevRegistrado)
                                        {
                                            //(B)
                                            horasprevias.FlgActivo = 0;
                                            _contextStaffing.Update(horasprevias);
                                            _contextStaffing.SaveChanges();
                                        }
                                    }
                                    
                                }

                                
                                //Si el usuario ingreso en una fecha y rol un valor que no existia previamente->Registrar
                                var checkSolicitud = horasStaffing.Where(a => a.IdRol == horasSolcitadasRank.idRank && a.FechaSolicitud == horasSolcitadasRank.fecha).FirstOrDefault();
                                if (checkSolicitud == null)
                                {
                                    var horasRank = new Models.Staffing.StTSolicitudHrsStaffing
                                    {
                                        IdSolicitudStaffing = idSolicitud,
                                        IdRol = horasSolcitadasRank.idRank,
                                        HorasSolicitud = horasSolcitadasRank.horas,
                                        FechaSolicitud = horasSolcitadasRank.fecha,
                                        FlgActivo=1,
                                        UsuCreacion = dto.oportunidad.UsuCreacion,
                                        FecCreacion = DateTime.Now
                                    };
                                    _contextStaffing.Add(horasRank);
                                    _contextStaffing.SaveChanges();
                                }

                            }
                            
                        }

                        */

                    }
                    else
                    {
                        //registar horas x Rank
                        foreach (var rank in dto.horasSolcitadasRanks)
                        {
                            var horasRank = new Models.Staffing.StTSolicitudHrsStaffing
                            {
                                IdSolicitudStaffing = idSolicitud,
                                IdRol = rank.idRank,
                                HorasSolicitud = rank.horas,
                                FechaSolicitud = rank.fecha,
                                UsuCreacion = dto.oportunidad.UsuCreacion,
                                FecCreacion = DateTime.Now
                            };
                            _contextStaffing.Add(horasRank);
                            _contextStaffing.SaveChanges();
                        }

                    }


                    #endregion
                    return new RespondSearchObject<List<DTOLista>>()
                    {
                        Objeto = null,
                        Mensaje = "Registro exitoso",
                        Flag = true
                    };
                }
                catch (Exception ex)
                {
                    return new RespondSearchObject<List<DTOLista>>()
                    {
                        Objeto = null,
                        Mensaje = ex.ToString(),
                        Flag = false
                    };
                }
            }


        }
        [HttpPost]
        [Route("calcularTNRYTD")]
        public RespondSearchObject<DTOOportunidadAcum> calcularTNRYTD(DTO.DTOOportunidad dto)
        {
            
            var items = _contextStaffing.StMSolicitudStaffing.Where(l => l.FlgActivo == 1 && l.IdOportunidad == dto.Id && l.FlgAprobado == 1).ToList();
            var horasSolicitadas = _contextStaffing.StTSolicitudHrsStaffing.Where(l => l.IdSolicitudStaffing == dto.IdSolicitudStaffing && l.FlgActivo == 1).Select(n=>  new DTOHorasSolicitudStaffing() { 
                Id=n.Id,
                FechaSolicitud=n.FechaSolicitud,
                IdRol=n.Id,
                HorasSolicitud=n.HorasSolicitud
            }).ToList();

            var acumulados = new DTOOportunidadAcum();
            acumulados.HorasLider = 0;
            acumulados.HorasSocio = 0;
            acumulados.HorasSeniorManager = 0;
            acumulados.HorasManager = 0;
            acumulados.HorasSenior = 0;
            acumulados.HorasStaff = 0;
            acumulados.HorasTrainee = 0;
            acumulados.TNRYTD = 0;

            acumulados.horasSolicitudStaffing = horasSolicitadas;
            foreach (var item in items)
            {
                acumulados.HorasLider += item.HrsAcumuladoLider == null ? 0 : item.HrsAcumuladoLider;
                acumulados.HorasSocio += item.HrsAcumuladoSocio==null?0: item.HrsAcumuladoSocio;
                acumulados.HorasSeniorManager += item.HrsAcumuladoSeniorManager == null ? 0 : item.HrsAcumuladoSeniorManager;
                acumulados.HorasManager += item.HrsAcumuladoManager == null ? 0 : item.HrsAcumuladoManager;
                acumulados.HorasSenior += item.HrsAcumuladoSenior == null ? 0 : item.HrsAcumuladoSenior;
                acumulados.HorasStaff += item.HrsAcumuladoStaff == null ? 0 : item.HrsAcumuladoStaff;
                acumulados.HorasTrainee += item.HrsAcumuladoTrainee == null ? 0 : item.HrsAcumuladoTrainee;
                acumulados.TNRYTD += item.TnrYtd == null ? 0 : item.TnrYtd;

            }
           
            try
            {
                return new RespondSearchObject<DTOOportunidadAcum>()
                {
                    Objeto = acumulados,
                    Mensaje = "Se encontro registros",
                    Flag = true
                };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<DTOOportunidadAcum>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }
    }
    
}
