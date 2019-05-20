using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Favorites
{
    class FavoritesActionHandler
    {
        private readonly IMediator _mediator;

        public FavoritesActionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
