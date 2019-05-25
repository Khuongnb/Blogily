using System;
using System.Collections.Generic;
using System.Text;

namespace Conduit.Infrastructure.CLParser.Articles
{
    public interface IArticlesActionHandler
    {
        // List all articles: list-articles
        int ListArticles(ListArticlesOption article);

        // Create new article: create-article --title="title" --description="des" --body="body" --tag-list="tag1","tag2" --username="username"
        int CreateArticle(CreateArticleOption article);

        // Get articles: get-articles --author="username"
        int GetArticles(GetArticlesOption opt);
 
        // Get article: get-article --slug=""
        int GetArticle(GetArticleOption opt);

        // Edit article: edit-article --slug="newtitle" --title="new title"
        // TODO: Fix slug generator
        int EditArticle(EditArticleOption opt);

        // Delete article: delete-article --slug=""
        // TODO: Fix slug gerenator
        int DeleteArticle(DeleteArticleOption opt);

    }
}
