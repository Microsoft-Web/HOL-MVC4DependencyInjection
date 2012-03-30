using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcMusicStore.Models;

namespace MvcMusicStore.Parts
{
    public interface IStoreService
    {
        IList<string> GetGenreNames();

        Genre GetGenreByName(string name);

        Album GetAlbum(int id);
    }
}
