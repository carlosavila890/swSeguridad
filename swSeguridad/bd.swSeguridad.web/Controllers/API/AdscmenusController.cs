using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bd.swseguridad.datos;
using bd.swseguridad.entidades.Negocio;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.swseguridad.entidades.Enumeradores;
using bd.swseguridad.entidades.Utils;
using bd.log.guardar.Enumeradores;
using bd.swseguridad.entidades.ViewModels;
using bd.swseguridad.entidades.Constantes;
using bd.swseguridad.entidades.ObjectTranfer;

namespace bd.swseguridad.web.Controllers.API
{
    /// <summary>
    /// Controladores API de los servicios web estos son los que trabajan directamente con la base de datos
    /// Tiene algunos aspectos que explicar como:
    /// [Produces("application/json")]: filtro especifica los formatos de 
    /// respuesta para una acci�n espec�fica (o controlador). 
    /// Al igual que la mayor�a de los filtros , 
    /// esto se puede aplicar en la acci�n, el controlador o el alcance global.
    /// para m�s informaci�n visitar:https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting
    /// [Route("api/Codificar")]:es la ruta del recurso del controlador en general
    /// public class NombreController : Controller
    /// [Route("Nombre")] en los m�todos es la ruta del recurso.
    /// para acceder a estos recursos es: Host + ruta concatenados 
    /// , ruta del controlador + ruta del m�todo
    /// [FromBody] es para capturar el objeto que se envia en el body 
    /// para m�s informaci�n visitar:https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api
    /// </summary>
    [Produces("application/json")]
    [Route("api/Adscmenus")]
    public class AdscmenusController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public AdscmenusController(SwSeguridadDbContext db)
        {
            this.db = db;
        }


        [HttpPost]
        [Route("ObtenerMenuPorUsuarioSistema")]
        public async Task<List<Adscmenu>> ObtenerMenuPorUsuarioSistema([FromBody] UsuarioSistema usuarioSistema)
        {
            var listaSalida = new List<Adscmenu>();

            var grupos = await db.Adscmiem.Where(m => m.AdmiEmpleado.ToUpper() == usuarioSistema.Usuario.ToUpper()).ToListAsync();
            foreach (var item in grupos)
            {
                var a = await db.Adscexe.Where(x => x.AdexGrupo == item.AdmiGrupo).ToListAsync();
                foreach (var s in a)
                {
                    var ds = await db.Adscmenu.Where(x => x.AdmeSistema == usuarioSistema.Sistema && x.AdmeAplicacion == s.AdexAplicacion).FirstOrDefaultAsync();
                    if (ds != null)
                    {
                        listaSalida.Add(ds);
                    }

                }
            }
            return listaSalida;

        }


        [HttpPost]
        [Route("ObtenerMenuPadrePorUsuarioSistema")]
        public async Task<List<Adscmenu>> ObtenerMenuPadrePorUsuarioSistema([FromBody] MenuPadreSistemaUsuario sistemaUsuario)
        {
            var listaSalida = new List<Adscmenu>();

                var grupos = await db.Adscmiem.Where(m => m.AdmiEmpleado.ToUpper() == sistemaUsuario.Usuario.ToUpper()).ToListAsync();
                foreach (var item in grupos)
                {
                    var a = await db.Adscexe.Where(x => x.AdexGrupo == item.AdmiGrupo).ToListAsync();
                    foreach (var s in a)
                    {
                        var ds = await db.Adscmenu.Where(x => x.AdmeSistema == sistemaUsuario.Sistema && x.AdmeAplicacion == s.AdexAplicacion && (x.AdmePadre==null || x.AdmePadre==Convert.ToString(0))).FirstOrDefaultAsync();
                    if (ds!=null)
                    {
                        listaSalida.Add(ds);
                    }
                         
                    }
                }
            return listaSalida;

        }

        [HttpPost]
        [Route("ObtenerHijosPorUsuarioSistema")]
        public async Task<List<Adscmenu>> ObtenerMenuPadrePorUsuarioSistema([FromBody] MenuHijoSistema menuHijoSistema)
        {
            var listaSalida = new List<Adscmenu>();

            var grupos = await db.Adscmiem.Where(m => m.AdmiEmpleado.ToUpper() == menuHijoSistema.Usuario.ToUpper()).ToListAsync();
            foreach (var item in grupos)
            {
                var a = await db.Adscexe.Where(x => x.AdexGrupo == item.AdmiGrupo).ToListAsync();
                foreach (var s in a)
                {
                    var ds = await db.Adscmenu.Where(x => x.AdmeSistema == menuHijoSistema.AdmeSistema && x.AdmeAplicacion == s.AdexAplicacion && x.AdmePadre == menuHijoSistema.AdmeAplicacion).FirstOrDefaultAsync();
                    if (ds!=null)
                    {
                        listaSalida.Add(ds);
                    }
                    
                }
            }
            return listaSalida;

        }


