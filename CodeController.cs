using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Text;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace JCMS.Web.Controllers
{
    public class CodeController : Controller
    {
        string contid = "id_to_replace";
        static string[] dbtp = { "int", "bigint", "nchar", "nvarchar", "datetime", "xml", "decimal" };
        static string basePath = @"C:\website\JCMS2\jamiuaz-cms\src\";

        private static bool createFiles = true;
        private readonly IHostingEnvironment _appEnvironment;

        public CodeController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }
        public IActionResult Index()
        {

            //}

            //GetController();
            //Ichildrepo_Gen();
            //childrepo_Gen();
            //Ichildservice_Gen();
            //childservice_Gen();

            //utility_one();

            //view_crud();

            //view_Index();
            //view_Create();
            //view_Edit();
            var result = getGrid();

            return View(result);
        }

        public IActionResult Create()
        {
            return View();
        }

        public void utility_one()
        {
            StringBuilder sb = new StringBuilder();
            string[] thestr = System.IO.File.ReadAllLines(@"C:\website\JCMS\src\JCMS\Controllers\Admin\crud.txt");
            foreach (var t in thestr)
            {
                sb.AppendFormat("\n sb.Append($\"#{0}\");", t);
            }
            System.IO.File.WriteAllText(@"C:\website\JCMS\src\JCMS\Controllers\Admin\crud1.txt", sb.ToString());
        }

        public void view_crud(string lib, string layer, string entity)
        {
            var grid = getGrid();
            var libs = grid.arrGenLib;

            foreach (var l in libs)
            {
                foreach (var ent in l.genEntity)
                {

                }
            }
        }
        public static void Controller_Gen()
        {
            string path = string.Empty;

            //foreach (var y in arrEntity)
            //{

            //}
        }
        public static void Ichildrepo_Gen(string lib, string layer, string entity)
        {

        }
        public static void childrepo_Gen(string lib, string layer, string entity)
        {

        }
        public static void Ichildservice_Gen(string lib, string layer, string entity)
        {

        }
        public static void childservice_Gen(string lib, string layer, string entity)
        {

        }
        public static string SCase(string input)
        {
            string output = string.Empty;

            //if(input.Contains("_"))
            //{
            string[] inputs = input.Split('_');
            string sentence1 = inputs[0].ToLower();
            output += sentence1[0].ToString().ToUpper() + sentence1.Substring(1);

            output += "_";

            string sentence2 = inputs[1].ToLower();
            output += sentence2[0].ToString().ToUpper() + sentence2.Substring(1);
            //}
            //else
            //{

            //}
            //string sentence = input.ToLower();
            //return sentence[0].ToString().ToUpper() + sentence.Substring(1);
            return output;
        }

        #region Template
        public static void sChildrepo_Gen(string lib, string layer, string entity)
        {
            string s = entity;
            StringBuilder sb = new StringBuilder();
            sb.Append(" using System;");
            sb.Append("\n using System.Collections.Generic;");
            sb.Append("\n using System.Linq;");
            sb.Append("\n using System.Data.Entity;");
            sb.Append("\n using JCMS.Entity;");
            sb.Append("\n using JCMS.User.IRepo;");
            sb.Append("\n using JCMS.Base.Repo;");
            sb.Append("\n ");
            sb.Append("\n namespace JCMS.User.Repo");
            sb.Append("\n {");
            sb.Append("\n     public class " + SCase(s) + "_Repo : Base_Repo<" + s + ">, I" + SCase(s) + "_Repo");
            sb.Append("\n     {");
            sb.Append("\n         public " + SCase(s) + "_Repo(IJDbContext context) : base(context)");
            sb.Append("\n         {");
            sb.Append("\n ");
            sb.Append("\n         }");
            sb.Append("\n     }");
            sb.Append("\n }");

            string dr = basePath + layer;
            if (!System.IO.Directory.Exists(dr)) System.IO.Directory.CreateDirectory(dr);
            string path = dr + SCase(s) + "_Repo.cs";
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            System.IO.File.AppendAllText(path, sb.ToString());
        }
        public static void sIchildservice_Gen(string lib, string layer, string entity)
        {
            string s = entity;
            StringBuilder sb = new StringBuilder();
            sb.Append("using JCMS.Entity;");
            sb.Append("\n using JCMS.Base.IService;");
            sb.Append("\n ");
            sb.Append("\n namespace JCMS.User.IService");
            sb.Append("\n {");
            sb.Append("\n     public interface I" + SCase(s) + "_Service : IBase_Service<" + s + ">");
            sb.Append("\n     {");
            sb.Append("\n ");
            sb.Append("\n     }");
            sb.Append("\n }");

            string dr = basePath + layer;
            if (!System.IO.Directory.Exists(dr)) System.IO.Directory.CreateDirectory(dr);
            string path = dr + "I" + SCase(s) + "_Service.cs";
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            System.IO.File.AppendAllText(path, sb.ToString());
        }
        public static void sChildservice_Gen(string lib, string layer, string entity)
        {
            string s = entity;
            StringBuilder sb = new StringBuilder();
            sb.Append(" using System;");
            sb.Append("\n using System.Collections.Generic;");
            sb.Append("\n using System.Linq;");
            sb.Append("\n using JCMS.Entity;");
            sb.Append("\n using JCMS.User.IService;");
            sb.Append("\n using JCMS.User.Repo;");
            sb.Append("\n using JCMS.User.IRepo;");
            sb.Append("\n using JCMS.Base.Service;");
            sb.Append("\n ");
            sb.Append("\n namespace JCMS.User.Service");
            sb.Append("\n {");
            sb.Append("\n     public class " + SCase(s) + "_Service : Base_Service<" + s + ">, I" + SCase(s) + "_Service");
            sb.Append("\n     {");
            sb.Append("\n         private I" + SCase(s) + "_Repo _gloRepo;");
            sb.Append("\n ");
            sb.Append("\n         public " + SCase(s) + "_Service(IBase_Repo<" + s + "> repo, I" + SCase(s) + "_Repo gloRepo) : base(repo)");
            sb.Append("\n         {");
            sb.Append("\n             _gloRepo = gloRepo;");
            sb.Append("\n         }");
            sb.Append("\n     }");
            sb.Append("\n }");

            string dr = basePath + layer;
            if (!System.IO.Directory.Exists(dr)) System.IO.Directory.CreateDirectory(dr);
            string path = dr + SCase(s) + "_Service.cs";
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            System.IO.File.AppendAllText(path, sb.ToString());

        }
        public static void sControllerGen(string lib, string layer, string entity)
        {
            string sl = string.Empty, zl = string.Empty;
            string ss = string.Empty, zs = string.Empty;

            string y = entity;
            zs = y.Split('_')[0];
            zl = zs[0].ToString().ToUpper() + zs.Substring(1);

            ss = y.Split('_')[1];
            sl = ss[0].ToString().ToUpper() + ss.Substring(1);


            StringBuilder sb = new StringBuilder();
            sb.Append("\n using System.Linq;");
            sb.Append("\n using Microsoft.AspNet.Mvc;");
            sb.Append("\n using Microsoft.AspNet.Authorization;");
            sb.Append($"\n using JCMS.{zl.Trim()}.IService;");
            sb.Append("\n using JCMS.Entity;");
            sb.Append("\n ");
            sb.Append("\n namespace JCMS.CMS.Controllers");
            sb.Append("\n     {");
            sb.Append("\n         [Authorize(Roles = \"Administrator, Super Administrator\")]");
            sb.Append($"\n        public class {sl}Controller : Controller");
            sb.Append("\n         {");
            sb.Append($"\n             private I{SCase(y)}_Service _{ss}_service;");
            sb.Append($"\n             public {sl}Controller(I{SCase(y)}_Service {ss}_service)");
            sb.Append("\n             {");
            sb.Append($"\n                 _{ss}_service = {ss}_service;");
            sb.Append("\n             }");
            sb.Append("\n             public ViewResult Index()");
            sb.Append("\n             {");
            sb.Append("\n                 return View();");
            sb.Append("\n             }");
            sb.Append("\n ");
            sb.Append("\n             public JsonResult GetAll()");
            sb.Append("\n             {");
            sb.Append($"\n                 var result = _{ss}_service.GetAll().ToList();");
            sb.Append("\n                 return Json(result);");
            sb.Append("\n             }");
            sb.Append("\n ");
            sb.Append("\n            public JsonResult Get(int id)");
            sb.Append("\n             {");
            sb.Append($"\n                var result = _{ss}_service.Get(id);");
            sb.Append("\n                 return Json(result);");
            sb.Append("\n             }");
            sb.Append("\n ");
            sb.Append("\n             [HttpPost]");
            sb.Append($"\n             public JsonResult create({y} {y})");
            sb.Append("\n             {");
            sb.Append("\n                 try");
            sb.Append("\n                 {");
            sb.Append($"\n                     _{ss}_service.Add({y});");
            sb.Append("\n                    return Json(\"Successful\");");
            sb.Append("\n                 }");
            sb.Append("\n                 catch");
            sb.Append("\n                {");
            sb.Append("\n                     return Json(\"fail\");");
            sb.Append("\n                 }");
            sb.Append("\n             }");
            sb.Append("\n ");
            sb.Append("\n ");
            sb.Append("\n            [HttpPost]");
            sb.Append($"\n            public JsonResult update({y} {y})");
            sb.Append("\n             {");
            sb.Append("\n                 try");
            sb.Append("\n                 {");
            sb.Append($"\n                     _{ss}_service.Update({y});");
            sb.Append("\n                     return Json(\"Successful\");");
            sb.Append("\n                 }");
            sb.Append("\n                 catch");
            sb.Append("\n                 {");
            sb.Append("\n                     return Json(\"fail\");");
            sb.Append("\n                 }");
            sb.Append("\n             }");
            sb.Append("\n ");
            sb.Append($"\n            public JsonResult destroy({y} {y})");
            sb.Append("\n             {");
            sb.Append("\n                 try");
            sb.Append("\n                 {");
            sb.Append($"\n                     _{ss}_service.Delete({y});");
            sb.Append("\n                    return Json(\"Successful\");");
            sb.Append("\n                 }");
            sb.Append("\n                 catch");
            sb.Append("\n                 {");
            sb.Append("\n                     return Json(\"fail\");");
            sb.Append("\n                 }");
            sb.Append("\n             }");
            sb.Append("\n         }");
            sb.Append("\n     }");

            string dr = basePath + layer;
            if (!System.IO.Directory.Exists(dr)) System.IO.Directory.CreateDirectory(dr);
            string path = dr + sl + "Controller.cs";
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            System.IO.File.AppendAllText(path, sb.ToString());

        }
        public static void sViewCrud(string lib, string layer, string entity)
        {

            string sl = string.Empty, zl = string.Empty;
            string ss = string.Empty, zs = string.Empty;

            string y = entity;
            zs = y.Split('_')[0];
            zl = zs[0].ToString().ToUpper() + zs.Substring(1);

            ss = y.Split('_')[1];
            sl = ss[0].ToString().ToUpper() + ss.Substring(1);

            string mmid = string.Empty;
            switch (y)
            {
                case "user_donation": mmid = "donationid"; break;
                case "user_office": mmid = "officeid"; break;
                case "user_mailing_list": mmid = "id"; break;
                case "user_payment": mmid = "paymentid"; break;
                case "user_profile": mmid = "UserId"; break;
                case "user_type": mmid = "RoleId"; break;
                case "web_setting": mmid = "configid"; break;
                case "web_templates": mmid = "templateid"; break;
                case "web_partial": mmid = "partialid"; break;
                case "web_banners": mmid = "bannerid"; break;
                case "content_monthayah": mmid = "ayahid"; break;
                case "content_solat": mmid = "solatid"; break;
                case "content_news": mmid = "newsid"; break;
                case "content_article": mmid = "articleid"; break;
                case "content_pages": mmid = "pageid"; break;
                case "media_images": mmid = "imageid"; break;
                case "media_documents": mmid = "documentid"; break;
                case "media_videos": mmid = "videoid"; break;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"\n @using JCMS.Config.Service");
            sb.Append($"\n @##");
            sb.Append($"\n     ViewBag.Title = \"Index\";");
            sb.Append($"\n     Layout = \"~/Views/Shared/_LayoutAdmin.cshtml\";");
            sb.Append($"\n ###");
            sb.Append($"\n ");
            sb.Append($"\n <h2>Page</h2>");
            sb.Append($"\n <div>");
            sb.Append($"\n     <a asp-action=\"Create\" class=\"btn btn-default\">Add New Page</a>");
            sb.Append($"\n </div>");
            sb.Append($"\n ");
            sb.Append($"\n <div class=\"{ss}-top\" data-siteurl=\"@Web_Setting_Service.website.ToString()\">");
            sb.Append($"\n     <div id=\"{ss}-grid\"></div>");
            sb.Append($"\n </div>");
            sb.Append($"\n ");
            sb.Append($"\n @section scripts##");
            sb.Append($"\n ");
            sb.Append($"\n     <script>");
            sb.Append($"\n         var siteurl;");
            sb.Append($"\n         $(document).ready(function () ##");
            sb.Append($"\n             siteurl = $(\".{ss}-top\").data(\"siteurl\")");
            sb.Append($"\n             var dataSource = new kendo.data.DataSource(##");
            sb.Append($"\n                 transport: ##");
            sb.Append($"\n                     read: function (options) ##");
            sb.Append($"\n                         $.ajax(##");
            sb.Append($"\n                             url: siteurl + \"{sl}/GetAll\",");
            sb.Append($"\n                             dataType: \"json\",");
            sb.Append($"\n                             type: 'GET',");
            sb.Append($"\n                             success: function (result) ##");
            sb.Append($"\n                                 options.success(result);");
            sb.Append($"\n                             ###,");
            sb.Append($"\n                             error: function (error) ##");
            sb.Append($"\n                                 options.error(error);");
            sb.Append($"\n                             ###");
            sb.Append($"\n                         ###);");
            sb.Append($"\n                     ###,");
            sb.Append($"\n                     create: function (options) ##");
            sb.Append($"\n                         $.ajax(##");
            sb.Append($"\n                             url: siteurl + \"{sl}/create\",");
            sb.Append($"\n                             dataType: \"json\",");
            sb.Append($"\n                             type: 'POST',");
            sb.Append($"\n                             data: ##");

            string[] mientity = System.IO.File.ReadAllLines(@"C:\website\JCMS\src\JEntity\" + y + ".cs");

            for (var me = 0; me < mientity.Length; me++)
            {
                if (mientity[me].Contains("{ get; set; }") && !mientity[me].Contains("virtual ICollection"))
                {
                    string met = mientity[me].Split(' ')[10];

                    if (me < (mientity.Length - 1))
                    {
                        sb.Append($"                                {met}: options.data.{met},");
                    }
                    else if (me == (mientity.Length - 1))
                    {
                        sb.Append($"                                {met}: options.data.{met}");
                    }
                }
            }

            sb.Append($"\n                             ###,");
            sb.Append($"\n                             success: function (result) ##");
            sb.Append($"\n                                 options.success(result);");
            sb.Append($"\n                             ###,");
            sb.Append($"\n                             error: function (error) ##");
            sb.Append($"\n                                 options.success(error);");
            sb.Append($"\n                             ###");
            sb.Append($"\n                         ###);");
            sb.Append($"\n                     ###,");
            sb.Append($"\n                     update: function (options) ##");
            sb.Append($"\n                         $.ajax(##");
            sb.Append($"\n                             url: siteurl + \"{sl}/update\",");
            sb.Append($"\n                             dataType: \"json\",");
            sb.Append($"\n                             type: 'POST',");
            sb.Append($"\n                             data: ##");
            for (var me = 0; me < mientity.Length; me++)
            {
                if (mientity[me].Contains("{ get; set; }") && !mientity[me].Contains("virtual ICollection"))
                {
                    string met = mientity[me].Split(' ')[10];

                    if (me < (mientity.Length - 1))
                    {
                        sb.Append($"                                {met}: options.data.{met},");
                    }
                    else if (me == (mientity.Length - 1))
                    {
                        sb.Append($"                                {met}: options.data.{met}");
                    }
                }
            }
            sb.Append($"\n                             ###,");
            sb.Append($"\n                             success: function (result) ##");
            sb.Append($"\n                                 options.success(result);");
            sb.Append($"\n                             ###,");
            sb.Append($"\n                             error: function (error) ##");
            sb.Append($"\n                                 options.error(error);");
            sb.Append($"\n                             ###");
            sb.Append($"\n                         ###);");
            sb.Append($"\n                     ###,");
            sb.Append($"\n                     destroy: function (options) ##");
            sb.Append($"\n                         $.ajax(##");
            sb.Append($"\n                             url: siteurl + \"{sl}/destroy\",");
            sb.Append($"\n                             dataType: \"json\",");
            sb.Append($"\n                             type: 'POST',");
            sb.Append($"\n                             data: ##");
            for (var me = 0; me < mientity.Length; me++)
            {
                if (mientity[me].Contains("{ get; set; }") && !mientity[me].Contains("virtual ICollection"))
                {
                    string met = mientity[me].Split(' ')[10];

                    if (me < (mientity.Length - 1))
                    {
                        sb.Append($"                                {met}: options.data.{met},");
                    }
                    else if (me == (mientity.Length - 1))
                    {
                        sb.Append($"                                {met}: options.data.{met}");
                    }
                }
            }
            sb.Append($"\n                             ###,");
            sb.Append($"\n                             success: function (result) ##");
            sb.Append($"\n                                 options.success(result);");
            sb.Append($"\n                             ###,");
            sb.Append($"\n                             error: function (error) ##");
            sb.Append($"\n                                 options.error(error);");
            sb.Append($"\n                             ###");
            sb.Append($"\n                         ###);");
            sb.Append($"\n                     ###");
            sb.Append($"\n                 ###,");
            sb.Append($"\n                 error: function (options) ##");
            sb.Append($"\n                     //alert(\"Status: \" + options.status + \"; Error message: \" + options.errorThrown);");
            sb.Append($"\n                 ###,");
            sb.Append($"\n                 pageSize: 10,");
            sb.Append($"\n                 schema: ##");
            sb.Append($"\n                     model: ##");
            sb.Append($"\n                         id: \"{mmid}\",");
            sb.Append($"\n                         fields: ##");

            //sb.Append($"\n                             ConfigName: ## type: \"string\" ###,");
            //sb.Append($"\n                             ConfigValue: ## type: \"string\" ###");
            foreach (var me in mientity)
            {
                if (me.Contains("{ get; set; }") && !me.Contains("virtual ICollection"))
                {
                    string met = me.Split(' ')[10];
                    string tp = me.Split(' ')[9].Replace("System.", "").Replace("Nullable<", "").Replace(">", "");
                    string ltp = string.Empty;
                    switch (tp)
                    {
                        case "int": ltp = "number"; break;
                        case "string": ltp = "string"; break;
                        case "DateTime": ltp = "date"; break;
                        case "bool": ltp = "bool"; break;
                        case "object": ltp = ""; break;
                        case "byte[]": ltp = ""; break;
                        case "TimeSpan": ltp = "date"; break;
                        case "decimal": ltp = "number"; break;

                    }
                    sb.Append($"\n                             {met}: ## type: \"{ltp}\" ###,");
                }
            }

            sb.Append($"\n                         ###");
            sb.Append($"\n                     ###");
            sb.Append($"\n                 ###");
            sb.Append($"\n             ###);");
            sb.Append($"\n ");
            sb.Append($"\n             var jgrid = $(\"#{ss}-grid\").kendoGrid(##");
            sb.Append($"\n                 dataSource: dataSource,");
            sb.Append($"\n                 pageable: true,");
            sb.Append($"\n                 height: 550,");
            sb.Append($"\n                 filterable: ##");
            sb.Append($"\n                     extra: false,");
            sb.Append($"\n                     operators: ##");
            sb.Append($"\n                         string: ##");
            sb.Append($"\n                             startswith: \"Starts with\",");
            sb.Append($"\n                             eq: \"Is equal to\",");
            sb.Append($"\n                             neq: \"Is not equal to\"");
            sb.Append($"\n                         ###");
            sb.Append($"\n                     ###");
            sb.Append($"\n                 ###,");
            sb.Append($"\n                 columns: [");

            //sb.Append($"\n                     ## field: \"ConfigName\", title: \"Key\" ###,");
            //sb.Append($"\n                     ## field: \"ConfigValue\", title: \"Value\" ###,");

            foreach (var me in mientity)
            {
                if (me.Contains("{ get; set; }") && !me.Contains("virtual ICollection"))
                {
                    string met = me.Split(' ')[10];
                    sb.Append($"\n                     ## field: \"{met}\", title: \"{met}\" ###, ");
                }
            }

            sb.Append($"\n                     ## command: [\"edit\", \"destroy\"], title: \"&nbsp;\" ###");
            sb.Append($"\n ");
            sb.Append($"\n                 ],");
            sb.Append($"\n                 editable: \"popup\",");
            sb.Append($"\n                 edit: function (e) ##");
            sb.Append($"\n                     if (!e.model.isNew()) ##");
            sb.Append($"\n                         e.container.find(\"input[name={mmid}]\").attr('disabled', true);");
            sb.Append($"\n                     ###");
            sb.Append($"\n                 ###");
            sb.Append($"\n             ###).data(\"kendoGrid\");");
            sb.Append($"\n ");
            sb.Append($"\n         ###);");
            sb.Append($"\n ");
            sb.Append($"\n         function dataBound() ##");
            sb.Append($"\n             this.table.find(\".k-grid-edit\").hide();");
            sb.Append($"\n         ###");
            sb.Append($"\n ");
            sb.Append($"\n     </script>");
            sb.Append($"\n ###");
            sb.Append($"\n ");

            string dr = basePath + layer + sl + @"\";
            if (!System.IO.Directory.Exists(dr)) System.IO.Directory.CreateDirectory(dr);
            string path = dr + "Index.cshtml";
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            System.IO.File.AppendAllText(path, sb.ToString().Replace("###", "}").Replace("##", "{"));
        }
        #endregion

        public gridLibModel getGrid()
        {
            var arrGenL = new Dictionary<string, string>();
            arrGenL.Add("irepo", @"IRepo\");
            arrGenL.Add("repo", @"Repo\");
            arrGenL.Add("iservice", @"IService\");
            arrGenL.Add("service", @"Service\");
            arrGenL.Add("controller", @"Controllers\");
            arrGenL.Add("view", @"View\");

            gridLibModel grid = new gridLibModel
            {
                arrGenLib = new genLibModel[]
                  {
                        new genLibModel { genLib = @"JCMS.Audit\", genEntity = new string[]{ } },
                        new genLibModel { genLib = @"JCMS.Config\", genEntity = new string[] { } },
                        new genLibModel { genLib = @"JCMS.Content\", genEntity = new string[] { "web_setting", "web_templates", "web_partial", "web_banners", "content_monthayah", "content_solat", "content_news", "content_article", "content_pages" } },
                        new genLibModel { genLib = @"JCMS.Events\", genEntity = new string[] { } },
                        new genLibModel { genLib = @"JCMS.Notification\", genEntity = new string[] { } },
                        new genLibModel { genLib = @"JCMS.User\", genEntity = new string[] { "user_donation", "user_office", "user_mailing_list", "user_payment", "user_profile", "user_type" } },
                        new genLibModel { genLib = @"JCMS.Web\", genEntity = new string[] { } },
                        new genLibModel { genLib = @"JCSM.Media\", genEntity = new string[] { "media_images", "media_documents", "media_videos" } },
                 },
                arrGenLayer = arrGenL
            };

            return grid;
        }
    }

    public class gridLibModel
    {
        public genLibModel[] arrGenLib { get; set; }
        public Dictionary<string, string> arrGenLayer { get; set; }
    }

    public class genLibModel
    {
        public string genLib { get; set; }
        public string[] genEntity { get; set; }
    }
}
