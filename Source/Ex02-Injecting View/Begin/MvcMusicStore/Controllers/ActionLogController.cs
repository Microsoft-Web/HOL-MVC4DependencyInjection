namespace MvcMusicStore.Controllers
{    
    using System.Linq;
    using System.Web.Mvc;
    using MvcMusicStore.Models;

    public class ActionLogController : Controller
    {
        private MusicStoreEntities storeDB = new MusicStoreEntities();

        // GET: /ActionLog/
        public ActionResult Index()
        {
            var model = this.storeDB.ActionLog.OrderByDescending(al => al.DateTime).ToList();

            return this.View(model);
        }
    }
}
