using Backend.DataBaseController;

namespace Backend.Models
{
    public class UserAnswerTest
    {
        public int id { get; set; }
        public int? TestModuleId { get; set; }
        public int? QuestionId { get; set; }
        public int? AccountUserId { get; set; }
        public string? answer { get; set; }

        public static int GetScoreTest(ApplicationContext db, int module, int user)
        {
            int testScore = 0;
            var module_questions = db.Questions.Where(q => q.TestModuleId == module).ToList();
            for (int i = 0; i < module_questions.Count(); i++)
            {
                if (module_questions[i].IsTest.Value)
                {
                    var answer = db.TestsUsers.FirstOrDefault(a => a.QuestionId == module_questions[i].id && a.AccountUserId == user);
                    var correct_answer = module_questions[i].correctAnswer;
                    if (correct_answer == answer.answer)
                    {
                        testScore += module_questions[i].costPoint.Value;
                    }
                }
            }

            //Код Армана по проверки кода
            return testScore;
        }
    }
}
