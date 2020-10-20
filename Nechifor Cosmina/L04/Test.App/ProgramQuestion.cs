 using Profile.Domain.CreateQuestionWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Profile.Domain.CreateQuestionWorkflow.CreateQuestionResult;

namespace Test.App
{
    class ProgramQuestion
    {
        static void Main(string[] args)
        {
            var tag = "c#,java".Split(',').ToList();
            var cmd = new CreateQuestionCmd("Can I have blocking remote call in Flux.generate state generator", "I am generating a Flux from a series of blocking REST API calls, each call depends on the result of previous call.", tag);
            var result = CreateQuestion(cmd);

            result.Match(
                    ProcessQuestionPosted,
                    ProcessQuestionNotPosted,
                    ProcessInvalidQuestion
                );

            Console.ReadLine();
        }

        private static ICreateQuestionResult ProcessInvalidQuestion(QuestionValidationFailed validationErrors)
        {
            Console.WriteLine("Question validation failed: ");
            foreach (var error in validationErrors.ValidationErrors)
            {
                Console.WriteLine(error);
            }
            return validationErrors;
        }
           

        private static ICreateQuestionResult ProcessQuestionNotPosted(QuestionNotPosted questionNotPostedResult)
        {
            Console.WriteLine($"Question not posted: {questionNotPostedResult.Reason}");
            return questionNotPostedResult;
        }

        private static ICreateQuestionResult ProcessQuestionPosted(QuestionPosted question)
        {
            Console.WriteLine($"Question: {question.Title}");
            return question;
        }

        public static ICreateQuestionResult CreateQuestion(CreateQuestionCmd createQuestionCommand)
        {
            if (string.IsNullOrEmpty(createQuestionCommand.Title))
            {
                var errors = new List<string>() { "Invalid question title" };
                return new QuestionValidationFailed(errors);
            }

            if (string.IsNullOrEmpty(createQuestionCommand.Body))
            {
                var errors = new List<string>() { "Invalid question body" };
                return new QuestionValidationFailed(errors);
            }

            if (createQuestionCommand.Tags.Count==0)
            {
                var errors = new List<string>() { "Invalid question tags" };
                return new QuestionValidationFailed(errors);
            }


            //if (new Random().Next(10) > 1)
            //{
            //    return new QuestionNotPosted("Title could not be verified");
            //}

            var result = new QuestionPosted(createQuestionCommand.Title);
            return result;
        }
    }
}
