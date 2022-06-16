namespace EPICOS_API.Models.Filters
{
    public class OfficeFilter
    {
        public int Siteid { get; set; }
        public string Name { get; set; }
        public int Page {get; set;} = 1;
        public int PageSize {get; set;} = 1000;
    }
}