namespace MvcMusicStore.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MvcMusicStore.Models;

    public interface IStoreService
    {
        IList<string> GetGenreNames();

        IList<Genre> GetGenres(int max = 0);

        Genre GetGenreByName(string name);

        Album GetAlbum(int id);
    }
}
