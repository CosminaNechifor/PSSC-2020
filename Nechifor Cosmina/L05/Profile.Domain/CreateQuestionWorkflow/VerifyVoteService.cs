using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Question.Domain.CreateQuestionWorkflow.QuestionText;

namespace Question.Domain.CreateQuestionWorkflow
{
    public class VerifyVoteService
    {
        public Task VerifyIfQuestionIsPosted(VerifiedQuestion question)
        {
            //aici lipseste logica prin care se verifica ca intrebarea e postata.Daca nu e postata nu poti vota
            return Task.CompletedTask;
        }
     }
}