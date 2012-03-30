using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class ActionLogController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        //
        // GET: /ActionLog/

        public ActionResult Index()
        {
            var model = storeDB.ActionLog.OrderByDescending(al => al.DateTime).ToList();

            return View(model);
        }

    }
}
