using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookMessenger.Models;

namespace FacebookMessenger.Storage
{
    public interface IMessageRepository
    {
        IList<Message> GetAllMessages();
        void AddMessage(Message message);
    }

    public class MessageRepository : IMessageRepository
    {
        private readonly IList<Message> messages;

        public MessageRepository()
        {
            messages = new List<Message>
            {
                new Message
                {
                    SenderId = "1",
                    Text = "1 sending a message"
                },
                new Message
                {
                    SenderId = "2",
                    Text = "2 sending a message"
                }
            };
        }

        public IList<Message> GetAllMessages()
        {
            return messages;
        }

        public void AddMessage(Message message)
        {
            messages.Add(message);
        }
    }
}
