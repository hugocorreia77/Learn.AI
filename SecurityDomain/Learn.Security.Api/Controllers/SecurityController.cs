using Learn.Core.Shared.Models.Response;
using Learn.Security.Models.Input;
using Learn.Security.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Learn.Security.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController(ISecurityService securityService) : Controller
    {

        [HttpPost("authenticate")]
        public async Task<BaseContentResponse<string>> AuthenticateAsync([FromBody] AuthenticationInput input, CancellationToken cancellationToken)
        {
            return await securityService.AuthenticateAsync(input.Username, input.Password, cancellationToken);
        }
    }
}
