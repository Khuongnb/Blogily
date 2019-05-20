using System;
using System.Collections.Generic;
using System.Text;
using Conduit.Infrastructure.CLParser;

namespace Conduit.Infrastructure
{
    interface ICommandLineExecuting
    {
        void Run();
    }
}
