using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Alura.GoogleMaps.Web.Models
{
    public class Aeroporto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> loc { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string code { get; set; }

        public static explicit operator Coordenada(Aeroporto aero)
        {
            return new Coordenada(aero.name,
                                  aero.loc.Coordinates.Latitude.ToString().Replace(",", "."),
                                  aero.loc.Coordinates.Longitude.ToString().Replace(",", "."));
        }
    }
}