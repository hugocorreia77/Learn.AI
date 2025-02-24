using Learn.Core.Shared.Models.Response;

namespace Learn.Security.Services.Abstractions.Interfaces
{
    public interface ISecurityService
    {
        Task<BaseContentResponse<string>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken); 
    }
}
