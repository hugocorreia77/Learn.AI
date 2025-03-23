using Learn.Quizz.Models.Enums;
using System.Collections.Generic;

namespace Learn.Quizz.Models.Quiz.Input
{
    public class CreateQuizInput
    {
        public required string Name { get; set; }
        public required QuizzType QuizType { get; set; }
        public required List<string> Categories { get; set; }
        public int NumberOfQuestions { get; set; } = 5;
    }
}
