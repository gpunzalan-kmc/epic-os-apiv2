namespace EPICOS_API.Models.Entities
{
    public class FloorResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public int OfficeID { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public int HubCount { get; set; }
        public int WorkpointCount {get;set;}
    }
}