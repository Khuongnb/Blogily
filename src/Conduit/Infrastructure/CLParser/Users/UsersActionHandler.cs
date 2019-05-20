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
    class UsersActionHandler: IUsersActionHandler
    {

        private readonly IActionHelper _helper;
        public UsersActionHandler(IActionHelper helper)
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

            _helper.PrintResult(res, successed:"User found: ");

            return 1;
        }

        public int UpdateUser(UpdateUserOption opt)
        {
            try
            {
                _helper.Username = opt.Username;
                var command = new Edit.Command()
                {
                    User = new Edit.UserData()
                    {
                        Username = opt.NewUsername,
                        Bio = opt.Bio,
                        Image = opt.Image,
                        Email = opt.Email,
                        Password = opt.Password
                    }
                };

                var res = _helper.SendAsync(command);

                _helper.PrintResult(res, successed: "Update user completed.!");
            }
            catch (UserNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return 1;
        }

        public int Login(LoginOption opt)
        {
            var command = new Login.Command()
            {
                User = new Login.UserData()
                {
                    Email = opt.Email,
                    Password = opt.Password
                }
            };

            var res = _helper.SendAsync(command);
            _helper.PrintResult(res, "Login Success.!");
            return 1;
        }
        
        // Get current user: No point to do

    }
}
