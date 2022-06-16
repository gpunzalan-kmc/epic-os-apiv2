using System;

namespace EPICOS_API.Models.Filters
{
    public class TelemeryFilter
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int OfficeID { get; set; }
        public int FloorID { get; set; }
    }
}