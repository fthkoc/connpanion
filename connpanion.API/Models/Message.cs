using System;

namespace connpanion.API.Models
{
    public class Message
    {
        public int ID { get; set; }
        public int SenderID { get; set; }
        public User Sender { get; set; }
        public int ReceiverID { get; set; }
        public User Receiver { get; set; }
        public string Content { get; set; }
        public bool isRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        public bool SenderDeleted { get; set; }
        public bool ReceiverDeleted { get; set; }
    }
}