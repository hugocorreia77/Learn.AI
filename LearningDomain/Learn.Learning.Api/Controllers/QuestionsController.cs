﻿using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Services.Abstractions;
using Learn.Learning.Models.Questions.Output;
using Learn.Learning.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn.Learning.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController(ILearningService learningService) : Controller
    {
        [Authorize]
        [HttpGet("")]
        public async Task<BaseContentResponse<CreateQuestionResult>> CreateQuestion(
            [FromQuery] string category,
            CancellationToken cancellationToken)
        {

            var result = await learningService.CreateQuestion(
                new Models.Questions.Input.CreateQuestionInput
                {
                    Category = category
                }
            , cancellationToken);

            return result;
        }
    }
}
