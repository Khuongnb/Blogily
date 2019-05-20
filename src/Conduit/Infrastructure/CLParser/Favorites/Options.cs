using CommandLine;

namespace Conduit.Infrastructure.CLParser.Favorites
{
    [Verb("add-favorite")]
    public class AddFavoriteOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option("slug", Required = true)]
        public string Slug { get; set; }
    }

    [Verb("delete-favorite")]
    public class DeleteFavoriteOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option("slug", Required = true)]
        public string Slug { get; set; }
    }
}
