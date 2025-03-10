using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Quizz.Models.Quiz.Input;
using Learn.Quizz.Models.Quiz.Result;
using Learn.Quizz.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Learn.Quizz.Api.Controllers
{
    [Authorize]
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
        public async Task<BaseContentResponse<QuizGameResult>> Create(CreateQuizInput input, CancellationToken cancellationToken)
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

        [HttpGet("{quizId}")]
        public async Task<BaseContentResponse<QuizGameResult>> GetGame([FromQuery] Guid quizId, CancellationToken cancellationToken)
        {
            try
            {
                return await _quizService.GetGameAsync(quizId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown getting the quiz.");
                return new BaseContentResponse<QuizGameResult>()
                    .SetFailed()
                    .AddError("It was not possible to get the quiz.");
            }
        }

        [HttpPost("answer")]
        public async Task<BaseContentResponse<bool>> SetAnswer([FromBody] AnswerInput answer, CancellationToken cancellationToken)
        {
            try
            {

                await _quizService.SetAttemptAsync(answer.QuizId, answer.QuestionId, answer.AttemptId, CancellationToken.None);
                return new BaseContentResponse<bool>(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown getting the quiz.");
                return new BaseContentResponse<bool>()
                    .SetFailed()
                    .AddError("It was not possible to get the quiz.");
            }
        }


    }
}
