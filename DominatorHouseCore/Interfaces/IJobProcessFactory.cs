using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    /// Each library have to implement its own factory
    /// </summary>
    public interface IJobProcessFactory
    {
        JobProcess Create(string moduleName);
    }
}
