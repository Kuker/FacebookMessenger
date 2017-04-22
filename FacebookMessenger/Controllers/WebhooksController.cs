using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FacebookMessenger.JsonModels;
using FacebookMessenger.Options;
using FacebookMessenger.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FacebookMessenger.Controllers
{
    [Route("api/[controller]")]
    public class WebhooksController : Controller
    {
        private readonly WebhookOptions webhookOptions;
        private readonly ILogger<WebhooksController> logger;
        private readonly IMessageRepository messageRepository;

        public WebhooksController(IOptions<WebhookOptions> webhookOptions, ILogger<WebhooksController> logger, IMessageRepository messageRepository)
        {
            this.logger = logger;
            this.messageRepository = messageRepository;
            this.webhookOptions = webhookOptions.Value;
        }

        [HttpGet]
        public string Get([FromQuery(Name = "hub.challenge")] string hubChallenge,
            [FromQuery(Name = "hub.mode")] string hubMode,
            [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
        {
            if (hubVerifyToken == webhookOptions.VerifyToken)
            {
                logger.LogInformation("Tokens match: {0}", hubVerifyToken);
                return hubChallenge;
            }

            logger.LogError("Tokens do not match. Got: {0}. Expected: {1}", hubVerifyToken, webhookOptions.VerifyToken);
            Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return "Token verification error";
        }

        [HttpPost]
        public void Webhooks()
        {
            string json;
            try
            {
                using (StreamReader sr = new StreamReader(this.Request.Body))
                {
                    json = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                logger.LogCritical("Could not parse request body.", e);
                return;
            }

            logger.LogInformation("Successfully read json from request body: {0}", json);

            FacebookRequestModel data = new FacebookRequestModel();
            try
            {
                data = JsonConvert.DeserializeObject<FacebookRequestModel>(json);
            }
            catch (Exception e)
            {
                logger.LogError("Could not convert json to {0}. {1}", nameof(FacebookRequestModel), e);
            }

            if (data.@object == "page")
            {
                foreach (var entry in data.entry)
                {
                    var pageId = entry.id;
                    var timeOfEvent = entry.time;

                    foreach (var evt in entry.messaging)
                    {
                        if (evt.message != null)
                        {
                            //todo
                            var message = new Models.Message
                            {
                                SenderId = evt.recipient.id,
                                Text = evt.message.text
                            };
                            messageRepository.AddMessage(message);
                            logger.LogInformation("Webook received message event. Message text: {0}",evt.message.text);
                        }
                        else
                        {
                            logger.LogInformation("Webhook received unknown event");
                        }
                    }
                }
            }

            Response.StatusCode = (int) HttpStatusCode.OK;
        }
    }
}
