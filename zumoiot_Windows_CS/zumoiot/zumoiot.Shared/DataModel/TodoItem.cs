using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace zumoiot
{
    public class IOTData
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "when")]
        public DateTime When { get; set; }

        [JsonProperty(PropertyName = "temp")]
        public Double Temperature { get; set; }

        [JsonProperty(PropertyName = "umid")]
        public Double Umidity { get; set; }

        [JsonProperty(PropertyName = "hwid")]
        public string DeviceLocation { get; set; }

        [JsonIgnore]
        public bool Selected { get; set; }

        [JsonIgnore]
        public string Text { 
            get {
#if WINDOWS_PHONE_APP
                return string.Format("T: {1,2:##}° - U: {2,3:###.0}% | {0}", this.DeviceLocation, this.Temperature, this.Umidity);
#else //WINDOWS_APP
                return string.Format("Temp: {1,2}° , Umid: {2,3}% | {0}", this.DeviceLocation, this.Temperature, this.Umidity);
#endif
            }
        }
    }
}
