using Learn.Core.Shared.Models.Response;
using Learn.Security.Models.Input;

namespace Learn.Security.Client.Abstractions
{
    public interface ILearnSecurityClient
    {
        Task<BaseContentResponse<string>> AuthenticateAsync(AuthenticationInput input, CancellationToken cancellationToken);
    }
}
