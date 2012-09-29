namespace MvcMusicStore.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using MvcMusicStore.Models;

    public class HomeController : Controller
    {
        private MusicStoreEntities storeDB = new MusicStoreEntities();

        // GET: /Home/
        public ActionResult Index()
        {
            return this.View();
        }

        // GET: /Store/Browse
        public string Browse(string genre)
        {
            string message = HttpUtility.HtmlEncode("Store.Browse, Genre = " + genre);

            return message;
        }

        // GET: /Store/Details
        public ActionResult Details(int id)
        {
            var album = new Album { Title = "Album " + id };

            return this.View(album);
        }
    }
}
