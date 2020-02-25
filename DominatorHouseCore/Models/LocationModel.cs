using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class LocationModel
    {

        [ProtoMember(1)]
        public string CountryName { get; set; }

        [ProtoMember(2)]
        public string CityName { get; set; }

        [ProtoMember(3)]
        public bool IsSelected { get; set; }


    }
}
