using CommandLine;

namespace Conduit.Infrastructure.CLParser.Comments
{
    [Verb("create-comment")]
    public class CreateCommentOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option("body", Required = true)]
        public string Body { get; set; }

        [Option("slug", Required=true)]
        public string Slug
        {
            get; set;
        }
    }

    [Verb("list-comments")]
    public class ListCommentsOption
    {
        [Option("slug", Required = true)]
        public string Slug { get; set; }
    }

    [Verb("delete-comment")]
    public class DeleteCommentOption
    {
        [Option("slug", Required = true)]
        public string Slug { get; set; }

        [Option("id", Required = true)]
        public int Id { get; set; }
    }

}
