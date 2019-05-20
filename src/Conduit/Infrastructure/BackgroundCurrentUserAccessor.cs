using System;
using System.Collections.Generic;
using System.Text;
using Conduit.Infrastructure.CLParser;

// ReSharper disable LocalizableElement

namespace Conduit.Infrastructure
{
    class BackgroundCurrentUserAccessor: ICurrentUserAccessor
    {
        private readonly IActionHelper _handler;
        public BackgroundCurrentUserAccessor(IActionHelper handler)
        {
            _handler = handler;
        }
        public string GetCurrentUsername()
        {
            return _handler.Username;
        }
    }
}
