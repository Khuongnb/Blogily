using CommandLine;

namespace Conduit.Infrastructure.CLParser.Profiles
{
    [Verb("get-profile")]
    public class GetProfileOption
    {
        [Option("username", Required = true)]
        public string Username { get; set; }
    }
}
