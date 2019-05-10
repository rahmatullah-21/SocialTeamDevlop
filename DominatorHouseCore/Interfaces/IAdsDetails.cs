using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    public interface IAdsDetails
    {
        string Id { get; set; }

        string PostId { get; set; }

        string OwnerId { get; set; }

        string CountryName { get; set; }
    }
}
