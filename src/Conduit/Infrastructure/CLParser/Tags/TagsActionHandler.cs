using System;
using Conduit.Features.Tags;
using Conduit.Features.Users;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Tags
{
    class TagsActionHandler: ITagsActionHandler
    {
        private readonly IActionHelper _helper;
        public TagsActionHandler(IActionHelper helper)
        {
            _helper = helper;
        }

        public int ListTags(ListTagsOption option)
        {
            var res = _helper.SendAsync(new List.Query());
            _helper.PrintResult(res);
            return 1;
        }
    }
}
