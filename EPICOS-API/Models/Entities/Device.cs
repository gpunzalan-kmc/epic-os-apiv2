using System.ComponentModel.DataAnnotations;

namespace EPICOS_API.Models.Entities
{
    public class Device
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Device name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "MAC Address is required.")]
        public string MAC { get; set; }
        [Required(ErrorMessage = "IP Address is required.")]
        public string IPaddress { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}