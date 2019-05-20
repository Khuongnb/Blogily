using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Profiles
{
    class ProfilesActionHandler
    {

        private readonly IMediator _mediator;

        public ProfilesActionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

    }
}
