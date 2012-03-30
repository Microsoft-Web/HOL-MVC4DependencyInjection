using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using MvcMusicStore.Services;

namespace MvcMusicStore.Pages
{
    public class MyBasePage : System.Web.Mvc.WebViewPage<MvcMusicStore.ViewModels.StoreBrowseViewModel>
    {
        [Dependency]
        public IMessageService MessageService { get; set; }

        public override void Execute()
        {
        }
    }
}