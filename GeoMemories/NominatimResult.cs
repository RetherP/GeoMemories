using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GeoMemories
{
    public class NominatimResult
    {
        [JsonPropertyName("place_id")]
        public long PlaceId { get; set; }
        [JsonPropertyName("lat")]
        public string lattitude { get; set; }
        [JsonPropertyName("lon")]
        public string longitude { get; set; }
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }
}
