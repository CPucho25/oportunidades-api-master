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

namespace BF.Controllers
{
    [Produces("application/json")]
    [Route("api/empresa")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly Models.Opotunidades.db_oportunidades _context;
        private readonly IMapper _mapper;
        public EmpresaController(Models.Opotunidades.db_oportunidades context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("buscarEmpresa")]
        public RespondSearchObject<List<OpMEmpresas>> buscarEmpresa(DTOEmpresa dto)
        {
            var listEmpresa = _context.OpMEmpresas.Where(item => item.FlgActivo == 1 &&
                                                                (dto.Ruc == "" ? 1 == 1 : EF.Functions.Like(item.Ruc, "%" + dto.Ruc + "%")) &&
                                                                (dto.RazonSocial == "" ? 1 == 1 : EF.Functions.Like(item.RazonSocial, "%" + dto.RazonSocial + "%"))&&
                                                                (dto.IdArea==0?1==1:dto.IdArea==item.IdArea)
                                                            )
                .Select(item => new OpMEmpresas
                {
                    Id = item.Id,
                    Ruc = item.Ruc,
                    RazonSocial = item.RazonSocial,
                    UsuCreacion = item.UsuCreacion,
                    FecCreacion = item.FecCreacion,
                    UsuModificacion = item.UsuModificacion,
                    FecModificacion = item.FecModificacion,
                    FlgActivo = item.FlgActivo
                }).OrderBy(item => item.Ruc).ThenBy(item => item.RazonSocial).ToList();

            if (listEmpresa.Count > 0)
            {
                return new RespondSearchObject<List<OpMEmpresas>>()
                {
                    Objeto = listEmpresa,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMEmpresas>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarEmpresa")]
        public async Task<RespondSearchObject<DTOEmpresa>> grabarEmpresa(DTO.DTOEmpresa dto)
        {
            var entity = new Models.OpMEmpresas
            {
                Ruc = dto.Ruc,
                RazonSocial = dto.RazonSocial,
                UsuCreacion = dto.UsuCreacion,
                FecCreacion = dto.FecCreacion,
                FlgActivo = dto.FlgActivo,
                IdArea=dto.IdArea
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOEmpresa>()
                {
                    Objeto = dto,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOEmpresa>()
                {
                    Objeto = dto,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarEmpresa")]
        public async Task<RespondSearchObject<DTOEmpresa>> editarEmpresa([FromBody] DTOEmpresa dto)
        {
            var entity = _context.OpMEmpresas.FirstOrDefault(item => item.Id == dto.Id);
            // Validate entity is not null
            if (entity != null)
            {
                // Make changes on entity
                entity.Ruc = dto.Ruc;
                entity.RazonSocial = dto.RazonSocial;
                entity.UsuModificacion = dto.UsuModificacion; 
                entity.FecModificacion = dto.FecModificacion; 
                entity.FlgActivo = dto.FlgActivo;
            }
            try
            {
                _context.OpMEmpresas.Update(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOEmpresa>()
                {
                    Objeto = dto,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOEmpresa>()
                {
                    Objeto = dto,
                    Mensaje = "No se actualizo los datos",
                    Flag = false
                };
            } 
        }

    }
}
