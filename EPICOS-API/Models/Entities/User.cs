using System.ComponentModel.DataAnnotations;

namespace EPICOS_API.Models.Entities
{
    public class User
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Firstname is required!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Lastname is required!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Username is required!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required!")]

        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Role is required!")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}