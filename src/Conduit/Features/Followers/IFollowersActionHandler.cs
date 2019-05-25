using System;
using System.Collections.Generic;
using System.Text;

namespace Conduit.Infrastructure.CLParser.Followers
{
    public interface IFollowersActionHandler
    {
        // Follow: follow --username="" --target=""
        int Follow(FollowOption opt);

        // Unfollow: unfollow --username="" --target=""
        int Unfollow(UnfollowOption opt);

    }
}
