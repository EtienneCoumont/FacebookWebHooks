using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
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
            try
            {
                string json;
                using (StreamReader sr = new StreamReader(this.Request.Body))
                {
                    json = sr.ReadToEnd();
                }

                StringBuilder sb = new StringBuilder();
                WriteStory(sb, json);
                sb.AppendLine();
                WriteDebug(sb, json);

                string msg = sb.ToString();

                _log.LogVerbose(msg);
                Mail.SendMail(_mailOptions, msg);
            }
            catch (Exception ex)
            {
                Mail.SendMail(_mailOptions, ex.Message);
            }
        }

        private void WriteDebug(StringBuilder sb, string json)
        {
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
