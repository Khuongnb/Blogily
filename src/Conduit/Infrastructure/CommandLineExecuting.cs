using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Conduit.Infrastructure.CLParser.Articles;
using Conduit.Infrastructure.CLParser.Comments;
using Conduit.Infrastructure.CLParser.Favorites;
using Conduit.Infrastructure.CLParser.Followers;
using Conduit.Infrastructure.CLParser.Profiles;
using Conduit.Infrastructure.CLParser.Tags;
using Conduit.Infrastructure.CLParser.Users;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Conduit.Infrastructure
{
    public class CommandLineExecuting: ICommandLineExecuting, IHostedService
    {

        private readonly string[] _args;
        private  Type[] _options;

        private readonly IArticlesActionHandler _articles;
        private readonly ICommentsActionHandler _comments;
        private readonly IFavoritesActionHandler _favorites;
        private readonly IFollowersActionHandler _followers;
        private readonly IProfilesActionHandler _profiles;
        private readonly ITagsActionHandler _tags;
        private readonly IUsersActionHandler _users;

        public CommandLineExecuting(
            CommandLineArgs cml, 
            IUsersActionHandler users, 
            IArticlesActionHandler articles, 
            ICommentsActionHandler comments, 
            IFavoritesActionHandler favorites, 
            IFollowersActionHandler followers, 
            IProfilesActionHandler profiles, 
            ITagsActionHandler tags)
        {
            _args = cml.Command;
            _users = users;
            _articles = articles;
            _favorites = favorites;
            _comments = comments;
            _followers = followers;
            _profiles = profiles;
            _tags = tags;

            RegisterOption();
        }

        private void RegisterOption()
        {
            _options = new[]
            {
                // Users
                typeof(CreateUserOption),
                typeof(GetUserOption),
                typeof(UpdateUserOption),
                typeof(LoginOption),
                // Articles
                typeof(CreateArticleOption),
                typeof(ListArticlesOption),
                typeof(GetArticlesOption),
                typeof(GetArticleOption),
                typeof(EditArticleOption),
                typeof(DeleteArticleOption),
                // Comments
                typeof(CreateCommentOption),
                typeof(ListCommentsOption),
                typeof(DeleteCommentOption),
                // Favorites
                typeof(AddFavoriteOption),
                typeof(DeleteFavoriteOption),
                // Followers
                typeof(FollowOption),
                typeof(UnfollowOption),
                // Profiles
                typeof(GetProfileOption),
                // Tags
                typeof(ListTagsOption)
            };
        }
        public void Run()
        {
            // ReSharper disable once LocalizableElement
#if DEBUG
            Console.WriteLine("RUNNING COMMAND LINE HANDLER");
#endif
            Parser.Default.ParseArguments(_args, _options)
                // Users
                .WithParsed<CreateUserOption>(opt => _users.CreateUser(opt))
                .WithParsed<GetUserOption>(opt => _users.GetUser(opt))
                .WithParsed<UpdateUserOption>(opt => _users.UpdateUser(opt))
                .WithParsed<LoginOption>(opt => _users.Login(opt))
                // Articles
                .WithParsed<ListArticlesOption>(opt => _articles.ListArticles(opt))
                .WithParsed<CreateArticleOption>(opt => _articles.CreateArticle(opt))
                .WithParsed<GetArticlesOption>(opt => _articles.GetArticles(opt))
                .WithParsed<GetArticleOption>(opt => _articles.GetArticle(opt))
                .WithParsed<EditArticleOption>(opt => _articles.EditArticle(opt))
                .WithParsed<DeleteArticleOption>(opt => _articles.DeleteArticle(opt))
                // Comments
                .WithParsed<CreateCommentOption>(opt => _comments.CreateComment(opt))
                .WithParsed<ListCommentsOption>(opt => _comments.ListComments(opt))
                .WithParsed<DeleteCommentOption>(opt => _comments.DeleteComment(opt))
                // Favorites
                .WithParsed<AddFavoriteOption>(opt => _favorites.AddFavorite(opt))
                .WithParsed<DeleteFavoriteOption>(opt => _favorites.DeleteFavorite(opt))
                // Followers
                .WithParsed<FollowOption>(opt => _followers.Follow(opt))
                .WithParsed<UnfollowOption>(opt => _followers.Unfollow(opt))
                // Profiles
                .WithParsed<GetProfileOption>(opt => _profiles.GetProfile(opt))
                // Tags
                .WithParsed<ListTagsOption>(opt => _tags.ListTags(opt))
                // If parse error
                .WithNotParsed(error =>
                {
                    Console.WriteLine("Error parsing command line: ");
                    Console.WriteLine(JsonConvert.SerializeObject(error, Formatting.Indented));
                });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
