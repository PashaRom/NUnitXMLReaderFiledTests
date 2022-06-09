using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NUnitXMLReader.Models
{
    public class ConfigModel
    {
        [JsonPropertyName("dependences")]
        public List<Dependant> Dependences { get; set; }       
    }
}
