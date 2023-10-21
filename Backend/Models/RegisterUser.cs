namespace Backend.Models
{
    public class RegisterUser
    {
        public string name { get; set; }
        public string surname { get; set; }
        public DateTime dataBirthday { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public bool admin { get; set; }
    }
}
