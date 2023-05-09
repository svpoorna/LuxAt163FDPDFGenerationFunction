using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAt163FDPDFGenerationFunction
{
    public class ResponseMessageModel
    {
        public MemoryStream memoryStream { get; set; }
        public bool isRequestSuccess { get; set; }
        public string responseMessage { get; set; }
    }
}
