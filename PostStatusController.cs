
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImpactWorks.FBGraph.Connector;
using ImpactWorks.FBGraph.Core;
using ImpactWorks.FBGraph.Interfaces;
using mmsv4.PostToFacebookWall;


namespace mmsv4.Controllers
{
    public class PostStatusController : Controller
    {
        //
        // GET: /PostStatus/

        public ActionResult Index()
        {
            return View();
        }

        Authentication auth = new Authentication();
        public ActionResult Success()
        {
            if (Request.QueryString["code"] != null)
            {
                string Code = Request.QueryString["code"];
                Session["facebookQueryStringValue"] = Code;
            }
            if (Session["facebookQueryStringValue"] != null)
            {
                Facebook facebook = auth.FacebookAuth();
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();
                IFeedPost FBpost = new FeedPost();
                if (Session["postStatus"].ToString() != "")
                {
                    FBpost.Message = Session["postStatus"].ToString();
                    facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);
                }
            }
            return View();
        }

        public JsonResult PostStatus(string msg)
        {
            Session["postStatus"] = msg;


            Facebook facebook = auth.FacebookAuth();
            if (Session["facebookQueryStringValue"] == null)
            {
                string authLink = facebook.GetAuthorizationLink();
                return Json(authLink);
            }

            if (Session["facebookQueryStringValue"] != null)
            {
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();
                IFeedPost FBpost = new FeedPost();
                if (Session["postStatus"].ToString() != "")
                {
                    FBpost.Message = Session["postStatus"].ToString();
                    facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);
                }

     /*For Link, Image and Video Sharing you just have to add some code within 
      Success action method. The code looks like this:

      Link Share:       */

                FBpost.Action = new FBAction { Name = "test link", Link = "https://www.google.co.uk/search?hl=en&site=imghp&tbm=isch&source=hp&biw=1366&bih=638&q=manuk&oq=manuk&gs_l=img.3..0l10.7431.8689.0.9031.5.5.0.0.0.0.124.428.4j1.5.0....0...1ac.1.64.img..0.5.427.gqtViOsBz8Q#hl=en&tbm=isch&q=Nigeria+muslim+association+of+nigeria&imgrc=U8D_zvwSFDdFKM%3A" };
                FBpost.Caption = "Test Image Share";
                FBpost.Description = "Test Image Desc";
                FBpost.ImageUrl = "https://www.google.co.uk/search?hl=en&site=imghp&tbm=isch&source=hp&biw=1366&bih=638&q=manuk&oq=manuk&gs_l=img.3..0l10.7431.8689.0.9031.5.5.0.0.0.0.124.428.4j1.5.0....0...1ac.1.64.img..0.5.427.gqtViOsBz8Q#hl=en&tbm=isch&q=Nigeria+muslim+association+of+nigeria&imgrc=U8D_zvwSFDdFKM%3A";
                FBpost.Message = "Check out test post";
                FBpost.Name = "Test Post";
                FBpost.Url = "http://softechblend.co.uk/";

              
                //Video Share:

                FBpost.Url = "https://www.youtube.com/watch?v=oQJa2Z25bWg";
                FBpost.Message = "Sharing a Youtube video";

                var postId = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);

               // Image Share:

                FBpost.Caption = "Image Share";

                FBpost.ImageUrl = "https://www.google.co.uk/search?hl=en&site=imghp&tbm=isch&source=hp&biw=1366&bih=638&q=manuk&oq=manuk&gs_l=img.3..0l10.7431.8689.0.9031.5.5.0.0.0.0.124.428.4j1.5.0....0...1ac.1.64.img..0.5.427.gqtViOsBz8Q#hl=en&tbm=isch&q=Nigeria+muslim+association+of+nigeria&imgrc=U8D_zvwSFDdFKM%3A";

                FBpost.Name = "Great Image";
                FBpost.Url = "https://www.google.co.uk/search?hl=en&site=imghp&tbm=isch&source=hp&biw=1366&bih=638&q=manuk&oq=manuk&gs_l=img.3..0l10.7431.8689.0.9031.5.5.0.0.0.0.124.428.4j1.5.0....0...1ac.1.64.img..0.5.427.gqtViOsBz8Q#hl=en&tbm=isch&q=Nigeria+muslim+association+of+nigeria&imgrc=U8D_zvwSFDdFKM%3A";

                 postId = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);


            }
            return Json("No");


        }


    }



}