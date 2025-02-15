using Learn.AI.Client.Abstractions;
using Learn.AI.Models.Questions.Result;
using Learn.Core.Shared.Models.Response;
using System.Net.Http.Json;
using System.Threading;

namespace Learn.AI.Client
{
    public class LearnAIClient(HttpClient httpClient) : ILearnAIClient
    {

        private readonly HttpClient _httpClient = httpClient;

        public async Task<BaseContentResponse<QuestionResult>?> GetNewQuestionAsync(string category, string language, CancellationToken cancellationToken)
        {
            var questionResult = await _httpClient.GetFromJsonAsync<BaseContentResponse<QuestionResult>>($"questions?category={category}&language={language}", cancellationToken);

            //if (!questionResult.IsSuccessStatusCode)
            //{
            //    return null; // Tratar erro de forma mais detalhada se necessário
            //}

            return questionResult;
        }

    }
}
