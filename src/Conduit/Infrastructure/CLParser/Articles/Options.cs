using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Conduit.Infrastructure.CLParser.Articles
{
    [Verb("list-articles")]
    class ListArticlesOption { }

    [Verb("create-article")]
    class CreateArticleOption
    {
        [Option("title")]
        public string Title { get; set; }

        [Option("description")]
        public string Description { get; set; }

        [Option("body")]
        public string Body { get; set; }

        [Option("tag-lists")]
        public IEnumerable<string> TagLists { get; set; }

        [Option("username")]
        public string Username { get; set; }

    }

}