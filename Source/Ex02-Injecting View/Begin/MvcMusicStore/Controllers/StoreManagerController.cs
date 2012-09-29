namespace MvcMusicStore.Controllers
{
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using MvcMusicStore.Models;

    public class StoreManagerController : Controller
    {
        private MusicStoreEntities db = new MusicStoreEntities();

        // GET: /StoreManager/
        public ActionResult Index()
        {
            var albums = this.db.Albums.Include(a => a.Genre).Include(a => a.Artist)
                .OrderBy(a => a.Price);
            return this.View(albums.ToList());
        }

        // GET: /StoreManager/Details/5
        public ActionResult Details(int id = 0)
        {
            Album album = this.db.Albums.Find(id);

            if (album == null)
            {
                return this.HttpNotFound();
            }

            return this.View(album);
        }

        // GET: /StoreManager/Create
        public ActionResult Create()
        {
            this.ViewBag.GenreId = new SelectList(this.db.Genres, "GenreId", "Name");
            this.ViewBag.ArtistId = new SelectList(this.db.Artists, "ArtistId", "Name");
            return this.View();
        }

        // POST: /StoreManager/Create
        [HttpPost]
        public ActionResult Create(Album album)
        {
            if (ModelState.IsValid)
            {
                this.db.Albums.Add(album);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            this.ViewBag.GenreId = new SelectList(this.db.Genres, "GenreId", "Name", album.GenreId);
            this.ViewBag.ArtistId = new SelectList(this.db.Artists, "ArtistId", "Name", album.ArtistId);
            return this.View(album);
        }

        // GET: /StoreManager/Edit/5
        public ActionResult Edit(int id = 0)
        {
            Album album = this.db.Albums.Find(id);
            if (album == null)
            {
                return this.HttpNotFound();
            }

            this.ViewBag.GenreId = new SelectList(this.db.Genres, "GenreId", "Name", album.GenreId);
            this.ViewBag.ArtistId = new SelectList(this.db.Artists, "ArtistId", "Name", album.ArtistId);

            return this.View(album);
        }

        // POST: /StoreManager/Edit/5
        [HttpPost]
        public ActionResult Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                this.db.Entry(album).State = EntityState.Modified;
                this.db.SaveChanges();

                return this.RedirectToAction("Index");
            }

            this.ViewBag.GenreId = new SelectList(this.db.Genres, "GenreId", "Name", album.GenreId);
            this.ViewBag.ArtistId = new SelectList(this.db.Artists, "ArtistId", "Name", album.ArtistId);

            return this.View(album);
        }

        // GET: /StoreManager/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Album album = this.db.Albums.Find(id);
            if (album == null)
            {
                return this.HttpNotFound();
            }

            return this.View(album);
        }

        // POST: /StoreManager/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = this.db.Albums.Find(id);
            this.db.Albums.Remove(album);
            this.db.SaveChanges();

            return this.RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}