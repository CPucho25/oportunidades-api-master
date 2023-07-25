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
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace BF.Controllers
{
    [Produces("application/json")]
    [Route("api/oportunidad")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OportunidadController : ControllerBase
    {
        private readonly Models.Opotunidades.db_oportunidades _context;
        private readonly IMapper _mapper;
        public OportunidadController(Models.Opotunidades.db_oportunidades context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Combos carga inicial
        [HttpPost]
        [Route("listarPeriodo")]
        public RespondSearchObject<List<DTO.DTOLista>> listarPeriodo()
        {
            var lista = _context.OpMPeriodo.Where(l => l.FlgActivo == 1).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarRazonSocial")]
        public RespondSearchObject<List<DTO.DTOLista>> listarRazonSocial(DTOOportunidad dto)
        {//&& l.IdArea==dto.IdArea  euscuvil
            var lista = _context.OpMEmpresas.Where(l => l.FlgActivo == 1).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.RazonSocial
            }).OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarMontoRestante")]
        public RespondSearchObject<List<VWListarMantProgramacionFacturacion>> listarMontoRestante(DTO.DTOMantPrograFacturacion dto)//Cambiar este DTO
        {
            
            var listOportunidad = new List<VWListarMantProgramacionFacturacion>();

            try
            {
                listOportunidad =  _context.VWListarMantProgramacionFacturacion.Where(o => o.FlgActivo == 1
                                                                                   && (dto.engagement == "" || dto.engagement == null ? 1 == 1 : o.engagement == dto.engagement)

                                                                                    )
                            .Select(o => new VWListarMantProgramacionFacturacion
                            {
                                SaldoFacturar = o.SaldoFacturar,
                            }).OrderBy(o => o.id_oportunidad).ThenBy(o => o.id_oportunidad).ToList();


                if (listOportunidad.Count > 0)
                    return new RespondSearchObject<List<VWListarMantProgramacionFacturacion>>()
                    {
                        Objeto = listOportunidad,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else

                    return new RespondSearchObject<List<VWListarMantProgramacionFacturacion>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarMantProgramacionFacturacion>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        


        [HttpPost]
        [Route("listarServicio")]
        public RespondSearchObject<List<DTO.DTOLista>> listarServicio(DTOOportunidad dto)
        {
            //
            var a = dto.IdArea;
            var lista = _context.VWListarServicio.Where(l => (dto.IdPeriodo == 0 ? 1 == 1 : l.IdPeriodo == dto.IdPeriodo)
                                                            && l.FlgActivo == 1
                                                            && (dto.IdArea == 0 ? 1==1:l.IdArea==dto.IdArea)
                                                        )
            .Select(l => new DTO.DTOLista
            {
                Id = l.IdServicio,
                Descripcion = l.Descripcion
            }).Distinct().OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarSubservicio")]
        public RespondSearchObject<List<DTO.DTOLista>> listarSubservicio(DTOOportunidad dto)
        {
            var lista = _context.VWListarSubservicio.Where(l => (dto.IdPeriodo == 0 ? 1 == 1 : l.IdPeriodo == dto.IdPeriodo)
                                                                && (dto.IdServicio == 0 ? 1 == 1 : l.IdServicio == dto.IdServicio)
                                                                && l.FlgActivo == 1
                                                            )
            .Select(l => new DTO.DTOLista
            {
                Id = l.IdSubservicio,
                Descripcion = l.Descripcion
            }).Distinct().OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarMoneda")]
        public RespondSearchObject<List<DTO.DTOLista>> listarMoneda(DTOUsuario dTOUsuario)
        {
            var lista = _context.OpMMoneda.Where(l => l.FlgActivo == 1 && dTOUsuario.IdArea==l.IdArea).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.DescripcionCorta
            }).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarTipo")]
        public RespondSearchObject<List<DTO.DTOLista>> listarTipo(DTOUsuario dTOUsuario)
        {
            var lista = _context.OpMTipo.Where(l => l.FlgActivo == 1 && dTOUsuario.IdArea == l.IdArea).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.DescripcionCorta
            }).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("obtenerTC")]
        public RespondSearchObject<List<VWListarTC>> obtenerTC(DTOTipoCambio dto)
        {
            var listMant = _context.VWListarTC.Where(item => item.FlgActivo == 1 && (dto.IdMoneda == 0 ? 1 == 1 : item.IdMoneda == dto.IdMoneda))
            .Select(item => new VWListarTC
            {
                Id = item.Id,
                IdMoneda = item.IdMoneda,
                Descripcion = item.Descripcion,
                TipoCambio = item.TipoCambio,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.IdMoneda).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<VWListarTC>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<VWListarTC>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("listarCondicion")]
        public RespondSearchObject<List<DTO.DTOLista>> listarCondicion(DTOUsuario dTOUsuario)
        {
            var lista = _context.OpMCondicion.Where(l => l.FlgActivo == 1 && l.IdArea==dTOUsuario.IdArea).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarGrpeco")]
        public RespondSearchObject<List<DTO.DTOLista>> listarGrpeco(DTOUsuario dto)
        {//&& dto.IdArea==l.IdArea   euscuvil
            var lista = _context.OpMGrpeco.Where(l => l.FlgActivo == 1).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarSector")]
        public RespondSearchObject<List<DTO.DTOLista>> listarSector()
        {
            var lista = _context.OpMSector.Where(l => l.FlgActivo == 1).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }
        [HttpPost]
        [Route("listarSocio")]
        public RespondSearchObject<List<DTO.DTOLista>> listarSocio(DTOUsuario dTOUsuario)
        {
            var lista = _context.UsuMUsuario.Where(l => l.FlgActivo == 1 && l.IdArea==dTOUsuario.IdArea && l.IdRol==2).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.DescripcionLarga
            }).OrderBy(l => l.Descripcion).Distinct().ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }
        [HttpPost]
        [Route("listarEstado")]
        public RespondSearchObject<List<DTO.DTOLista>> listarEstado(DTOUsuario dTOUsuario)
        {
            var lista = _context.OpMEstado.Where(l => l.FlgActivo == 1 && dTOUsuario.IdArea==l.IdArea).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarFee")]
        public RespondSearchObject<List<DTO.DTOLista>> listarFee()
        {
            var lista = _context.OpMFee.Where(l => l.FlgActivo == 1).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).ToList();

            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarGastos")]
        public RespondSearchObject<List<DTO.DTOLista>> listarGastos()
        {
            var lista = _context.OpMGastos.Where(l => l.FlgActivo == 1).Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = lista,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }
        #endregion

        [HttpPost]
        [Route("listarOportunidadEmpresa")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarOportunidadXempresa>>>> listarOportunidadEmpresa(DTO.DTOOportunidad dto) 
        {
            var listOportunidadEmp = new List<VWListarOportunidadXempresa>();

            try 
            {
                switch (dto.IdRol) 
                {
                    //Administrador
                    case 1:
                        listOportunidadEmp = await _context.VWListarOportunidadXempresas.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                                    && dto.IdArea==o.IdArea)
                            .Select(o => new VWListarOportunidadXempresa
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdCondicion = o.IdCondicion,
                                DetCondicion = o.DetCondicion,
                                IdGrpeco = o.IdGrpeco,
                                DetGrpeco = o.DetGrpeco,
                                IdSector = o.IdSector,
                                DetSector = o.DetSector,
                                TotOportEmp = o.TotOportEmp
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.RazonSocial).ToListAsync();
                            break;

                    //Gerente
                    case 3:
                        listOportunidadEmp = await _context.VWListarOportunidadXempresas.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                                    && dto.IdArea == o.IdArea)
                            .Select(o => new VWListarOportunidadXempresa
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdCondicion = o.IdCondicion,
                                DetCondicion = o.DetCondicion,
                                IdGrpeco = o.IdGrpeco,
                                DetGrpeco = o.DetGrpeco,
                                IdSector = o.IdSector,
                                DetSector = o.DetSector,
                                TotOportEmp = o.TotOportEmp
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.RazonSocial).ToListAsync();
                        break;

                    //SemiAdministrador
                    case 6:
                        listOportunidadEmp = await _context.VWListarOportunidadXempresas.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                                    && dto.IdArea == o.IdArea)
                            .Select(o => new VWListarOportunidadXempresa
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdCondicion = o.IdCondicion,
                                DetCondicion = o.DetCondicion,
                                IdGrpeco = o.IdGrpeco,
                                DetGrpeco = o.DetGrpeco,
                                IdSector = o.IdSector,
                                DetSector = o.DetSector,
                                TotOportEmp = o.TotOportEmp
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.RazonSocial).ToListAsync();
                        break;

                    default:
                        var emptyList = new List<VWListarOportunidadXempresa>();
                        listOportunidadEmp = emptyList;
                        break;
                }

                if (listOportunidadEmp.Count > 0)
                    return new RespondSearchObject<List<VWListarOportunidadXempresa>>()
                    {
                        Objeto = listOportunidadEmp,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else
                    return new RespondSearchObject<List<VWListarOportunidadXempresa>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarOportunidadXempresa>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [Route("listarOportunidadEmpresaPT")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarOportunidadXempresa>>>> listarOportunidadEmpresaPT(DTO.DTOOportunidad dto)
        {
            var listOportunidadEmp = new List<VWListarOportunidadXempresa>();

            try
            {
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidadEmp = await _context.VWListarOportunidadXempresasPT.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                                    && dto.IdArea == o.IdArea)
                            .Select(o => new VWListarOportunidadXempresa
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                IdCondicion=o.IdCondicion,
                                DetCondicion=o.DetCondicion,
                                RazonSocial = o.RazonSocial,
                                IdGrpeco = o.IdGrpeco,
                                DetGrpeco = o.DetGrpeco,
                                TotOportEmp = o.TotOportEmp
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.RazonSocial).ToListAsync();
                        break;

                    //Gerente
                    case 3:
                        listOportunidadEmp = await _context.VWListarOportunidadXempresasPT.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                                    && dto.IdArea == o.IdArea)
                            .Select(o => new VWListarOportunidadXempresa
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                IdCondicion = o.IdCondicion,
                                DetCondicion = o.DetCondicion,
                                RazonSocial = o.RazonSocial,
                                IdGrpeco = o.IdGrpeco,
                                DetGrpeco = o.DetGrpeco,
                                TotOportEmp = o.TotOportEmp
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.RazonSocial).ToListAsync();
                        break;

                    //SemiAdministrador
                    case 6:
                        listOportunidadEmp = await _context.VWListarOportunidadXempresasPT.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                                    && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                                    && dto.IdArea == o.IdArea)
                            .Select(o => new VWListarOportunidadXempresa
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                IdCondicion = o.IdCondicion,
                                DetCondicion = o.DetCondicion,
                                RazonSocial = o.RazonSocial,
                                IdGrpeco = o.IdGrpeco,
                                DetGrpeco = o.DetGrpeco,
                                TotOportEmp = o.TotOportEmp
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.RazonSocial).ToListAsync();
                        break;

                    default:
                        var emptyList = new List<VWListarOportunidadXempresa>();
                        listOportunidadEmp = emptyList;
                        break;
                }

                if (listOportunidadEmp.Count > 0)
                    return new RespondSearchObject<List<VWListarOportunidadXempresa>>()
                    {
                        Objeto = listOportunidadEmp,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else
                    return new RespondSearchObject<List<VWListarOportunidadXempresa>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarOportunidadXempresa>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("listarOportunidad")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarOportunidad>>>> listarOportunidad(DTO.DTOOportunidad dto)
        {
            var listOportunidad = new List<VWListarOportunidad>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                PsmFlag=o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                Rentabilidad = o.Rentabilidad,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio
                    case 2:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1 && o.IdSocio == dto.IdSocio
                                && dto.IdArea==o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                Rentabilidad = o.Rentabilidad,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Gerente
                    case 3:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1 
                                                                                        && o.IdGerente == dto.IdGerente
                                                                                        && dto.IdArea==o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                         && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                Rentabilidad = o.Rentabilidad,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //EA
                    case 4:
                        listOportunidad = await _context.VWOportunidadEA.Where(o => o.FlgActivo == 1 && o.IdEa == dto.IdEa
                        &&dto.IdArea==dto.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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

                                ErpPresupuesto=o.ErpPresupuesto,
                                Rentabilidad = o.Rentabilidad,
                                HorasSocio =o.HorasSocio,
                                HorasManager=o.HorasManager,
                                HorasSenior=o.HorasSenior,
                                HorasStaff=o.HorasStaff,
                                HorasTrainee=o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniormanager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio Lider
                    case 5:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                        && dto.IdArea==o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                PsmFlag=o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                Rentabilidad = o.Rentabilidad,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                         && o.IdArea==dto.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                PsmFlag=o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                Rentabilidad = o.Rentabilidad,
                                eaf= o.eaf,
                                margen = o.margen,
                                totalHoras = o.totalHoras,
                                idOportunidad = o.idOportunidad,
                                propuestaFirmada = o.propuestaFirmada,
                                numeroPace = o.numeroPace,
                                rutaGear = o.rutaGear,
                                rutaWorkSpace = o.rutaWorkSpace,
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
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
            catch (Exception ex) {
                return new RespondSearchObject<List<VWListarOportunidad>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }                                 
        }
        [HttpPost]
        [Route("listarOportunidadStaffing")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarOportunidad>>>> listarOportunidadStaffing(DTO.DTOOportunidad dto)
        {
            //Estado:Aprobado/Renovado
            var listOportunidad = new List<VWListarOportunidad>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                        && (o.IdEstado==1 || o.IdEstado==6)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio
                    case 2:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1 && o.IdSocio == dto.IdSocio
                                && dto.IdArea == o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                        && (o.IdEstado == 1 || o.IdEstado == 6)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Gerente
                    case 3:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1 && o.IdGerente == dto.IdGerente
                                                                                        && dto.IdArea == o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                        && (o.IdEstado == 1 || o.IdEstado == 6)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //EA
                    case 4:
                        listOportunidad = await _context.VWOportunidadEA.Where(o => o.FlgActivo == 1 && o.IdEa == dto.IdEa
                        && dto.IdArea == dto.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                        && (o.IdEstado == 1 || o.IdEstado == 6)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniormanager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio Lider
                    case 5:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                        && dto.IdArea == o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                        && (o.IdEstado == 1 || o.IdEstado == 6)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                         && o.IdArea == dto.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                        && (o.IdEstado == 1 || o.IdEstado == 6)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
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
        [Route("listarOportunidadPT")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarOportunidad>>>> listarOportunidadPT(DTO.DTOOportunidad dto)
        {
            var listOportunidad = new List<VWListarOportunidad>();

            
            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio
                    case 2:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1 && o.IdSocio == dto.IdSocio
                                && dto.IdArea == o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
                                
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Gerente
                    case 3:
                        listOportunidad = await _context.OpMOportunidad.Where(o => o.FlgActivo == 1 && o.IdSocgercliNavigation.IdGerente == dto.IdGerente
                                                                                        && dto.IdArea == o.IdSocgercliNavigation.IdGerenteNavigation.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdServicio == dto.IdServicio)
                                                                                         && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdSubservicio == dto.IdSubservicio)
                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo =(int) o.IdSocgercliNavigation.IdPeriodo,
                                DetPeriodo = o.IdSocgercliNavigation.IdPeriodoNavigation.Descripcion,
                                IdSocio = (int)o.IdSocgercliNavigation.IdSocio,
                                NomSocio = o.IdSocgercliNavigation.IdSocioNavigation.DescripcionLarga,
                                IdGerente = (int)o.IdSocgercliNavigation.IdGerente,
                                NomGerente = o.IdSocgercliNavigation.IdGerenteNavigation.DescripcionLarga,
                                IdEmpresa = (int)o.IdSocgercliNavigation.IdEmpresa,
                                Ruc = o.IdSocgercliNavigation.IdEmpresaNavigation.Ruc,
                                RazonSocial = o.IdSocgercliNavigation.IdEmpresaNavigation.RazonSocial,
                                IdServicio = (int)o.IdSocgercliNavigation.IdServicio,
                                DetServicio = o.IdSocgercliNavigation.IdServicioNavigation.Descripcion,
                                //IdSubservicio = o.IdSubservicio,
                                //DetSubservicio = o.DetSubservicio,
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
                                IdRol = o.IdSocgercliNavigation.IdGerenteNavigation.IdRol,
                                IdArea = o.IdSocgercliNavigation.IdGerenteNavigation.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                
                                //Ner = _context.OpMJobner.Where(l=>l.FlgActivo==1 && l.Job==int.Parse(o.Engagement1)).Select(n=>n.Ner).FirstOrDefault() ,
                                
                                PorcentajeEjecucion=o.PorcentajeEjecucion,
                                PsmFlag=o.PsmFlag
                                
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //EA
                    case 4:

                        //&& o.IdEa == dto.IdEa
                        listOportunidad = await _context.OpMOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdGerente == dto.IdGerente)
                                                                                        && dto.IdArea == o.IdSocgercliNavigation.IdGerenteNavigation.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdServicio == dto.IdServicio)
                                                                                         && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSocgercliNavigation.IdSubservicio == dto.IdSubservicio)
                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = (int)o.IdSocgercliNavigation.IdPeriodo,
                                DetPeriodo = o.IdSocgercliNavigation.IdPeriodoNavigation.Descripcion,
                                IdSocio = (int)o.IdSocgercliNavigation.IdSocio,
                                NomSocio = o.IdSocgercliNavigation.IdSocioNavigation.DescripcionLarga,
                                IdGerente = (int)o.IdSocgercliNavigation.IdGerente,
                                NomGerente = o.IdSocgercliNavigation.IdGerenteNavigation.DescripcionLarga,
                                IdEmpresa = (int)o.IdSocgercliNavigation.IdEmpresa,
                                Ruc = o.IdSocgercliNavigation.IdEmpresaNavigation.Ruc,
                                RazonSocial = o.IdSocgercliNavigation.IdEmpresaNavigation.RazonSocial,
                                IdServicio = (int)o.IdSocgercliNavigation.IdServicio,
                                DetServicio = o.IdSocgercliNavigation.IdServicioNavigation.Descripcion,
                                //IdSubservicio = o.IdSubservicio,
                                //DetSubservicio = o.DetSubservicio,
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
                                IdRol = o.IdSocgercliNavigation.IdGerenteNavigation.IdRol,
                                IdArea = o.IdSocgercliNavigation.IdGerenteNavigation.IdArea,
                                UsuCreacion = o.UsuCreacion,
                                FecCreacion = o.FecCreacion,
                                UsuModificacion = o.UsuModificacion,
                                FecModificacion = o.FecModificacion,
                                FlgActivo = o.FlgActivo,
                                //Ner = _context.OpMJobner.Where(l => l.FlgActivo == 1 && l.Job == int.Parse(o.Engagement1)).Select(n => n.Ner).FirstOrDefault(),
                                PorcentajeEjecucion = o.PorcentajeEjecucion,
                                PsmFlag = o.PsmFlag
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio Lider
                    case 5:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                        && dto.IdArea == o.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                         && o.IdArea == dto.IdArea
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                HorasLider = o.HorasLider,
                                HorasSeniorManager = o.HorasSeniorManager,
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
        
        //euscuvilca Nuevo cambios
        [HttpPost]
        [Route("grabarProgramacionFact")]
        public RespondSearchObject<DTO.DTOPrograFacturacion> grabarTipo(DTO.DTOPrograFacturacion mant)
        {
            //La estructura de abajo e sla misma la q se envia por el request en el FrontEnd            
            var entity = new Models.OpMPrograFacturacion
            {
                tipo = mant.tipo,
                referencia = mant.referencia,
                IdMoneda = mant.IdMoneda,
                monto = mant.monto,
                facturaConsolidada = mant.factConsoli,
                fecha_estimacion = mant.fecha_estimacion,
                fecha_emision = mant.fecha_emision==null? (DateTime?)null: mant.fecha_emision,
                nroFactura = mant.nroFactura,
                engagement = mant.engagement,
                id_oportunidad = mant.id_oportunidad,
                IdFacturado = mant.IdFacturado,
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOPrograFacturacion>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch(Exception e)
            {
                return new RespondSearchObject<DTOPrograFacturacion>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        
        [HttpPost]
        [Route("editarPrograFact")]
        public async Task<RespondSearchObject<DTOPrograFacturacion>> editarPrograFact([FromBody] DTOPrograFacturacion mant)
        {
            var entity = _context.OpMPrograFacturacion.FirstOrDefault(item => item.Id == mant.Id);
            // Validate entity is not null
            if (entity != null)
            {
                // Make changes on entity
                entity.referencia = mant.referencia;
                entity.monto = mant.monto;
                entity.facturaConsolidada = mant.factConsoli;
                entity.fecha_estimacion = mant.fecha_estimacion;
                entity.fecha_emision = mant.fecha_emision;
                entity.nroFactura= mant.nroFactura;
            }
            try
            {
                // Update entity in DbSet Usuario
                _context.OpMPrograFacturacion.Update(entity);
                // Save changes in database Usuario
                _context.SaveChanges();

                //return msg
                return new RespondSearchObject<DTOPrograFacturacion>()
                {
                    Objeto = mant,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch (Exception e)
            {
                return new RespondSearchObject<DTOPrograFacturacion>()
                {
                    Objeto = mant,
                    Mensaje = "No se actualizo los datos",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("actProgramacionFact")]
        public async Task<RespondSearchObject<DTOActualizarProgramacionFact2>> actProgramacionFac(DTOActualizarProgramacionFact2 mant)
        {            
            try
            {
                //Ubicamos el registro en la tabla Oportunidad a editar
                var entity = _context.OpMOportunidad.FirstOrDefault(item => item.Engagement1 == mant.engagement);
                //La estructura de abajo e sla misma la q se envia por el request en el FrontEnd            
                if (entity != null)
                {
                    entity.rzFacturarDif = mant.rzFacturarDif;
                    entity.rucFacturarDif = mant.rucFacturarDif;
                    entity.nroCompraOc = mant.nroCompraOc;
                    entity.hes = mant.hes;
                    entity.otroDocumento = mant.otroDocumento;

                }

                _context.Update(entity);
                _context.SaveChanges();
                /*
                return new RespondSearchObject<DTOActualizarProgramacionFact2>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
                */
            }
            catch (Exception e)
            {
                return new RespondSearchObject<DTOActualizarProgramacionFact2>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
            //Ubicando la empresa a agregar sus datos de contacto y demás
            //Ubicamos el registro en la tabla Oportunidad a editar

            var entity2 = _context.OpMEmpresas.FirstOrDefault(item => item.Ruc == mant.ruc);
            //La estructura de abajo e sla misma la q se envia por el request en el FrontEnd            
            if (entity2 != null)
            {
                entity2.datosContacto = mant.datosContacto;
                entity2.condicionesProce = mant.condicionesProce;

            }

            try
            {
                _context.Update(entity2);
                _context.SaveChanges();

                return new RespondSearchObject<DTOActualizarProgramacionFact2>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch (Exception e)
            {
                return new RespondSearchObject<DTOActualizarProgramacionFact2>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }

        }


        [HttpPost]
        [Route("eliminarDetFacturacion")]
        public RespondSearchObject<DTO.DTOPrograFacturacion> eliminarDetFacturacion(DTO.DTOPrograFacturacion mant)
        {
           
            //var entity = _context.Find(entidad);

            try
            {
                var entity = new Models.OpMPrograFacturacion
                {
                    Id = mant.Id,
                };

                _context.Remove(entity);
                    _context.SaveChanges();
                    Console.WriteLine("Registro eliminado exitosamente.");
                

                return new RespondSearchObject<DTOPrograFacturacion>()
                {
                    Objeto = { },
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch (Exception e)
            {
                return new RespondSearchObject<DTOPrograFacturacion>()
                {
                    Objeto = { },
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }


        [HttpPost]
        [Route("grabarOporunidadEmpresa")]
        public async Task<RespondSearchObject<DTOMantenimiento>> grabarOporunidadEmpresa([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.OpMEstado.FirstOrDefault(item => item.Id == mant.Id);
            // Validate entity is not null
            if (entity != null)
            {
                // Make changes on entity
                entity.Descripcion = mant.Descripcion;
                entity.UsuModificacion = mant.UsuModificacion;
                entity.FecModificacion = mant.FecModificacion;
                entity.FlgActivo = mant.FlgActivo;
            }
            try
            {
                // Update entity in DbSet Usuario
                _context.OpMEstado.Update(entity);
                // Save changes in database Usuario
                _context.SaveChanges();

                //return msg
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se actualizo los datos",
                    Flag = false
                };
            }
        }
        [HttpPost]
        [Route("editarOporunidadEmpresa")]
        public async Task<RespondSearchObject<DTOOportunidadEmpresa>> editarOporunidadEmpresa(DTO.DTOOportunidadEmpresa dto)
        {
            try
            {
                var ListSocGerCli = new List<DTOLista>();

                ListSocGerCli = await _context.OpRSocGerCli.Where(l => l.IdPeriodo == dto.IdPeriodo && l.IdEmpresa == dto.IdRuc).Select(l => new DTOLista
                {
                    Id = l.Id,
                    Descripcion = l.IdPeriodo + l.IdEmpresa + ""
                }).ToListAsync();

                 //Validamos si se encuentra activo el flag limpiar datos
                foreach (DTOLista SGC in ListSocGerCli)
                {
                    //Ubicamos el registro a editar

                    var entity = _context.OpMOportunidad.FirstOrDefault(item => item.IdSocgercli == SGC.Id );

                    //var entity = _context.OpMOportunidad.FirstOrDefault(item => item.Id == SGC.Id && item.IdCondicion == dto.IdCondicionKey
                    //                                                                          && item.IdGrpeco == dto.IdGrpecoKey
                    //                                                                          && item.IdSector == dto.IdSectorKey);
                    switch (dto.IdArea)
                    {
                        case 1:
                            if (entity != null)
                            {
                                //registro auditoría
                                var auditoria = new Models.OpLAuditoria();

                                //Nuevo registro de auditoría
                                auditoria = new Models.OpLAuditoria
                                {
                                    IdOportunidad = entity.Id,
                                    IdCondicion = dto.IdCondicion,
                                    IdGrpeco = dto.IdGrpeco,
                                    IdSector = dto.IdSector,
                                    IdEstado = entity.IdEstado,
                                    IdFee = entity.IdFee,
                                    IdMoneda = entity.IdMoneda,
                                    Fee = entity.Fee,
                                    Itan = entity.Itan,
                                    TotalMonto = entity.TotalMonto,
                                    IdGastos = entity.IdGastos,
                                    GastosFijos = entity.GastosFijos,
                                    GastosDetalle = entity.GastosDetalle,
                                    Engagement1 = entity.Engagement1,
                                    Engagement2 = entity.Engagement2,
                                    CompetenciaRdj = entity.CompetenciaRdj,
                                    QuienGano = entity.QuienGano,
                                    Comentarios = entity.Comentarios,
                                    UsuCreacion = dto.UsuModificacion,
                                    FecCreacion = DateTime.Now,
                                    FlgActivo = entity.FlgActivo,

                                    PsmFlag = entity.PsmFlag,
                                    erpPresupuesto = entity.erpPresupuesto,

                                    horasLider = entity.horasLider,
                                    horasSeniormanager = entity.horasSeniormanager,
                                    horasSocio = entity.horasSocio,
                                    horasManager = entity.horasManager,
                                    horasSenior = entity.horasSenior,
                                    horasStaff = entity.horasStaff,
                                    horasTrainee = entity.horasTrainee,


                                };

                                _context.Add(auditoria);
                                _context.SaveChanges();

                                //Editamos registro de oportunidad
                                #region Oportunidad
                                entity.Id = entity.Id;
                                entity.IdCondicion = dto.IdCondicion;
                                entity.IdGrpeco = dto.IdGrpeco;
                                entity.IdSector = dto.IdSector;

                                entity.UsuModificacion = dto.UsuModificacion;
                                entity.FecModificacion = dto.FecModificacion;
                                #endregion

                                _context.OpMOportunidad.Update(entity);
                                _context.SaveChanges();
                            }
                            break;
                        case 2:
                            if (entity != null)
                            {
                                //registro auditoría
                                var auditoria = new Models.OpLAuditoria();

                                //Nuevo registro de auditoría
                                auditoria = new Models.OpLAuditoria
                                {
                                    IdOportunidad = entity.Id,
                                    IdCondicion = dto.IdCondicion,
                                    IdGrpeco = dto.IdGrpeco,
                                    //IdSector = dto.IdSector,
                                    IdEstado = entity.IdEstado,
                                    IdFee = entity.IdFee,
                                    IdMoneda = entity.IdMoneda,
                                    Fee = entity.Fee,
                                    Itan = entity.Itan,
                                    TotalMonto = entity.TotalMonto,
                                    IdGastos = entity.IdGastos,
                                    GastosFijos = entity.GastosFijos,
                                    GastosDetalle = entity.GastosDetalle,
                                    Engagement1 = entity.Engagement1,
                                    Engagement2 = entity.Engagement2,
                                    CompetenciaRdj = entity.CompetenciaRdj,
                                    QuienGano = entity.QuienGano,
                                    Comentarios = entity.Comentarios,
                                    UsuCreacion = dto.UsuModificacion,
                                    FecCreacion = DateTime.Now,
                                    FlgActivo = entity.FlgActivo,

                                    PsmFlag = entity.PsmFlag,
                                    erpPresupuesto = entity.erpPresupuesto,
                                    PorcentajeEjecucion=entity.PorcentajeEjecucion,

                                    horasLider = entity.horasLider,
                                    horasSeniormanager = entity.horasSeniormanager,
                                    horasSocio = entity.horasSocio,
                                    horasManager = entity.horasManager,
                                    horasSenior = entity.horasSenior,
                                    horasStaff = entity.horasStaff,
                                    horasTrainee = entity.horasTrainee,


                                };

                                _context.Add(auditoria);
                                _context.SaveChanges();

                                //Editamos registro de oportunidad, no aplica Sector
                                #region Oportunidad
                                entity.Id = entity.Id;
                                entity.IdCondicion = dto.IdCondicion;
                                entity.IdGrpeco = dto.IdGrpeco;
                                //entity.IdSector = dto.IdSector;

                                entity.UsuModificacion = dto.UsuModificacion;
                                entity.FecModificacion = dto.FecModificacion;
                                #endregion

                                _context.OpMOportunidad.Update(entity);
                                _context.SaveChanges();
                            }
                            break;
                        default:
                            break;
                    }
                    
                }

                return new RespondSearchObject<DTOOportunidadEmpresa>()
                {
                    Objeto = dto,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };

            }
            catch (Exception ex)
            {
                return new RespondSearchObject<DTOOportunidadEmpresa>()
                {
                    Objeto = dto,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarOportunidad")]
        public async Task<RespondSearchObject<DTOOportunidad>> editarOportunidad(DTO.DTOOportunidad dto)
        {
            switch (dto.IdArea)
            {
                case 1:
                    //BTC
                    try
                    {
                        //Ubicamos el registro a editar
                        var entity = await _context.OpMOportunidad.FirstOrDefaultAsync(item => item.Id == dto.Id);
                        //Registro auditoría 
                        var auditoria = new Models.OpLAuditoria();

                        //Validamos si se encuentra activo el flag limpiar datos
                        if (dto.FlgLimpiar == 1)
                        {
                            //Limpiamos registro de oportunidad
                            #region Oportunidad
                            entity.Id = dto.Id;
                            //entity.IdCondicion = dto.IdCondicion;
                            //entity.IdGrpeco = dto.IdGrpeco;
                            //entity.IdSector = dto.IdSector;
                            entity.IdEstado = null;
                            entity.IdFee = null;
                            entity.IdMoneda = null;
                            entity.Fee = null;
                            entity.Itan = null;
                            entity.TarifHoraria = null;
                            entity.CantHoras = null;
                            entity.TotalMonto = null;
                            entity.FeeSublinea = null;
                            entity.TarifHorariaSublinea = null;
                            entity.CantHorasSublinea = null;
                            entity.TotalSublinea = null;
                            entity.IdGastos = null;
                            entity.GastosFijos = null;
                            entity.GastosDetalle = null;
                            entity.Engagement1 = null;
                            entity.Engagement2 = null;
                            entity.CompetenciaRdj = null;
                            entity.QuienGano = null;
                            entity.Comentarios = null;
                            entity.UsuModificacion = dto.UsuModificacion;
                            entity.FecModificacion = dto.FecModificacion;
                            entity.FlgActivo = dto.FlgActivo;

                            entity.PsmFlag = dto.PsmFlag;
                            entity.erpPresupuesto = dto.erpPresupuesto;
                            entity.rentabilidad = dto.rentabilidad;

                            entity.erpPresupuesto = null;
                            //INI euscuvil 13-04-2023 Agregando nuevos campos
                            entity.eaf = null;
                            entity.margen = null;
                            entity.totalHoras = null;
                            entity.idOportunidad = null;
                            entity.propuestaFirmada = null;
                            entity.numeroPace = null;
                            entity.rutaGear = null;
                            entity.rutaWorkSpace = null;
                            //Fin euscuvil 13-04-2023 Agregando nuevos campos
                            entity.horasSocio = null;
                            entity.horasManager = null;
                            entity.horasSenior = null;
                            entity.horasStaff = null;
                            entity.horasTrainee = null;
                            entity.horasLider = null;
                            entity.horasSeniormanager = null;
                            #endregion

                            _context.OpMOportunidad.Update(entity);
                            _context.SaveChanges();

                            return new RespondSearchObject<DTOOportunidad>()
                            {
                                Objeto = dto,
                                Mensaje = "Se Actualizo los datos",
                                Flag = true
                            };
                        }
                        //En caso no se encuentre activo el flag limpiar datos, se procederá con la actualización del registro
                        else
                        {
                            //Nuevo registro de auditoría
                            auditoria = new Models.OpLAuditoria
                            {
                                IdOportunidad = entity.Id,
                                IdCondicion = entity.IdCondicion,
                                IdGrpeco = entity.IdGrpeco,
                                IdSector = entity.IdSector,
                                IdEstado = entity.IdEstado,
                                IdFee = entity.IdFee,
                                IdMoneda = entity.IdMoneda,
                                Fee = entity.Fee,
                                Itan = entity.Itan,
                                TotalMonto = entity.TotalMonto,
                                IdGastos = entity.IdGastos,
                                GastosFijos = entity.GastosFijos,
                                GastosDetalle = entity.GastosDetalle,
                                Engagement1 = entity.Engagement1,
                                Engagement2 = entity.Engagement2,
                                CompetenciaRdj = entity.CompetenciaRdj,
                                QuienGano = entity.QuienGano,
                                Comentarios = entity.Comentarios,
                                UsuCreacion = dto.UsuModificacion,
                                FecCreacion = DateTime.Now,
                                FlgActivo = entity.FlgActivo,

                                PsmFlag = entity.PsmFlag,

                                erpPresupuesto = dto.erpPresupuesto,
                                rentabilidad = dto.rentabilidad,//euscuvil 11-01-2023
                                //INI euscuvil 13-04-2023 Agregando nuevos campos
                                eaf = dto.eaf,
                                margen = dto.margen,
                                totalHoras = dto.totalHoras,
                                idOportunidad = dto.idOportunidad,
                                propuestaFirmada = dto.propuestaFirmada,
                                numeroPace = dto.numeroPace,
                                rutaGear = dto.rutaGear,
                                rutaWorkSpace = dto.rutaWorkSpace,
                                //Fin euscuvil 13-04-2023 Agregando nuevos campos

                                horasSocio = dto.horasSocio,
                                horasManager = dto.horasManager,
                                horasSenior = dto.horasSenior,
                                horasStaff = dto.horasStaff,
                                horasTrainee = dto.horasTrainee,
                                horasLider = dto.horasLider,
                                horasSeniormanager = dto.horasSeniormanager,
                                PorcentajeEjecucion=dto.porcentajeEjecucion,
                            };

                            _context.Add(auditoria);
                            _context.SaveChanges();

                            //Editamos registro de oportunidad
                            #region Oportunidad
                            entity.Id = dto.Id;
                            if (dto.IdCondicion != 0)
                            {
                                entity.IdCondicion = dto.IdCondicion;
                            }
                            if (dto.IdGrpeco != 0)
                            {
                                entity.IdGrpeco = dto.IdGrpeco;
                            }
                            if (dto.IdSector != 0)
                            {
                                entity.IdSector = dto.IdSector;
                            }
                            if (dto.IdEstado != 0)
                            {
                                entity.IdEstado = dto.IdEstado;
                            }
                            if (dto.IdFee != 0)
                            {
                                entity.IdFee = dto.IdFee;
                            }
                            if (dto.IdMoneda != 0)
                            {
                                entity.IdMoneda = dto.IdMoneda;
                            }
                            entity.Fee = dto.Fee;
                            entity.Itan = dto.Itan;
                            entity.TarifHoraria = dto.TarifHoraria;
                            entity.CantHoras = dto.CantHoras;
                            entity.TotalMonto = dto.TotalMonto;
                            entity.FeeSublinea = dto.FeeSublinea;
                            entity.TarifHorariaSublinea = dto.TarifHorariaSublinea;
                            entity.CantHorasSublinea = dto.CantHorasSublinea;
                            entity.TotalSublinea = dto.TotalSublinea;
                            if (dto.IdGastos != 0)
                            {
                                entity.IdGastos = dto.IdGastos;
                            }
                            entity.GastosFijos = dto.GastosFijos;
                            entity.GastosDetalle = dto.GastosDetalle;
                            entity.Engagement1 = dto.Engagement1;
                            entity.Engagement2 = dto.Engagement2;
                            entity.CompetenciaRdj = dto.CompetenciaRdj;
                            entity.QuienGano = dto.QuienGano;
                            entity.Comentarios = dto.Comentarios;
                            entity.UsuModificacion = dto.UsuModificacion;
                            entity.FecModificacion = dto.FecModificacion;
                            entity.FlgActivo = dto.FlgActivo;

                            entity.PsmFlag = dto.PsmFlag;
                            entity.erpPresupuesto = dto.erpPresupuesto;
                            entity.rentabilidad = dto.rentabilidad;
                            //INI euscuvil 13-04-2023 Agregando nuevos campos
                            entity.eaf = dto.eaf;
                            entity.margen = dto.margen;
                            entity.totalHoras = dto.totalHoras;
                            entity.idOportunidad = dto.idOportunidad;
                            entity.propuestaFirmada = dto.propuestaFirmada;
                            entity.numeroPace = dto.numeroPace;
                            entity.rutaGear = dto.rutaGear;
                            entity.rutaWorkSpace = dto.rutaWorkSpace;
                            //Fin euscuvil 13-04-2023 Agregando nuevos campos
                            entity.horasSocio = dto.horasSocio;
                            entity.horasManager = dto.horasManager;
                            entity.horasSenior = dto.horasSenior;
                            entity.horasStaff = dto.horasStaff;
                            entity.horasTrainee = dto.horasTrainee;

                            entity.horasLider = dto.horasLider;
                            entity.horasSeniormanager = dto.horasSeniormanager;
                            #endregion

                            _context.OpMOportunidad.Update(entity);
                            _context.SaveChanges();

                            return new RespondSearchObject<DTOOportunidad>()
                            {
                                Objeto = dto,
                                Mensaje = "Se Actualizo los datos",
                                Flag = true
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        return new RespondSearchObject<DTOOportunidad>()
                        {
                            Objeto = dto,
                            Mensaje = ex.ToString(),
                            Flag = false
                        };
                    }

                case 2:
                    //PT
                    try
                    {
                        //Ubicamos el registro a editar
                        var entity = await _context.OpMOportunidad.FirstOrDefaultAsync(item => item.Id == dto.Id);
                        //Registro auditoría 
                        var auditoria = new Models.OpLAuditoria();
                        if (entity != null) {
                            //Update
                            //Validamos si se encuentra activo el flag limpiar datos
                            if (dto.FlgLimpiar == 1)
                            {
                                //Limpiamos registro de oportunidad
                                #region Oportunidad
                                entity.Id = dto.Id;
                                //entity.IdCondicion = dto.IdCondicion;
                                //entity.IdGrpeco = dto.IdGrpeco;
                                //entity.IdSector = dto.IdSector;
                                entity.IdEstado = null;
                                entity.IdFee = null;
                                entity.IdMoneda = null;
                                entity.Fee = null;
                                entity.Itan = null;
                                entity.TarifHoraria = null;
                                entity.CantHoras = null;
                                entity.TotalMonto = null;
                                entity.FeeSublinea = null;
                                entity.TarifHorariaSublinea = null;
                                entity.CantHorasSublinea = null;
                                entity.TotalSublinea = null;
                                entity.IdGastos = null;
                                entity.GastosFijos = null;
                                entity.GastosDetalle = null;
                                entity.Engagement1 = null;
                                entity.Engagement2 = null;
                                entity.CompetenciaRdj = null;
                                entity.QuienGano = null;
                                entity.Comentarios = null;
                                entity.UsuModificacion = dto.UsuModificacion;
                                entity.FecModificacion = dto.FecModificacion;
                                entity.FlgActivo = dto.FlgActivo;

                                entity.PsmFlag = dto.PsmFlag;
                                entity.erpPresupuesto = dto.erpPresupuesto;
                                entity.rentabilidad = dto.rentabilidad;//euscuvil 11-01-2023

                                entity.erpPresupuesto = null;
                                entity.horasSocio = null;
                                entity.horasManager = null;
                                entity.horasSenior = null;
                                entity.horasStaff = null;
                                entity.horasTrainee = null;
                                entity.horasLider = null;
                                entity.horasSeniormanager = null;

                                entity.PorcentajeEjecucion = null;
                                #endregion

                                _context.OpMOportunidad.Update(entity);
                                _context.SaveChanges();

                                return new RespondSearchObject<DTOOportunidad>()
                                {
                                    Objeto = dto,
                                    Mensaje = "Se Actualizo los datos",
                                    Flag = true
                                };
                            }
                            //En caso no se encuentre activo el flag limpiar datos, se procederá con la actualización del registro
                            else
                            {
                                //Nuevo registro de auditoría
                                auditoria = new Models.OpLAuditoria
                                {
                                    IdOportunidad = entity.Id,
                                    IdCondicion = entity.IdCondicion,
                                    IdGrpeco = entity.IdGrpeco,
                                    IdSector = entity.IdSector,
                                    IdEstado = entity.IdEstado,
                                    IdFee = entity.IdFee,
                                    IdMoneda = entity.IdMoneda,
                                    Fee = entity.Fee,
                                    Itan = entity.Itan,
                                    TotalMonto = entity.TotalMonto,
                                    IdGastos = entity.IdGastos,
                                    GastosFijos = entity.GastosFijos,
                                    GastosDetalle = entity.GastosDetalle,
                                    Engagement1 = entity.Engagement1,
                                    Engagement2 = entity.Engagement2,
                                    CompetenciaRdj = entity.CompetenciaRdj,
                                    QuienGano = entity.QuienGano,
                                    Comentarios = entity.Comentarios,
                                    UsuCreacion = dto.UsuModificacion,
                                    FecCreacion = DateTime.Now,
                                    FlgActivo = entity.FlgActivo,

                                    PsmFlag = entity.PsmFlag,

                                    erpPresupuesto = dto.erpPresupuesto,
                                    rentabilidad = dto.rentabilidad,//euscuvil 11-01-2023
                                    horasSocio = dto.horasSocio,
                                    horasManager = dto.horasManager,
                                    horasSenior = dto.horasSenior,
                                    horasStaff = dto.horasStaff,
                                    horasTrainee = dto.horasTrainee,
                                    horasLider = dto.horasLider,
                                    horasSeniormanager = dto.horasSeniormanager,

                                    PorcentajeEjecucion=dto.porcentajeEjecucion,
                                };

                                _context.Add(auditoria);
                                _context.SaveChanges();

                                //Editamos registro de oportunidad
                                #region Oportunidad
                                entity.Id = dto.Id;
                                if (dto.IdCondicion != 0)
                                {
                                    entity.IdCondicion = dto.IdCondicion;
                                }
                                if (dto.IdGrpeco != 0)
                                {
                                    entity.IdGrpeco = dto.IdGrpeco;
                                }
                                if (dto.IdSector != 0)
                                {
                                    entity.IdSector = dto.IdSector;
                                }
                                if (dto.IdEstado != 0)
                                {
                                    entity.IdEstado = dto.IdEstado;
                                }
                                if (dto.IdFee != 0)
                                {
                                    entity.IdFee = dto.IdFee;
                                }
                                if (dto.IdMoneda != 0)
                                {
                                    entity.IdMoneda = dto.IdMoneda;
                                }
                                entity.Fee = dto.Fee;
                                entity.Itan = dto.Itan;
                                entity.TarifHoraria = dto.TarifHoraria;
                                entity.CantHoras = dto.CantHoras;
                                entity.TotalMonto = dto.HonorarioTotal;// dto.TotalMonto;
                                entity.FeeSublinea = dto.FeeSublinea;
                                entity.TarifHorariaSublinea = dto.TarifHorariaSublinea;
                                entity.CantHorasSublinea = dto.CantHorasSublinea;
                                entity.TotalSublinea = dto.TotalSublinea;
                                if (dto.IdGastos != 0)
                                {
                                    entity.IdGastos = dto.IdGastos;
                                }
                                entity.GastosFijos = dto.GastosFijos;
                                entity.GastosDetalle = dto.GastosDetalle;
                                entity.Engagement1 = dto.Engagement1;
                                entity.Engagement2 = dto.Engagement2;
                                entity.CompetenciaRdj = dto.CompetenciaRdj;
                                entity.QuienGano = dto.QuienGano;
                                entity.Comentarios = dto.Comentarios;
                                entity.UsuModificacion = dto.UsuModificacion;
                                entity.FecModificacion = dto.FecModificacion;
                                entity.FlgActivo = dto.FlgActivo;

                                entity.PsmFlag = dto.PsmFlag;
                                entity.erpPresupuesto = dto.erpPresupuesto;
                                entity.rentabilidad = dto.rentabilidad;//euscuvil 11-01-2023
                                entity.horasSocio = dto.horasSocio;
                                entity.horasManager = dto.horasManager;
                                entity.horasSenior = dto.horasSenior;
                                entity.horasStaff = dto.horasStaff;
                                entity.horasTrainee = dto.horasTrainee;

                                entity.horasLider = dto.horasLider;
                                entity.horasSeniormanager = dto.horasSeniormanager;

                                entity.PorcentajeEjecucion = dto.porcentajeEjecucion;
                                #endregion

                                _context.OpMOportunidad.Update(entity);
                                _context.SaveChanges();

                                return new RespondSearchObject<DTOOportunidad>()
                                {
                                    Objeto = dto,
                                    Mensaje = "Se Actualizo los datos",
                                    Flag = true
                                };
                            }

                        }
                        else {
                            //Insert SocGerCliente
                            #region SocGerCliente
                            var socgercli = new Models.OpRSocgercli
                            {
                                IdPeriodo=dto.IdPeriodo,
                                IdGerente = dto.IdGerente,
                                IdSocio = dto.IdSocio,
                                IdEmpresa=dto.IdEmpresa,
                                IdServicio=dto.IdServicio,
                                FecCreacion = dto.FecCreacion,
                                UsuCreacion = dto.UsuCreacion,
                                FlgActivo = 1,
                            };
                            _context.OpRSocGerCli.Add(socgercli);
                            _context.SaveChanges();

                            var lastId = _context.OpRSocGerCli.Where(l => l.IdPeriodo==dto.IdPeriodo && l.FlgActivo == 1 && l.IdGerente == dto.IdGerente && l.IdSocio == dto.IdSocio && l.IdEmpresa == dto.IdEmpresa && l.IdServicio == dto.IdServicio).Select(n => n.Id).ToList().Last();
                            #endregion
                            //Insert Oportunidades
                            //Validamos si se encuentra activo el flag limpiar datos
                            if (dto.FlgLimpiar == 1)
                            {
                                //Limpiamos registro de oportunidad
                                #region Oportunidad
                                //Nuevo registro 
                                var oportunidad = new Models.OpMOportunidad
                                {      
                                    IdSocgercli= lastId,
                                    IdCondicion = null,
                                    IdGrpeco = null,
                                    IdSector = null,
                                    IdEstado = null,
                                    IdFee = null,
                                    IdMoneda = null,
                                    Fee = null,
                                    Itan = null,
                                    TotalMonto = null,
                                    IdGastos = null,
                                    GastosFijos = null,
                                    GastosDetalle = null,
                                    Engagement1 = null,
                                    Engagement2 = null,
                                    CompetenciaRdj = null,
                                    QuienGano = null,
                                    Comentarios = null,
                                    UsuCreacion = dto.UsuModificacion,
                                    FecCreacion = DateTime.Now,
                                    FlgActivo = dto.FlgActivo,

                                    PsmFlag = null,

                                    erpPresupuesto = null,
                                    rentabilidad = null,
                                    horasSocio = null,
                                    horasManager = null,
                                    horasSenior = null,
                                    horasStaff = null,
                                    horasTrainee = null,
                                    horasLider = null,
                                    horasSeniormanager = null,
                                    PorcentajeEjecucion=null,
                                };
                                #endregion

                                _context.OpMOportunidad.Add(entity);
                                _context.SaveChanges();

                                return new RespondSearchObject<DTOOportunidad>()
                                {
                                    Objeto = dto,
                                    Mensaje = "Se Actualizo los datos",
                                    Flag = true
                                };
                            }
                            //En caso no se encuentre activo el flag limpiar datos, se procederá con la actualización del registro
                            else
                            {
                                //Agregar registro de oportunidad
                                /*
                                if (dto.IdSector == 0)
                                    dto.IdSector = 19;
                                */
                                #region Oportunidad               
                                var oportunidad = new Models.OpMOportunidad
                                {
                                    IdSocgercli = lastId,
                                    IdCondicion = dto.IdCondicion,
                                    IdGrpeco = dto.IdGrpeco,
                                    //IdSector = dto.IdSector,
                                    IdEstado = dto.IdEstado,
                                    //IdFee = dto.IdFee,
                                    IdMoneda = dto.IdMoneda,
                                    Fee = dto.Fee,
                                    Itan = dto.Itan,
                                    TotalMonto = dto.TotalMonto,
                                    //IdGastos = dto.IdGastos,
                                    GastosFijos = dto.GastosFijos,
                                    GastosDetalle = dto.GastosDetalle,
                                    Engagement1 = dto.Engagement1,
                                    Engagement2 = dto.Engagement2,
                                    CompetenciaRdj = dto.CompetenciaRdj,
                                    QuienGano = dto.QuienGano,
                                    Comentarios = dto.Comentarios,
                                    UsuCreacion = dto.UsuModificacion,
                                    FecCreacion = DateTime.Now,
                                    FlgActivo = dto.FlgActivo,

                                    PsmFlag = dto.PsmFlag,

                                    erpPresupuesto = dto.erpPresupuesto,
                                    rentabilidad = dto.rentabilidad,//euscuvil 11-01-2023
                                    horasSocio = dto.horasSocio,
                                    horasManager = dto.horasManager,
                                    horasSenior = dto.horasSenior,
                                    horasStaff = dto.horasStaff,
                                    horasTrainee = dto.horasTrainee,
                                    horasLider = dto.horasLider,
                                    horasSeniormanager = dto.horasSeniormanager,
                                    PorcentajeEjecucion = dto.porcentajeEjecucion,
                            };
                                #endregion

                                _context.OpMOportunidad.Add(oportunidad);
                                _context.SaveChanges();

                                return new RespondSearchObject<DTOOportunidad>()
                                {
                                    Objeto = dto,
                                    Mensaje = "Se Actualizo los datos",
                                    Flag = true
                                };
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        return new RespondSearchObject<DTOOportunidad>()
                        {
                            Objeto = dto,
                            Mensaje = ex.ToString(),
                            Flag = false
                        };
                    }

                    
                default:
                    return new RespondSearchObject<DTOOportunidad>()
                    {
                        Objeto = null,
                        Mensaje = "",
                        Flag = false
                    };
            }            
        }

        [HttpPost]
        [Route("visualizarOportunidad")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarOportunidad>>>> visualizarOportunidad(DTO.DTOOportunidad dto)
        {
            var listOportunidad = new List<VWListarOportunidad>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                Rentabilidad = o.Rentabilidad,//euscuvil 11-01-2023
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio
                    case 2:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1 && o.IdSocio == dto.IdSocio
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                    )
                            .Select(o => new VWListarOportunidad
                            {
                                Id = o.Id,
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
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
                                Rentabilidad = o.Rentabilidad,//euscuvil 11-01-2023
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //Socio Lider
                    case 5:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
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
                                Rentabilidad = o.Rentabilidad,//euscuvil 11-01-2023
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ThenBy(o => o.DetServicio).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listOportunidad = await _context.VWListarOportunidad.Where(o => o.FlgActivo == 1
                                                                                        && (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdServicio == 0 ? 1 == 1 : o.IdServicio == dto.IdServicio)
                                                                                        && (dto.IdSubservicio == 0 ? 1 == 1 : o.IdSubservicio == dto.IdSubservicio)
                                                                                        && (dto.IdArea == 0 ? 1 == 1 : o.IdArea == dto.IdArea)
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

                                Ner=o.Ner,
                                PsmFlag=o.PsmFlag,

                                ErpPresupuesto = o.ErpPresupuesto,
                                Rentabilidad = o.Rentabilidad,//euscuvil 11-01-2023
                                HorasSocio = o.HorasSocio,
                                HorasManager = o.HorasManager,
                                HorasSenior = o.HorasSenior,
                                HorasStaff = o.HorasStaff,
                                HorasTrainee = o.HorasTrainee,
                                HorasLider=o.HorasLider,
                                HorasSeniorManager=o.HorasSeniorManager,
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
        [Route("visualizarBusqPrograFacturacion")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarProgramacionFacturacion>>>> visualizarBusqPrograFacturacion(DTO.DTOPrograFacturacion dto)//Cambiar este DTO
        {
            var listOportunidad = new List<VWListarProgramacionFacturacion>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:

                        break;
                    //Socio
                    case 2:

                        break;
                    //Socio Lider
                    case 5:

                        break;
                    //SemiAdministrador
                    case 6:
                        //Aca se deberia llamar al SP, pero antes se debe crear la clase con los atributos
                        //que se devolverá
                        listOportunidad = await _context.VWListarProgramacionFacturacion.Where(o => o.FlgActivo == 1
                                                                                   //&& (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                   && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                   && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                   && (dto.engagement == "0" || dto.engagement == "" || dto.engagement == null ? 1 == 1 : o.engagement == dto.engagement)
                                                                                   && (dto.id_oportunidad == null ||
                                                                                    dto.id_oportunidad == 0
                                                                                    ? 1 == 1 :
                                                                                    o.id_oportunidad == dto.id_oportunidad)
                                                                                    )
                            .Select(o => new VWListarProgramacionFacturacion
                            {
                                Id = o.Id,
                                IdFacturado = o.IdFacturado,
                                DetFacturado = o.DetFacturado,
                                IdSocio = o.IdSocio,
                                IdGerente = o.IdGerente,
                                engagement = o.engagement,
                                tipo = o.tipo,
                                referencia = o.referencia,
                                IdMoneda = o.IdMoneda,
                                DetMoneda = o.DetMoneda,
                                monto= o.monto,
                                fecha_estimacion = o.fecha_estimacion,
                                fecha_emision = o.fecha_emision,
                                nroFactura = o.nroFactura,
                                FactConsoli = o.FactConsoli,                           
                                id_oportunidad = o.id_oportunidad,
                                FlgActivo = o.FlgActivo,
                                /*
                                rzFacturarDif = o.rzFacturarDif,
                                rucFacturarDif = o.rucFacturarDif,
                                nroCompraOc = o.nroCompraOc,
                                hes = o.hes,
                                otroDocumento = o.otroDocumento,
                                condicionesProce = o.condicionesProce,
                                datosContacto = o.datosContacto,*/
                                //IdPeriodo = o.IdPeriodo,
                                //DetPeriodo = o.DetPeriodo,
                                //FlgActivo = o.Ruc,
                            }).OrderBy(o => o.fecha_estimacion).ThenBy(o => o.fecha_estimacion).ToListAsync();
                                    
                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<VWListarProgramacionFacturacion>();
                        listOportunidad = emptyList;
                        break;
                }

                if (listOportunidad.Count > 0)
                    return new RespondSearchObject<List<VWListarProgramacionFacturacion>>()
                    {
                        Objeto = listOportunidad,
                        Mensaje = "Se encontro Registro Si",
                        Flag = true
                    };
                else

                    return new RespondSearchObject<List<VWListarProgramacionFacturacion>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarProgramacionFacturacion>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("visualizarMantPrograFacturacion")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarMantProgramacionFacturacion>>>> visualizarMantPrograFacturacion(DTO.DTOMantPrograFacturacion dto)//Cambiar este DTO
        {
            var listOportunidad = new List<VWListarMantProgramacionFacturacion>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:

                        break;
                    case 2:

                        break;
                    //Socio Lider
                    case 5:

                        break;
                    //SemiAdministrador
                    case 6:
                        //Aca se deberia llamar al SP, pero antes se debe crear la clase con los atributos
                  
                    //Socio      //que se devolverá
                        listOportunidad = await _context.VWListarMantProgramacionFacturacion.Where(o => //o.FlgActivo == 1
                                                                                   //&& (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                  (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                   && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                   && (dto.engagement == "" || dto.engagement == null ||
                                                                                    dto.engagement == "0" 
                                                                                    ? 1 == 1 : 
                                                                                    o.engagement == dto.engagement)
                                                                                    && (dto.id_oportunidad == null ||
                                                                                    dto.id_oportunidad == 0
                                                                                    ? 1 == 1 :
                                                                                    o.id_oportunidad == dto.id_oportunidad)
                                                                                   && (
                                                                                    dto.IdFacturado == 0
                                                                                    ? 1 == 1 : 
                                                                                    (dto.IdFacturado == 1 ? o.Fee == o.Facturado:
                                                                                    o.Facturado< o.Fee)
                                                                                    
                                                                                    )

                                                                                    )
                            .Select(o => new VWListarMantProgramacionFacturacion
                            {
                                Id = o.Id,
                                id_oportunidad = o.id_oportunidad,
                                engagement = o.engagement,
                                IdFacturado = o.IdFacturado,
                                DetFacturado = o.DetFacturado,
                                IdSocio = o.IdSocio,
                                nom_socio = o.nom_socio,
                                IdGerente = o.IdGerente,
                                nom_gerente = o.nom_gerente,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdMoneda = o.IdMoneda,
                                DetMoneda = o.DetMoneda,
                                Fee = o.Fee,
                                ImporteProgramado = o.ImporteProgramado,
                                Facturado = o.Facturado,
                                SaldoFacturar = o.SaldoFacturar,

                                FlgActivo = o.FlgActivo,
                                //IdPeriodo = o.IdPeriodo,
                                //DetPeriodo = o.DetPeriodo,
                                //FlgActivo = o.Ruc,
                                //Nuevos valores
                                rzFacturarDif = o.rzFacturarDif,
                                rucFacturarDif = o.rucFacturarDif,
                                nroCompraOc = o.nroCompraOc,
                                hes = o.hes,
                                otroDocumento = o.otroDocumento,
                                condicionesProce = o.condicionesProce,
                                datosContacto = o.datosContacto,
                            }).OrderBy(o => o.id_oportunidad).ThenBy(o => o.id_oportunidad).ToListAsync();

                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<VWListarMantProgramacionFacturacion>();
                        listOportunidad = emptyList;
                        break;
                }

                if (listOportunidad.Count > 0)
                    return new RespondSearchObject<List<VWListarMantProgramacionFacturacion>>()
                    {
                        Objeto = listOportunidad,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else

                    return new RespondSearchObject<List<VWListarMantProgramacionFacturacion>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarMantProgramacionFacturacion>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("visualizarReporteFacturacion")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarReporteFacturacion>>>> visualizarReporteFacturacion(DTO.DTOMantPrograFacturacion dto)//Cambiar este DTO
        {
            var listOportunidad = new List<VWListarReporteFacturacion>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:

                        break;
                    case 2:

                        break;
                    //Socio Lider
                    case 5:

                        break;
                    //SemiAdministrador
                    case 6:
                        //Aca se deberia llamar al SP, pero antes se debe crear la clase con los atributos
                        //Colocando la fecha en base a la que hará el filtrado
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
                        

                        if (dto.IdSemana == 2)
                        {
                            fechaInicio = lunesSemanaPasada;
                            fechaFin = domingoSemanaPasada;
                        }
                        else if (dto.IdSemana == 3)
                        {
                            fechaInicio = lunesSemanaActual;
                            fechaFin = domingoSemanaActual;
                        }
                        else if (dto.IdSemana == 4)
                        {
                            fechaInicio = lunesSemanaProxima;
                            fechaFin = domingoSemanaProxima;
                        }
                        else
                        {
                            // En caso de que IdSemana no sea 1, 2 o 3, puedes establecer un valor por defecto o manejar el escenario según tus necesidades.
                            fechaInicio = DateTime.MinValue;
                            fechaFin = DateTime.MaxValue;
                        }

                        // Utiliza las variables fechaInicio y fechaFin según sea necesario en tu código posterior.
                        // Establecer las horas, minutos y segundos en 0
                        fechaInicio = fechaInicio.Date;
                        fechaFin = fechaFin.Date;


                        //Socio      //que se devolverá
                        listOportunidad = await _context.VWListarReporteFacturacion.Where(o => o.FlgActivo == 1
                                                                                   //&& (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                   && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                   && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                   && (dto.engagement == "" || dto.engagement == null ? 1 == 1 : o.engagement == dto.engagement)
                                                                                    //Colocando filtros para fecha Inicio y fecha Fin
                                                                                   &&(o.fechaEstimada>= fechaInicio && o.fechaEstimada<= fechaFin)
                                                                                   //&&(dto.IdFacturado==1? o.DetFacturado!="" || o.DetFacturado != null :o.DetFacturado==null|| o.DetFacturado == "")
                                                                                   && (dto.IdFacturado == 1 ? o.nroFactura != "" || o.nroFactura != null : o.nroFactura == null || o.nroFactura == "")
                                                                                    )
                            .Select(o => new VWListarReporteFacturacion
                            {
                                Id = o.Id,
                                id_oportunidad = o.id_oportunidad,
                                engagement = o.engagement,
                                IdFacturado = o.IdFacturado,
                                DetFacturado = o.DetFacturado,
                                IdSocio = o.IdSocio,
                                nom_socio = o.nom_socio,
                                IdGerente = o.IdGerente,
                                nom_gerente = o.nom_gerente,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial,
                                IdServicio = o.IdServicio,
                                DetServicio = o.DetServicio,
                                IdMoneda = o.IdMoneda,
                                DetMoneda = o.DetMoneda,
                                Fee = o.Fee,
                                ImporteProgramado = o.ImporteProgramado,
                                Facturado = o.Facturado,
                                SaldoFacturar = o.SaldoFacturar,
                                fechaEstimada = o.fechaEstimada,
                                importeFactura = o.importeFactura,
                                FlgActivo = o.FlgActivo,
                                FactConsoli = o.FactConsoli,
                                //IdPeriodo = o.IdPeriodo,
                                //DetPeriodo = o.DetPeriodo,
                                //FlgActivo = o.Ruc,
                            }).OrderBy(o => o.fechaEstimada).ThenBy(o => o.fechaEstimada).ToListAsync();//OrderBy(o => o.id_oportunidad).ThenBy(o => o.id_oportunidad).ToListAsync();

                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<VWListarReporteFacturacion>();
                        listOportunidad = emptyList;
                        break;
                }

                if (listOportunidad.Count > 0)
                    return new RespondSearchObject<List<VWListarReporteFacturacion>>()
                    {
                        Objeto = listOportunidad,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else

                    return new RespondSearchObject<List<VWListarReporteFacturacion>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarReporteFacturacion>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }


        [HttpPost]
        [Route("visualizarRelacion")]
        public async Task<ActionResult<RespondSearchObject<List<VWListarRelacionOportunidad>>>> visualizarRelacion(DTO.DTOOportunidad dto)
        {
            var listOportunidad = new List<VWListarRelacionOportunidad>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidad = await _context.VWListarRelacionOportunidad.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                    )
                            .Select(o => new VWListarRelacionOportunidad
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                RazonSocial = o.RazonSocial
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomSocio).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listOportunidad = await _context.VWListarRelacionOportunidad.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                        && (dto.IdArea == 0 ? 1 == 1 : o.IdArea == dto.IdArea)
                                                                                    )
                            .Select(o => new VWListarRelacionOportunidad
                            {
                                IdPeriodo = o.IdPeriodo,
                                DetPeriodo = o.DetPeriodo,
                                IdSocio = o.IdSocio,
                                NomSocio = o.NomSocio,
                                IdGerente = o.IdGerente,
                                NomGerente = o.NomGerente,
                                IdEmpresa = o.IdEmpresa,
                                Ruc = o.Ruc,
                                IdArea = o.IdArea,//Se agregó - euscuvil
                                RazonSocial = o.RazonSocial
                            }).OrderBy(o => o.DetPeriodo).ThenBy(o => o.NomSocio).ThenBy(o => o.NomGerente)
                                    .ThenBy(o => o.RazonSocial).ToListAsync();
                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<VWListarRelacionOportunidad>();
                        listOportunidad = emptyList;
                        break;
                }

                if (listOportunidad.Count > 0)
                    return new RespondSearchObject<List<VWListarRelacionOportunidad>>()
                    {
                        Objeto = listOportunidad,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else

                    return new RespondSearchObject<List<VWListarRelacionOportunidad>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWListarRelacionOportunidad>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("visualizarRelacionServicio")]
        public async Task<ActionResult<RespondSearchObject<List<VWRelacionServicio>>>> visualizarRelacionServicio(DTO.DTOOportunidad dto)
        {
            var listOportunidad = new List<VWRelacionServicio>();

            try
            {
                //Validamos el rol del usuario quién está solicitando la información
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listOportunidad = await _context.VWRelacionServicio.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                    )
                            .Select(o => new VWRelacionServicio
                            {
                                Value = o.Value,
                                Label = o.Label,
                                IdPeriodo = o.IdPeriodo,
                                IdSocio = o.IdSocio,
                                IdGerente = o.IdGerente,
                                IdEmpresa = o.IdEmpresa
                            }).OrderBy(o => o.Label).ToListAsync();
                        break;
                    //SemiAdministrador
                    case 6:
                        listOportunidad = await _context.VWRelacionServicio.Where(o => (dto.IdPeriodo == 0 ? 1 == 1 : o.IdPeriodo == dto.IdPeriodo)
                                                                                        && (dto.IdSocio == 0 ? 1 == 1 : o.IdSocio == dto.IdSocio)
                                                                                        && (dto.IdGerente == 0 ? 1 == 1 : o.IdGerente == dto.IdGerente)
                                                                                        && (dto.IdEmpresa == 0 ? 1 == 1 : o.IdEmpresa == dto.IdEmpresa)
                                                                                    )
                            .Select(o => new VWRelacionServicio
                            {
                                Value = o.Value,
                                Label = o.Label,
                                IdPeriodo = o.IdPeriodo,
                                IdSocio = o.IdSocio,
                                IdGerente = o.IdGerente,
                                IdEmpresa = o.IdEmpresa
                            }).OrderBy(o => o.Label).ToListAsync();
                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<VWRelacionServicio>();
                        listOportunidad = emptyList;
                        break;
                }

                if (listOportunidad.Count > 0)
                    return new RespondSearchObject<List<VWRelacionServicio>>()
                    {
                        Objeto = listOportunidad,
                        Mensaje = "Se encontro Registro",
                        Flag = true
                    };
                else

                    return new RespondSearchObject<List<VWRelacionServicio>>()
                    {
                        Objeto = null,
                        Mensaje = "No se encontro Registro",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<VWRelacionServicio>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

    }
}
