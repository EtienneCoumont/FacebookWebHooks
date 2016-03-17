using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    /// <summary>
    /// Facebook Options. See appsettings.json for details
    /// </summary>
    public class FacebookOptions
    {
        public string VerifyToken { get; set; }
        public bool ShouldVerifySignature { get; set; }
        public string AppSecret { get; set; }
        public string AppToken { get; set; }
    }
}
