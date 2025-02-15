using Learn.Core.Shared.Models.Response;
using Learn.Learning.Models.Questions.Output;
using Learn.Learning.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Learn.Learning.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController(ILearningService learningService) : Controller
    {
        [HttpGet("")]
        public async Task<BaseContentResponse<CreateQuestionResult>> CreateQuestion(string category, string language, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(language))
            {
                language = "pt-PT";
            }

            var result = await learningService.CreateQuestion(new Models.Questions.Input.CreateQuestionInput { Category = category }
            , cancellationToken);

            return result;
        }
    }
}
