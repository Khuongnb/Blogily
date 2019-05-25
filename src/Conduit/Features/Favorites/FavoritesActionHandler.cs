using System;
using System.Collections.Generic;
using System.Text;
using Conduit.Features.Favorites;
using Conduit.Infrastructure.Errors;
using MediatR;

namespace Conduit.Infrastructure.CLParser.Favorites
{
    class FavoritesActionHandler: IFavoritesActionHandler
    {
        private readonly IActionHelper _helper;

        public FavoritesActionHandler(IActionHelper helper)
        {
            _helper = helper;
        }

        public int AddFavorite(AddFavoriteOption opt)
        {
            try
            {
                _helper.Username = opt.Username;
                var command = new Add.Command(opt.Slug);
                var res = _helper.SendAsync(command);
                _helper.PrintResult(res, successed: "Favorited.!");
            }
            catch (UserNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return 1;
        }

        public int DeleteFavorite(DeleteFavoriteOption opt)
        {
            try
            {
                _helper.Username = opt.Username;
                var command = new Delete.Command(opt.Slug);
                var res = _helper.SendAsync(command);
                _helper.PrintResult(res, successed: "Favorited.!");
            }
            catch (UserNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 1;
        }
    }
}
