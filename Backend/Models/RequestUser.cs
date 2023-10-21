using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class RequestUser
    {
        public RequestUser() { }
        public int Id { get; set; }
        public string roleRequest { get; set; }
        public string skillPerson { get; set; }
        public string statusRequest { get; set; } = "waiting";

        public int AccountUserId { get; set; }
        [JsonIgnore]
        public AccountUser AccountUser { get; set; }
    }
}
