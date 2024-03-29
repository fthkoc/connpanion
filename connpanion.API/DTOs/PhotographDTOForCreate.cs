using System;
using Microsoft.AspNetCore.Http;

namespace connpanion.API.DTOs
{
    public class PhotographDTOForCreate
    {
        public string URL { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
    
        public PhotographDTOForCreate()
        {
            DateAdded = DateTime.Now;
        }
    }
}