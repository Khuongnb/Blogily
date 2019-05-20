using System;
using System.Collections.Generic;
using System.Text;
using Conduit.Features.Profiles;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Profiles
{
    class ProfilesActionHandler: IProfilesActionHandler
    {
        private readonly IActionHelper _helper;

        public ProfilesActionHandler(IActionHelper helper)
        {
            _helper = helper;
        }

        public int GetProfile(GetProfileOption opt)
        {
            var query = new Details.Query()
            {
                Username = opt.Username
            };

            var res = _helper.SendAsync(query);
            _helper.PrintResult(res, successed:"Found user.!");
            return 1;
        }
    }
}
