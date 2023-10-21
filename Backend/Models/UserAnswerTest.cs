namespace Backend.Models
{
    public class UserAnswerTest
    {
        public int id { get; set; }
        public int TestModulId { get; set; }
        public TestModule TestModule { get; set; }
        public List<UserAnswerTask> answers { get; set; }
        public int AccountUserId { get; set; }
        public AccountUser user { get; set; }

        public int testScore { get; set; }

        private void SetScoreTest()
        {
            testScore = 0;
            //for(int i = 0; i < TestModule.Questions.Count; i++) 
            //{
            //    if (TestModule.Questions[i].IsTest)
            //    {
            //        if (TestModule.Questions[i].correctAnswer == answers[i].userAnswer)
            //        {
            //            testScore += TestModule.Questions[i].costPoint;
            //        }
            //    }
            //}

            //Код Армана по проверки кода
        }
    }
}
