using System;

namespace connpanion.API.DTOs
{
    public class PhotographDTOForReturn
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMainPhotograph { get; set; }
        public string publicID { get; set; }
    }
}