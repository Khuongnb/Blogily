using System;
using System.Collections.Generic;
using System.Text;

namespace Conduit.Infrastructure.CLParser.Users
{
    public interface IUsersActionHandler
    {
        // Create user: create-user --username="username" --password="password" --email="yfam@dev.inc"
        int CreateUser(CreateUserOption option);

        // Get user: get-user --username="username"
        int GetUser(GetUserOption opt);

        // Update user: update-user --username="usename" --new-username="username" --bio="doctor" --password="123" [ect...]
        // TODO: Require elaborate test
        int UpdateUser(UpdateUserOption opt);

        // Login: login --email="" --password=""
        int Login(LoginOption opt);

        // Get current user: No point to do since using --username as current user
    }
}
