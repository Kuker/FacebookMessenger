using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookMessenger.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacebookMessenger.Controllers
{
    [Route("api/[controller]")]
    public class WebhooksController : Controller
    {
        private readonly WebhookOptions webhookOptions;
        private readonly ILogger<WebhooksController> logger;

        public WebhooksController(IOptions<WebhookOptions> webhookOptions, ILogger<WebhooksController> logger)
        {
            this.logger = logger;
            this.webhookOptions = webhookOptions.Value;
        }

        [HttpGet]
        public string Get([FromQuery(Name = "hub.challenge")] string hubChallenge,
            [FromQuery(Name = "hub.mode")] string hubMode,
            [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
        {
            if (hubVerifyToken == webhookOptions.VerifyToken)
            {
                logger.LogDebug("Tokens match: {0}", hubVerifyToken);
                return hubChallenge;
            }

            logger.LogError("Tokens do not match. Got: {0}. Expected: {1}", hubVerifyToken, webhookOptions.VerifyToken);
            return "Token verification error";
        }
    }
}
