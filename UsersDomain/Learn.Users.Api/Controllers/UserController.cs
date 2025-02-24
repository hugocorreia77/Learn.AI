using Learn.Core.Shared.Models.Response;
using Learn.Users.Models.Input;
using Learn.Users.Repository.Models;
using Learn.Users.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Learn.Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : Controller
    {
        [HttpPost("create")]
        public Task<BaseContentResponse<User>> CreateUser([FromBody] CreateUserInput createUser, CancellationToken cancellationToken)
        {
            return userService.CreateUserAsync(createUser, cancellationToken);
        }
    }
}
