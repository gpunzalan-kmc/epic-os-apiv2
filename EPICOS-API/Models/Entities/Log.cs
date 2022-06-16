using System;

namespace EPICOS_API.Models.Entities
{
    public class Log
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public string MAC { get; set; }
        public string IPaddress { get; set; }
        public string Message { get; set; }
        public bool IsDeleted { get; set; }
    }
}