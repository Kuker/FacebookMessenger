using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FacebookMessenger.Models;
using FacebookMessenger.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FacebookMessenger.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageRepository messageRepository;

        public HomeController(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Messages()
        {
            var messages = messageRepository.GetAllMessages();
            return View(messages);
        }

        [HttpPost]
        public IActionResult AddMessage([FromBody]Message message)
        {
            messageRepository.AddMessage(message);
            return new OkResult();
        }
    }
}
