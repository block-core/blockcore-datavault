using Blockcore.DataVault.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Settings
{
    public class BlockcoreDataVaultSettings
    {
        public ApiKeys API { get; set; }
    }

    public class ApiKeys
    {
        public List<ApiKey> Keys { get; set; }
    }
}
