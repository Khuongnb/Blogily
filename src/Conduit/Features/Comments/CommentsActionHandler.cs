using System;
using Conduit.Features.Articles;
using Conduit.Infrastructure.Errors;
using Create = Conduit.Features.Comments.Create;
using Delete = Conduit.Features.Comments.Delete;
using List = Conduit.Features.Comments.List;

namespace Conduit.Infrastructure.CLParser.Comments
{
    class CommentsActionHandler: ICommentsActionHandler
    {
        private readonly IActionHelper _helper;

        public CommentsActionHandler(IActionHelper helper)
        {
            _helper = helper;
        }

        public int CreateComment(CreateCommentOption opt)
        {
            try
            {
                _helper.Username = opt.Username;
                var command = new Create.Command()
                {
                    Comment = new Create.CommentData()
                    {
                        Body = opt.Body
                    }
                    ,
                    Slug = opt.Slug
                };

                var res = _helper.SendAsync(command);
                _helper.PrintResult(res,"Comment created.!");
            }
            catch (UserNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return 1;
        }

        public int ListComments(ListCommentsOption opt)
        {
            var query = new List.Query(opt.Slug);
            var res = _helper.SendAsync(query);
            _helper.PrintResult(res, successed: "Found " + res.Result.Comments.Count + " comments.");
            return 1;
        }

        public int DeleteComment(DeleteCommentOption opt)
        {
            var command = new Delete.Command(opt.Slug, opt.Id);
            var res = _helper.SendAsync(command);
            _helper.PrintResult(res, successed: "Comment " + opt.Id + " deleted.");
            return 1;
        }
    }
}
