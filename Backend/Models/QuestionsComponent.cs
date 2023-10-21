using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class QuestionsComponent
    {
        public QuestionsComponent() { }
        public int id { get; set; }
        public string taskToUser { get; set; }
        public string? answerOptions { get; set; }
        public string? correctAnswer { get; set; }
        public bool? IsTest { get; set; }
        public int? costPoint { get; set; }
        public int? TestModuleId { get; set; }

        public void Update(QuestionsComponent component)
        {
            if (component.taskToUser != null)
                taskToUser = component.taskToUser;
            if (component.answerOptions != null)
                answerOptions = component.answerOptions;
            if (component.correctAnswer != null)
                correctAnswer = component.correctAnswer;
            if (component.IsTest != null)
                IsTest = component.IsTest;
            if (component.costPoint != null)
                costPoint = component.costPoint;
        }
    }
}
