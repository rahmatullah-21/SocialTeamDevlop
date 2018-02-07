using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    public interface IUser
    {
        string Username { get; set; }

        string FullName { get; set; }

        string ProfilePicUrl { get; set;}
    }
}
