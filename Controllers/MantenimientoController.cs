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
    [Route("api/mantenimiento")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MantenimientoController : ControllerBase
    {
        private readonly Models.Opotunidades.db_oportunidades _context;
        private readonly IMapper _mapper;
        public MantenimientoController(Models.Opotunidades.db_oportunidades context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }   

        [HttpPost]
        [Route("listaRol")]
        public RespondSearchObject<List<DTO.DTOLista>> listarRol()
        {
            var listRol = _context.UsuMRol.Where(l => l.Id != 1)
            .Select(l => new DTO.DTOLista {
                Id=l.Id,
                Descripcion=l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = listRol,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarRolMeta")]
        public RespondSearchObject<List<DTO.DTOLista>> listarRolMeta()
        {
            var listRol = _context.UsuMRol.Where(l => (l.Id == 2 || l.Id == 3))
            .Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = listRol,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }
        [HttpPost]
        [Route("listarFacturadoMeta")]
        public RespondSearchObject<List<DTO.DTOLista>> listarFacturadoMeta()
        {
            var listRol = _context.OpMFacturacion.Where(l => (l.Id == 2 || l.Id == 3))
            .Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = listRol,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }
        [HttpPost]
        [Route("listaFacturado")]
        public RespondSearchObject<List<DTO.DTOLista>> listaFacturado()
        {
            //Antes estaba con Where(l => l.Id != 1)
            var listRol = _context.OpMFacturacion.Where(l => l.Id != 0)
            .Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = listRol,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listaArea")]
        public RespondSearchObject<List<DTO.DTOLista>> listaArea()
        {
            var listRol = _context.UsuMArea.Select(l => new DTO.DTOLista
            {
                Id = l.Id,
                Descripcion = l.Descripcion
            }).OrderBy(l => l.Descripcion).ToList();
            return new RespondSearchObject<List<DTO.DTOLista>>()
            {
                Objeto = listRol,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("listarGerente")]
        public RespondSearchObject<List<DTOLista>> listarGerente(DTO.DTOOportunidad dto)
        {            
            try
            {
                var listGerente = new List<DTOLista>();
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listGerente = _context.VWListarGerente.Where(p => (dto.IdPeriodo == 0 ? p.IdPeriodo == 1 : p.IdPeriodo == dto.IdPeriodo))
                        .Select(l => new DTOLista
                        {
                            Id = l.IdGerente,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();

                        break;
                    //Socio
                    case 2:
                        listGerente = _context.VWListarGerente.Where(p => (dto.IdPeriodo == 0 ? p.IdPeriodo == 1 : p.IdPeriodo == dto.IdPeriodo) 
                        &&p.IdArea==dto.IdArea                                                    
                        && p.IdSocio == dto.IdUsuario)
                        .Select(l => new DTOLista
                        {
                            Id = l.IdGerente,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();
                        
                        break;
                    //EA
                    case 4:
                        switch (dto.IdArea)
                        {
                            case 1:
                                listGerente = _context.VWListarGerentexEA.Where(p => p.IdEa == dto.IdUsuario)
                                    .Select(l => new DTOLista
                                    {
                                        Id = l.IdGerente,
                                        Descripcion = l.Descripcion
                                    }).Distinct().OrderBy(l => l.Descripcion).ToList();
                                break;
                            case 2:
                                listGerente = _context.UsuMUsuario.Where(p=>p.IdRol==3 && p.FlgActivo==1 && p.IdArea==dto.IdArea)
                                    .Select(l => new DTOLista
                                    {
                                        Id = l.Id,
                                        Descripcion = l.DescripcionLarga
                                    }).Distinct().OrderBy(l => l.Descripcion).ToList();
                                break;
                            default:
                                break;
                        }
                        break;

                        

                        //break;
                    //Socio Lider
                    case 5:
                        listGerente = _context.VWListarGerente.Where(p => (dto.IdPeriodo == 0 ? p.IdPeriodo == 1 : p.IdPeriodo == dto.IdPeriodo))
                        .Select(l => new DTOLista
                        {
                            Id = l.IdGerente,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();

                        break;
                    //SemiAdministrador
                    case 6:
                        listGerente = _context.VWListarGerente.Where(p => (dto.IdPeriodo == 0 ? p.IdPeriodo == 1 : p.IdPeriodo == dto.IdPeriodo) && p.IdArea == dto.IdArea)
                        .Select(l => new DTOLista
                        {
                            Id = l.IdGerente,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();

                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<DTOLista>();
                        listGerente = emptyList;
                        break;
                }
                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = listGerente,
                    Mensaje = "Se Actualizo los datos",
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
        [Route("listarSocio")]
        public RespondSearchObject<List<DTOLista>> listarSocio(DTO.DTOOportunidad dto)
        {
            try
            {
                var listSocio = new List<DTOLista>();
                switch (dto.IdRol)
                {
                    //Administrador
                    case 1:
                        listSocio = _context.VWListarSocio.Where(p => (dto.IdPeriodo == 0 ? 1 == 1 : p.IdPeriodo == dto.IdPeriodo))
                        .Select(l => new DTOLista
                        {
                            Id = l.IdSocio,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();

                        break;
                    //Gerente
                    case 3:
                        listSocio = _context.VWListarSocio.Where(p => (dto.IdPeriodo == 0 ?1 == 1 : p.IdPeriodo == dto.IdPeriodo)
                                                                            && p.IdGerente == dto.IdGerente
                                                                            && p.IdArea == dto.IdArea)
                        .Select(l => new DTOLista
                        {
                            Id = l.IdSocio,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();

                        break;
                    //Socio Lider
                    case 5:
                        listSocio = _context.VWListarSocio.Where(p => (dto.IdPeriodo == 0 ? 1== 1 : p.IdPeriodo == dto.IdPeriodo) && p.IdArea == dto.IdArea)
                        .Select(l => new DTOLista
                        {
                            Id = l.IdSocio,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();

                        break;
                    //SemiAdministrador
                    case 6:
                        //listSocio = _context.VWListarSocio.Where(p => (dto.IdPeriodo == 0 ? p.IdPeriodo == 1 : p.IdPeriodo == dto.IdPeriodo) && p.IdArea == dto.IdArea)
                        listSocio = _context.VWListarSocio.Where(p => (dto.IdPeriodo == 0 ? 1 == 1 : p.IdPeriodo == dto.IdPeriodo) && p.IdArea == dto.IdArea)
                        .Select(l => new DTOLista
                        {
                            Id = l.IdSocio,
                            Descripcion = l.Descripcion
                        }).Distinct().OrderBy(l => l.Descripcion).ToList();

                        break;
                    //Cualquier otro rol
                    default:
                        var emptyList = new List<DTOLista>();
                        listSocio = emptyList;
                        break;
                }
                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = listSocio,
                    Mensaje = "Se Actualizo los datos",
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
        [Route("listarUsuario")]
        public RespondSearchObject<List<DTOLista>> listarUsuario(DTOMeta dto)
        {
            try
            {
                var listUsuario = _context.UsuMUsuario.Where(p => (p.IdRol == 2 || p.IdRol == 3)
                                                                && (dto.IdRol == 0 ? 1 == 1 : p.IdRol == dto.IdRol)
                                                                && (dto.IdArea == 0 ? 1 == 1 : p.IdArea == dto.IdArea)
                                                                && p.FlgActivo == 1)
                    .Select(l => new DTOLista
                    {
                        Id = l.Id,
                        Descripcion = l.DescripcionLarga
                    }).OrderBy(p => p.Descripcion).ToList();

                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = listUsuario,
                    Mensaje = "Retorno de datos",
                    Flag = true
                };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = true
                };
            }
        }

        #region Area
        [HttpPost]
        [Route("buscarArea")]
        public RespondSearchObject<List<UsuMArea>> buscarArea(DTOMantenimiento mant)
        {
            var listMant = _context.UsuMArea.Where(item =>(mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
                .Select(item => new UsuMArea
                {
                    Id = item.Id,
                    Descripcion = item.Descripcion,
                    FlgActivo = item.FlgActivo
                }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<UsuMArea>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<UsuMArea>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarArea")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarArea(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.UsuMArea
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarArea")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarArea([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.UsuMArea.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.UsuMArea.Update(entity);
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
        #endregion

        #region Rol
        [HttpPost]
        [Route("buscarRol")]
        public RespondSearchObject<List<UsuMRol>> buscarRol(DTOMantenimiento mant)
        {
            var listMant = _context.UsuMRol.Where(item => (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
            .Select(item => new UsuMRol
            {
                Id = item.Id,
                Descripcion = item.Descripcion,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<UsuMRol>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<UsuMRol>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarRol")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarRol(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.UsuMRol
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarRol")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarRol([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.UsuMRol.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.UsuMRol.Update(entity);
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
        #endregion

        #region Subservicio
        [HttpPost]
        [Route("buscarSubservicio")]
        public RespondSearchObject<List<OpMSubservicio>> buscarSubservicio(DTOMantenimiento mant)
        {
            var listMant = _context.OpMSubservicio.Where(item => (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
            .Select(item => new OpMSubservicio
            {
                Id = item.Id,
                Descripcion = item.Descripcion,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<OpMSubservicio>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMSubservicio>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarSubservicio")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarSubservicio(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.OpMSubservicio
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarSubservicio")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarSubservicio([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.OpMSubservicio.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.OpMSubservicio.Update(entity);
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
        #endregion

        #region Moneda
        [HttpPost]
        [Route("buscarMoneda")]
        public RespondSearchObject<List<OpMMoneda>> buscarMoneda(DTOMoneda moneda)
        {
            var listMant = _context.OpMMoneda.Where(item =>(moneda.IdArea==0?1==1:moneda.IdArea==item.IdArea) && (moneda.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.DescripcionLarga, "%" + moneda.Descripcion + "%")) && 
                                                    (moneda.DescripcionCorta == "" ? 1 == 1 : EF.Functions.Like(item.DescripcionCorta, "%" + moneda.DescripcionCorta + "%"))
                                                    )
            .Select(item => new OpMMoneda
            {
                Id = item.Id,
                DescripcionLarga = item.DescripcionLarga,
                DescripcionCorta = item.DescripcionCorta,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.DescripcionLarga).ToList();

            if (listMant.Count() > 0)
            {
                return new RespondSearchObject<List<OpMMoneda>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMMoneda>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarMoneda")]
        public RespondSearchObject<DTO.DTOMoneda> grabarMoneda(DTO.DTOMoneda moneda)
        {
            var entity = new Models.OpMMoneda
            {
                DescripcionLarga = moneda.Descripcion,
                DescripcionCorta = moneda.DescripcionCorta,
                FlgActivo = moneda.FlgActivo,
                UsuCreacion = moneda.UsuCreacion,
                FecCreacion = DateTime.Now
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMoneda>()
                {
                    Objeto = moneda,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMoneda>()
                {
                    Objeto = moneda,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarMoneda")]
        public async Task<RespondSearchObject<DTOMoneda>> editarMoneda([FromBody] DTOMoneda moneda)
        {
            var entity = _context.OpMMoneda.FirstOrDefault(item => item.Id == moneda.Id);
            // Validate entity is not null
            if (entity != null)
            {
                // Make changes on entity
                entity.DescripcionLarga = moneda.Descripcion;
                entity.DescripcionCorta = moneda.DescripcionCorta;
                entity.FlgActivo = moneda.FlgActivo;
            }
            try
            {
                // Update entity in DbSet Usuario
                _context.OpMMoneda.Update(entity);
                // Save changes in database Usuario
                _context.SaveChanges();

                //return msg
                return new RespondSearchObject<DTOMoneda>()
                {
                    Objeto = moneda,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMoneda>()
                {
                    Objeto = moneda,
                    Mensaje = "No se actualizo los datos",
                    Flag = false
                };
            }
        }
        #endregion

        #region Tipo de Cambio
        [HttpPost]
        [Route("buscarTC")]
        public RespondSearchObject<List<VWListarTC>> buscarTC(DTOTipoCambio dto)
        {
            var listMant = _context.VWListarTC.Where(item => item.FlgActivo == 1 && (dto.IdMoneda==0?1==1:item.IdMoneda == dto.IdMoneda))
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
        [Route("grabarTC")]
        public RespondSearchObject<DTO.DTOTipoCambio> grabarTC(DTO.DTOTipoCambio dto)
        {
            //Validamos si se registró anteriormente un tipo de cambio para la moneda seleccionada
            var entity = _context.OpMTipocambio.FirstOrDefault(item => item.IdMoneda == dto.IdMoneda);
            //De encontrarse un registro, se procederá a actualizar el tipo de cambio
            if (entity != null)
            {
                entity.TipoCambio = dto.TipoCambio;
                entity.UsuModificacion = dto.UsuModificacion;
                entity.FecModificacion = dto.FecModificacion;
                entity.FlgActivo = dto.FlgActivo;

                try
                {
                    _context.OpMTipocambio.Update(entity);
                    _context.SaveChanges();

                    return new RespondSearchObject<DTOTipoCambio>()
                    {
                        Objeto = dto,
                        Mensaje = "Se Actualizo los datos",
                        Flag = true
                    };
                }
                catch
                {
                    return new RespondSearchObject<DTOTipoCambio>()
                    {
                        Objeto = dto,
                        Mensaje = "No se actualizo los datos",
                        Flag = false
                    };
                }
            }
            //De no encontrarse un tipo de cambio, éste será registrado
            else { 
                entity = new Models.OpMTipocambio
                {
                    IdMoneda = dto.IdMoneda,
                    TipoCambio = dto.TipoCambio,
                    UsuCreacion = dto.UsuCreacion,
                    FecCreacion = DateTime.Now,
                    FlgActivo = dto.FlgActivo
                };

                try
                {
                    _context.Add(entity);
                    _context.SaveChanges();

                    return new RespondSearchObject<DTOTipoCambio>()
                    {
                        Objeto = dto,
                        Mensaje = "Registro exitoso",
                        Flag = true
                    };
                }
                catch (Exception ex)
                {
                    return new RespondSearchObject<DTOTipoCambio>()
                    {
                        Objeto = dto,
                        Mensaje = ex.ToString(),
                        Flag = false
                    };
                }
            }            
        }

        [HttpPost]
        [Route("editarTC")]
        public async Task<RespondSearchObject<DTOTipoCambio>> editarTC([FromBody] DTOTipoCambio dto)
        {
            var entity = _context.OpMTipocambio.FirstOrDefault(item => item.Id == dto.Id);
            // Validate entity is not null
            if (entity != null)
            {
                // Make changes on entity
                entity.TipoCambio = dto.TipoCambio;
                entity.UsuModificacion = dto.UsuModificacion;
                entity.FecModificacion = dto.FecModificacion;
                entity.FlgActivo = dto.FlgActivo;
            }
            try
            {
                // Update entity in DbSet Usuario
                _context.OpMTipocambio.Update(entity);
                // Save changes in database Usuario
                _context.SaveChanges();

                //return msg
                return new RespondSearchObject<DTOTipoCambio>()
                {
                    Objeto = dto,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOTipoCambio>()
                {
                    Objeto = dto,
                    Mensaje = "No se actualizo los datos",
                    Flag = false
                };
            }
        }
        #endregion

        #region Condicion
        [HttpPost]
        [Route("buscarCondicion")]
        public RespondSearchObject<List<OpMCondicion>> buscarCondicion(DTOMantenimiento mant)
        {
            var listMant = _context.OpMCondicion.Where(item => (item.IdArea == 0 ? 1 == 1 : item.IdArea == item.IdArea) && (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
            .Select(item => new OpMCondicion
            {
                Id = item.Id,
                Descripcion = item.Descripcion,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<OpMCondicion>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMCondicion>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarCondicion")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarCondicion(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.OpMCondicion
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarCondicion")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarCondicion([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.OpMCondicion.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.OpMCondicion.Update(entity);
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
        #endregion

        #region Grupo Económico
        [HttpPost]
        [Route("buscarGrpeco")]
        public RespondSearchObject<List<OpMGrpeco>> buscarGrpeco(DTOMantenimiento mant)
        {
            var listMant = _context.OpMGrpeco.Where(item => (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
            .Select(item => new OpMGrpeco
            {
                Id = item.Id,
                Descripcion = item.Descripcion,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<OpMGrpeco>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMGrpeco>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarGrpeco")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarGrpeco(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.OpMGrpeco
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarGrpeco")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarGrpeco([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.OpMGrpeco.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.OpMGrpeco.Update(entity);
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
        #endregion

        #region Sector
        [HttpPost]
        [Route("buscarSector")]
        public RespondSearchObject<List<OpMSector>> buscarSector(DTOMantenimiento mant)
        {
            var listMant = _context.OpMSector.Where(item => (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
            .Select(item => new OpMSector
            {
                Id = item.Id,
                Descripcion = item.Descripcion,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<OpMSector>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMSector>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarSector")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarSector(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.OpMSector
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarSector")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarSector([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.OpMSector.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.OpMSector.Update(entity);
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
        #endregion

        #region Estado
        [HttpPost]
        [Route("buscarEstado")]
        public RespondSearchObject<List<OpMEstado>> buscarEstado(DTOMantenimiento mant)
        {
            var listMant = _context.OpMEstado.Where(item => (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
            .Select(item => new OpMEstado
            {
                Id = item.Id,
                Descripcion = item.Descripcion,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<OpMEstado>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMEstado>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarEstado")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarEstado(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.OpMEstado
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarEstado")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarEstado([FromBody] DTOMantenimiento mant)
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
        #endregion

        #region Servicio
        [HttpPost]
        [Route("buscarServicio")]
        public RespondSearchObject<List<OpMServicio>> buscarServicio(DTOMantenimiento mant)
        {
            var listMant = _context.OpMServicio.Where(item =>  (mant.IdArea == 0 ? 1 == 1 : item.IdArea == mant.IdArea)&&
            (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
            .Select(item => new OpMServicio
            {
                Id = item.Id,
                Descripcion = item.Descripcion,
                FlgActivo = item.FlgActivo
            }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<OpMServicio>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMServicio>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarServicio")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarServicio(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.OpMServicio
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarServicio")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarServicio([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.OpMServicio.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.OpMServicio.Update(entity);
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
        #endregion

        #region Periodo
        [HttpPost]
        [Route("buscarPeriodo")]
        public RespondSearchObject<List<OpMPeriodo>> buscarPeriodo(DTOMantenimiento mant)
        {
            var listMant = _context.OpMPeriodo.Where(item => (mant.Descripcion == "" ? 1 == 1 : EF.Functions.Like(item.Descripcion, "%" + mant.Descripcion + "%")))
                .Select(item => new OpMPeriodo
                {
                    Id = item.Id,
                    Descripcion = item.Descripcion,
                    FlgActivo = item.FlgActivo
                }).OrderBy(item => item.Descripcion).ToList();

            if (listMant.Count > 0)
            {
                return new RespondSearchObject<List<OpMPeriodo>>()
                {
                    Objeto = listMant,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<OpMPeriodo>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarPeriodo")]
        public RespondSearchObject<DTO.DTOMantenimiento> grabarPeriodo(DTO.DTOMantenimiento mant)
        {
            var entity = new Models.OpMPeriodo
            {
                Descripcion = mant.Descripcion,
                FlgActivo = mant.FlgActivo,
                UsuCreacion = mant.UsuCreacion,
                FecCreacion = mant.FecCreacion
            };
            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOMantenimiento>()
                {
                    Objeto = mant,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarPeriodo")]
        public async Task<RespondSearchObject<DTOMantenimiento>> editarPeriodo([FromBody] DTOMantenimiento mant)
        {
            var entity = _context.OpMPeriodo.FirstOrDefault(item => item.Id == mant.Id);
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
                _context.OpMPeriodo.Update(entity);
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
        [Route("listarPeriodo")]
        public RespondSearchObject<List<DTOLista>> listarPeriodo()
        {
            try
            {
                var listPeriodo = _context.OpMPeriodo.Where(p => p.FlgActivo == 1)
                    .Select(l => new DTOLista
                    {
                        Id = l.Id,
                        Descripcion = l.Descripcion
                    }).OrderBy(p => p.Descripcion).ToList();

                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = listPeriodo,
                    Mensaje = "Retorno de datos",
                    Flag = true
                };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<List<DTOLista>>()
                {
                    Objeto = null,
                    Mensaje = ex.ToString(),
                    Flag = true
                };
            }
        }
        #endregion

        #region Empresas
        [HttpPost]
        [Route("obtenerRuc")]
        public RespondSearchObject<List<DTO.DTORuc>> obtenerRuc(DTORazonSocial razon)
        {
            var ruc = _context.OpMEmpresas.Where(n => n.Id == razon.Id)
                .Select(l => new DTO.DTORuc
                {
                    Id = l.Id,
                    Ruc = l.Ruc
                }).ToList();

            return new RespondSearchObject<List<DTO.DTORuc>>()
            {
                Objeto = ruc,
                Mensaje = "Se Actualizo los datos",
                Flag = true
            };
        }

        [HttpPost]
        [Route("obtenerRazonSocial")]
        public RespondSearchObject<List<DTO.DTORazonSocial>> obtenerRazonSocial(DTORuc ruc)
        {
            var razonsocial = _context.OpMEmpresas.Where(n => (EF.Functions.Like(n.Ruc, '%' + ruc.Ruc + '%')))
                .Select(l => new DTO.DTORazonSocial
                {
                    Id = l.Id,
                    Descripcion = l.RazonSocial
                }).Take(1).ToList();

            if (razonsocial.Count > 0)
            {
                return new RespondSearchObject<List<DTO.DTORazonSocial>>()
                {
                    Objeto = razonsocial,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<DTO.DTORazonSocial>>()
                {
                    Objeto = null,
                    Mensaje = "Se Actualizo los datos",
                    Flag = false
                };
            }
        }
        #endregion

        #region Meta
        [HttpPost]
        [Route("buscarMeta")]
        public RespondSearchObject<List<VWListarMeta>> buscarMeta(DTOMeta dto)
        {
            var listMeta = _context.VWListarMeta.Where(item => (dto.IdPeriodo == 0 ? 1 == 1 : item.IdPeriodo == dto.IdPeriodo) 
                                                             && (dto.IdUsuario == 0 ? 1 == 1 : item.IdUsuario == dto.IdUsuario)
                                                             && (dto.IdRol == 0 ? 1 == 1 : item.IdRol == dto.IdRol)
                                                             && item.FlgActivo == 1
                                            )
                .Select(item => new VWListarMeta
                {
                    Id = item.Id,
                    IdPeriodo = item.IdPeriodo,
                    DetPeriodo = item.DetPeriodo,
                    IdUsuario = item.IdUsuario,
                    Descripcion = item.Descripcion,
                    IdRol = item.IdRol,
                    Meta = item.Meta,
                    FlgActivo = item.FlgActivo
                }).OrderBy(item => item.Descripcion).ToList();

            if (listMeta.Count > 0)
            {
                return new RespondSearchObject<List<VWListarMeta>>()
                {
                    Objeto = listMeta,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<VWListarMeta>>()
                {
                    Objeto = null,
                    Mensaje = "No existen registros con los filtros ingresados.",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarMeta")]
        public RespondSearchObject<DTO.DTOMeta> grabarMeta(DTO.DTOMeta mant)
        {            
            try
            {
                var entity = new Models.OpMMeta
                {
                    IdPeriodo = mant.IdPeriodo,
                    IdUsuario = mant.IdUsuario,
                    Meta = mant.Meta,
                    UsuCreacion = mant.UsuCreacion,
                    FecCreacion = DateTime.Now,
                    FlgActivo = mant.FlgActivo
                };

                _context.Add(entity);
                _context.SaveChanges();

                return new RespondSearchObject<DTOMeta>()
                {
                    Objeto = mant,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch(Exception ex)
            {
                return new RespondSearchObject<DTOMeta>()
                {
                    Objeto = mant,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("editarMeta")]
        public async Task<RespondSearchObject<DTOMeta>> editarMeta([FromBody] DTOMeta mant)
        {            
            try
            {
                var entity = _context.OpMMeta.FirstOrDefault(item => item.Id == mant.Id);

                if (entity != null)
                {
                    entity.IdPeriodo = mant.IdPeriodo;
                    entity.IdUsuario = mant.IdUsuario;
                    entity.Meta = mant.Meta;
                    entity.UsuModificacion = mant.UsuModificacion;
                    entity.FecModificacion = mant.FecModificacion;
                    entity.FlgActivo = mant.FlgActivo;
                }

                _context.OpMMeta.Update(entity);
                _context.SaveChanges();

                //return msg
                return new RespondSearchObject<DTOMeta>()
                {
                    Objeto = mant,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch(Exception ex)
            {
                return new RespondSearchObject<DTOMeta>()
                {
                    Objeto = mant,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }
        #endregion

        [HttpPost]
        [Route("registrarRelacionOportunidad")]
        public async Task<RespondSearchObject<DTOOportunidad>> registrarRelacionOportunidad(DTO.DTOOportunidad dto)
        {
            try
            {
                //Bool de registros duplicados
                var boolDupReg = true;
                //Entidad op_r_socgercli
                var reasignacion = new Models.OpRSocgercli();
                //Lista de servicios (se modifico y agregó codigo duro del area BTC = 1, en algun momento se tendrá que modificar para diferentes areas)
                var listServicios = _context.OpMServicio.Where(k => k.IdArea == 1 && k.FlgActivo == 1).Select(l => l.Id).ToList();
                //Lista de subservicios
                var listSubServicios = new List<DTOLista>();
                //Nueva relación
                var newRel = new Models.OpRSocgercli();
                //Nueva oportunidad
                var oportunidad = new Models.OpMOportunidad();

                foreach (int serv in listServicios)
                {
                    switch (serv)
                    {
                        //ASESORÍA FISCALIZACIÓN
                        case 13:
                            listSubServicios = _context.OpMSubservicio.Where(l => l.Id == 2 || l.Id == 3 || l.Id == 4).Select(l => new DTOLista
                            {
                                Id = l.Id,
                                Descripcion = l.Descripcion
                            }).ToList();

                            foreach (DTOLista subserv in listSubServicios)
                            {
                                reasignacion = _context.OpRSocGerCli.FirstOrDefault(rel => rel.IdPeriodo == dto.IdPeriodo && rel.IdSocio == dto.IdSocio
                                                                                    && rel.IdGerente == dto.IdGerente && rel.IdEmpresa == dto.IdEmpresa
                                                                                    && rel.IdServicio == serv && rel.IdSubservicio == subserv.Id
                                                                            );
                                if (reasignacion == null)
                                {
                                    //Insertamos nueva relación
                                    newRel = new Models.OpRSocgercli
                                    {
                                        IdPeriodo = dto.IdPeriodo,
                                        IdSocio = dto.IdSocio,
                                        IdGerente = dto.IdGerente,
                                        IdEmpresa = dto.IdEmpresa,
                                        IdServicio = serv,
                                        IdSubservicio = subserv.Id,
                                        UsuCreacion = dto.UsuCreacion,
                                        FecCreacion = dto.FecCreacion,
                                        FlgActivo = dto.FlgActivo
                                    };

                                    _context.Add(newRel);
                                    _context.SaveChanges();

                                    //Insertamos nueva oportunidad
                                    oportunidad = new Models.OpMOportunidad
                                    {
                                        IdSocgercli = newRel.Id,
                                        UsuCreacion = dto.UsuCreacion,
                                        FecCreacion = dto.FecCreacion,
                                        FlgActivo = dto.FlgActivo
                                    };

                                    _context.Add(oportunidad);
                                    _context.SaveChanges();

                                    boolDupReg = false;
                                }
                                else
                                {

                                    reasignacion.FlgActivo = 1;

                                    _context.OpRSocGerCli.Update(reasignacion);
                                    _context.SaveChanges();
                                }
                            };

                            break;
                        //ASESORÍA TRIBUTARIA
                        case 14:
                            listSubServicios = _context.OpMSubservicio.Where(l => l.Id == 2 || l.Id == 5 || l.Id == 6 || l.Id == 7 || l.Id == 8).Select(l => new DTOLista
                            {
                                Id = l.Id,
                                Descripcion = l.Descripcion
                            }).ToList();

                            foreach (DTOLista subserv in listSubServicios)
                            {
                                reasignacion = _context.OpRSocGerCli.FirstOrDefault(rel => rel.IdPeriodo == dto.IdPeriodo && rel.IdSocio == dto.IdSocio
                                                                                    && rel.IdGerente == dto.IdGerente && rel.IdEmpresa == dto.IdEmpresa
                                                                                    && rel.IdServicio == serv && rel.IdSubservicio == subserv.Id
                                                                            );
                                if (reasignacion == null)
                                {
                                    //Insertamos nueva relación
                                    newRel = new Models.OpRSocgercli
                                    {
                                        IdPeriodo = dto.IdPeriodo,
                                        IdSocio = dto.IdSocio,
                                        IdGerente = dto.IdGerente,
                                        IdEmpresa = dto.IdEmpresa,
                                        IdServicio = serv,
                                        IdSubservicio = subserv.Id,
                                        UsuCreacion = dto.UsuCreacion,
                                        FecCreacion = dto.FecCreacion,
                                        FlgActivo = dto.FlgActivo
                                    };

                                    _context.Add(newRel);
                                    _context.SaveChanges();

                                    //Insertamos nueva oportunidad
                                    oportunidad = new Models.OpMOportunidad
                                    {
                                        IdSocgercli = newRel.Id,
                                        UsuCreacion = dto.UsuCreacion,
                                        FecCreacion = dto.FecCreacion,
                                        FlgActivo = dto.FlgActivo
                                    };

                                    _context.Add(oportunidad);
                                    _context.SaveChanges();

                                    boolDupReg = false;
                                }
                                else
                                {

                                    reasignacion.FlgActivo = 1;

                                    _context.OpRSocGerCli.Update(reasignacion);
                                    _context.SaveChanges();
                                }
                            };

                            break;
                        //OTROS SERVICIOS
                        default:
                            listSubServicios = _context.OpMSubservicio.Where(l => l.Id == 1).Select(l => new DTOLista
                            {
                                Id = l.Id,
                                Descripcion = l.Descripcion
                            }).ToList();

                            foreach (DTOLista subserv in listSubServicios)
                            {
                                reasignacion = _context.OpRSocGerCli.FirstOrDefault(rel => rel.IdPeriodo == dto.IdPeriodo && rel.IdSocio == dto.IdSocio
                                                                                    && rel.IdGerente == dto.IdGerente && rel.IdEmpresa == dto.IdEmpresa
                                                                                    && rel.IdServicio == serv && rel.IdSubservicio == subserv.Id
                                                                            );
                                if (reasignacion == null)
                                {
                                    //Insertamos nueva relación
                                    newRel = new Models.OpRSocgercli
                                    {
                                        IdPeriodo = dto.IdPeriodo,
                                        IdSocio = dto.IdSocio,
                                        IdGerente = dto.IdGerente,
                                        IdEmpresa = dto.IdEmpresa,
                                        IdServicio = serv,
                                        IdSubservicio = subserv.Id,
                                        UsuCreacion = dto.UsuCreacion,
                                        FecCreacion = dto.FecCreacion,
                                        FlgActivo = dto.FlgActivo
                                    };

                                    _context.Add(newRel);
                                    _context.SaveChanges();

                                    //Insertamos nueva oportunidad
                                    oportunidad = new Models.OpMOportunidad
                                    {
                                        IdSocgercli = newRel.Id,
                                        UsuCreacion = dto.UsuCreacion,
                                        FecCreacion = dto.FecCreacion,
                                        FlgActivo = dto.FlgActivo
                                    };

                                    _context.Add(oportunidad);
                                    _context.SaveChanges();

                                    boolDupReg = false;
                                }
                                else
                                {
                                    
                                    reasignacion.FlgActivo = 1;

                                    _context.OpRSocGerCli.Update(reasignacion);
                                    _context.SaveChanges();
                                }
                            };

                            break;
                    }
                }

                if (!boolDupReg)
                    return new RespondSearchObject<DTOOportunidad>()
                    {
                        Objeto = dto,
                        Mensaje = "Nueva relación registrada.",
                        Flag = true
                    };
                else
                    return new RespondSearchObject<DTOOportunidad>()
                    {
                        Objeto = dto,
                        Mensaje = "Se detectó que la relación ya tiene oportunidades registradas.\nEn caso de corrección, dirigirse a la interfaz Actualizar Oportunidad.",
                        Flag = false
                    };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<DTOOportunidad>()
                {
                    Objeto = dto,
                    Mensaje = ex.ToString(), //"Error en servicios.",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("reasignarOportunidad")]
        public async Task<RespondSearchObject<DTOReasignar>> reasignarOportunidad(DTO.DTOReasignar dto) 
        {
            try
            {
                //Lista de servicios a editar
                var listaServicio = dto.LstServicio;
                //Variable a actualizar
                var entity = new Models.OpRSocgercli();
                //Relación nuevas llaves ingresadas
                var newRelEntity = new Models.OpRSocgercli();
                //Lista de subservicios
                var listSubServicios = new List<DTOLista>();
                //Bool de registros duplicados
                var boolRegDup = true;
                //Bool nuevos registros
                var boolNewReg = false;

                List<int> servicios = new List<int>();
                var listaElementoEliminar = new List<OpRSocgercli>(); 




                foreach (DTORelacionServicio serv in listaServicio) {
                    servicios.Add( serv.Value);

                    switch (serv.Value) {
                        //ASESORÍA FISCALIZACIÓN
                        case 13:
                            listSubServicios = _context.OpMSubservicio.Where(l => l.Id == 2 || l.Id == 3 || l.Id == 4).Select(l => new DTOLista
                            {
                                Id = l.Id,
                                Descripcion = l.Descripcion
                            }).ToList();

                            foreach (DTOLista subserv in listSubServicios) 
                            {
                                //Validamos si existen registros previos con las nuevas llaves ingresadas
                                newRelEntity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == dto.IdPeriodo && item.IdSocio == dto.IdSocio
                                                                                   && item.IdGerente == dto.IdGerente && item.IdEmpresa == dto.IdEmpresa
                                                                                   && item.IdServicio == serv.Value && item.IdSubservicio == subserv.Id);
                                if (newRelEntity == null) 
                                {
                                    //Realizamos la búsqueda del ID a editar
                                    entity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == serv.IdPeriodo && item.IdSocio == serv.IdSocio
                                                                                        && item.IdGerente == serv.IdGerente && item.IdEmpresa == serv.IdEmpresa
                                                                                        && item.IdServicio == serv.Value && item.IdSubservicio == subserv.Id);
                                    if (entity != null)
                                    {
                                        entity.IdPeriodo = dto.IdPeriodo;
                                        entity.IdSocio = dto.IdSocio;
                                        entity.IdGerente = dto.IdGerente;
                                        entity.IdEmpresa = dto.IdEmpresa;
                                        entity.UsuModificacion = dto.UsuModificacion;
                                        entity.FecModificacion = dto.FecModificacion;
                                        entity.FlgActivo = dto.FlgActivo;

                                        _context.OpRSocGerCli.Update(entity);
                                        _context.SaveChanges();

                                        boolRegDup = false;
                                    }
                                }
                            };
                            
                            break;
                        //ASESORÍA TRIBUTARIA
                        case 14:
                            listSubServicios = _context.OpMSubservicio.Where(l => l.Id == 2 || l.Id == 5 || l.Id == 6 || l.Id == 7 || l.Id == 8).Select(l => new DTOLista
                            {
                                Id = l.Id,
                                Descripcion = l.Descripcion
                            }).ToList();

                            foreach (DTOLista subserv in listSubServicios)
                            {
                                //Validamos si existen registros previos con las nuevas llaves ingresadas
                                newRelEntity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == dto.IdPeriodo && item.IdSocio == dto.IdSocio
                                                                                   && item.IdGerente == dto.IdGerente && item.IdEmpresa == dto.IdEmpresa
                                                                                   && item.IdServicio == serv.Value && item.IdSubservicio == subserv.Id);
                                if (newRelEntity == null)
                                {
                                    //Realizamos la búsqueda del ID a editar
                                    entity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == serv.IdPeriodo && item.IdSocio == serv.IdSocio
                                                                                        && item.IdGerente == serv.IdGerente && item.IdEmpresa == serv.IdEmpresa
                                                                                        && item.IdServicio == serv.Value && item.IdSubservicio == subserv.Id);
                                    if (entity != null)
                                    {
                                        entity.IdPeriodo = dto.IdPeriodo;
                                        entity.IdSocio = dto.IdSocio;
                                        entity.IdGerente = dto.IdGerente;
                                        entity.IdEmpresa = dto.IdEmpresa;
                                        entity.UsuModificacion = dto.UsuModificacion;
                                        entity.FecModificacion = dto.FecModificacion;
                                        entity.FlgActivo = dto.FlgActivo;

                                        _context.OpRSocGerCli.Update(entity);
                                        _context.SaveChanges();

                                        boolRegDup = false;
                                    }
                                    else
                                        boolNewReg = true;
                                }
                            };

                            break;
                        //OTROS SERVICIOS
                        default:
                            //Validamos si existen registros previos con las nuevas llaves ingresadas(si no exite nueva relacion será igual a NULL)
                            newRelEntity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == dto.IdPeriodo && item.IdSocio == dto.IdSocio
                                                                               && item.IdGerente == dto.IdGerente && item.IdEmpresa == dto.IdEmpresa
                                                                               && item.IdServicio == serv.Value);
                            
                            //Nuevo servicio que se agrego en la pantalla y gerente nuevo
                            if (newRelEntity == null)
                            {
                                //Realizamos la búsqueda del ID a editar en gerente antiguo
                                entity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == serv.IdPeriodo && item.IdSocio == serv.IdSocio
                                                                               && item.IdGerente == serv.IdGerente && item.IdEmpresa == serv.IdEmpresa
                                                                               && item.IdServicio == serv.Value);
                                if (entity != null)
                                {
                                    entity.IdPeriodo = dto.IdPeriodo;
                                    entity.IdSocio = dto.IdSocio;
                                    entity.IdGerente = dto.IdGerente;
                                    entity.IdEmpresa = dto.IdEmpresa;
                                    entity.UsuModificacion = dto.UsuModificacion;
                                    entity.FecModificacion = dto.FecModificacion;
                                    entity.FlgActivo = dto.FlgActivo;

                                    _context.OpRSocGerCli.Update(entity);
                                    _context.SaveChanges();

                                    boolRegDup = false;
                                }
                                else
                                    boolNewReg = true;
                            }
                            else
                            {//euscuvil 15-11-2022
                                //Realizamos la búsqueda del ID a editar
                                entity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == dto.IdPeriodo && item.IdSocio == dto.IdSocio
                                                                               && item.IdGerente == dto.IdGerente && item.IdEmpresa == dto.IdEmpresa
                                                                               && item.IdServicio == serv.Value);
                               
                                if (entity != null)
                                {
                                    /*entity.IdPeriodo = dto.IdPeriodo;
                                    entity.IdSocio = dto.IdSocio;
                                    entity.IdGerente = dto.IdGerente;
                                    entity.IdEmpresa = dto.IdEmpresa;*/
                                    entity.UsuModificacion = dto.UsuModificacion;
                                    entity.FecModificacion = dto.FecModificacion;
                                    entity.FlgActivo = dto.FlgActivo;

                                    _context.OpRSocGerCli.Update(entity);
                                    _context.SaveChanges();

                                    boolRegDup = false;

                                    //Ahora cambiando el flag a los demás registros pero que tiene otros 'gerentes'
                                    /*entity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == dto.IdPeriodo && item.IdSocio == dto.IdSocio
                                                                                   && item.IdGerente != dto.IdGerente && item.IdEmpresa == dto.IdEmpresa
                                                                                   && item.IdServicio == serv.Value);*/

                                    entity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == dto.IdPeriodo && item.IdSocio == dto.IdSocio
                                                                                   && item.IdGerente == serv.IdGerente && item.IdEmpresa == dto.IdEmpresa
                                                                                   && item.IdServicio == serv.Value);

                                    if (entity != null) {
                                        //Cambiando su estado
                                        entity.FlgActivo = 0;//dto.FlgActivo
                                        _context.OpRSocGerCli.Update(entity);
                                        _context.SaveChanges();
                                    }
                                    


                                }   
                                else
                                    boolNewReg = true;
                            }

                            break;
                    }
                };

                //Se obtiene la lista de servicios que se envía desde la reasignación
                //if(listaServicio.Count > 0)
                //{
                //Se obtiene la lista de servicios que tiene el manager actual diferentes a la lista de servicios recibidos como parametro
                /*var listaEliminar = _context.OpRSocGerCli.Where(item => item.IdPeriodo == listaServicio[0].IdPeriodo && item.IdSocio == listaServicio[0].IdSocio
                                                                                    && item.IdGerente == listaServicio[0].IdGerente && item.IdEmpresa == listaServicio[0].IdEmpresa
                                                                                    && servicios.Any(i1 => i1 != item.IdServicio)).Select(o => new OpRSocgercli
                                                                                    {
                                                                                        Id = o.Id,
                                                                                        IdServicio = o.IdServicio,
                                                                                        IdSubservicio = o.IdSubservicio,
                                                                                        IdGerente = o.IdGerente,
                                                                                        IdSocio = o.IdSocio,
                                                                                        IdPeriodo = o.IdPeriodo,
                                                                                        UsuModificacion = o.UsuModificacion,
                                                                                        FecModificacion = o.FecModificacion,
                                                                                        UsuCreacion = o.UsuCreacion,
                                                                                        FecCreacion = o.FecCreacion,
                                                                                        FlgActivo = o.FlgActivo

                                                                                    }).ToList();*/
                var listaEliminar = _context.OpRSocGerCli.Where(item => item.IdPeriodo == listaServicio[0].IdPeriodo && item.IdSocio == listaServicio[0].IdSocio
                                                                                    && item.IdGerente == listaServicio[0].IdGerente && item.IdEmpresa == listaServicio[0].IdEmpresa 
                                                                                    ).Select(o => new OpRSocgercli
                                                                                    {
                                                                                        Id = o.Id,
                                                                                        IdServicio = o.IdServicio,
                                                                                        IdSubservicio = o.IdSubservicio,
                                                                                        IdGerente = o.IdGerente,
                                                                                        IdSocio = o.IdSocio,
                                                                                        IdPeriodo = o.IdPeriodo,
                                                                                        IdEmpresa = o.IdEmpresa,
                                                                                        UsuModificacion = o.UsuModificacion,
                                                                                        FecModificacion = o.FecModificacion,
                                                                                        UsuCreacion = o.UsuCreacion,
                                                                                        FecCreacion = o.FecCreacion,
                                                                                        FlgActivo = o.FlgActivo

                                                                                    }).ToList();


                //Recorre todos los servicios que tiene el manager antiguo y los inactiva ( esta caso cuando se elimina servicios al manager actual o se reasigna a nuevo manager y los servicios del manager actual se inactivan) 

                foreach (OpRSocgercli itemEliminar in listaEliminar)
                {
                     
                 
                    if (servicios.Contains(int.Parse( itemEliminar.IdServicio.ToString() )) == false)
                    {

                        /*newRelEntity = _context.OpRSocGerCli.FirstOrDefault(item => item.IdPeriodo == itemEliminar.IdPeriodo && item.IdSocio == itemEliminar.IdSocio
                                                                             && item.IdGerente == itemEliminar.IdGerente && item.IdEmpresa == itemEliminar.IdEmpresa
                                                                             && item.IdServicio == itemEliminar.IdServicio && item.IdSubservicio == itemEliminar.IdSubservicio);*/


                        newRelEntity = _context.OpRSocGerCli.FirstOrDefault(item => item.Id == itemEliminar.Id );


                        newRelEntity.FlgActivo = 0;
                        _context.OpRSocGerCli.Update(newRelEntity);
                        _context.SaveChanges();

                    }
                    else
                    {

                        newRelEntity = _context.OpRSocGerCli.FirstOrDefault(item => item.Id == itemEliminar.Id);

                        newRelEntity.FlgActivo = 1;
                        _context.OpRSocGerCli.Update(newRelEntity);
                        _context.SaveChanges();
                    }
                       

                 }
                    

                //}


                if (!boolRegDup && !boolNewReg)
                    return new RespondSearchObject<DTOReasignar>()
                    {
                        Objeto = dto,
                        Mensaje = "Datos actualizados.",
                        Flag = true
                    };
                else if(boolNewReg)
                    return new RespondSearchObject<DTOReasignar>()
                    {
                        Objeto = dto,
                        Mensaje = "Se actualizaron los datos, sin embargo, se identificó que un servicio ingresado no cuenta con una relación existente. \nPor favor, ingresar a la sección Nueva Oportunidad y proceder con el registro del servicio.",
                        Flag = true
                    };
                else
                    return new RespondSearchObject<DTOReasignar>()
                        {
                            Objeto = dto,
                            Mensaje = "Se detectó que las llaves ingresadas ya cuentan con una relación existente. \nPor favor, ingresar la relacción de datos correcta para continuar con la actualización.",
                            Flag = false
                        };
            }
            catch (Exception ex)
            {
                return new RespondSearchObject<DTOReasignar>()
                {
                    Objeto = dto,
                    Mensaje = "Error en servicios.", //ex.ToString(),
                    Flag = false
                };
            }   
        }

    }
}
