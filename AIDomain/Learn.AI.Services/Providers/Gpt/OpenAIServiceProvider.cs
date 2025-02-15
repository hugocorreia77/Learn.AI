using Learn.AI.Models;
using Learn.AI.Models.Questions;
using Learn.AI.Models.Questions.Result;
using Learn.AI.Services.Providers.Gpt.Configuration;
using Learn.AI.Services.Providers.Interfaces;
using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;

namespace Learn.AI.Services.Providers.Gpt
{
    public sealed class OpenAIServiceProvider(ILogger<OpenAIServiceProvider> _logger,  IOptionsSnapshot<OpenAIConfiguration> _configuration)
        : IAIProvider
    {

        public async Task<BaseContentResponse<QuestionResult>> GenerateQuestion(string category, QuestionDifficulty difficulty = QuestionDifficulty.Medium, string language = "pt-PT", CancellationToken cancellationToken = default)
        {
            try
            {
                OpenAIClient client = new(_configuration.Value.OpenAiKey);

                string userPrompt = $"Faz uma pergunta sobre {category} com dificuldade {difficulty} e devolve 4 opções de escolha onde apenas uma é correta. Responde em {language}";
                var messages = new List<Message>
                {   
                    new(Role.System, "És um professor"),
                    new(Role.User, userPrompt)
                };

                _logger.LogInformation("Asking: {Prompt} ", userPrompt);

                var request = new ChatRequest(messages
                                                , model: _configuration.Value.Model
                                                , responseFormat: ChatResponseFormat.Json
                                                , jsonSchema: typeof(QuestionResult)
                                                 );
                var response = await client.ChatEndpoint.GetCompletionAsync(request, cancellationToken);

                _logger.LogInformation("OpenAI: {AIResponse}", response.FirstChoice.Message);

                var model = JsonConvert.DeserializeObject<QuestionResult>(response.FirstChoice.Message);

                return new BaseContentResponse<QuestionResult>
                {
                    Data = model
                }.SetSucceeded();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while requesting AI.");
                return new BaseContentResponse<QuestionResult>()
                    .SetFailed()
                    .AddError(ex.Message);
            }
        }
    }
}
