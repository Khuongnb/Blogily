using System;
using Conduit.Features.Users;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Tags
{
    public class UserData
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
    class TagsActionHandler
    {
        private readonly IMediator _mediator;
        public TagsActionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public int ListTags(ListTagsOption option)
        {

            
            return 1;
        }
    }
}
