using CommandLine;

namespace Conduit.Infrastructure.CLParser.Followers
{
    [Verb("follow", HelpText = "Follow a user")]
    public class FollowOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option('t',"target", Required = true)]
        public string TargetUsername { get; set; }
    }

    [Verb("unfollow", HelpText = "Unfollow a user")]
    public class UnfollowOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option('t',"target", Required = true)]
        public string TargetUsername { get; set; }
    }
}
