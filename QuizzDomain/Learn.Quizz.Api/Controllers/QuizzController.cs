using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Quizz.Models.Quiz.Input;
using Learn.Quizz.Models.Quiz.Result;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Learn.Quizz.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuizzController : ControllerBase
    {
        private readonly ILogger<QuizzController> _logger;
        private readonly IQuizService _quizService;

        public QuizzController(ILogger<QuizzController> logger, IQuizService quizService)
        {
            _logger = logger;
            _quizService = quizService;
        }

        [HttpPost("Create")]
        public async Task<BaseContentResponse<QuizGameResult>> CreateGameAsync(CreateQuizInput input, CancellationToken cancellationToken)
        {
            try
            {
                return await _quizService.CreateGameAsync(input, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception thrown creating a new quiz.");
                return new BaseContentResponse<QuizGameResult>()
                .SetFailed()
                .AddError("It was not possible to create a new quiz.");
            }
        }

        [HttpPost("Join")]
        public int Join()
        {
            return 1;
        }

    }
}
