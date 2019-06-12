using System;

namespace connpanion.API.DTOs
{
    public class MessageDTOForCreate
    {
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }

        public MessageDTOForCreate()
        {
            MessageSent = DateTime.Now;
        }
    }
}