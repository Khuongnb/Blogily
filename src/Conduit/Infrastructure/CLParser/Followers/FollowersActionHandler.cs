using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Followers
{
    class FollowersActionHandler
    {
        private readonly IMediator _mediator;

        public FollowersActionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
