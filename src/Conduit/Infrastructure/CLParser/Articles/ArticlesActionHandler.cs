using System;
using System.Linq;
using Conduit.Features.Articles;
using MediatR;
using Newtonsoft.Json;
using AutoMapper;

namespace Conduit.Infrastructure.CLParser.Articles
{
    class ArticlesActionHandler
    {
        private readonly ActionHelper _helper;

        public ArticlesActionHandler(ActionHelper helper)
        {
            _helper = helper;
        }

        public int ListArticles(ListArticlesOption article)
        {
            var res = _helper.SendAsync(new List.Query("", "", "", null, null));

            _helper.PrintResult(res);

            return 0;
        }

        public int CreateArticle(CreateArticleOption article)
        {
            _helper.Username = article.Username;
            var command = new Create.Command()
            {
                Article = new Create.ArticleData()
                {
                    Title = article.Title,
                    Description = article.Description,
                    Body = article.Body,
                    TagList = article.TagLists.ToArray()
                }
            };

            var res = _helper.SendAsync(command);

            _helper.PrintResult(res);
            return 0;
        }

    }

}
