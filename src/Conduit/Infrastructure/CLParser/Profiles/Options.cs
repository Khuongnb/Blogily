using CommandLine;

namespace Conduit.Infrastructure.CLParser.Profiles
{
    [Verb("get-profile", HelpText = "Get user profile by username")]
    public class GetProfileOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }
    }
}
