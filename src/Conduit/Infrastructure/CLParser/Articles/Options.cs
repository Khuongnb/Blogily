using System.Collections.Generic;
using CommandLine;
namespace Conduit.Infrastructure.CLParser.Articles
{
    [Verb("list-articles")]
    public class ListArticlesOption { }

    [Verb("create-article")]
    public class CreateArticleOption
    {
        [Option("title", Required = true)]
        public string Title { get; set; }

        [Option("description")]
        public string Description { get; set; }

        [Option("body")]
        public string Body { get; set; }

        [Option("tag-list")]
        public IEnumerable<string> Tags { get; set; }

        [Option("username", Required = true)]
        public string Username { get; set; }

    }

    [Verb("get-articles")]
    public class GetArticlesOption
    {
        [Option("tag")]
        public string Tag { get; set; }

        [Option("author")]
        public string Author { get; set; }

        [Option("favorite-user")]
        public string FavoriteUser { get; set; }

        [Option("limit", Default = null)]
        public int? Limit { get; set; }

        [Option("offset", Default = null)]
        public int? Offset { get; set; }

        [Option("is-feed", Default = false)]
        public bool IsFeed { get; set; }
    }

    [Verb("get-article")]
    public class GetArticleOption
    {
        [Option("slug")]
        public string Slug { get; set; }
    }


    [Verb("edit-article")]
    public class EditArticleOption
    {
        [Option("slug")]
        public string Slug { get; set; }

        [Option("title")]
        public string Title { get; set; }

        [Option('d', "description")]
        public string Description { get; set; }

        [Option("body")]
        public string Body { get; set; }

        [Option("tag-list")]
        public IEnumerable<string> Tags { get; set; }

    }

    [Verb("delete-article")]
    public class DeleteArticleOption
    {
        [Option("slug")]
        public string Slug { get; set; }
    }
}