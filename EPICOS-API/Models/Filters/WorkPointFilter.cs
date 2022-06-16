namespace EPICOS_API.Models.Filters
{
    public class WorkPointFilter
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string isActive { get; set; }

        public string FloorID { get; set; }

        public string OfficeID { get; set; }

        public string LocationType { get; set; }

        public int Limit {get; set;} = 10;
        public int Page {get; set;} = 1;
    }
}