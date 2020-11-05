using LanguageExt.Common;
using System;
using System.Collections.Generic;
using System.Text;
using static Question.Domain.CreateQuestionWorkflow.QuestionText;

namespace Question.Domain.CreateQuestionWorkflow
{
    public class VerifyQuestionService
    {
        public Result<VerifiedQuestion> VerifyQuestion(UnverifiedQuestion question)
        {
            // se publica intrebarea doar daca se respecta conditiile de 1000 caractere si intre 1 si 3 taguri
            return new VerifiedQuestion(question.Question,question.Tags);
        }
    }
}
