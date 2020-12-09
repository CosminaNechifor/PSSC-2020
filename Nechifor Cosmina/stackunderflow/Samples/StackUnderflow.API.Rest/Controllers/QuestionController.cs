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
using LanguageExt;
using GrainInterfaces;
using Orleans;
using Microsoft.AspNetCore.Http;

namespace StackUnderflow.API.AspNetCore.Controllers
{
    [ApiController]
    [Route("question")]
    public class QuestionController : ControllerBase
    {
        private readonly IInterpreterAsync _interpreter;
        private readonly StackUnderflowContext _dbContext;
        private readonly IClusterClient _client;

        public QuestionController(IInterpreterAsync interpreter, StackUnderflowContext dbContext, IClusterClient client)
        {
            _interpreter = interpreter;
            _dbContext = dbContext;
            _client = client;
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
                       let user = createQuestionResult.SafeCast<CreateQuestionResult.QuestionCreated>().Select(p => p.Author)
                       let confirmationQuestionCmd = new ConfirmationQuestionCmd(user)
                       from ConfirmationQuestionResult in QuestionDomain.ConfirmQuestion(confirmationQuestionCmd)
                       select new { createQuestionResult, ConfirmationQuestionResult };
            var r = await _interpreter.Interpret(expr, ctx, dependencies);
            _dbContext.SaveChanges();
            return r.createQuestionResult.Match(
                created => (IActionResult)Ok(created.Question.PostId),
                notCreated => StatusCode(StatusCodes.Status500InternalServerError, "Question could not be created."),//todo return 500 (),
            invalidRequest => BadRequest("Invalid request."));

        }
        private TryAsync<ConfirmationAcknowledgement> SendEmail(ConfirmationLetter letter)
       => async () =>
       {
           var emialSender = _client.GetGrain<IEmailSender>(0);
           await emialSender.SendEmailAsync(letter.Letter);
           return new ConfirmationAcknowledgement(Guid.NewGuid().ToString());
       };
    }
}
