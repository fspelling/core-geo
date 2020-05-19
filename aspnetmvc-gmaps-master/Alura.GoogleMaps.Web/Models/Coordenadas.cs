using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alura.GoogleMaps.Web.Models
{
    public class Coordenada
    {
        public string Nome { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public Coordenada(string nome, string lat, string lng)
        {
            Nome = nome;
            Latitude = lat;
            Longitude = lng;
        }
    }
}