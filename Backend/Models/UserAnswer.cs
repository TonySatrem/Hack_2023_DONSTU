namespace Backend.Models
{
    public class UserAnswerTask
    {
        public int Id { get; set; }
        public string? userAnswer { get; set; }
        public int? speedCompleteCodeInSecond { get; set; }
    }
}
