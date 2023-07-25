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
    [Route("api/jobner")]
    [ApiController]
    public class JobNerController : ControllerBase
    {
        private readonly Models.Opotunidades.db_oportunidades _context;
        private readonly IMapper _mapper;

        public JobNerController(Models.Opotunidades.db_oportunidades context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("cargaMasiva")]
        public async Task<RespondSearchObject<List<DTOJobNer>>> cargaMasiva(DTOCargaMasivaJobNer lista)
        {
            bool flag = true;
            foreach(var item in lista.cargaMasivas)
            {
                if(item.jobID>0 && item.ner >= 0)
                {
                    //Verifica si existe en bd
                    var entity = _context.OpMJobner.Where(l => l.FlgActivo == 1 && l.Job == item.jobID).FirstOrDefault();
                    //si existe realizar update

                    //sino existe insertar
                    if (entity == null)
                    {
                        var entityJobNer = new Models.OpMJobner
                        {
                            Job = item.jobID,
                            Ner = item.ner,

                            FlgActivo = 1,
                            UsuCreacion = lista.usuCreacion,
                            FecCreacion=DateTime.Now

                        };
                        
                        _context.OpMJobner.Add(entityJobNer);
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Make changes on entity

                        //Job=item.jobID,
                        entity.Ner = item.ner;
                        entity.FlgActivo = 1;
                        entity.UsuModificacion = lista.usuModificacion;
                        entity.FecModificacion = DateTime.Now;

                        try
                        {
                            _context.ChangeTracker.DetectChanges();
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            flag = false;
                            return new RespondSearchObject<List<DTOJobNer>>()
                            {
                                Objeto = null,
                                Mensaje = "No se Registro",
                                Flag = flag
                            };
                        }

                    }

                }

            }
            return new RespondSearchObject<List<DTOJobNer>>()
            {
                Objeto = null,
                Mensaje = "No se Registro",
                Flag = flag
            };
        }
        [HttpPost]
        [Route("buscarJobNer")]
        public async Task<RespondSearchObject<List<DTO.DTOJobNer>>> buscarJobNer(DTOJobNer dto)
        {
            //.Where(item =>  (dto.jobID.ToString()==""? true: item.Job.ToString().Contains(dto.jobID.ToString())))
            var entity = await _context.OpMJobner.Where(item => (dto.jobID == 0 ? true : item.Job.ToString().Contains(dto.jobID.ToString()))).Select(n=>new DTOJobNer() { 
                Id=n.Id,
                jobID=n.Job,
                ner=n.Ner,
                FlgActivo=n.FlgActivo
            }).ToListAsync();
            // añadir usu_creación y flag
            if (entity.Count()>0)
            {

                return new RespondSearchObject<List<DTOJobNer>>()
                {
                    Objeto = entity,
                    Mensaje = "Se encontro registros",
                    Flag = true
                };
            }
            else
            {
                return new RespondSearchObject<List<DTOJobNer>>()
                {
                    Objeto = null,
                    Mensaje = "No se encontro registros",
                    Flag = false
                };
            }
        }

        [HttpPost]
        [Route("grabarJobNer")]

        public  RespondSearchObject<DTO.DTOJobNer> grabarJobNer(DTOJobNer dto)
        {
            //21824381
            
            if (dto.jobID.ToString().Length == 8)
            {

            }

            var entity = new Models.OpMJobner
            {
                Job = dto.jobID,
                Ner = dto.ner,

                FlgActivo = dto.FlgActivo,
                UsuCreacion = dto.UsuCreacion,
                
            };
            /*
            entity.Property(p => p.Id)
    .UseSqlServerIdentityColumn();

            builder.Property(p => p.Id)
                .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            */
            try
            {
                //_context.OpMJobner.Add(entity);
                _context.OpMJobner.Add(entity);
                _context.SaveChanges();
                return new RespondSearchObject<DTOJobNer>()
                {
                    Objeto = dto,
                    Mensaje = "Registro exitoso",
                    Flag = true
                };
            }
            catch(Exception ex)
            {
                return new RespondSearchObject<DTOJobNer>()
                {
                    Objeto = dto,
                    Mensaje = "No se Registro",
                    Flag = false
                };
            }
        }



        [HttpPost]
        [Route("editarJobNer")]
        public  RespondSearchObject<DTOJobNer> editarJobNer( DTOJobNer dto)
        {
            // añadir flag y usu_modificacion
            var entity = _context.OpMJobner.FirstOrDefault(item => item.Id == dto.Id);
            // Validate entity is not null
            if (entity != null)
            {
                // Make changes on entity
                entity.Job = dto.jobID;
                entity.Ner = dto.ner;
                entity.FlgActivo = dto.FlgActivo;

                entity.UsuModificacion = dto.UsuModificacion;
                entity.FecModificacion = DateTime.Now;
                
            }
            try
            {
                //_context.OpMJobner.Update(entity);

                _context.ChangeTracker.DetectChanges();
                _context.SaveChanges();

                return new RespondSearchObject<DTOJobNer>()
                {
                    Objeto = dto,
                    Mensaje = "Se Actualizo los datos",
                    Flag = true
                };
            }
            catch(Exception ex)
            {

                return new RespondSearchObject<DTOJobNer>()
                {
                    Objeto = dto,
                    Mensaje = "No se actualizo los datos",
                    Flag = false
                };
            }
        }
    }
}
