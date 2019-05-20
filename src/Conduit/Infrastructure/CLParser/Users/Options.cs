using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Conduit.Infrastructure.CLParser.Users
{
    [Verb("create-user")]
    class CreateUserOption
    {
        [Option("email", Required = true)]
        public string Email { get; set; }


        [Option("password", Required = true)]
        public string Password { get; set; }


        [Option("username", Required = true)]
        public string Username { get; set; }
    }

    [Verb("get-user")]
    class GetUserOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }
    }
}
