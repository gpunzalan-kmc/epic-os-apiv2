namespace EPICOS_API.Models.Entities
{
    public class Site
    {
        public int BuildingID { get; set; }
        public string Name { get; set; }
        public string Line1 { get; set; }
        public int HubCount { get; set; }
        public int WorkPointCount { get; set; }
        public int FloorCount { get; set; }
    }
}