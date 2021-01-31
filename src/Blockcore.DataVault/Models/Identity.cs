using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Models
{
    public class Identity
    {
        public Identity()
        {

        }

        public string Id { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public string ApiKey { get; set; }
    }
}
