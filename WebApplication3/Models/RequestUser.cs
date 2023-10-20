namespace WebApplication3.Models
{
    public class RequestUser
    {
        public RequestUser() { }
        public int Id { get; set; }
        public string roleRequest { get; set; }
        public string skillPerson { get; set; }

        public int AccountUserId { get; set; }
        public AccountUser AccountUser { get; set; }
    }
}
