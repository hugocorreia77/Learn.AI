namespace Learn.Quizz.Models.Hub
{
    public static class HubMethods
    {
        public static class ServerToClient
        {
            public static readonly string QuestionSent = "QuestionSent";
            public static readonly string PlayerJoined = "PlayerJoined";
            public static readonly string PlayerUnjoined = "PlayerUnjoined";
            public static readonly string QuestionSolutionSent = "QuestionSolutionSent";
            public static readonly string GameStarting = "GameStarting";
            public static readonly string GameEnded = "GameEnded";
        }

        public static class ClientToServer
        {
        }


    }
}
