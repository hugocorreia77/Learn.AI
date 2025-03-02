namespace Learn.AI.Services.Prompts
{
    public static class QuestionPrompts
    {
        public const string IndividualQuestion = "Faz uma pergunta sobre {0} com dificuldade {1} e devolve 4 opções de escolha onde apenas uma é correta. Responde em {2}";
        public const string MultipleQuestions = "De forma aleatória, faz um total de {0} perguntas sobre os seguintes temas {1} " +
            " com dificuldade {2} e devolve para cada pergunta 4 opções de escolha onde apenas uma é correta. Deves usar o idioma {3}";
    }
}
