using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mmsv4.Models;
using System.IO;

namespace mmsv4.Controllers
{
	[Authorize(Roles = "Administrator, Super Administrator")]
	public class VideoController : Controller
	{
		private mmsv4Entities db = new mmsv4Entities();

		public ActionResult Index()
		{
			var media_videos = db.media_videos.OrderByDescending(x => x.datecreated);
			return View(media_videos.ToList());
		}

		public ActionResult Youtube()
		{
			YouTubeModel ytm = new YouTubeModel();
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
				ytm = serialise.Deserialize<YouTubeModel>(jsonResult);

			}
			catch (Exception ex)
			{

			}
			return View(ytm.items.ToList());
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(media_videos media_videos)
		{
			if (ModelState.IsValid)
			{
				db.media_videos.Add(media_videos);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(media_videos);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			media_videos media_videos = db.media_videos.Find(id);
			if (media_videos == null)
			{
				return HttpNotFound();
			}
			return View(media_videos);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(media_videos media_videos)
		{
			if (ModelState.IsValid)
			{
				db.Entry(media_videos).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(media_videos);
		}

		public ActionResult Delete(int? id)
		{
			media_videos media_videos = db.media_videos.Find(id);
			db.media_videos.Remove(media_videos);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

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
