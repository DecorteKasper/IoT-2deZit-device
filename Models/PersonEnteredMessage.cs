using System;

namespace IoT_2deZit_device.Models
{
    public class PersonEnteredMessage
    {
        public string DeviceId { get; set; }

        public string Naam { get; set; }

        public int Leeftijd { get; set; }

        public DateTime Datum { get; set; }

        public int GuidId { get; set; }
    }
}
