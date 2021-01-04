using Orleans;
using Orleans.Streams;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementation
{
    class QuestionGrain: Grain
    {
        private  StackUnderflowContext _dbContext;
        private QuestionGrain state;

        public QuestionGrain(StackUnderflowContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnActivateAsync()
        {
            var key = this.GetPrimaryKey();
            Post post = new Post();

            var expPostId = from postId in post.PostId.ToString()
                      where postId.Equals(key.ToString())
                      select postId;

            var expParentPostId = from parentPostId in post.ParentPostId.ToString()
                            where parentPostId.Equals(key.ToString())
                            select parentPostId;


            // subscribe to replys stream
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<string>(Guid.Empty, "LETTER");
            await stream.SubscribeAsync((IAsyncObserver<string>)this);
            
           // return base.OnActivateAsync();
        }

        //public Question GetQuestionWithReplys()
        //{

        //}
    }
}
