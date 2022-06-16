namespace EPICOS_API.Models.Filters
{
    public class HubFilter
    {
        
        public string Id { get; set; }
        public string Name { get; set; }
        public string OfficeID { get; set; }
        public string FloorID { get; set; }

        public int Limit {get; set;} = 20;
        public int Page {get; set;} = 1;
    }
}