using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Conduit.Infrastructure.Errors;
using MediatR;
using Newtonsoft.Json;

namespace Conduit.Infrastructure.CLParser
{
    public class ActionHelper
    {

        private readonly IMediator _mediator;

        public ActionHelper(IMediator mediator)
        {
            _mediator = mediator;
        }


        public string Username { get; set; }

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
            }
            else
            {
                Console.WriteLine(failed);
                try
                {
                    Console.WriteLine(JsonConvert.SerializeObject(
                        ((RestException) result.Exception?.InnerException)?.Errors, Formatting.Indented));
                }
                catch
                {
                    Console.WriteLine(JsonConvert.SerializeObject(result.Exception, Formatting.Indented));
                }
            }
        }

    }
}
