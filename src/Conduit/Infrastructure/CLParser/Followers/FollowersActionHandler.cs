using System;
using System.Collections.Generic;
using System.Text;
using Conduit.Features.Followers;
using Conduit.Infrastructure.Errors;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Followers
{
    class FollowersActionHandler: IFollowersActionHandler
    {
        private readonly IActionHelper _helper;

        public FollowersActionHandler(IActionHelper helper)
        {
            _helper = helper;
        }

        public int Follow(FollowOption opt)
        {
            try
            {

                _helper.Username = opt.Username;
                var command = new Add.Command(opt.TargetUsername);
                var res = _helper.SendAsync(command);
                _helper.PrintResult(res, successed:"User "+opt.Username + " now following user " + opt.TargetUsername);
            }
            catch (UserNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }
            return 1;
        }

        public int Unfollow(UnfollowOption opt)
        {
            try
            {

                _helper.Username = opt.Username;
                var command = new Delete.Command(opt.TargetUsername);
                var res = _helper.SendAsync(command);
                _helper.PrintResult(res, successed: "User " + opt.Username + " now following user " + opt.TargetUsername);
            }
            catch (UserNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }
            return 1;
        }
    }
}
