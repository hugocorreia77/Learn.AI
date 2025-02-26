using Learn.AI.Client.Abstractions;
using Learn.AI.Models.Questions.Result;
using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Threading;

namespace Learn.AI.Client
{
    public class LearnAIClient(ILogger<LearnAIClient> Logger, HttpClient AiClient) : ILearnAIClient
    {

        //private readonly HttpClient _httpClient = HttpClient;

        public async Task<BaseContentResponse<QuestionResult>?> GetNewQuestionAsync(string category, string language, CancellationToken cancellationToken)
        {

            var questionResult = await AiClient.GetAsync($"questions?category={category}&language={language}", cancellationToken);

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

    }
}
