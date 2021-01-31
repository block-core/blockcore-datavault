using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Models
{
    public class ResultModel
    {
        [DataMember(Name = "result")]
        public object Result { get; set; }
    }
}
