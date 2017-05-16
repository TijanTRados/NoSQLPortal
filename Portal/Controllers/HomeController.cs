using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Drawing;
using Portal.Models;


namespace Portal.Controllers
{
    public class HomeController : Controller
    {
        private IMongoDatabase database;

        //Dohvaćanje iz baze podataka
        public async Task<ActionResult> Index()
        {
            var client = new MongoClient("mongodb://localhost");
            database = client.GetDatabase("portal");
            var articlesCol = database.GetCollection<Article>("vijesti");
            var output = await articlesCol.Find(Builders<Article>.Filter.Empty).Sort(Builders<Article>.Sort.Descending("_id")).Limit(10).ToListAsync();
            ArticlesModel articles = new ArticlesModel();
            articles.Articles = output;
            return View(articles);
        }

        //Dodavanje u bazu podataka
        public async Task<ActionResult> Add(string naziv, string sadrzaj, string autor)
        {
            var client = new MongoClient("mongodb://localhost");
            database = client.GetDatabase("portal");
            var articlesCol = database.GetCollection<Article>("vijesti");
            var novi = new Data.Article();
            novi.naziv = naziv;
            novi.sadrzaj = sadrzaj;
            novi.autor = autor;
            novi.link = null;
            await articlesCol.InsertOneAsync(novi);
            return RedirectToAction("Index");

        }

        //Dodavanje slike za određeni post
        public async Task<ActionResult> AddPicture(HttpPostedFileBase picture, string naziv)
        {
            if (picture.ContentLength > 0 && picture != null)
            {
                var client = new MongoClient("mongodb://localhost");
                database = client.GetDatabase("portal");
                var articlesCol = database.GetCollection<Article>("vijesti");
                var filter = Builders<Article>.Filter.Eq("naziv", naziv);
                using (var reader = new System.IO.BinaryReader(picture.InputStream))
                {
                    byte[] data = reader.ReadBytes(picture.ContentLength);
                    var update = Builders<Article>.Update.Set("link", data);
                    var output = await articlesCol.UpdateOneAsync(filter, update);
                }
            }
            return RedirectToAction("Index");
        }

        //Pohranjivanje komentara
        public async Task<ActionResult> Update(string naziv, string komentar)
        {
            var client = new MongoClient("mongodb://localhost");
            database = client.GetDatabase("portal");
            var articlesCol = database.GetCollection<Article>("vijesti");
            var filter = Builders<Article>.Filter.Eq("naziv", naziv);
            var update = Builders<Article>.Update.Push("komentari", komentar);
            var output = await articlesCol.UpdateOneAsync(filter, update);
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}