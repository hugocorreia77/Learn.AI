using Learn.AI.Models.Questions.Result;
using Learn.AI.Services.Providers.Interfaces;
using Learn.Core.Shared.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace Learn.AI.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController(IAIProvider provider) : Controller
    {

        [HttpGet("")]
        public async Task<BaseContentResponse<QuestionResult>> GetQuestion(string category, string language, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(language))
            {
                language = "pt-PT";
            }

            var question = await provider.GenerateQuestion(category, 
                language : language,
                cancellationToken: cancellationToken);

            return question;
        }
    }
}
