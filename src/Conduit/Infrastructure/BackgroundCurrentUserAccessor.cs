using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable LocalizableElement

namespace Conduit.Infrastructure
{
    class BackgroundCurrentUserAccessor: ICurrentUserAccessor
    {
        private readonly ICommandLineHandler _handler;
        public BackgroundCurrentUserAccessor(ICommandLineHandler handler)
        {
            _handler = handler;
        }

        public void Print()
        {
            Console.WriteLine("Print in user accessor");
        }

        public string GetCurrentUsername()
        {
            Console.WriteLine(_handler.GetHelper());
            Console.WriteLine(_handler.GetHelper().Username);
            return _handler.GetHelper().Username;
        }
    }
}
