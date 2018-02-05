using System;

namespace DominatorHouseCore.Requests
{
    public class ResponseParameter : IResponseParameter
    {
        public bool HasError { get; set; }

        public string Response { get; set; }

        public Exception Exception { get; set; }

    }


    public interface IResponseParameter
    {
         bool HasError { get; set; }

         string Response { get; set; }

         Exception Exception { get; set; }

    }


}
