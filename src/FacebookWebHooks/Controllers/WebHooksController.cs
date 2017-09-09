using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace FacebookWebHooks.Controllers
{
    [Route("api/[controller]")]
    public class WebHooksController : Controller
    {
        FacebookOptions _fbOptions;
        MailOptions _mailOptions;
        ILogger<WebHooksController> _log;

        public WebHooksController(IOptions<FacebookOptions> fbOptions, IOptions<MailOptions> mailOptions,
            ILogger<WebHooksController> logger)
        {
            _fbOptions = fbOptions.Value;
            _mailOptions = mailOptions.Value;
            _log = logger;
        }

        // GET: api/webhooks
        [HttpGet]
        public string Get([FromQuery(Name = "hub.mode")] string hub_mode,
            [FromQuery(Name = "hub.challenge")] string hub_challenge,
            [FromQuery(Name = "hub.verify_token")] string hub_verify_token)
        {
            if (_fbOptions.VerifyToken == hub_verify_token)
            {
                _log.LogInformation("Get received. Token OK : {0}", hub_verify_token);
                return hub_challenge;
            }
            else
            {
                _log.LogError("Error. Token did not match. Got : {0}, Expected : {1}", hub_verify_token, _fbOptions.VerifyToken);
                return "error. no match";
            }
        }

        // POST api/values
        [HttpPost]
        public void Post()
        {
            string json;
            try
            {
                using (StreamReader sr = new StreamReader(this.Request.Body))
                {
                    json = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical("Error during body read.", ex);
                Mail.SendMail(_mailOptions, "Error during body read : " + ex.Message);
                return;
            }

            StringBuilder sb = new StringBuilder();
            bool errorRaised = false;
            try
            {
                if (_fbOptions.ShouldVerifySignature)
                {
                    VerifySignature(json);
                }
                WriteStory(sb, json);
            }
            catch (Exception ex)
            {
                errorRaised = true;
                sb.AppendLine("Error during webhook processing : " + ex.Message);
            }

            WriteDebug(sb, json);

            string msg = sb.ToString();
            if (errorRaised)
                _log.LogError(msg);
            else
                _log.LogTrace(msg);

            try
            {
                Mail.SendMail(_mailOptions, msg);
            }
            catch (Exception ex)
            {
                _log.LogError("Can't send email", ex);
            }
        }

        private void VerifySignature(string json)
        {
            var signatures = this.Request.Headers.Where(h => h.Key == "X-Hub-Signature").ToArray();
            if (signatures.Length == 0)
                throw new Exception("X-Hub-Signature not found");
            if (signatures.Length >= 2)
                throw new Exception("Many X-Hub-Signature found");
            string headerHash = signatures[0].Value;
            if (headerHash == null)
                throw new Exception("X-Hub-Signature is null");
            if (!headerHash.StartsWith("sha1="))
                throw new Exception("Unexpected format of X-Hub-Signature : " + headerHash);
            headerHash = headerHash.Substring(5);

            string myHash = Hash.ComputeHash(_fbOptions.AppSecret, json);
            if (myHash == null)
                throw new Exception("Unexpected null hash");
            if (!myHash.Equals(headerHash, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Hash did not match. Expected {myHash}. But header was {headerHash}");
        }

        private void WriteDebug(StringBuilder sb, string json)
        {
            sb.AppendLine();
            sb.Append("<pre>");
            sb.Append(this.Request.QueryString.ToUriComponent());
            foreach (var header in this.Request.Headers)
            {
                sb.Append(header.Key + ": " + header.Value + "\r\n");
            }
            sb.Append("\r\n" + json + "\r\n\r\n" + Hash.ComputeHash(_fbOptions.AppSecret, json));
            sb.Append("</pre>");
        }

        private void WriteStory(StringBuilder sb, string json)
        {
            // alternative : var json = JToken.Parse(body);
            var updateObj = JsonConvert.DeserializeObject<UpdateObject>(json);

            switch (updateObj.Object)
            {
                case ObjectEnum.Page:
                    if (updateObj.Entry == null)
                    {
                        sb.AppendLine("Null Entry");
                        break;
                    }
                    foreach (var entry in updateObj.Entry)
                    {
                        WriteEntry(sb, entry);
                    }
                    break;
                default:
                    sb.AppendLine("Not implemented object : " + updateObj.Object);
                    break;
            }
        }

        private void WriteEntry(StringBuilder sb, Entry entry)
        {
            if (entry.Changes == null)
            {
                sb.AppendLine($"Null Changes for entry {entry.Id}");
                return;
            }
            foreach (var change in entry.Changes)
            {
                WriteChange(sb, change);
            }
        }

        private void WriteChange(StringBuilder sb, Change change)
        {
            if (change.Field != "feed")
            {
                sb.AppendLine("Field updated : " + change.Field);
                return;
            }
            WriteValue(sb, change.Value);
        }

        private void WriteValue(StringBuilder sb, Value value)
        {
            if (value == null)
            {
                sb.AppendLine("Value null");
                return;
            }

            switch (value.Item)
            {
                case "share":
                    sb.AppendLine($"{value.SenderName} shared the link {value.Link}<br/>");
                    sb.AppendLine(value.Message);
                    break;
                case "like":
                    if (value.Verb != "add")
                    {
                        sb.AppendLine($"Unknown verb for like {value.Verb}");
                    }
                    else if (value.PostId == null)
                    {
                        sb.AppendLine($"{value.UserId} liked the page");
                    }
                    else
                    {
                        sb.AppendLine($"{value.SenderName} liked the post {value.PostId}");
                    }
                    break;
                case "photo":
                    if (value.Verb != "add")
                    {
                        sb.AppendLine($"Unknown verb for photo {value.Verb}");
                    }
                    else
                    {
                        sb.AppendLine($"{value.SenderName} posted a new photo:");
                        sb.AppendLine(value.Link);
                        sb.AppendLine($"<img src=\"{value.Link}\" style=\"width:100%;\"/>");
                        if (!string.IsNullOrEmpty(value.Message))
                            sb.AppendLine(value.Message);
                    }
                    break;
                case "status":
                    if (value.Verb != "add")
                    {
                        sb.AppendLine($"Unknown verb for status {value.Verb}");
                    }
                    else
                    {
                        sb.AppendLine($"{value.SenderName} posted a new status:");
                        sb.AppendLine(value.Message);
                    }
                    break;
                default:
                    sb.AppendLine($"Unknown item '{value.Item}'");
                    break;
            }
        }
    }
}
