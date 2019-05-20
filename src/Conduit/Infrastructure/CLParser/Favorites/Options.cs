using CommandLine;

namespace Conduit.Infrastructure.CLParser.Favorites
{
    [Verb("add-favorite", HelpText = "Like an article")]
    public class AddFavoriteOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option("slug", Required = true)]
        public string Slug { get; set; }
    }

    [Verb("delete-favorite", HelpText = "Unlike an article")]
    public class DeleteFavoriteOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option("slug", Required = true)]
        public string Slug { get; set; }
    }
}
