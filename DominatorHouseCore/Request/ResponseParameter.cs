using System;
using DominatorHouseCore.Interfaces;

namespace DominatorHouseCore.Request
{
    public class ResponseParameter : IResponseParameter
    {
        public bool HasError { get; set; }

        public string Response { get; set; }

        public Exception Exception { get; set; }

    }
}
