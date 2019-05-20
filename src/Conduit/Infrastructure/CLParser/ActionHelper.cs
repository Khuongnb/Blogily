using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conduit.Infrastructure.Errors;
using MediatR;
using Newtonsoft.Json;
// ReSharper disable LocalizableElement

namespace Conduit.Infrastructure.CLParser
{
    public class ActionHelper: IActionHelper
    {
        private readonly IMediator _mediator;
        private readonly ConduitContext _context;
        private string _username;

        public ActionHelper(IMediator mediator, ConduitContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public string Username
        {
            get => _username;
            set
            {
                var x = value;
                if (_context.Persons.FirstOrDefault(o => o.Username == x) == null)
                {
                    throw new UserNotFoundException("User not found");
                }
                _username = x;
            }
        }

        public async Task<T> SendAsync<T>(IRequest<T> command)
        {
            return await _mediator.Send(command);
        }

        public void PrintResult<T>(Task<T> result, string successed="", string failed="Errors:")
        {
            if (result.IsCompletedSuccessfully)
            {
                Console.WriteLine(successed);
                Console.WriteLine(JsonConvert.SerializeObject(result.Result, Formatting.Indented));
                Console.WriteLine();
            }
            else
            {
                var ex = result.Exception?.InnerException;
                Console.Write(failed);

                if (ex is RestException exception)
                    Console.WriteLine(JsonConvert.SerializeObject(exception?.Errors, Formatting.None));
                else
                    Console.WriteLine(result.Exception?.GetBaseException());
                Console.WriteLine();
            }
        }

    }
}
