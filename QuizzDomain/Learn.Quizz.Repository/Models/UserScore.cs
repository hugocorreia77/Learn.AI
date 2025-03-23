using Learn.Users.Models;

namespace Learn.Quizz.Repository.Models
{
    public class UserScore
    {
        public required UserReference User { get; set; }
        public int Score { get; set; } = 0;
    }
}
