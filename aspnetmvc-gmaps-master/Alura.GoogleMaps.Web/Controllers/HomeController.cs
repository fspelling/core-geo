using Alura.GoogleMaps.Web.Geocoding;
using Alura.GoogleMaps.Web.Models;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Alura.GoogleMaps.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //coordenadas quaisquer para mostrar o mapa
            var coordenadas = new Coordenada("São Paulo", "-23.64340873969638", "-46.730219057147224");
            return View(coordenadas);
        }

        public async Task<JsonResult> Localizar(HomeViewModel model)
        {
            Debug.WriteLine(model);

            //Captura a posição atual e adiciona a lista de pontos
            Coordenada coordLocal = ObterCoordenadasDaLocalizacao(model.Endereco);
            var aeroportosProximos = new List<Coordenada>();
            aeroportosProximos.Add(coordLocal);

            //Captura a latitude e longitude locais
            double lat = Convert.ToDouble(coordLocal.Latitude.Replace(".", ","));
            double lon = Convert.ToDouble(coordLocal.Longitude.Replace(".", ","));

            //Testa o tipo de aeroporto que será usado na consulta
            string tipoAero = model.Tipo == TipoAeroporto.Internacionais ? "International" : 
                                model.Tipo == TipoAeroporto.Municipais ? "Municipal" : string.Empty;

            //Captura o valor da distancia
            int distancia = model.Distancia * 1000;

            //Conecta MongoDB
            var mongodb = new MongodbConnection();

            //Configura o ponto atual no mapa
            var locAtual = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(lon, lat));

            // filtro
            var builder = Builders<Aeroporto>.Filter;
            FilterDefinition<Aeroporto> filter;

            if (tipoAero == string.Empty)
                filter = builder.NearSphere(p => p.loc, locAtual, distancia);
            else
                filter = builder.NearSphere(p => p.loc, locAtual, distancia) & builder.Eq(p => p.type, tipoAero);

            //Captura a lista
            var listaAero = await mongodb.Airports.Find(filter).ToListAsync();

            //Escreve os pontos
            listaAero.ForEach(aereo => aeroportosProximos.Add((Coordenada)aereo));

            return Json(aeroportosProximos);
        }

        private Coordenada ObterCoordenadasDaLocalizacao(string endereco)
        {
            try
            {
                string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={endereco}";
                Debug.WriteLine(url);

                var coord = new Coordenada("Não Localizado", "-10", "-10");
                var response = new WebClient().DownloadString(url);
                var googleGeocode = JsonConvert.DeserializeObject<GoogleGeocodeResponse>(response);
                //Debug.WriteLine(googleGeocode);

                if (googleGeocode.status == "OK")
                {
                    coord.Nome = googleGeocode.results[0].formatted_address;
                    coord.Latitude = googleGeocode.results[0].geometry.location.lat;
                    coord.Longitude = googleGeocode.results[0].geometry.location.lng;
                }

                return coord;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}