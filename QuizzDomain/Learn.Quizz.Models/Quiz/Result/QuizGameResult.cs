using Learn.Quizz.Models.Enums;
using Learn.Users.Models;
using System;

namespace Learn.Quizz.Models.Quiz.Result
{
    public class QuizGameResult
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required int NumberOfQuestions { get; set; }
        public required int CurrentQuestion { get; set; }
        public required GameStatus Status { get; set; }
        public required UserReference Owner { get; set; }
    }
}
