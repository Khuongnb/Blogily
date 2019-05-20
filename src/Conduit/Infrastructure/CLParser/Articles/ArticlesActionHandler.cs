using System.Linq;
using Conduit.Features.Articles;

namespace Conduit.Infrastructure.CLParser.Articles
{
    class ArticlesActionHandler : IArticlesActionHandler
    {
        private readonly IActionHelper _helper;

        public ArticlesActionHandler(IActionHelper helper)
        {
            _helper = helper;
        }

        public int ListArticles(ListArticlesOption article)
        {
            var query = new List.Query("", "", "", null, null);
            var res = _helper.SendAsync(query);

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
                    TagList = article.Tags.ToArray()
                }
            };

            var res = _helper.SendAsync(command);

            _helper.PrintResult(res, successed: article.Title + " article created.");
            return 0;
        }

        public int GetArticles(GetArticlesOption opt)
        {
            var query = new List.Query(opt.Tag, opt.Author, opt.FavoriteUser, opt.Limit, opt.Offset)
            {
                IsFeed = opt.IsFeed
            };

            var res = _helper.SendAsync(query);

            _helper.PrintResult(res, successed: "Found " + res.Result.ArticlesCount + " articles");
            return 1;
        }

        public int GetArticle(GetArticleOption opt)
        {
            var query = new Details.Query(opt.Slug);

            var res = _helper.SendAsync(query);

            _helper.PrintResult(res, successed: "Found " + opt.Slug + " article.!");
            return 1;
        }

        public int EditArticle(EditArticleOption opt)
        {
            var command = new Edit.Command()
            {
                Article = new Edit.ArticleData()
                {
                    Body = opt.Body,
                    Description = opt.Description,
                    TagList = opt.Tags.ToArray(),
                    Title = opt.Title
                },
                Slug = opt.Slug
            };

            var res = _helper.SendAsync(command);

            _helper.PrintResult(res, successed: "Article updated.!");

            return 1;
        }

        public int DeleteArticle(DeleteArticleOption opt)
        {
            var command = new Delete.Command(opt.Slug);
            var res = _helper.SendAsync(command);
            _helper.PrintResult(res, "Deleted article " + opt.Slug);
            return 1;
        }
    }
}