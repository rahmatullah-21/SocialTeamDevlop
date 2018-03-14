using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    //public class IPost
    //{
    //    string Id { get; set; }
    //}

    public interface IPost
    {
        string Id { get; set; }
        string Caption { get; set; }
        string Code { get; set; }

    }

}
