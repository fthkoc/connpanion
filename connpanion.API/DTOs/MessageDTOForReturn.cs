using System;

namespace connpanion.API.DTOs
{
    public class MessageDTOForReturn
    {
        public int ID { get; set; }
        public int SenderID { get; set; }
        public string SenderKnownAs { get; set; }
        public string SenderPhotoURL { get; set; }
        public int ReceiverID { get; set; }
        public string ReceiverKnownAs { get; set; }
        public string ReceiverPhotoURL { get; set; }
        public string Content { get; set; }
        public bool isRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}