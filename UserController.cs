using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Security;
using mmsv4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mmsv4.Controllers
{
    [Authorize(Roles = "Administrator, Super Administrator")]
    public class UserController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            IEnumerable<UserProfile> result = db.UserProfiles.ToList();
            Parallel.ForEach(result, c => c.UserRole = Roles.GetRolesForUser(c.UserName));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.UserProfiles.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(UserProfile UserProfile)
        {
            try
            {
                db.UserProfiles.Add(UserProfile);
                db.SaveChanges();
                return Json(UserProfile, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(UserProfile UserProfile)
        {
            try
            {
                string[] userRoles = Roles.GetRolesForUser(UserProfile.UserName);

                if (userRoles.Length > 0)
                {
                    Roles.RemoveUserFromRoles(UserProfile.UserName, userRoles);
                }

                string[] nRole = { };
                if (UserProfile.UserRole != null && UserProfile.UserRole.Length > 0)
                {
                    nRole = UserProfile.UserRole;
                    Roles.AddUserToRoles(UserProfile.UserName, nRole);
                }

                db.Entry(UserProfile).State = EntityState.Modified;
                db.SaveChanges();
                return Json(UserProfile, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(UserProfile UserProfile)
        {
            try
            {
                db.UserProfiles.Remove(UserProfile);
                db.SaveChanges();
                return Json("Successful", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }
    }

    public static class UserExtension
    {
        //public IEnumerable<UserProfile> ExtUserProfile(this IEnumerable<UserProfile> source, IEnumerable<webpages_UsersInRole> result)
    }
}