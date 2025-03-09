using Learn.AI.Client.Abstractions;
using Learn.AI.Models.Questions.Result;
using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Learn.AI.Client
{
    public class LearnAIClient(ILogger<LearnAIClient> Logger, HttpClient AiClient) : ILearnAIClient
    {

        public async Task<BaseContentResponse<QuestionResult>?> GetQuestionAsync(string category, CancellationToken cancellationToken)
        {
            var questionResult = await AiClient.GetAsync($"questions?category={category}", cancellationToken);

            var questionAsString = await questionResult.Content.ReadAsStringAsync(cancellationToken);
            Logger.LogDebug("Question received: {Question}", questionAsString);

            if (!questionResult.IsSuccessStatusCode)
            {
                Logger.LogError("Erro ao obter a resposta da API: {ApiResponse}", questionResult);
                return new BaseContentResponse<QuestionResult>()
                                    .SetFailed()
                                    .AddError("Não foi possível obter uma resposta da API de AI.");
            }

            try
            {
                var question = JsonConvert.DeserializeObject<BaseContentResponse<QuestionResult>>(questionAsString);
                return question;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao fazer parse da resposta.");
                return new BaseContentResponse<QuestionResult>()
                    .SetFailed()
                    .AddError(ex.Message);
            }
        }

        public async Task<BaseContentResponse<List<QuestionResult>>> GetQuestionsAsync(int numberOfQuestions, List<string> categories, CancellationToken cancellationToken)
        {
            var categoriesFilter = $"&categories={string.Join("&categories=", categories)}";
            var questionResult = await AiClient.GetAsync($"questions/multiple?numberOfQuestions={numberOfQuestions}{categoriesFilter}", cancellationToken);

            var questionAsString = await questionResult.Content.ReadAsStringAsync(cancellationToken);
            Logger.LogDebug("Questions received: {Questions}", questionAsString);

            if (!questionResult.IsSuccessStatusCode)
            {
                Logger.LogError("Erro ao obter a resposta da API: {ApiResponse}", questionResult);
                return new BaseContentResponse<List<QuestionResult>>()
                                    .SetFailed()
                                    .AddError("Não foi possível obter uma resposta da API de AI.");
            }

            try
            {
                var question = JsonConvert.DeserializeObject<BaseContentResponse<List<QuestionResult>>>(questionAsString);
                return question;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao fazer parse da resposta.");
                return new BaseContentResponse<List<QuestionResult>>()
                    .SetFailed()
                    .AddError(ex.Message);
            }
        }
    }
}
