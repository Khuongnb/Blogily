using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Conduit.Infrastructure.CLParser.Users
{
    [Verb("create-user", HelpText = "Create new user")]
    public class CreateUserOption
    {
        [Option("email", Required = true)]
        public string Email { get; set; }

        [Option("password", Required = true)]
        public string Password { get; set; }

        [Option("username", Required = true)]
        public string Username { get; set; }
    }

    [Verb("login", HelpText = "Login")]
    public class LoginOption
    {
        [Option("email", Required = true)]
        public string Email { get; set; }
        
        [Option("password", Required = true)]
        public string Password { get; set; }
    }

    [Verb("get-user", HelpText = "Get user by username")]
    public class GetUserOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }
    }

    [Verb("update-user", HelpText = "Update user profile")]
    public class UpdateUserOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }
        
        [Option("new-username")]
        public string NewUsername { get; set; }

        [Option("email")]
        public string Email { get; set; }

        [Option("password")]
        public string Password { get; set; }

        [Option("bio")]
        public string Bio { get; set; }

        [Option("image")]
        public string Image { get; set; }

    }
}
