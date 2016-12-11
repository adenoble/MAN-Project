using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using mmsv4.Models;
using System.Data.Entity;
using System.Net;
using System.Text;

namespace mmsv5
{
    public class manukutility : Controller
    {
        mmsv4Entities db = new mmsv4Entities();

        public string StaticEmail()
        {
            var user = db.UserProfiles.Find(50);
            string err = string.Empty;
            string strmfor = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<p>Salaam alaykum Bro " + user.FirstName + " " + user.LastName + "</p>");
                sb.Append("<p><br/>This is confirm that MANUK has received and acknowledged the article you submited on MANUK website.");
                sb.Append("<br/>We have reviewed, approved and published the article on the website for the benefit of the Umah.<br/></p>");
                sb.Append("<p>Jazakallah Khair.</p>");
                sb.Append(Email.EmailFooter());
                Email.Send(Config.email, Config.nameAbr, "contact@jamiuaz.net", "Article Approved and Published on MANUK Website", sb.ToString(), true);

                return "<p>Send Successfully. Thank you " + user.FirstName + " " + user.LastName + ".</p>";
            }
            catch (Exception ex)
            {
                return "<p>Thank you " + user.FirstName + " " + user.LastName + ".</p> <p>An Error occur, the message has not been sent<br/>Please try again.</p>" + ex.ToString();
            }
        }
    }
}