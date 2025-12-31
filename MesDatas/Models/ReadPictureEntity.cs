using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesDatas.Services;

namespace MesDatas.Models
{
    class ReadPictureEntity
    {
        public string ReadPlcAddress { get; set; }

        public string WritePlcAddress { get; set; }

        public AddressCombine ReadSNPlcAddress { get; set; }
    }
}