        [HttpGet]
        [Route("ListarMenuDistinct")]
        public async Task<List<Adscmenu>> GetMenusDistinct()
        {
            try
            {
                return await db.Adscmenu.GroupBy(x => x.AdmeSistema).Select(group => group.First()).ToListAsync(); ;
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new List<Adscmenu>();
            }
        }

        public async Task<Adscmenu> ObtenerMenuPadreRecursivo(Adscmenu adscmenu)
        {
            var menu =await db.Adscmenu.Where(x => x.AdmeAplicacion == adscmenu.AdmePadre).FirstOrDefaultAsync();

            switch (menu.AdmePadre)
            {
                case null:
                    return menu;
                    
                case "0":
                    return menu;
            }
           
           menu= await ObtenerMenuPadreRecursivo(menu);
           return menu;
        }

        //M�dulo es cuando el padre primario es null o 0
        [HttpPost]
        [Route("GetMenuPadre")]
        public async Task<Response> GetMenuPadre([FromBody] ModuloAplicacion moduloAplicacion )
        {
            try
            {

               var request= await db.Adscmenu
                                    .Where(x=>x.AdmeSistema==moduloAplicacion.NombreAplicacion 
                                           && x.AdmeControlador==moduloAplicacion.Path).FirstOrDefaultAsync();
                if (request==null)
                {
                    return new Response { IsSuccess = false };
                }


                if (request.AdmePadre==null || request.AdmePadre== Convert.ToString(0))
                {
                    return new Response { IsSuccess = true, Resultado = request };
                }

                var menu =await ObtenerMenuPadreRecursivo(request);
            
                return new Response { IsSuccess=true,Resultado=menu }; 
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response {IsSuccess=false};
            }
        }


        // GET: api/Adscmenus
        [HttpGet]
        [Route("ListarMenu")]
        public async Task<List<Adscmenu>> GetMenus()
        {
            try
            {
                return await db.Adscmenu.OrderBy(x => x.AdmeSistema).ThenBy(x => x.AdmeAplicacion).ToListAsync();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new List<Adscmenu>();
            }
        }

