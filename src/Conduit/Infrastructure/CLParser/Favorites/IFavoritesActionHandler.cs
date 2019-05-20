using System;
using System.Collections.Generic;
using System.Text;

namespace Conduit.Infrastructure.CLParser.Favorites
{
    public interface IFavoritesActionHandler
    {
        // Add favorite: add-favorite --username="" --slug=""
        int AddFavorite(AddFavoriteOption opt);


        // Delete favorite: delete-favorite --username="" --slug=""
        int DeleteFavorite(DeleteFavoriteOption opt);
    }
}
