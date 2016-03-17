using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    /// <summary>
    /// Mail Options. Set in appsettings.json
    /// </summary>
    public class MailOptions
    {
        public string FromName { get; set; }
        public string FromMail { get; set; }
        public string ToName { get; set; }
        public string ToMail { get; set; }

        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpUseSsl { get; set; }
        public string SmtpLogin { get; set; }
        public string SmtpPassword { get; set; }
    }
}
