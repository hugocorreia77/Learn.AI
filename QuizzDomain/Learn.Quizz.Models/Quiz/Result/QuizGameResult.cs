using Learn.Core.Shared.Models.User;
using Learn.Quizz.Models.Enums;

namespace Learn.Quizz.Models.Quiz.Result
{
    public class QuizGameResult
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required int NumberOfQuestions { get; set; }
        public required int CurrentQuestion { get; set; }
        public required GameStatus Status { get; set; }
        public required UserReference Owner { get; set; }
    }
}
