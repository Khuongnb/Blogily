using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Conduit.Infrastructure.CLParser
{
    public interface IActionHelper
    {
        string Username { get; set; }
        Task<T> SendAsync<T>(IRequest<T> command);
        void PrintResult<T>(Task<T> result, string successed = "", string failed = "Errors:");
    }
}
