
using System;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using mmsv4.Models;

using System.Web.Script.Serialization;
using ASPSnippets.FaceBookAPI;
using System.Collections.Generic;

using System.Web.UI;
using System.Web.UI.WebControls;

namespace mmsv4.Controllers
{
    [Authorize(Roles = "Administrator, Super Administrator")]
    public class NewsController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var result = db.content_news
                .OrderByDescending(x => x.datecreated)
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.content_news.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(content_news content_news)
        {
            try
            {
                db.content_news.Add(content_news);
                db.SaveChanges();
                return Json(content_news, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(content_news content_news)
        {
            try
            {
                db.Entry(content_news).State = EntityState.Modified;
                db.SaveChanges();
                return Json(content_news, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(content_news content_news)
        {
            try
            {
                var nws = db.content_news.Find(content_news.newsid);
                db.content_news.Remove(nws);
                db.SaveChanges();
                return Json(content_news, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        public partial class CS : System.Web.UI.Page
        {
            protected void Login(object sender, EventArgs e)
            {
                FaceBookConnect.Authorize("user_photos,email", Request.Url.AbsoluteUri.Split('?')[0]);
            }

            protected void Page_Load(object sender, EventArgs e)
            {
                FaceBookConnect.API_Key = "<Your API Key>";
                FaceBookConnect.API_Secret = "<Your API Secret>";
                if (!IsPostBack)
                {
                    if (Request.QueryString["error"] == "access_denied")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User has denied access.')", true);
                        return;
                    }

                    string code = Request.QueryString["code"];
                    if (!string.IsNullOrEmpty(code))
                    {
                        string data = FaceBookConnect.Fetch(code, "me");
                        FaceBookUser faceBookUser = new JavaScriptSerializer().Deserialize<FaceBookUser>(data);
                        faceBookUser.PictureUrl = string.Format("https://graph.facebook.com/{0}/picture", faceBookUser.Id);

                        /*
                        pnlFaceBookUser.Visible = true;
                        lblId.Text = faceBookUser.Id;
                        lblUserName.Text = faceBookUser.UserName;
                        lblName.Text = faceBookUser.Name;
                        lblEmail.Text = faceBookUser.Email;
                        ProfileImage.ImageUrl = faceBookUser.PictureUrl;
                        btnLogin.Enabled = false;

                        */
                    }
                }
            }
        }
        public class FaceBookUser
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string UserName { get; set; }
            public string PictureUrl { get; set; }
            public string Email { get; set; }
        }




    }
}