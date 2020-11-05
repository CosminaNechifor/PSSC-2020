using LanguageExt;
using Question.Domain.CreateQuestionWorkflow;
using Question.Domain.PostAnswerWorkflow.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication.ExtendedProtection;

namespace Test.App
{
    class ProgramQuestion
    {
        /* static void Main(string[] args)
         {
             var tagsResult = "C#,Java,c".Split(",").ToList();
             var questionResult = UnverifiedQuestion.Create("Can I have blocking remote call in Flux.generate state generator",tagsResult);

             questionResult.Match(
                 Succ: question =>
                 {
                     ReviewQuestionText(question);
                     VoteQuestion(question);
                     Console.WriteLine("Question is valid.");
                     return Unit.Default;
                 },
                 Fail: ex =>
                 {
                     Console.WriteLine($"Invalid question. Reason: {ex.Message}");
                     return Unit.Default;
                 }
              );

             Console.ReadLine();
         }    

         private static void ReviewQuestionText(UnverifiedQuestion question)
         {
             var verifiedQuestionResult = new VerifyQuestionService().VerifyQuestion(question);
             verifiedQuestionResult.Match(
                 verifiedQuestion =>
                 {
                     new ReviewQuestionService().RewriteQuestionText(verifiedQuestion).Wait();
                     return Unit.Default;
                 },
                 ex =>
                 {
                     Console.WriteLine("Question could not be verified");
                     return Unit.Default;
                 }
                 );
         }
         private static void VoteQuestion(UnverifiedQuestion question)
         {
             var voteQuestionResult = new VerifyQuestionService().VerifyQuestion(question);
             voteQuestionResult.Match(
                 voteQuestion =>
                 {
                     new VerifyVoteService().VerifyIfQuestionIsPosted(voteQuestion).Wait();
                     return Unit.Default;
                 },
                 ex =>
                 {
                     Console.WriteLine("You can't vote");
                     return Unit.Default;
                 }
                 );
         } */

        static void Main(string[] args)
        {

            //var order = new ValidateReplyResult.UnvalidateReply(100, new LanguageCheckResult.UnverifiedText("test"), 1);
            ////var result=.... aici urmeaza partea de orderOperation dar m-am incurcat si nu mi-a iesit. este pusa ca si comentariu incercarea
            // result.Match(
            //     validOrder =>
            //     {
            //        Console.WriteLine("Order is calid");
            //     },
            //        error =>
            //        {
            //            Console.WriteLine($"Invalid order.");
            //        }
            //    );

            var expr = from createReplyResult in BoundedContext.ValidateReplyText(10, "test", 1)
                       let validReply = (ValidateReplyResult.ValidateReplyText)createReplyResult
                       from checkLanguageResult in BoundedContext.LanguageCheck(validReply.AnswerText)
                       from ownerAck in BoundedContext.SendAckToQuestionOwner(checkLanguageResult.ToString(), 10, 1)
                       from authorAck in BoundedContext.SendAckToReplyAuthor(checkLanguageResult.ToString(), 10, 2)
                       select (validReply, checkLanguageResult, ownerAck, authorAck);


        }
    }
}
