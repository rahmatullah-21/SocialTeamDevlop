using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Process
{
    public class JobProcessResult
    {
        public bool IsProcessCompleted { get; set; }

        public bool IsProcessSuceessfull { get; set; }

        public bool HasNoResult { get; set; }

        public string maxId { get; set; }

    }
}
