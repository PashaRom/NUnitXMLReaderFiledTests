using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NUnitXMLReader.Models
{
    public class Dependant
    {
        [JsonPropertyName("main")]
        public int Main { get; set; }
        [JsonPropertyName("dependents")]
        public List<int> Dependents { get; set; }
    }
}
