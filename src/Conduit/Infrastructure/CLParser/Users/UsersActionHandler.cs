using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Conduit.Features.Users;
using Conduit.Infrastructure.Errors;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
// ReSharper disable LocalizableElement

namespace Conduit.Infrastructure.CLParser.Users
{
    class UsersActionHandler
    {

        private readonly ActionHelper _helper;
        public UsersActionHandler(ActionHelper helper)
        {
            _helper = helper;
        }

        public int CreateUser(CreateUserOption option)
        {
            Console.WriteLine("Creating user...");
            var command = new Create.Command()
            {
                User = new Create.UserData()
                {
                    Email = option.Email,
                    Password = option.Password,
                    Username = option.Username
                }
            };

            var res = _helper.SendAsync(command);

            _helper.PrintResult(res,"Create user completed...");
            return 1;
        }


        public int GetUser(GetUserOption opt)
        {
            var query = new Details.Query()
            {
                Username = opt.Username
            };

            var res = _helper.SendAsync(query);

            _helper.PrintResult(res);

            return 0;
        }

    }
}
