using System;
using System.Collections.Generic;
using System.Text;

namespace Conduit.Infrastructure.CLParser.Profiles
{
    public interface IProfilesActionHandler
    {
        // Get profile: get-profile --username=""
        int GetProfile(GetProfileOption opt);
    }
}
