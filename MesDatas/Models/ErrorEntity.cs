using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    class ErrorEntity
    {
        public string FeedBackAddress { get; set; }
        public bool IsBlockingError { get; set; }
        public string UserMessage { get; set; }
        public string LogMessage { get; set; }
        public DateTime timeStamp;
    }
}