        // GET: api/Adscmenus/5
        [HttpPost]
        [Route("SeleccionarAdscMenu")]
        public async Task<Response> GetAdscMenu([FromBody] Adscmenu adscmenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido,
                    };
                }

                var adscgrpSeleccionado = await db.Adscmenu.SingleOrDefaultAsync(m => m.AdmeSistema == adscmenu.AdmeSistema && m.AdmeAplicacion == adscmenu.AdmeAplicacion);

                if (adscgrpSeleccionado == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.Satisfactorio,
                    Resultado = adscgrpSeleccionado,
                };
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Error,
                };
            }
        }

        [HttpPut]
        [Route("EditarAdscmenu")]
        public async Task<Response> PutAdscmenu([FromBody] Adscmenu adscmenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido
                    };
                }

                var adscmenuSeleccionado = await db.Adscmenu.SingleOrDefaultAsync(m => m.AdmeAplicacion == adscmenu.AdmeAplicacion && m.AdmeSistema == adscmenu.AdmeSistema);
                if (adscmenuSeleccionado != null)
                {
                    try
                    {
                        adscmenuSeleccionado.AdmeDescripcion = adscmenu.AdmeDescripcion;
                        adscmenuSeleccionado.AdmeElemento = adscmenu.AdmeElemento;
                        adscmenuSeleccionado.AdmeEnsamblado = adscmenu.AdmeEnsamblado;
                        adscmenuSeleccionado.AdmeEstado = adscmenu.AdmeEstado;
                        adscmenuSeleccionado.AdmeObjetivo = adscmenu.AdmeObjetivo;
                        adscmenuSeleccionado.AdmeOrden = adscmenu.AdmeOrden;
                        adscmenuSeleccionado.AdmePadre = adscmenu.AdmePadre;
                        adscmenuSeleccionado.AdmeTipo = adscmenu.AdmeTipo;
                        adscmenuSeleccionado.AdmeTipoObjeto = adscmenu.AdmeTipo;
                        adscmenuSeleccionado.AdmeUrl = adscmenu.AdmeUrl;
                        adscmenuSeleccionado.AdmeControlador = adscmenu.AdmeControlador;
                        db.Adscmenu.Update(adscmenuSeleccionado);
                        await db.SaveChangesAsync();

                        return new Response
                        {
                            IsSuccess = true,
                            Message = Mensaje.Satisfactorio,
                            Resultado=adscmenuSeleccionado,
                        };

                    }
                    catch (Exception ex)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                            ExceptionTrace = ex,
                            Message = Mensaje.Excepcion,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                            UserName = "",

                        });
                        return new Response
                        {
                            IsSuccess = false,
                            Message = Mensaje.Error,
                        };
                    }
                }

                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.ExisteRegistro
                };
            }
            catch (Exception)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Excepcion
                };
            }
        }


        [HttpPost]
        [Route("ListarPadresPorSistema")]
        public async Task<List<Adscmenu>> ListarPadresPorSistema([FromBody] Adscmenu adscmenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new List<Adscmenu>();
                }

                var ListaHijos = await db.Adscmenu.Where(x => x.AdmeSistema == adscmenu.AdmeSistema).ToListAsync();
                return ListaHijos;

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new List<Adscmenu>();
            }
        }

        [HttpPost]
        [Route("DetalleAdscmenu")]
        public async Task<DetalleMenu> DetailsAdscgrp([FromBody] Adscmenu adscmenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new DetalleMenu();
                }

                var menu = await db.Adscmenu.FirstOrDefaultAsync(x=>x.AdmeSistema==adscmenu.AdmeSistema && x.AdmeAplicacion==adscmenu.AdmeAplicacion);

                if (menu==null)
                {
                    return new DetalleMenu();
                }

                var listaHijos =await db.Adscmenu.Where(x=>x.AdmePadre==adscmenu.AdmeAplicacion).ToListAsync();

                var detalleMenu = new DetalleMenu
                {
                    AdmeAplicacion=menu.AdmeAplicacion,
                    AdmeDescripcion=menu.AdmeDescripcion,
                    AdmeElemento=menu.AdmeElemento,
                    AdmeEnsamblado=menu.AdmeEnsamblado,
                    AdmeEstado=menu.AdmeEstado,
                    AdmeObjetivo=menu.AdmeObjetivo,
                    AdmeOrden=menu.AdmeOrden,
                    AdmePadre=menu.AdmePadre,
                    AdmeSistema=menu.AdmeSistema,
                    AdmeTipo=menu.AdmeTipo,
                    AdmeTipoObjeto=menu.AdmeTipoObjeto,
                    AdmeUrl=menu.AdmeUrl,
                    ListaHijos=listaHijos,
                };

                return detalleMenu;

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new DetalleMenu();
            }
        }


        // POST: api/Adscmenus
        [HttpPost]
        [Route("InsertarAdscmenu")]
        public async Task<Response> PostAdscgrp([FromBody] Adscmenu adscmenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido
                    };
                }

                var respuesta = Existe(adscmenu);
                if (!respuesta.IsSuccess)
                {
                    db.Adscmenu.Add(adscmenu);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = Mensaje.Satisfactorio,
                        Resultado=adscmenu,
                    };
                }

                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.ExisteRegistro
                };

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Error,
                };
            }
        }

        // DELETE: api/Adscmenus/5
        [HttpPost]
        [Route("EliminarAdscmenu")]
        public async Task<Response> DeleteAdscgrp([FromBody]Adscmenu adscmenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido,
                    };
                }
                var adscmenuSeleccionado = await db.Adscmenu.SingleOrDefaultAsync(m => m.AdmeAplicacion == adscmenu.AdmeAplicacion && m.AdmeSistema == adscmenu.AdmeSistema);
                if (adscmenuSeleccionado == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }
                db.Adscmenu.Remove(adscmenuSeleccionado);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.Satisfactorio,
                    Resultado=adscmenuSeleccionado,
                };
            }
            catch (Exception ex)
            {

                if (ex.InnerException.Message.Contains(Constantes.Referencia))
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.BorradoNoSatisfactorio,
                    };

                }
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Error,
                };
            }
        }

        public Response Existe(Adscmenu adscmenu)
        {
            var sistema = adscmenu.AdmeSistema.ToUpper().TrimEnd().TrimStart();
            var aplicacion = adscmenu.AdmeAplicacion.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscmenu.Where(p => p.AdmeSistema.ToUpper().TrimStart().TrimEnd() == sistema && p.AdmeAplicacion.ToUpper().TrimStart().TrimEnd() == aplicacion).FirstOrDefault();
            if (loglevelrespuesta != null)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.ExisteRegistro,
                    Resultado = null,
                };
            }
            return new Response
            {
                IsSuccess = false,
                Resultado = loglevelrespuesta,
            };
        }
    }
}