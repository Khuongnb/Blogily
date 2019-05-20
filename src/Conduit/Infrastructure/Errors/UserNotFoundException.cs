using System;
using System.Collections.Generic;
using System.Text;

namespace Conduit.Infrastructure.Errors
{
    public class UserNotFoundException: Exception
    {
        public UserNotFoundException(string message): base(message)
        {
            
        }
    }
}
