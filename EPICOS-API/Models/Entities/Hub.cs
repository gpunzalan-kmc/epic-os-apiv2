namespace EPICOS_API.Models.Entities
{
    public class Hub : Device
    {

        public Hub(){

        }
        public int DeviceType { get; set; }
        public int OfficeID { get; set; }
        public int FloorID { get; set; }
        

        // [ForeignKey(nameof(ID))]
        // public virtual Office Office { get; set; }

        // [ForeignKey(nameof(ID))]
        public Floor Floor { get; set; }
    }
}