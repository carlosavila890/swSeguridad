﻿using System;

using Microsoft.AspNetCore.Http;


namespace bd.swseguridad.entidades.ObjectTranfer
{
  public  class PermisoUsuarioSwExternos
    {
        public int Id { get; set; }
        public string NombreServicio { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public object parametros { get; set; }
        public DateTime Fecha { get; set; }
    }
}
