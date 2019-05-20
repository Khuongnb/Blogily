using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Comments
{
    class CommentsActionHandler
    {
        private readonly ActionHelper _helper;

        public CommentsActionHandler(ActionHelper helper)
        {
            _helper = helper;
        }
    }
}
