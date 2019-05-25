using CommandLine;

namespace Conduit.Infrastructure.CLParser.Profiles
{
    [Verb("get-profile", HelpText = "Get user profile by username")]
    public class GetProfileOption
    {
        [Option("username", Default = "")]
        public string Username { get; set; }

        [Option("target", Required = true)]
        public string TargetUser { get; set; }
    }
}
