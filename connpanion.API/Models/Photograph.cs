using System;

namespace connpanion.API.Models
{
    public class Photograph
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMainPhotograph { get; set; }
        public User User { get; set; }
        public int UserID { get; set; }
    }
}