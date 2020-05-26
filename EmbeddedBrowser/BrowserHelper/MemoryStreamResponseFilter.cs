using CefSharp;
using DominatorHouseCore;
using System;
using System.IO;

namespace EmbeddedBrowser.BrowserHelper
{
    public class MemoryStreamResponseFilter : IResponseFilter
    {
        private MemoryStream memoryStream;

        bool IResponseFilter.InitFilter()
        {
            //NOTE: We could initialize this earlier, just one possible use of InitFilter
            memoryStream = new MemoryStream();
            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                //Calculate how much data we can read, in some instances dataIn.Length is
                //greater than dataOut.Length
                dataInRead = Math.Min(dataIn.Length, dataOut.Length);
                dataOutWritten = dataInRead;

                var readBytes = new byte[dataInRead];
                dataIn.Read(readBytes, 0, readBytes.Length);
                dataOut.Write(readBytes, 0, readBytes.Length);

                //Write buffer to the memory stream
                memoryStream.Write(readBytes, 0, readBytes.Length);

                //If we read less than the total amount avaliable then we need
                //return FilterStatus.NeedMoreData so we can then write the rest
                if (dataInRead < dataIn.Length)
                {
                    return FilterStatus.NeedMoreData;
                }

                if (memoryStream.Length > 0)
                    Data = memoryStream.ToArray();

                return FilterStatus.Done;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                dataInRead = 0;
                dataOutWritten = 0;
                return FilterStatus.Error;
            }
        }

        void IDisposable.Dispose()
        {
            try
            {
                memoryStream?.Dispose();
                memoryStream = null;
            }
            catch(Exception ex) { ex.DebugLog(); }
        }

        private byte[] _data;

        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }
}
