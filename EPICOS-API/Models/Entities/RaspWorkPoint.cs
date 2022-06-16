namespace EPICOS_API.Models.Entities
{
    public class RaspWorkPoint
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string MAC { get; set; }
        public string IPaddress { get; set; }
        public int Type { get; set; }

        public enum Types
        {
            Sensor = 1,
            Hub = 2,
            Laptop = 3,
            Desktop = 4,
            Phone = 5,
            Tablet = 6
        }
    }
}