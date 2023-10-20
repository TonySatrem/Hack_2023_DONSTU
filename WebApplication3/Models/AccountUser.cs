

namespace WebApplication3.Models
{
    public class AccountUser
    {
        public AccountUser(string password) 
        {
            hashPassword = Helper.SHA512(password);
        }
        public int Id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public DateTime dataBirthday { get; set; }
        public string login { get; set; }
        public string hashPassword { get; set; }
        public bool isAdmin { get; set; }

        public string? description { get; set; }
        public string? contactUser { get; set; }

        public List<RequestUser> RequestUser { get; set; }
    }
}
