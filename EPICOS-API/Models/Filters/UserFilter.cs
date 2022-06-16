namespace EPICOS_API.Models.Filters
{
    public class UserFilter
    {
        public string ID { get; set; }
        // public string FirstName { get; set; }
        // public string LastName { get; set; }
        // public string UserName { get; set; }
        // public int CompanyID { get; set; }
        // public string Phone { get; set; }
        // public string EmailAddress { get; set; }
        // public int RoleID { get; set; }
        // public bool IsActive { get; set; }
        // public bool IsDeleted { get; set; }
        public int Limit {get; set;} = 20;
        public int Page {get; set;} = 1;
    }
}