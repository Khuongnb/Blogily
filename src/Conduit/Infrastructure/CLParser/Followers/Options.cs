using CommandLine;

namespace Conduit.Infrastructure.CLParser.Followers
{
    [Verb("follow")]
    public class FollowOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option('t',"target", Required = true)]
        public string TargetUsername { get; set; }
    }

    [Verb("unfollow")]
    public class UnfollowOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }

        [Option('t',"target", Required = true)]
        public string TargetUsername { get; set; }
    }
}
