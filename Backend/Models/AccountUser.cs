using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class AccountUser
    {
        public AccountUser FromRegister(RegisterUser user)
        {
            name = user.name;
            surname = user.surname;
            dataBirthday = user.birthDate;
            phonenumber = user.phonenumber;
            hashPassword = Helper.SHA512(user.password);
            isAdmin = user.admin;
            return this;
        }

        public AccountUser Update(AccountUser user)
        {
            if (user.name != null)
                name = user.name;
            if (user.surname != null)
                surname = user.surname;
            if (user.dataBirthday != null)
                dataBirthday = user.dataBirthday;
            if (user.phonenumber != null)
                phonenumber = user.phonenumber;
            if (user.isAdmin != null)
                isAdmin = user.isAdmin;
            return this;
        }

        public int Id { get; set; }
        public string? name { get; set; }
        public string? surname { get; set; }
        public DateTime? dataBirthday { get; set; }
        public string? phonenumber { get; set; }
        [JsonIgnore]
        public string? hashPassword { get; set; }
        public bool? isAdmin { get; set; }

        [JsonIgnore]
        public string? token { get; set; }
        public string? description { get; set; }
        public string? contactUser { get; set; }

        public List<RequestUser> RequestUser { get; set; }
        public List<UserAnswerTest> completeTest { get; set; }
    }
}
