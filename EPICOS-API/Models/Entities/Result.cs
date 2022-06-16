using System;

namespace EPICOS_API.Models.Entities
{
    public class Result
    {
        public int ID { get; set; }
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
        public string ExceptionMessage { get; set; }
    }
}