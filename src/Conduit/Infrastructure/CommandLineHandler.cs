using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CommandLine;
using Conduit.Infrastructure.CLParser;
using Conduit.Infrastructure.CLParser.Articles;
using Conduit.Infrastructure.CLParser.Comments;
using Conduit.Infrastructure.CLParser.Favorites;
using Conduit.Infrastructure.CLParser.Followers;
using Conduit.Infrastructure.CLParser.Profiles;
using Conduit.Infrastructure.CLParser.Tags;
using Conduit.Infrastructure.CLParser.Users;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Conduit.Infrastructure
{
    public class CommandLineHandler: ICommandLineHandler, IHostedService
    {

        private readonly String[] _args;
        private  Type[] _options;

        private readonly ArticlesActionHandler _articles;
        private readonly CommentsActionHandler _comments;
        private readonly FavoritesActionHandler _favorites;
        private readonly FollowersActionHandler _followers;
        private readonly ProfilesActionHandler _profiles;
        private readonly TagsActionHandler _tags;
        private readonly UsersActionHandler _users;

        private readonly ActionHelper _helper;
        
        public CommandLineHandler(CommandLineArgs cml, IMediator mediator, IMapper mapper)
        {
            _args = cml.Command;

            _helper = new ActionHelper(mediator);
            
            _articles = new ArticlesActionHandler(_helper);
            _comments = new CommentsActionHandler(_helper);
            _favorites = new FavoritesActionHandler(mediator);
            _followers = new FollowersActionHandler(mediator);
            _profiles = new ProfilesActionHandler(mediator);
            _tags = new TagsActionHandler(mediator);
            _users = new UsersActionHandler(_helper);

            RegisterOption();
        }

        public ActionHelper GetHelper()
        {
            Console.WriteLine(_helper.Username);
            return _helper;
        }

        private void RegisterOption()
        {
            _options = new Type[]
            {
                typeof(ListArticlesOption),
                typeof(CreateArticleOption),
                typeof(CreateUserOption),
                typeof(GetUserOption)
            };
        }
        public void Run()
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine("RUNNING COMMAND LINE HANDLER");

            Parser.Default.ParseArguments(_args, _options)
                .WithParsed<ListArticlesOption>(opt => _articles.ListArticles(opt))
                .WithParsed<CreateArticleOption>(opt => _articles.CreateArticle(opt))
                .WithParsed<CreateUserOption>(opt => _users.CreateUser(opt))
                .WithParsed<GetUserOption>(opt => _users.GetUser(opt))
                .WithNotParsed(error =>
                {
                    Console.WriteLine("Error");
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
