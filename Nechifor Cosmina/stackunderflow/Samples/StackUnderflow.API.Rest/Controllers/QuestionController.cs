using Access.Primitives.EFCore;
using Access.Primitives.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Access.Primitives.Extensions.ObjectExtensions;
using StackUnderflow.Domain.Core.Contexts.Question;
using StackUnderflow.Domain.Core.Contexts.Question.CreateQuestion;
using StackUnderflow.Domain.Core.Contexts.Question.SendConfirmation;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackUnderflow.API.AspNetCore.Controllers
{
    [ApiController]
    [Route("question")]
    public class QuestionController : ControllerBase
    {
        private readonly IInterpreterAsync _interpreter;
        private readonly StackUnderflowContext _dbContext;

        public QuestionController(IInterpreterAsync interpreter, StackUnderflowContext dbContext)
        {
            _interpreter = interpreter;
            _dbContext = dbContext;
        }
    }

    [HttpPost("post")]
    public async Task<IActionResult> CreateAndConfirmationQuestion([FromBody] CreateQuestionCmd createQuestionCmd)
    {
        QuestionWriteContext ctx = new QuestionWriteContext(
           new EFList<Post>(_dbContext.Post),
           new EFList<User>(_dbContext.User));

        var dependencies = new QuestionDependencies();
        dependencies.GenerateConfirmationToken = () => Guid.NewGuid().ToString();
        dependencies.SendConfirmationEmail = (ConfirmationLetter letter) => async () => new ConfirmationAcknowledgement(Guid.NewGuid().ToString());

        var expr = from createQuestionResult in QuestionDomain.CreateQuestion(createQuestionCmd)
                   let question = createQuestionResult.SafeCast<CreateQuestionResult.QuestionCreated>.Select(p => p.Question)
                   let confirmationQuestionCmd = new ConfirmationQuestionCmd(question)
                   from ConfirmationQuestionResult in QuestionDomain.ConfirmQuestion(confirmationQuestionCmd)
                   select new { createQuestionResult, ConfirmationQuestionResult };
        var r = await _interpreter.Interpret(expr, ctx, dependencies);
        _dbContext.SaveChanges();
        return r.createQuestionResult.Match(
            created => (IActionResult)Ok(created.Post. .TenantId),
            notCreated => StatusCode(StatusCodes.Status500InternalServerError, "Question could not be created."),//todo return 500 (),
        invalidRequest => BadRequest("Invalid request."));

    }
}
