using CommandLine;

namespace Conduit.Infrastructure.CLParser.Comments
{
    [Verb("create-comment", HelpText = "Add new comment")]
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

    [Verb("list-comments", HelpText = "List all comment of slug")]
    public class ListCommentsOption
    {
        [Option("slug", Required = true)]
        public string Slug { get; set; }
    }

    [Verb("delete-comment", HelpText = "Delete a comment")]
    public class DeleteCommentOption
    {
        [Option("slug", Required = true)]
        public string Slug { get; set; }

        [Option("id", Required = true)]
        public int Id { get; set; }
    }

}
