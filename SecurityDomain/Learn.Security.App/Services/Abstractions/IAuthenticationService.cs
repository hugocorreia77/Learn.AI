using Learn.Core.Shared.Models.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Learn.Security.App.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<BaseResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}
