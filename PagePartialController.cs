using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mmsv4.Models;
using System.Web.Security;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace mmsv4.Controllers
{
    public class PagePartialController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();

        #region Partial Pages

        public ActionResult _ourExec(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            getexec_Result pg = db.getexec(id).FirstOrDefault();
            if (pg == null)
            {
                return HttpNotFound();
            }
            return PartialView(pg);
        }

        [ChildActionOnly]
        public ActionResult _Executive()
        {
            return PartialView(db.getexecutive().ToList());
        }

        [ChildActionOnly]
        public ActionResult _HomeBanner()
        {
            var prlHomeBanner = db.web_banners.Include(x => x.media_images).Where(b => b.enable == true);

            if (prlHomeBanner == null)
            {
                return new EmptyResult();
            }

            return PartialView(prlHomeBanner);
        }
        
        [ChildActionOnly]
        public ActionResult _HomeVideo()
        {

            List<Item> lstItem = new List<Item>();
            try
            {
                string jsonResult;
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=4&playlistId=UURxZMdI2WGHimM4cvRNn91A&key=AIzaSyACE8gpRkTNXlz21vpuLB1kkX8aQ7_X1Xg");
                rq.Method = "GET";
                rq.ContentType = "application/json";
                HttpWebResponse rs = (HttpWebResponse)rq.GetResponse();
                using (StreamReader sr = new StreamReader(rs.GetResponseStream()))
                {
                    jsonResult = sr.ReadToEnd();
                }
                System.Web.Script.Serialization.JavaScriptSerializer serialise = new System.Web.Script.Serialization.JavaScriptSerializer();
                YouTubeModel ytm = serialise.Deserialize<YouTubeModel>(jsonResult);
                lstItem = ytm.items;
                lstItem = lstItem.Where(x => x.id != "UUYRuYf9JvtOeaBpoHyrX-jaKM_y_YZysR").ToList();

            }
            catch (Exception ex)
            {

            }

            var prlHomeVideo = lstItem.FirstOrDefault();

            if (prlHomeVideo == null)
            {
                return new EmptyResult();
            }

            return PartialView(prlHomeVideo);
        }

        [ChildActionOnly]
        public ActionResult _HomeAyah()
        {
            var prlHomeAyah = db.content_monthayah_get_latest().FirstOrDefault();

            //if (prlHomeAyah == null)
            //{
            //    return new EmptyResult();
            //}

            return PartialView(prlHomeAyah);
        }

        [ChildActionOnly]
        public ActionResult _HomeArticle()
        {
            var prlHomeArticle = db.content_article_get_latest().FirstOrDefault();

            //if (prlHomeArticle == null)
            //{
            //    return new EmptyResult();
            //}

            return PartialView(prlHomeArticle);
        }

        [ChildActionOnly]
        public ActionResult _HomeDonation()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult _HomeVisitor()
        {
            //var joinIsoTwo = from ana in myAnalytics.GetAna()
            //                 join iso in myAnalytics.LstCountrIsoTwo()
            //                 on ana.country equals iso.name
            //                 select new { ana.country, ana.no, iso.code };


            //ViewBag.Analytics = myAnalytics.GetAna().Sum(x => x.no);
            //ViewBag.AnalyticsCcn = myAnalytics.GetAna().Count();
            //ViewBag.AnalyticsLst = myAnalytics.GetAna().OrderByDescending(x => x.no);
            //string keyFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/API Project-d9f2287391f8.p12"));

            //ViewBag.AnalyticsLst = keyFile;
            // return PartialView();
            List<Item> lstItem = new List<Item>();
            try
            {
                string jsonResult;
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=1&videoId=DWuOqM1Q7VY&playlistId=UURxZMdI2WGHimM4cvRNn91A&key=AIzaSyACE8gpRkTNXlz21vpuLB1kkX8aQ7_X1Xg");
                rq.Method = "GET";
                rq.ContentType = "application/json";
                HttpWebResponse rs = (HttpWebResponse)rq.GetResponse();
                using (StreamReader sr = new StreamReader(rs.GetResponseStream()))
                {
                    jsonResult = sr.ReadToEnd();
                }
                System.Web.Script.Serialization.JavaScriptSerializer serialise = new System.Web.Script.Serialization.JavaScriptSerializer();
                YouTubeModel ytm = serialise.Deserialize<YouTubeModel>(jsonResult);
                lstItem = ytm.items;

            }
            catch (Exception ex)
            {

            }

            var prlHomeVideo = lstItem.FirstOrDefault();

            if (prlHomeVideo == null)
            {
                return new EmptyResult();
            }

            return PartialView(prlHomeVideo);
        }

        [ChildActionOnly]
        public ActionResult _HomeNews()
        {
            var prlHomeNews = db.content_news.Where(x => x.enable == true).OrderByDescending(x => x.datecreated).ToList();

            if (prlHomeNews == null)
            {
                return new EmptyResult();
            }

            return PartialView(prlHomeNews);
        }

        [ChildActionOnly]
        public ActionResult _Video(int? page)
        {


            //var products = MyProductDataSource.FindAllProducts(); //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

            //var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            //var onePageOfProducts = products.ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            //ViewBag.OnePageOfProducts = onePageOfProducts;
            //return View();

            //var prlVideo = db.media_videos.Where(x => x.enable == true).OrderByDescending(x => x.datecreated).ToList();
            //return PartialView(prlVideo);
            List<Item> lstItem = new List<Item>();
            try
            {
                string jsonResult;
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId=UURxZMdI2WGHimM4cvRNn91A&key=AIzaSyACE8gpRkTNXlz21vpuLB1kkX8aQ7_X1Xg");
                rq.Method = "GET";
                rq.ContentType = "application/json";
                HttpWebResponse rs = (HttpWebResponse)rq.GetResponse();
                using (StreamReader sr = new StreamReader(rs.GetResponseStream()))
                {
                    jsonResult = sr.ReadToEnd();
                }
                System.Web.Script.Serialization.JavaScriptSerializer serialise = new System.Web.Script.Serialization.JavaScriptSerializer();
                YouTubeModel ytm = serialise.Deserialize<YouTubeModel>(jsonResult);
                lstItem = ytm.items;
                lstItem = lstItem.Where(x => x.id != "UUYRuYf9JvtOeaBpoHyrX-jaKM_y_YZysR").ToList();

                //var pageNumber = page ?? 1;
                //var onePageOfytm = ytm.items.ToPagedList(pageNumber, 25);
                //ViewBag.onePageOfytm = onePageOfytm;

            }
            catch (Exception ex)
            {

            }
            return PartialView(lstItem.ToList());
        }

        [ChildActionOnly]
        public ActionResult _Ayah()
        {
            var prlAyah = db.content_monthayah.Where(x => x.enable == true).OrderByDescending(x => x.datecreated).ToList();
            return PartialView(prlAyah);
        }

        [ChildActionOnly]
        public ActionResult _Article()
        {
            var prlArticle = db.content_article.Where(x => x.enable == true).OrderByDescending(x => x.datecreated).ToList();
            return PartialView(prlArticle);
        }

        [ChildActionOnly]
        public ActionResult _News()
        {
            var prlNews = db.content_news.Where(x => x.enable == true).OrderByDescending(x => x.datecreated).ToList();
            return PartialView(prlNews);
        }

        [ChildActionOnly]
        public ActionResult _DaySolat()
        {
            int SDay = DateTime.Now.Day;
            int SMnt = DateTime.Now.Month;

            var prlSolat = db.content_solat.Where(x => x.solatDay == SDay && x.solatMnt == SMnt).FirstOrDefault();
            return PartialView(prlSolat);
        }

        [ChildActionOnly]
        public ActionResult _MonthSolat()
        {
            int SMnt = DateTime.Now.Month;

            var prlSolat = db.content_solat.Where(x => x.solatMnt == SMnt).ToList();
            return PartialView(prlSolat);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
