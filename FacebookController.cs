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
    public class FacebookController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }


        public partial class CS : System.Web.UI.Page
        {
            protected void Login(object sender, EventArgs e)
            {
                FaceBookConnect.Authorize("user_photos,email", Request.Url.AbsoluteUri.Split('?')[0]);
            }

            protected void Page_Load(object sender, EventArgs e)
            {
                FaceBookConnect.API_Key = "<353832594985788>";
                FaceBookConnect.API_Secret = "<308444d08eca45509d1e662d485b84ef>";
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