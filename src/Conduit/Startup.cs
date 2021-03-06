﻿using AutoMapper;
using Conduit.Features.Profiles;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using Conduit.Infrastructure.Security;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using Conduit.Infrastructure.CLParser;
using Conduit.Infrastructure.CLParser.Articles;
using Conduit.Infrastructure.CLParser.Comments;
using Conduit.Infrastructure.CLParser.Favorites;
using Conduit.Infrastructure.CLParser.Followers;
using Conduit.Infrastructure.CLParser.Profiles;
using Conduit.Infrastructure.CLParser.Tags;
using Conduit.Infrastructure.CLParser.Users;
using Microsoft.Extensions.Hosting;
// ReSharper disable InconsistentNaming

namespace Conduit
{
    public class Startup
    {
        public const string DEFAULT_DATABASE_CONNECTIONSTRING = "Filename=realworld.db";
        public const string DEFAULT_DATABASE_PROVIDER = "sqlite";

        private static IConfiguration _config;
        public Action<IConfigurationBuilder> AppConfig = AppConfiguration;
        public Action<IServiceCollection> StaticServicesConfig = ConfigureServices;
        public Action<HostBuilderContext, ILoggingBuilder> RuntimeConfigure = Configure;

        private static void AppConfiguration(IConfigurationBuilder configuration)
        {
            _config = configuration.AddEnvironmentVariables().Build();
        }

        public void AddNonStaticServices(IServiceCollection services)
        {
            services.AddAutoMapper(GetType().Assembly);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DBContextTransactionPipelineBehavior<,>));
            services.AddScoped(typeof(IActionHelper), typeof(ActionHelper));
            services.AddScoped(typeof(IUsersActionHandler), typeof(UsersActionHandler));
            services.AddScoped(typeof(IArticlesActionHandler), typeof(ArticlesActionHandler));
            services.AddScoped(typeof(ICommentsActionHandler), typeof(CommentsActionHandler));
            services.AddScoped(typeof(IFavoritesActionHandler), typeof(FavoritesActionHandler));
            services.AddScoped(typeof(IFollowersActionHandler), typeof(FollowersActionHandler));
            services.AddScoped(typeof(IProfilesActionHandler), typeof(ProfilesActionHandler));
            services.AddScoped(typeof(ITagsActionHandler), typeof(TagsActionHandler));

            // take the connection string from the environment variable or use hard-coded database name
            var connectionString = _config.GetValue<string>("ASPNETCORE_Conduit_ConnectionString") ??
                                   DEFAULT_DATABASE_CONNECTIONSTRING;
            // take the database provider from the environment variable or use hard-coded database provider
            var databaseProvider = _config.GetValue<string>("ASPNETCORE_Conduit_DatabaseProvider");
            if (string.IsNullOrWhiteSpace(databaseProvider))
                databaseProvider = DEFAULT_DATABASE_PROVIDER;
            services.AddDbContext<ConduitContext>(options =>
            {
                if (databaseProvider.ToLower().Trim().Equals("sqlite"))
                    options.UseSqlite(connectionString);
                else if (databaseProvider.ToLower().Trim().Equals("sqlserver"))
                {
                    // only works in windows container
                    options.UseSqlServer(connectionString);
                }
                else
                    throw new Exception("Database provider unknown. Please check configuration");
            });

            services.AddLocalization(x => x.ResourcesPath = "Resources");

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(x =>
            {
                x.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                x.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }}
                });
                x.SwaggerDoc("v1", new Info { Title = "RealWorld API", Version = "v1" });
                x.CustomSchemaIds(y => y.FullName);
                x.DocInclusionPredicate((version, apiDescription) => true);
                x.TagActionsBy(y => new List<string>()
                {
                    y.GroupName
                });
            });
            services.AddCors();
            services.AddMvc(opt =>
                {
                    opt.Conventions.Add(new GroupByApiRootConvention());
                    opt.Filters.Add(typeof(ValidatorActionFilter));
                })
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddFluentValidation(cfg =>
                {
                    cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
                });

            //services.AddAutoMapper(GetType().Assembly);

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUserAccessor, BackgroundCurrentUserAccessor>();
            services.AddScoped<IProfileReader, ProfileReader>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddJwt();


            // Willing to run service
            services.AddSingleton<IHostedService, CommandLineExecuting>();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(HostBuilderContext context, ILoggingBuilder builder)
        {

            builder.Services.BuildServiceProvider().GetRequiredService<ConduitContext>().Database.EnsureCreated();

        }
    }
}