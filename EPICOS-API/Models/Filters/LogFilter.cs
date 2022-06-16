namespace EPICOS_API.Models.Filters
{
    public class LogFilter
    {
        public string MAC { get; set; }
        public string IPaddress { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;

    }
}