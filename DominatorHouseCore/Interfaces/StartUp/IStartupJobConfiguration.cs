using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces.StartUp
{
    public interface IStartupJobConfiguration
    {
        JobConfiguration JobConfiguration { get; set; }
    }
}
