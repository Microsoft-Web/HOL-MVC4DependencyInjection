using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.ViewModels
{
    public class StoreManagerViewModel
    {
        public Album Album { get; set; }
        public SelectList Artists { get; set; }
        public SelectList Genres { get; set; }
    }
}