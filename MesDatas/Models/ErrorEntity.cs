using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesDatas.Utility;

namespace MesDatas.Models
{
    class ErrorEntity
    {
        public string FeedBackAddress { get; set; }

        public short? FeedbackValue { get; set; }

        public bool IsBlockingError { get; set; }

        public string UserMessage { get; set; }

        public string LogMessage { get; set; }

        internal RouteCheckTraceContext RouteCheckTrace { get; set; }

        public DateTime timeStamp;
    }
}
