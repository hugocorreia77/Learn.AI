using Learn.Core.Shared.Models.Response;
using Learn.Security.Client.Abstractions;
using Learn.Security.Models.Input;
using Microsoft.Extensions.Logging;
using Learn.Core.Shared.Extensions;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Learn.Security.Client
{
    public class LearnSecurityClient(ILogger<LearnSecurityClient> Logger, HttpClient SecurityClient) : ILearnSecurityClient
    {
        public async Task<BaseContentResponse<string>> AuthenticateAsync(AuthenticationInput input, CancellationToken cancellationToken)
        {
            JsonContent content = JsonContent.Create(input);

            var authenticationResult = await SecurityClient.PostAsync($"security/authenticate",
                content,
                cancellationToken);

            var questionAsString = await authenticationResult.Content.ReadAsStringAsync(cancellationToken);

            if (!authenticationResult.IsSuccessStatusCode)
            {
                Logger.LogError("Erro ao obter a resposta da API: {ApiResponse}", authenticationResult);
                return new BaseContentResponse<string>()
                                    .SetFailed()
                                    .AddError("Não foi possível obter uma resposta da API de Security.");
            }

            try
            {
                return JsonConvert.DeserializeObject<BaseContentResponse<string>>(questionAsString);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao fazer parse da resposta.");
                return new BaseContentResponse<string>()
                    .SetFailed()
                    .AddError(ex.Message);
            }
        }
    }
}
