using Learn.AI.Models.Questions.Result;
using Learn.AI.Services.Providers.Interfaces;
using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn.AI.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController(IAIProvider provider, IUserContextService userContextService) : Controller
    {

        [HttpGet("")]
        public async Task<BaseContentResponse<QuestionResult>> GetQuestion([FromQuery] string category, CancellationToken cancellationToken)
        {
            var question = await provider.GenerateQuestion(category, 
                language : userContextService.GetSelectedLanguage(),
                cancellationToken: cancellationToken);

            return question;
        }

        [HttpGet("multiple")]
        public async Task<BaseContentResponse<List<QuestionResult>>> GetQuestions([FromQuery] int numberOfQuestions, [FromQuery] List<string> categories, CancellationToken cancellationToken)
        {
            var questionsResult = await provider.GenerateQuestions(
                numberOfQuestions: numberOfQuestions, 
                categories: categories, 
                difficulty: Models.Questions.QuestionDifficulty.Medium,
                language: userContextService.GetSelectedLanguage(),
                cancellationToken: cancellationToken);

            return questionsResult;
        }
    }
}
