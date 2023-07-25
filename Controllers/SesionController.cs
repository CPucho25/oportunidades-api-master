using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Newtonsoft.Json;
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
    [Route("api/sesion")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SesionController : ControllerBase
    {
        private readonly Models.Opotunidades.db_oportunidades _context;
        private readonly IMapper _mapper;
        public SesionController(Models.Opotunidades.db_oportunidades context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("generarpasswords/{id}")]
        public string EncriptarPass(string id)
        {
            Encriptador e = new Encriptador();
            return e.HashPaswword(id); //retorna true o false comprarndo el string con el password en la BD
        }        

        [HttpPost]
        [Route("login")]
        public ActionResult<RespuestaLogin> ValidarSesion([FromBody] PeticionLogin peticion)
        {
            UsuMUsuario usuario = _context.UsuMUsuario.Where(u => u.Descripcion.ToLower() == peticion.username.ToLower() && u.FlgActivo==1).FirstOrDefault();

            if (usuario != null)
            {
                var listGrupos = new List<DTOGrupoFormulario>();
                var listFormuArea = new List<DTOFormulario>();


                listGrupos = _context.OpMGrupoFormulario.Where(f => f.FlgActivo == 1 ).Select(grupo => new DTOGrupoFormulario
                {
                    idGrupo = grupo.Id, //identificador del grupo-seccion del aplicativo
                    idArea=usuario.IdArea,
                    nombre = grupo.Nombre,//nombre del grupo
                    //defino el area al que pertence el usuario
                    Formulario = _context.OpMFormulario.Where(n =>  n.IdGrupo == grupo.Id && n.Area == usuario.IdArea).Select(item => new DTOFormulario
                    {
                        //Detalle de elementos disponibles en el formulario dentro de un grupo
                        id = item.Id,
                        id_grupo = item.IdGrupo,
                        area = item.Area,
                        id_elemento = item.IdElemento,
                        flg_activo=item.FlgActivo,
                        //tipo = _context.OpMTipoElementos.Where(l => l.id == item.id_elemento).Select(ll => ll.descripcion).FirstOrDefault(),
                        //algunos elementos son lista de opciones, quienes no cumplan este formato las opciones* seran vacias
                        /*
                        opciones = (_context.OpMFormulioOpciones.Where(m => m.id_formulario == item.id && m.FlgActivo == 1).Select(mm => new DTOFormularioOpciones
                        {
                            id = mm.id,
                            descripcion = mm.descripcion
                        }).ToList())
                        */

                    }).ToList()
                }).ToList();

                /*
                // añadir el flag al select
                listGrupos = _context.OpMGrupoFormulario.Where(f => f.FlgActivo == 1).Select(item => new DTOGrupoFormulario
                {
                    id = item.id,
                    nombre = item.nombre,
                    Formulario = new List<DTOFormulario>()
                }).ToList();
                listFormuArea = _context.OpMFormulario
                    .Join(_context.OpMElemento, 
                        f => f.id_elemento,
                        s => s.id,
                        (f,s) => new{ id = f.id, tipo = s.id})
                    .Join(_context.OpMTipoElementos,
                        f => f.id,
                        s => s)
                    .Where(l => l.area == usuario.IdArea && l.FlgActivo == 1).Select(item => new DTOFormulario {
                    tipo = item.id_elemento,
                    id_grupo = item.id_grupo,
                    area = item.area,
                })
                .ToList();

                foreach (var grupo in listGrupos)
                {
                    foreach(var btn in listFormuArea)
                    {
                        if(btn.id_grupo == grupo.id)
                        {
                            grupo.Formulario.Add(btn);
                        }
                    }
                };
                
                */
                Encriptador e = new Encriptador();
                // VALIDAR USUARIO Y CONTRASEÑA
                if (e.CompararPasswords(peticion.password, usuario.Password))
                {
                    // Security Key
                    string securityKey = "this is my custom Secret key for authnetication";

                    // Simetric security key
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

                    // Singin Credencials
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    // Crear token
                    //      Agregar claims
                    var claims = new List<Claim>()
                    {
                    new Claim("FlgAdmin",usuario.FlgAdmin.ToString()),
                    new Claim("nombre",usuario.DescripcionLarga.ToString()),
                    new Claim("Rol","1"),//Solo accede al entorno de EMPRESA-EMPLEADO-DEPENDIENTE
                    new Claim("idRol",usuario.IdRol.ToString()),
                    new Claim("idArea",usuario.IdArea.ToString()),
                    new Claim("idioma","2"),
                    new Claim("idusuario",usuario.Id.ToString()),
                    new Claim("permisos", JsonConvert.SerializeObject(listGrupos))
                    };

                    var token = new JwtSecurityToken(
                        issuer: "YOUR_ISSUER_VALUE",
                        audience: "YOUR_AUDIENCE_VALUE",
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds,
                        claims: claims
                    );

                    // Pasar token a String
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    int v = (int)usuario.Id;
                    //BfTDatosGenerales dtgen = _context.BfTDatosGenerales.Where(m=>m.FlgActivo==1 && m.IdUsuario== (int)usuario.Id).FirstOrDefault();
                    
                    return new RespuestaLogin()
                    {
                        IsAuth = true,
                        Mensaje = "Ingreso correcto",
                        token = tokenString,
                        Idioma = 2,//1 ingles, 2 español
                    };
                }
                else
                {
                    return new RespuestaLogin()
                    {
                        IsAuth = false,
                        Mensaje = "Contraseña incorrecta.",
                        token = null
                    };
                }
            }
            return new RespuestaLogin()
            {
                IsAuth = false,
                Mensaje = "El usuario no existe.",
                token = null
            };
        }
     
        #region Usuario
        [HttpPost]
        [Route("buscarUsuario")]
        public RespondSearchObject<List<DTOUsuario>> buscarUsuario(DTOUsuario usuario)
        {

            var listUsuarios = _context.UsuMUsuario.Where(p => (usuario.IdRol == 0 ? 1 == 1 : usuario.IdRol == p.IdRol) &&
                                                                (usuario.IdArea == 0 ? 1 == 1 : usuario.IdArea == p.IdArea) &&
                                                                (usuario.IdRol == 0 ? 1 == 1 : p.FlgAdmin == usuario.FlgAdmin) &&
                                                                (usuario.Descripcion == "" ? 1 == 1 : EF.Functions.Like(p.Descripcion, "%" + usuario.Descripcion + "%")) &&
                                                                (usuario.DescripcionLarga == "" ? 1 == 1 : EF.Functions.Like(p.DescripcionLarga, "%" + usuario.DescripcionLarga + "%")) &&

                                                                (usuario.IdArea == 0 ? 1 == 1 : p.IdArea == usuario.IdArea) &&
                                                                p.FlgActivo == usuario.FlgActivo
                                                        )
                .Select(p => new DTOUsuario
                {
                    Id = p.Id,
                    Descripcion = p.Descripcion,
                    DescripcionLarga = p.DescripcionLarga,
                    Password = p.Password,
                    //Correo = p.correo,
                    FlgActivo = p.FlgActivo,
                    FlgAdmin = p.FlgAdmin,
                    IdArea = p.IdArea,
                    IdRol = p.IdRol,
                    lstGerente = _context.OpREaGernt.Where(l => l.Id == p.Id).Select(l => new DTORelacionGerente
                    {
                        Id = l.Id,
                        IdGerente = l.IdGerente
                    }).ToList()
                }).OrderBy(p => p.Descripcion).ToList();

            if (listUsuarios.Count > 0)
            {
                return new RespondSearchObject<List<DTOUsuario>>()
                {
                    Objeto = listUsuarios,
                    Mensaje = "Se encontro Registro",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<DTOUsuario>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro Registro",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarUsuario")]
        public async Task<RespondSearchObject<DTOUsuario>> grabarUsuario(DTOUsuario dtousuario)
        {
            var entity = new Models.UsuMUsuario
            {
                IdArea = dtousuario.IdArea,
                IdRol = dtousuario.IdRol,
                Descripcion = dtousuario.Descripcion,
                Password = EncriptarPass(dtousuario.Password),
                DescripcionLarga = dtousuario.DescripcionLarga,
                UsuCreacion = dtousuario.UsuCreacion,
                FecCreacion = dtousuario.FecCreacion,
                FlgAdmin = dtousuario.FlgAdmin,
                FlgActivo = dtousuario.FlgActivo
            };

            try
            {
                _context.Add(entity);
                _context.SaveChanges();

                if (dtousuario.IdRol == 4)
                {
                    //Insert op_r_eagernt relationship
                    var idUser = _context.UsuMUsuario.Last();

                    foreach (var temp in dtousuario.lstGerente)
                    {
                        var relacion = new Models.OpREaGernt
                        {
                            //Id = idUser.Id,
                            IdEa= idUser.Id,
                            IdGerente = temp.IdGerente,
                            UsuCreacion = dtousuario.UsuCreacion,
                            FecCreacion = dtousuario.FecCreacion,
                            FlgActivo = dtousuario.FlgActivo,
                        };

                        //Grabar op_r_eagernt
                        _context.Add(relacion);
                        _context.SaveChanges();
                    }
                }

                return new RespondSearchObject<DTOUsuario>()
                {
                    Objeto = dtousuario,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return new RespondSearchObject<DTOUsuario>()
                {
                    Objeto = dtousuario,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }           
        }

        [HttpPost]
        [Route("editarUsuario")]
        public async Task<RespondSearchObject<DTOUsuario>> editarUsuario([FromBody] DTOUsuario usuario)
        {
            // Retrieve entity by id
            var entity = _context.UsuMUsuario.FirstOrDefault(item => item.Id == usuario.Id);

            var password = new String("");

            if (usuario.Password == "") 
                password = entity.Password;            
            else
                password = EncriptarPass(usuario.Password);

            // Validate entity is not null
            if (entity != null)
            {
                entity.Descripcion = usuario.Descripcion;
                entity.Password = password;
                entity.DescripcionLarga = usuario.DescripcionLarga;
                entity.IdRol = usuario.IdRol;
                entity.IdArea = usuario.IdArea;
                entity.FecModificacion = DateTime.Now;
                entity.UsuModificacion = usuario.UsuModificacion;
                entity.FlgActivo = usuario.FlgActivo;
                entity.FlgAdmin = usuario.FlgAdmin;
            }
            try
            {
                _context.UsuMUsuario.Update(entity);
                _context.SaveChanges();

                if (usuario.IdRol == 4)
                {
                    //Eliminamos las relaciones existentes del usuario en la tabla op_r_eagernt
                    var eliminarRelacion = _context.OpREaGernt.Where(l => l.Id == usuario.Id)
                        .Select(l => new OpREaGernt
                        {
                            Id = l.Id,
                            IdGerente = l.IdGerente,
                            UsuCreacion = l.UsuCreacion,
                            FecCreacion = l.FecCreacion,
                            FlgActivo = l.FlgActivo
                        }).ToList();

                    if (eliminarRelacion.Count > 0)
                        foreach (var delrow in eliminarRelacion)
                        {
                            _context.Remove(delrow);
                            _context.SaveChanges();
                        }

                    //Creamos nueva relación
                    foreach (var temp in usuario.lstGerente)
                    {
                        var relacion = new Models.OpREaGernt
                        {
                            Id = entity.Id,
                            IdGerente = temp.IdGerente,
                            UsuCreacion = usuario.UsuModificacion,
                            FecCreacion = usuario.FecModificacion,
                            FlgActivo = usuario.FlgActivo,
                        };

                        //Grabar op_r_eagernt
                        _context.Add(relacion);
                        _context.SaveChanges();
                    }
                }

                return new RespondSearchObject<DTOUsuario>()
                {
                    Objeto = usuario,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch(Exception ex)
            {
                return new RespondSearchObject<DTOUsuario>()
                {
                    Objeto = usuario,
                    Mensaje = ex.ToString(),
                    Flag = false
                };
            }
        }
        #endregion

        [HttpPost]
        [Route("cambiarClave")]
        public async Task<RespondSearchObject<DTOCambioClave>> cambiarClave([FromBody] DTOCambioClave dto)
        {
            Encriptador e = new Encriptador();

            var usuario = _context.UsuMUsuario.FirstOrDefault(item => item.Id == dto.IdUsuario);
            // Validate entity is not null
            if (usuario != null && e.CompararPasswords(dto.OldPassword, usuario.Password))
            {
                usuario.Password = EncriptarPass(dto.NewPassword); 
                usuario.UsuModificacion = dto.UsuModificacion;
                usuario.FecModificacion = dto.FecModificacion; 
            }

            try
            {
                _context.UsuMUsuario.Update(usuario);
                _context.SaveChanges();

                //return msg
                return new RespondSearchObject<DTOCambioClave>()
                {
                    Objeto = dto,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch
            {
                return new RespondSearchObject<DTOCambioClave>()
                {
                    Objeto = dto,
                    Mensaje = "No se actualizo los datos",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("getUser")]
        public RespondSearchObject<UsuMUsuario> getUser(DTO.DTOUsuario dto)
        {
            var usuario = _context.UsuMUsuario.FirstOrDefault(item => item.Id == dto.Id);

            return new RespondSearchObject<UsuMUsuario>()
            {
                Objeto = usuario,
                Mensaje = "Se encontro Registro",
                Flag = true
            };
        }
    }
}
