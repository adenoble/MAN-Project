using mmsv4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;
using System.IO;
using System.Net.Mail;

namespace mmsv4.Controllers
{
    public class HomeController : Controller
    {
        mmsv4Entities db = new mmsv4Entities();

        public ActionResult Index()
        {
            return View();
        }

        public string Mypath()
        {
            var pt = Server.MapPath("~/EmailTemp");
            return pt;
        }

        public ActionResult Default(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            content_pages pg = db.content_pages.Find(id);
            if (pg == null)
            {
                return HttpNotFound();
            }

            //ViewBag.Template = db.web_templates.Find(pg.web_templates);
            if (pg.partialid.HasValue)
            {
                var partial = db.web_partial.Find(pg.partialid);
                ViewBag.Partial = partial.partial;
            }

            //var img = db.media_images.Find(pg.imageid);
            return View(pg);
        }

        public ActionResult Ayah(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            content_monthayah content_monthayah = db.content_monthayah.Find(id);
            if (content_monthayah == null)
            {
                return HttpNotFound();
            }
            return View(content_monthayah);
        }

        public ActionResult Article(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            content_article content_article = db.content_article.Find(id);
            if (content_article == null)
            {
                return HttpNotFound();
            }
            return View(content_article);
        }

        public ActionResult News(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            content_news content_news = db.content_news.Find(id);
            if (content_news == null)
            {
                return HttpNotFound();
            }
            return View(content_news);
        }

        public ActionResult Contact()
        {
            content_pages pg = db.content_pages.Find(17);
            if (pg == null)
            {
                return HttpNotFound();
            }

            //ViewBag.Template = db.web_templates.Find(pg.web_templates);
            if (pg.partialid.HasValue)
            {
                var partial = db.web_partial.Find(pg.partialid);
                ViewBag.Partial = partial.partial;
            }

            //var img = db.media_images.Find(pg.imageid);
            return View(pg);
        }

        public ActionResult DonateOnline()
        {
            return View();
        }

        public ActionResult Conference()
        {
            return View();
        }

        public ActionResult Conference_Declaration()
        {
            return View();
        }

        [HttpPost]
        public string _contactForm(string mfor, string name, string tel, string email, string subject, string message)
        {
            string err = string.Empty;
            string strmfor = string.Empty;
            try
            {
                switch (Convert.ToInt32(mfor))
                {
                    case 1:
                        strmfor = "info@manuk.org";
                        break;
                    case 2:
                        strmfor = "imam@manuk.org";
                        break;
                    case 3:
                        strmfor = "Admin@markaz.manuk.org";
                        break;
                    case 4:
                        strmfor = "mosquesappeal@manuk.org";
                        break;
                }
                Email.SendConfirmation(email, name, strmfor, "Contact", message, true);
                Email.SendManuk(email, name, strmfor, subject, message, true);

                return "<p>Hello " + name + "</p><p> We will get back to you as soon as possible.</p><p>Thank you.</p>";
            }
            catch (Exception ex)
            {
                return "<p>Thank you " + name + ".</p> <p>An Error occur, the message has not been sent<br/>Please try again.</p>" + ex.ToString();
            }
        }

        [HttpPost]
        public string _EmailSubscribeForm(string pName, string pEmail, string pTel)
        {
            string err = string.Empty;

            try
            {
                var suscriber = db.UpdateLists.Where(e => e.email == pEmail);
                if (suscriber == null || suscriber.Count() == 0)
                {
                    var lst = new UpdateList { fullname = pName, email = pEmail, telephone = pTel, subscribe = true, datecreated = DateTime.Now };
                    db.UpdateLists.Add(lst);
                    db.SaveChanges();
                    return "<p>Thanks for subscribing to our mailing list.</p>";
                }
                else
                {
                    var lst = db.UpdateLists.Find(suscriber.FirstOrDefault().id);
                    lst.fullname = pName;
                    lst.telephone = pTel;
                    lst.datemodified = DateTime.Now;
                    db.Entry(lst).State = EntityState.Modified;
                    db.SaveChanges();
                    return "<p>Email already subscribe. Thanks for updating your details.</p>";
                }
            }
            catch (Exception ex)
            {
                return ":( Error as occurred" + ex.Message.ToString();
            }
        }

        [HttpPost]
        public string _ConferenceForm(string pName, string pEmail, string pTel, string pAttendees)
        {
            string err = string.Empty;

            int intAttendee = 0;
            if (!string.IsNullOrWhiteSpace(pAttendees))
            {
                intAttendee = Convert.ToInt32(pAttendees);
            }
            else
            {
                intAttendee = 1;
            }

            try
            {
                var suscriber = db.EventLists.Where(e => e.email == pEmail).FirstOrDefault();
                if (suscriber == null || string.IsNullOrWhiteSpace(suscriber.email))
                {
                    var lst = new EventList { fullname = pName, email = pEmail, telephone = pTel, attendees = intAttendee, attendRef = Generate_Reg_No(), datecreated = DateTime.Now };
                    db.EventLists.Add(lst);
                    db.SaveChanges();

                    Send_Conf_Confirm_Email(lst.fullname, lst.email, lst.attendRef);

                    return "<p>Thank you.</p><p>Your registration was successfull. "
                        + "Your Register Number is : <strong>" + lst.attendRef + "</strong>.</p>"
                        + "<p>Confirmation has been sent to you Email: " + lst.email + "</p>";
                }
                else
                {
                    var lst = db.EventLists.Find(suscriber.id);
                    lst.fullname = pName;
                    lst.telephone = pTel;
                    lst.attendees = intAttendee;
                    lst.datemodified = DateTime.Now;
                    db.Entry(lst).State = EntityState.Modified;
                    db.SaveChanges();

                    Send_Conf_Confirm_Email(lst.fullname, lst.email, lst.attendRef);

                    return "<p>Thank you.</p><p>Your registration update was successfull. "
                        + "Your Register Number is : <strong>" + lst.attendRef + "</strong>.</p>"
                        + "<p>Confirmation has been sent to you Email: " + lst.email + "</p>";
                }
            }
            catch (Exception ex)
            {
                return ":( Error as occurred" + ex.Message.ToString();
            }
        }

        public ActionResult EmailUnSubscribe(int? pId)
        {
            string email = string.Empty;
            if (pId != null)
            {
                var lst = db.UpdateLists.Find(pId);
                email = lst.email;
            }
            ViewBag.Email = email; ;

            return View();
        }

        [HttpPost]
        public string _EmailUnSubscribeForm(int pId, string pEmail, bool pSubscribe)
        {
            string err = string.Empty;
            try
            {
                var lst = db.UpdateLists.Find(pId);
                lst.subscribe = false;
                lst.datemodified = DateTime.Now;
                db.Entry(lst).State = EntityState.Modified;
                db.SaveChanges();

                return "<p>Sorry to see you leaving!<br/>You have now been unsubscribe.<br/>You will receive no further update from us.<br/><br/>Thanks.</p>";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        [ChildActionOnly]
        public ActionResult _RelatedVideos(string videoId)
        {
            var rltVideos = db.media_videos.Where(x => ArrayClass.csvToArrayInt(videoId).Contains(x.videoid));
            return PartialView(rltVideos.ToList());
        }

        [ChildActionOnly]
        public ActionResult _RelatedDocuments(string docIds)
        {
            var rltDocs = db.media_documents.Where(x => ArrayClass.csvToArrayInt(docIds).Contains(x.documentid));
            return PartialView(rltDocs.ToList());
        }

        private static string Generate_Reg_No()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return string.Format("MANUK-{0:D8}", random);
        }

        public string[] attendes()
        {
            string[] arrAttendees = {   "Muslima Adelani #muslimadelan@yahoo.co.uk",
                                        "Muhsinat Kamardeen#kamlad4real@yahoo.com",
                                        "Fadeelah Yaqub#fadeelahyaqub@yahoo.com",
                                        "Adam Fauziah Ada#ada.adam89@yahoo.com",
                                        "Adam Abdullah Idoko#imamcentral@yahoo.com",
                                        "Muyinat Opeolu #tolaopeolu@aol.com",
                                        "Monsura Aduragba Abiodun Oyenuga #adua12345@hotmail.com",
                                        "Aba#optimal11@yahoo.com",
                                        "Sister Ajarat Oloba #aoloba@yahoo.co.uk",
                                        "Mariam Moshood#alajoke@aol.com",
                                        "ibrahim Oyedele#brightbidex2003@yahoo.co.uk",
                                        "Mustapha Akande#mbakande@hotmail.com",
                                        "mudirakat o ogun#mudiratogun@yahoo.com",
                                        "Latifa Yusuf - Eleja#latifayusufeleja@yahoo.co.uk",
                                        "Wasiu#wadekoya@yahoo.com",
                                        "Olaide Kafilat Adekoya#kaffyoo@yahoo.com",
                                        "modinat gbadamosi#adedolapogbadamosi@yahoo.co.uk",
                                        "Basirat Salaam#motun_bass@yahoo.com",
                                        "shakirat Alli #adebukolabakare@yahoo.com",
                                        "Sulaiman Quadri#quadri111@yahoo.com",
                                        "Ms Bilikis Omotayo#bomotayo@hotmail.com",
                                        "alabibola@ymail.com#alabibola@ymail.com",
                                        "Muibat Ajani #muibat_a@hotmail.com",
                                        "Monsur Emiabata#memiabata@yahoo.com",
                                        "Kamal deen Sadiku #tawiobanuso@gmail.com",
                                        "Titi Bakare#ytiamiyu@gmail.com",
                                        "Abdurazaq Adeyemi#adeyemi69@yahoo.com",
                                        "Saka Sulaiman #sakasulaiman@gmail.com",
                                        "Aminat Ayoola#ammeennaah@yahoo.co.uk",
                                        "Raheemah Animashaun#abeessy@yahoo.com",
                                        "Lukman igbalaiye#mailukman1000@yahoo.com",
                                        "Hadiza suleiman #marliyya@yahoo.com",
                                        "Zayneb Benyoucef#jentacos@hotmail.com",
                                        "Abdul-Yekeen Keshiro#ade_keshy2006@yahoo.co.uk",
                                        "Alhaja Taibat Bisiriyu#amabat3@gmail.com",
                                        "Yusuf Assan#offanna@yahoo.com",
                                        "Isiaka Owolabi#olaowolabi@yahoo.com",
                                        "Nasir Okeowo#dapobaba@gmail.com",
                                        "Ozlem OLGUN KIYICI#oz_ok@yahoo.co.uk",
                                        "Kayode QUADRI#kayquadri@gmail.com",
                                        "T U Uthman#tauthman@yahoo.co.uk",
                                        "Muftau Akinwale#muftak2@gmail.com",
                                        "O. Onatade#alldatstrak@googlemail.com",
                                        "BM  Oketunde #mutiu22@hotmail.co.uk",
                                        "abdul kadir sogbanmu#ksogbanmu@gmail.com",
                                        "Misturah Muhammad#mojihassan5@gmail.com",
                                        "Oleolo Muhammad#amoleolo@outlook.com",
                                        "Sirajah Habeebu#sirajah.habeebu@gmail.com",
                                        "Luqman Abolaji#luqmanabolaji@yahoo.com",
                                        "Olatunde Sanusi#latrasglo@yahoo.co.uk",
                                        "Fatai Popoola#fopopson2002@yahoo.co.uk",
                                        "alhaja layet#sajork@yahoo.com",
                                        "Falilat Mohammed-Akanbi#falilatmohammedakanbi@gmail.com",
                                        "Mohammad Ajibola Daodu#b.daodu@sky.com",
                                        "Amudat Adesina #amudatade@aol.com",
                                        "Kaosara Yesufu#kaosarayesufu@yahoo.co.uk",
                                        "Sariat Adediwura#sariatadediwura@yahoo.co.uk",
                                        "Abraham Atunrase#abratun@aol.com",
                                        "KhadijatTemitope Durosinmi-Etti#ettitk@gmail.com"};
            return arrAttendees;
        }

        public string[] embassy()
        {
            string[] arrEmbasy = { 
                                     "jamiuaz@yahoo.co.uk",
                                     "ayindej@uk-innovation.group.com",
                        "info@afghanistanembassy.org.uk", 
                        "embassy.london@mfa.gov.al", 
                        "info@algerianembassy.org.uk", 
                        "andorra.embassyuk@btopenworld.com", 
                        "embassy@angola.org.uk", 
                        "eruni@cancilleria.gob.ar", 
                        "clond@cancilleria.gob.ar", 
                        "armemb@armenianembassyuk.com", 
                        "london-ob@bmeia.gv.at", 
                        "aneum@talk21.com", 
                        "austrianconsulate@focusscotland.co.uk", 
                        "london@mission.mfa.gov.az", 
                        "information@bahamashclondon.net", 
                        "information@bahrainembassy.co.uk", 
                        "info@bhclondon.org.uk", 
                        "london@foreign.gov.bb", 
                        "uk.london@mfa.gov.by", 
                        "ryebelarusconsul@gmail.com", 
                        "London@diplobel.fed.be", 
                        "vdv@ntlworld.com", 
                        "pimossi@smith-imossi.gi", 
                        "consulbelgiumscotland@gmail.com", 
                        "petergreen@imcsuk.biz", 
                        "alan.binnington@rbc.com", 
                        "david.bradshaw@hay-kilner.co.uk", 
                        "john@jmeeus.co.uk", 
                        "consulbelgium@blcc.co.uk", 
                        "bzhc-lon@btconnect.com", 
                        "mrutland@aol.com", 
                        "embassy@bhembassy.co.uk", 
                        "infolondres@brazil.org.uk", 
                        "london.uk@mfa.gov.bn", 
                        "Embassy.London@mfa.bg", 
                        "info@burundiembassy.org.uk", 
                        "camemb.eng@mfa.gov.kh",
                        "cambodianembassy@btconnect.com", 
                        "info@cameroonhighcommission.co.uk", 
                        "ldn@international.gc.ca", 
                        "canada.consul@burness.co.uk", 
                        "honconcanbelfast@yahoo.co.uk", 
                        "dan.clayton-jones@talk21.com", 
                        "embachile@embachile.co.uk", 
                        "chinaconsul_eb_uk@mfa.gov.cn", 
                        "political@chinese-embassy.org.uk", 
                        "chinaconsul_man_uk@mfa.gov.cn", 
                        "elondres@cancilleria.gov.co", 
                        "costarica@btconnect.com", 
                        "croemb.london@mvpei.hr", 
                        "embacuba@cubaldn.com", 
                        "cyphclondon@dial.pipex.com", 
                        "cyprusleeds@gmail.com", 
                        "london@embassy.mzv.cz", 
                        "Edinburgh@honorary.mzv.cz", 
                        "Cardiff@honorary.mzv.cz", 
                        "belfast@honorary.mzv.cz", 
                        "info@czechcentre.org.uk", 
                        "lonamb@um.dk", 
                        "embassy@dominicanembassy.org.uk", 
                        "eecugranbretania@mmrree.gov.ec", 
                        "ceculondres@mmrree.gov.ec", 
                        "consulate.london@mfa.gov.eg", 
                        "embasalondres@netscapeonline.co.uk", 
                        "london@mfa.ee", 
                        "jpb@bpe.co.uk", 
                        "rogerjones.battle@btinternet.com", 
                        "info@ethioembassy.org.uk", 
                        "mail@fijihighcommission.org.uk", 
                        "sanomat.lon@formin.fi", 
                        "kate.crosby@raeburns.co.uk", 
                        "jbdavis@tiscali.co.uk", 
                        "geoff.parsons@virgin.net", 
                        "julian.phillips@judiciary.gsi.gov.uk", 
                        "jamesryeland@georgehammond.plc.uk", 
                        "finnish.consulate@dpandl.co.uk", 
                        "michaelwalker@dial.pipex.com", 
                        "arturo@capurro.gi", 
                        "t.brucejones@jamesjones.co.uk", 
                        "jriihiluoma@applebyglobal.com", 
                        "nigelpryke7@hotmail.com", 
                        "finnishconsulate@johngood.co.uk", 
                        "alvolam@hotmail.co.uk", 
                        "anything@tighnacraig.shetland.co.uk", 
                        "bgill01@btconnect.com", 
                        "chris.rostron@ntlworld.com", 
                        "alan.hogg@obcgroup.com", 
                        "john.walton39@btinternet.com", 
                        "stephen.bolton2@btinternet", 
                        "tony@wdtamlyn.co.uk", 
                        "mark@bradley111.fsnet.co.uk", 
                        "david.stone@talktalk.net", 
                        "tim.herbert@mourant.com", 
                        "presse.londres-amba@diplomatie.gouv.fr", 
                        "ecrire@consulfrance-edimbourg.org", 
                        "london.emb@mfa.gov.ge",
                        "embassy@geoemb.plus.com", 
                        "info@london.diplo.de", 
                        "political@greekembassy.org.uk", 
                        "consulategeneral@greekembassy.org.uk", 
                        "info@mfewings.com", 
                        "hagleycourt@aol.com", 
                        "melpo@blueyonder.co.uk", 
                        "james@imossi.gib.gi", 
                        "Greek-Consulate@nt1world.com", 
                        "hondurasuk@lineone.net", 
                        "mission.lon@mfa.gov.hu", 
                        "konz.lon@kum.hu", 
                        "icemb.london@utn.stjr.is", 
                        "114343.3045@compuserve.com", 
                        "cgidubai@emirates.net.ae", 
                        "indianconsulate@btconnect.com", 
                        "dsrana@andrashouse.co.uk", 
                        "kbri@btconnect.com", 
                        "info@iran-embassy.org.uk", 
                        "lonemb@iraqmofamail.net", 
                        "public3@london.mfa.gov.il", 
                        "ambasciata.londra@esteri.it", 
                        "consolato.londra@esteri.it", 
                        "consolato.edimburgo@esteri.it", 
                        "consolato.manchester@esteri.it", 
                        "jamhigh@jhcuk.com", 
                        "info@ld.mofa.go.jp", 
                        "info.cgj@btconnect.com",
                        "ryouji.cgj@btconnect.com", 
                        "diane@stjohnwales.org.uk", 
                        "info@jordanembassyuk.org", 
                        "london@kazembassy.org.uk", 
                        "kcomm45@aol.com", 
                        "mravellwalsh@btopenworld.com", 
                        "mail@kyrgyz-embassy.org.uk",
                        "embassy@kyrgyz-embassy.org.uk", 
                        "embassy.uk@mfa.gov.lv", 
                        "emb.leb@btinternet.com", 
                        "lhc@lesotholondon.org.uk", 
                        "info@embassyofliberia.org.uk", 
                        "londres.amb@mae.etat.lu", 
                        "info@macedonianembassy.org.uk", 
                        "embamadlon@yahoo.co.uk", 
                        "tourism@malawihighcomm.prestel.co.uk", 
                        "republicofmalawi@btconnect.com",
                        "malawihighcom@btconnect.com", 
                        "mwlon@btconnect.com", 
                        "maltahighcommission.london@gov.mt", 
                        "maltaconsul.gibraltar@gov.mt", 
                        "maltaconsul.isleofman@gov.mt", 
                        "londonmhc@btinternet.com", 
                        "mexuk@sre.gob.mx", 
                        "embassy.london@mfa.md", 
                        "evelyne.genta@virgin.net", 
                        "office@embassyofmongolia.co.uk", 
                        "mail@sifamaldn.org",
                        "mbelmahi@hotmail.com", 
                        "moroccanconsulate.uk@pop3.hiway.co.uk", 
                        "info@mozambiquehc.org.uk", 
                        "Melondon@btconnect.com", 
                        "nauru@weald.co.uk", 
                        "eon@nepembassy.org.uk", 
                        "nlconsulab@aol.com", 
                        "shipping@allroute.com", 
                        "nlconsulate.bham@btconnect.com", 
                        "consul@samsmithtravel.com", 
                        "info@netherlands-consulate.co.uk", 
                        "angelita.latimer@gi.abnamro.com", 
                        "admin@np-holdings.com", 
                        "sbmulder@ibl.bm", 
                        "nlconsul@kettlewell.com", 
                        "mieke.slater@harvesthousing.org.uk", 
                        "jonastra@aol.com", 
                        "consular@kandb-produce.co.uk", 
                        "sze@walkermorris.co.uk", 
                        "tony@willowby.eclipse.co.uk", 
                        "aboutnz@newzealandhc.org.uk", 
                        "balancenz@aol.com", 
                        "nzconsul@mckaynorwell.co.uk", 
                        "emb.ofnicaragua@firgin.net", 
                        "nisclondon@aol.com", 
                        "emb.london@mfa.no", 
                        "norwayconsgen.edinburgh@gmail.com", 
                        "fionas@mackinnons.com", 
                        "norwegian@mairshipping.co.uk", 
                        "rnc@jfhornby.com", 
                        "theembassy@omanembassy.org.uk", 
                        "consular@phclondon.org", 
                        "parepbirmingham@btconnect.com", 
                        "parepbradford@hotmail.com", 
                        "pakconsulate@btconnect.com", 
                        "pak_consulate@hotmail.com", 
                        "info@phclondon.org", 
                        "info@panaconsul.com", 
                        "info@png.org.uk",
                        "jkekedo@aol.com", 
                        "embapar@btconnect.com", 
                        "postmaster@peruembassy-uk.com", 
                        "londonpe@dfa.gov.ph", 
                        "london@msz.gov.pl", 
                        "edinburgh@polishconsulate.org", 
                        "manchester.kg.sekretariat@msz.gov.pl", 
                        "london@portembassy.co.uk", 
                        "london@mofa.gov.qa", 
                        "roemb@roemb.co.uk", 
                        "visa@edconsul.co.uk", 
                        "uk@ambarwanda.org.uk",
                        "ambalondres@minaffet.gov.rw", 
                        "ukemb@mofa.gov.sa", 
                        "senegalembassy@hotmail.co.uk",
                        "senegalembassy@btconnect.com", 
                        "londre@jugisek.demon.co.uk", 
                        "consulate@seychelles-gov.net", 
                        "info@slhc-uk.org.uk", 
                        "singhc_lon@sgmfa.gov.sg", 
                        "emb.london@mzv.sk", 
                        "vdb@mzz-dkp.gov.si", 
                        "Vlo@mzz-dkp.gov.si", 
                        "london.general@foreign.gov.za", 
                        "koreanembinuk@mofat.go.kr", 
                        "emb.londres@maec.es", 
                        "cgspedimburgo@mail.mae.es", 
                        "cg.manchester@mail.mae.es", 
                        "mail@slhc-london.co.uk", 
                        "info@sudan-embassy.co.uk", 
                        "ajethu@honoraryconsul.info", 
                        "enquiries@swaziland.org.uk", 
                        "ambassaden.london@gov.se", 
                        "lon.vertretung@eda.admin.ch", 
                        "tro@taiwan-tro.uk.net", 
                        "reception.troedi@gmail.com", 
                        "balozi@tanzania-online.gov.uk", 
                        "csinfo@thaiembassyuk.org.uk", 
                        "office@tongahighcom.co.uk", 
                        "london@tunisianembassy.co.uk", 
                        "embassy.london@mfa.gov.tr", 
                        "tkm-embassy-uk@btconnect.org.uk", 
                        "info@ugandahighcomission.co.uk", 
                        "emb_gb@mfa.gov.ua", 
                        "gc_gbe@mfa.gov.ua",
                        "edinburgh@consulateukr.co.uk", 
                        "informationuk@mofa.gov.ae", 
                        "emburuguay@emburuguay.org.uk", 
                        "info@uzbekembassy.org", 
                        "info@venezlon.co.uk", 
                        "vanphong@vietnamembassy.org.uk", 
                        "admin@yemenembassy.co.uk", 
                        "info@zambiahc.org.uk", 
                        "zimlondon@yahoo.co.uk" };
            return arrEmbasy;
        }

        public string Sendtoembassy()
        {
            Dictionary<string, string> hasSend = new Dictionary<string, string>();

            var strLogo = Path.Combine(Config.website + "/EmailTemp/image/logo.png");
            var strYT = Path.Combine(Config.website + "/EmailTemp/image/48x48_youtube.png");
            var strEidinpark = Path.Combine(Config.website + "/EmailTemp/image/slide1.png");
            var strAppeal = Path.Combine(Config.website + "/EmailTemp/image/new_masjid_slide.jpg");
            var strTwitter = Path.Combine(Config.website + "/EmailTemp/image/48x48_twitter.png");
            var strFacebook = Path.Combine(Config.website + "/EmailTemp/image/48x48_facebook.png");

            // StringBuilder msb = new StringBuilder();
            try
            {
                foreach (var email in embassy())
                    {
                        string strBody = System.IO.File.ReadAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/EmailTemp/declaration_embassy.txt")));
                        strBody = strBody.Replace("%strLogo%", strLogo).Replace("%EIDINPARK%", strEidinpark).Replace("%APPEAL%", strAppeal).Replace("%strYT%", strYT).Replace("%strTwitter%", strTwitter).Replace("%strFacebook%", strFacebook);
                    
                        if (!hasSend.ContainsKey(email))
                        {
                            MailAddress fromAddr = new MailAddress("secretariat@manuk.org", "MANUK Secretary General");
                            MailAddress toAddr = new MailAddress(email);

                            using (SmtpClient client = new SmtpClient("relay-hosting.secureserver.net"))
                            {
                                using (MailMessage message = new MailMessage(fromAddr, toAddr))
                                {
                                    message.Subject = "London Declaration on Islam and World Peace";
                                    message.Body = strBody;
                                    message.IsBodyHtml = true;
                                    client.Send(message);
                                }
                            }
                            //msb.Append(email + "<br/>" + strBody + "<br/><br/><br/>");

                            hasSend.Add(email, email);
                        }
                    }

                return "Success" + "<br/>"; // +msb.ToString();
            }
            catch (Exception ex)
            {
                return "Fail " + ex.ToString();
            }

        }

        public string Sendtoattendees()
        {
            Dictionary<string, string> hasSend = new Dictionary<string, string>();

            var strLogo = Path.Combine(Config.website + "/EmailTemp/image/logo.png");
            var strYT = Path.Combine(Config.website + "/EmailTemp/image/48x48_youtube.png");
            var strEidinpark = Path.Combine(Config.website + "/EmailTemp/image/slide1.png");
            var strAppeal = Path.Combine(Config.website + "/EmailTemp/image/new_masjid_slide.jpg");
            var strTwitter = Path.Combine(Config.website + "/EmailTemp/image/48x48_twitter.png");
            var strFacebook = Path.Combine(Config.website + "/EmailTemp/image/48x48_facebook.png");
            //StringBuilder msb = new StringBuilder();

            try
            {
                foreach (var email in attendes())
                {
                    string[] arr = email.Split('#');
                    string name = arr[0];
                    string emais = arr[1];

                    string strBody = System.IO.File.ReadAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/EmailTemp/declaration_attendees.txt")));
                    strBody = strBody.Replace("%strLogo%", strLogo).Replace("%EIDINPARK%", strEidinpark).Replace("%APPEAL%", strAppeal).Replace("%strYT%", strYT).Replace("%strTwitter%", strTwitter).Replace("%strFacebook%", strFacebook).Replace("%FULLNAME%", name);

                    if (!hasSend.ContainsKey(emais))
                    {
                        MailAddress fromAddr = new MailAddress("secretariat@manuk.org", "MANUK Secretary General");
                        MailAddress toAddr = new MailAddress(emais);

                        using (SmtpClient client = new SmtpClient("relay-hosting.secureserver.net"))
                        {
                            using (MailMessage message = new MailMessage(fromAddr, toAddr))
                            {
                                message.Subject = "London Declaration on Islam and World Peace";
                                message.Body = strBody;
                                message.IsBodyHtml = true;
                                client.Send(message);
                            }
                        }
                        //msb.Append(name + "<br/>" + emais + "<br/>" + strBody + "<br/><br/><br/>");

                        hasSend.Add(emais, emais);
                    }
                }

                return "Success" + "<br/>"; // +msb.ToString();
            }
            catch (Exception ex)
            {
                return "Fail " + ex.ToString();
            }

        }


        public void Send_Conf_Confirm_Email(string names, string email, string regcode)
        {
            try
            {
                var strLogo = Path.Combine(Config.website + "/EmailTemp/image/logo.png");
                var strYT = Path.Combine(Config.website + "/EmailTemp/image/48x48_youtube.png");
                var strEidinpark = Path.Combine(Config.website + "/EmailTemp/image/slide1.png");
                var strAppeal = Path.Combine(Config.website + "/EmailTemp/image/new_masjid_slide.jpg");
                var strTwitter = Path.Combine(Config.website + "/EmailTemp/image/48x48_twitter.png");
                var strFacebook = Path.Combine(Config.website + "/EmailTemp/image/48x48_facebook.png");

                var strBody = System.IO.File.ReadAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/EmailTemp/peace_attendee_email.txt")));
                strBody = strBody.Replace("%strLogo%", strLogo);
                //strBody = strBody.Replace("%VIDEOS%", ysb.ToString());
                strBody = strBody.Replace("%EIDINPARK%", strEidinpark);
                strBody = strBody.Replace("%APPEAL%", strAppeal);
                strBody = strBody.Replace("%strYT%", strYT);
                strBody = strBody.Replace("%strTwitter%", strTwitter);
                strBody = strBody.Replace("%strFacebook%", strFacebook);
                strBody = strBody.Replace("%FULLNAME%", names);
                strBody = strBody.Replace("%REGNumber%", regcode);
                Email.Send("secretariat@manuk.org ", "MANUK Secretary General", email, "Confirmation - London Conference On Islam and World Peace", strBody, true);
            }
            catch (Exception ex)
            {

            }

        }

        public string testxx()
        {
            return Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/image/logo.png"));
        }

        public string Send_Reminder()
        {
            Dictionary<string, string> hasSend = new Dictionary<string, string>();

            var strLogo = Path.Combine(Config.website + "/EmailTemp/image/logo.png");
            var strYT = Path.Combine(Config.website + "/EmailTemp/image/48x48_youtube.png");
            var strEidinpark = Path.Combine(Config.website + "/EmailTemp/image/slide1.png");
            var strAppeal = Path.Combine(Config.website + "/EmailTemp/image/new_masjid_slide.jpg");
            var strTwitter = Path.Combine(Config.website + "/EmailTemp/image/48x48_twitter.png");
            var strFacebook = Path.Combine(Config.website + "/EmailTemp/image/48x48_facebook.png");

            StringBuilder msb = new StringBuilder();
            try
            {
                foreach (var xx in lstReminder())
                {
                    var strBody = System.IO.File.ReadAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/EmailTemp/peace_reminder_email.txt")));
                    strBody = strBody.Replace("%strLogo%", strLogo);
                    strBody = strBody.Replace("%EIDINPARK%", strEidinpark);
                    strBody = strBody.Replace("%APPEAL%", strAppeal);
                    strBody = strBody.Replace("%strYT%", strYT);
                    strBody = strBody.Replace("%strTwitter%", strTwitter);
                    strBody = strBody.Replace("%strFacebook%", strFacebook);
                    strBody = strBody.Replace("%FULLNAME%", xx.fullname);
                    strBody = strBody.Replace("%REGNumber%", xx.aref);

                    if (!hasSend.ContainsKey(xx.email))
                    {
                        Email.Send("secretariat@manuk.org ", "MANUK Secretary General", xx.email, "Reminder - Invite to London Conference On Islam and World Peace", strBody, true);
                        hasSend.Add(xx.email, xx.email);
                        msb.Append(xx.fullname + "<br/>" + xx.email + "<br/>" + strBody + "<br/><br/><br/>");
                    }
                }

                string mymsb = "Success" + "<br/>" + msb.ToString();
                return mymsb;
            }
            catch (Exception ex)
            {
                return "Fail " + ex.ToString();
            }

        }

        public string Send_Register()
        {
            Dictionary<string, string> hasSend = new Dictionary<string, string>();

            var strLogo = Path.Combine(Config.website + "/EmailTemp/image/logo.png");
            var strYT = Path.Combine(Config.website + "/EmailTemp/image/48x48_youtube.png");
            var strEidinpark = Path.Combine(Config.website + "/EmailTemp/image/slide1.png");
            var strAppeal = Path.Combine(Config.website + "/EmailTemp/image/new_masjid_slide.jpg");
            var strTwitter = Path.Combine(Config.website + "/EmailTemp/image/48x48_twitter.png");
            var strFacebook = Path.Combine(Config.website + "/EmailTemp/image/48x48_facebook.png");

            StringBuilder msb = new StringBuilder();
            try
            {
                foreach (var xx in lstNotRegister())
                {
                    var strBody = System.IO.File.ReadAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/EmailTemp/peace_register_email.txt")));
                    strBody = strBody.Replace("%strLogo%", strLogo);
                    strBody = strBody.Replace("%EIDINPARK%", strEidinpark);
                    strBody = strBody.Replace("%APPEAL%", strAppeal);
                    strBody = strBody.Replace("%strYT%", strYT);
                    strBody = strBody.Replace("%strTwitter%", strTwitter);
                    strBody = strBody.Replace("%strFacebook%", strFacebook);
                    strBody = strBody.Replace("%FULLNAME%", xx.fullname);
                    strBody = strBody.Replace("%REGNumber%", xx.aref);

                    if (!hasSend.ContainsKey(xx.email))
                    {
                        Email.Send("secretariat@manuk.org ", "MANUK Secretary General", xx.email, "Invite to London Conference On Islam and World Peace", strBody, true);
                        hasSend.Add(xx.email, xx.email);
                        msb.Append(xx.fullname + "<br/>" + xx.email + "<br/>" + strBody + "<br/><br/><br/>");
                    }
                }

                return "Success" + "<br/>" + msb.ToString();
            }
            catch (Exception ex)
            {
                return "Fail " + ex.ToString();
            }

        }

        public List<LPC> lstReminder()
        {
            List<LPC> reminder = new List<LPC>();

            reminder.Add(new LPC { fullname = "Jamiu Ayinde", email = "jamiuaz@yahoo.co.uk", aref = "MANUK-88443191" });
            reminder.Add(new LPC { fullname = "Mistura Yusuf", email = "tokunboy@aol.com", aref = "MANUK-73092351" });
            reminder.Add(new LPC { fullname = "Falilat Alabi ", email = "eluwbum@yahoo.co.uk", aref = "MANUK-84756163" });
            reminder.Add(new LPC { fullname = "Temilola M Animashaun", email = "Temi_ani@yahoo.com", aref = "MANUK-35879077" });
            reminder.Add(new LPC { fullname = "Prof. Mashood Baderin", email = "baderinuk@gmail.com", aref = "MANUK-91039390" });
            reminder.Add(new LPC { fullname = "Sherifat Ameen", email = "sherryameen@gmail.com", aref = "MANUK-47038018" });
            reminder.Add(new LPC { fullname = "Luqman Onanuga ", email = "deleonas@hotmail.com", aref = "MANUK-23714690" });
            reminder.Add(new LPC { fullname = "Muntaka sanusi", email = "s_muntaka@yahoo.co.uk", aref = "MANUK-84027323" });
            reminder.Add(new LPC { fullname = "Abdul-Jelil Oladejo", email = "oladejoyinka@yahoo.com", aref = "MANUK-05294139" });
            reminder.Add(new LPC { fullname = "Shamsuldeen Olakitan Adeniji", email = "olaadeniji@hotmail.com", aref = "MANUK-07544860" });
            reminder.Add(new LPC { fullname = "Hussaynah Raji", email = "tayturs@yahoo.com", aref = "MANUK-73064126" });
            reminder.Add(new LPC { fullname = "Alh Bola Tade", email = "btad53@gmail.com", aref = "MANUK-85561840" });
            reminder.Add(new LPC { fullname = "Muhammad Ibrahim Usman ", email = "abbatee2003@yahoo.com", aref = "MANUK-54267132" });
            reminder.Add(new LPC { fullname = "Dr Semiu Gbadebo", email = "s_gbadebo@yahoo.com", aref = "MANUK-85317636" });
            reminder.Add(new LPC { fullname = "RAJI ISMAIL TUNDE", email = "ismillion@gmail.com", aref = "MANUK-34169134" });
            reminder.Add(new LPC { fullname = "Hajia Fawzah Adepoju-Akinwale", email = "fjadepoju@yahoo.co.uk", aref = "MANUK-28595269" });
            reminder.Add(new LPC { fullname = "Aina-Obe  Shamsuddin Bolatito", email = "sam1421h@yahoo.com", aref = "MANUK-35037160" });
            reminder.Add(new LPC { fullname = "Oloso  Kashif  Kolade", email = "samtitooye@gmail.com", aref = "MANUK-58553106" });
            reminder.Add(new LPC { fullname = "Mahamood Mubarak Ali", email = "mahamoodmubarak95@gmail.com", aref = "MANUK-29154145" });
            reminder.Add(new LPC { fullname = "AJADI YUSUF ABDULJAMIH", email = "yusufoba@ymail.com", aref = "MANUK-99125293" });
            reminder.Add(new LPC { fullname = "Hasan Qasim Okikiola", email = "islammyparadise@yahoo.com", aref = "MANUK-01250519" });
            reminder.Add(new LPC { fullname = "Dr. Ibrahim Olatunde Uthman", email = "ibrahimuthman@yahoo.com", aref = "MANUK-28153599" });
            reminder.Add(new LPC { fullname = "RAHEEMSON NADIRAT ADEOLA", email = "nadeer_85@yahoo.com", aref = "MANUK-59489101" });
            reminder.Add(new LPC { fullname = "QUADRI LATEEFAT OMOTOLA", email = "ummulkhayr234@gmail.com", aref = "MANUK-80856500" });
            reminder.Add(new LPC { fullname = "Tariq Shaekh", email = "t.shaekh@muntadaaid.org", aref = "MANUK-47170165" });
            reminder.Add(new LPC { fullname = "Mrs Faosat Adesanya ", email = "faodosade@yahoo.com", aref = "MANUK-71804265" });
            reminder.Add(new LPC { fullname = "Muhammad Lawal Maidoki", email = "ibnhamman@gmail.com", aref = "MANUK-68683664" });
            reminder.Add(new LPC { fullname = "ABDURAHMAN ABDULATEEF ", email = "abdulateefabdurahman@gmail.com", aref = "MANUK-84388783" });
            reminder.Add(new LPC { fullname = "mohammed kamel ali", email = "kamelmohammed2002@yahoo.co.in", aref = "MANUK-53027494" });
            reminder.Add(new LPC { fullname = "Mrs Ajibola Gbadebo", email = "ragbadebo@hotmail.com", aref = "MANUK-10508196" });
            reminder.Add(new LPC { fullname = "Musa Adeniyan ", email = "musaadeniyan@msn.com", aref = "MANUK-32472577" });
            reminder.Add(new LPC { fullname = "adeola k hafeez", email = "havisadeogun@gmail.com", aref = "MANUK-44824622" });
            reminder.Add(new LPC { fullname = "Abdoulbaq Ladi Balogun", email = "positiveleo2@yahoo.com", aref = "MANUK-99794646" });
            reminder.Add(new LPC { fullname = "Kabiru Ado Yau", email = "kabiruadoyau@yahoo.com", aref = "MANUK-34174448" });
            reminder.Add(new LPC { fullname = "yusuf afolabi", email = "lordafoo@yahoo.com", aref = "MANUK-14925860" });
            reminder.Add(new LPC { fullname = "ISMAIL KOLADE NURUDEEN", email = "nhururdin@gmail.com", aref = "MANUK-12625305" });
            reminder.Add(new LPC { fullname = "ASHO FATAH", email = "fatah.asho@yahoo.co.uk", aref = "MANUK-80377226" });
            reminder.Add(new LPC { fullname = "HameedLawal", email = "hamid.lawal@btinternet.com", aref = "MANUK-92143662" });
            reminder.Add(new LPC { fullname = "Aminu muritala", email = "aminumuritala@gmail.com", aref = "MANUK-16799832" });
            reminder.Add(new LPC { fullname = "", email = "adisa4salam@gmail.com", aref = "MANUK-98988779" });
            reminder.Add(new LPC { fullname = "Jimoh Mumini Remi", email = "remtan01@yahoo.com", aref = "MANUK-04865402" });
            reminder.Add(new LPC { fullname = "Olayinka Abdulai Alimi", email = "yinkaalimi@hotmail.com", aref = "MANUK-83314598" });
            reminder.Add(new LPC { fullname = "Abdelkader  Toumi", email = "toumikader@yahoo.fr", aref = "MANUK-31354205" });
            reminder.Add(new LPC { fullname = "Mustapha Abdulsalam", email = "mustaphaabdulsalam@ymail.com", aref = "MANUK-79733988" });
            reminder.Add(new LPC { fullname = "Dr. Abubakar Adamu", email = "adamu7360@gmail.com", aref = "MANUK-87223330" });
            reminder.Add(new LPC { fullname = "Dr. Abubakar Adamu", email = "mercyofislamabu53@yahoo.com", aref = "MANUK-18647017" });
            reminder.Add(new LPC { fullname = "DR.BUSARI KEHINDE KAMORUDEEN", email = "kkbusari@gmail.com", aref = "MANUK-98706538" });
            reminder.Add(new LPC { fullname = "jibrin bature GARBA", email = "baturrjibrin60@gmail.com", aref = "MANUK-52380509" });
            reminder.Add(new LPC { fullname = "Yumiko Scintu", email = "yumikoscintu@gmail.com", aref = "MANUK-82365280" });
            reminder.Add(new LPC { fullname = "Alhaja Basirat Apatira ", email = "yinkaapat@gmail.come", aref = "MANUK-16056362" });
            reminder.Add(new LPC { fullname = "Alhaja Basirat Apatira ", email = "yinkaapat@gmail.com", aref = "MANUK-25326041" });
            reminder.Add(new LPC { fullname = "J.A.S.ANIMASHAUN", email = "a1n1i1@aol.co.uk", aref = "MANUK-10353501" });
            reminder.Add(new LPC { fullname = "J Hendricks", email = "nurahnoor@aol.com", aref = "MANUK-68874817" });
            reminder.Add(new LPC { fullname = "MOHAMMED UMAR ABDULLATEEF", email = "abdullateefmuhammed@yahoo.co.uk", aref = "MANUK-29740217" });
            reminder.Add(new LPC { fullname = "Monsour aremu", email = "bidosko01@yahoo.com", aref = "MANUK-25953036" });
            reminder.Add(new LPC { fullname = "Asimi Mumini Ayinla", email = "abdulmumeen0@gmail.com", aref = "MANUK-79643094" });
            reminder.Add(new LPC { fullname = "Ajibola Popoola", email = "jimberland88@yahoo.com", aref = "MANUK-95026079" });
            reminder.Add(new LPC { fullname = "Tezleem subair", email = "teslesm.subair@yahoo.co.uk", aref = "MANUK-90715084" });
            reminder.Add(new LPC { fullname = "NOSIRU ALOGBA", email = "OWODUNNIESQ@GMAIL.COM", aref = "MANUK-57834326" });
            reminder.Add(new LPC { fullname = "Hassan Mohammed Hassan", email = "hassanhassan15759@yahoo.com", aref = "MANUK-80120198" });
            reminder.Add(new LPC { fullname = "Hassan Mohammed Hassan", email = "hassanhassan15759@yahoo.com", aref = "MANUK-86694034" });
            reminder.Add(new LPC { fullname = "JIBRIN BATURE GARBA", email = "baturejibrin60@gmail.com", aref = "MANUK-78596922" });
            reminder.Add(new LPC { fullname = "Yahkub Tiamiyu", email = "yahkubtiamiyu@yahoo.com", aref = "MANUK-55058459" });
            reminder.Add(new LPC { fullname = "Mariam Bukenya", email = "mariambk18@gmail.com", aref = "MANUK-56892806" });
            reminder.Add(new LPC { fullname = "Ann Lang", email = "annlang@t-online.de", aref = "MANUK-18564643" });
            reminder.Add(new LPC { fullname = "kudirat olaitan adenekan", email = "iyabodeakinboyede@yahoo.com", aref = "MANUK-75881649" });
            return reminder;
        }

        public List<LPC> lstNotRegister()
        {
            List<LPC> register = new List<LPC>();
            register.Add(new LPC { fullname = "MAN UK", email = "secretariat@manuk.org", aref = "" });
            register.Add(new LPC { fullname = "Adesina M. Alabi", email = "eluwsina@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Idris Eletu", email = "edriseletu@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "Mudathir Yussuff", email = "matyussuff@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Taowfiq Ibrahim-Fagbohun", email = "taofagbohun@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Muyinat Opeolu", email = "tolaopeolu@aol.com", aref = "" });
            register.Add(new LPC { fullname = "Saidat Oketunde", email = "princessokes2000@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Kudirat Obikoya-Oladapo", email = "yinka80@hotmail.com", aref = "" });
            register.Add(new LPC { fullname = "Abdul-Yekeen Keshinro", email = "ade_keshy2006@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Sherifat Ibrahim", email = "sherifat.ibrahim@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "Abd-Ghani Ajobo", email = "ganewajobo@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Kamaldeen Sadiku", email = "taiwobanuso@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Tajudeen Salami", email = "tsalami52@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Abraham Atunrase", email = "abratun@aol.com", aref = "" });
            register.Add(new LPC { fullname = "Alhaji adam olayinka awolola", email = "adamoawolola2008@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "kazeem Afolabi", email = "kazeemafolabi03@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Ahmed Adesina", email = "ahmed.adesina@hotmail.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Aminu Shittu", email = "ameen_vet@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "Zainab Adewusi", email = "zainabo@hotmail.co.uk", aref = "" });
            register.Add(new LPC { fullname = "MUTIU ABIODUN SODIQ", email = "mutthewlahi@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "amadu balde", email = "amjubal711@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Ismail Arifi", email = "ismailarifi90@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Azeezat Akande", email = "zee13@hotmail.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Shuqrah Muheeb", email = "kemimuheeb@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "Junior Mazele", email = "jmazele@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Muinat Adeyemo", email = "muinatadeyemo@hotmail.co.uk", aref = "" });
            register.Add(new LPC { fullname = "usman nasir", email = "elfaun4@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "AHMED  YUSUF MA-INJI", email = "abuyassad@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "abdul wahab williams", email = "abdulwahab_williams@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "idowu ibrahim olaotan", email = "nice4noble@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Aminata Soumare", email = "amisoumare@hotmail.fr", aref = "" });
            register.Add(new LPC { fullname = "Abdulqudus Abdulwahab", email = "abdulqudusayinla@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Zubairu Hamidu jega", email = "jegahameed@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "Ayorinde Jubril  Balogun", email = "ayorindebalogun2015@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "abdullateef jimoh", email = "grpolice99@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Saheed sulaemon", email = "sir_heed1067@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "Elhadj Diallo", email = "elhadjdiallo9@live.com", aref = "" });
            register.Add(new LPC { fullname = "Zainab Achukwu", email = "zainabachukw1@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Bilikisu  Sacage", email = "basadiku@aol.com", aref = "" });
            register.Add(new LPC { fullname = "BranyAlbult BranyAlbultZN", email = "crystalworners@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "aremu lukman adeyinka aremu", email = "aremuadeyinka@naij.com", aref = "" });
            register.Add(new LPC { fullname = "Melisominccab MelisominccabDK", email = "melissarriley@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "Abdur Razaq Salaudeen", email = "optimaxcare@aol.com", aref = "" });
            register.Add(new LPC { fullname = "Abdur Razaq Salaudeen", email = "optimaxcare@aol.com", aref = "" });
            register.Add(new LPC { fullname = "Noor mohamed", email = "yoniscade05@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "AshleyMt AshleyMtNK", email = "allisoncox@mail.ru", aref = "" });
            register.Add(new LPC { fullname = "Zainab  issah", email = "zainababi75@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Sherif adegbenle", email = "iamcheriph@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Adekunle Rasheed  Okusi", email = "kunle_okusi@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "Luqman  Hackim Olu Omoajanaku Burunbamu mohammed", email = "mluqman.akim@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Alhaji Musiliudeen Bishi", email = "tankr45@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "jibjabst jibjabst", email = "jibsajabsa@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "Ilyas ohuabunwa", email = "onuimolga@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "bilal hamimid", email = "bibo34h@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "Denisesr DenisesrAC", email = "kqpu3580@virtuf.info", aref = "" });
            register.Add(new LPC { fullname = "uzxokemohevok ilozeba", email = "akukiac@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ijavumufuva uzanokac", email = "ecihofz@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "nipuzexoirui amixievugutic", email = "emarmeasa@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "oguwobu usawupe", email = "yeemad@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "oludewibeh kegikaxibid", email = "irelenowu@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "naqafosucpowq iigataamaqine", email = "ayefil@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "afejixes arawuna", email = "esacecuw@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "idafemey onteonia", email = "iduduf@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "Joshtachep JoshtachepFF", email = "mail@newmedicforum.com", aref = "" });
            register.Add(new LPC { fullname = "olexojow okacorefupowo", email = "ofizizefi@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "poqdeze uqukevo", email = "axidevaqm@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "umokquz edeasebyegae", email = "uufowoz@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "ozpevidadi foyugnist", email = "oaorady@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "agagadafivup ivawuzve", email = "aceguonco@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "aguvfurca uociihu", email = "afaqelev@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "aviqegatiqna ohuquxojo", email = "goidaeje@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "ibohewuetu edahiwabok", email = "duqoto@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "dowomwifiph uiyofafevma", email = "issubij@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "ezyoaqidatgox ikeduneeebeg", email = "iticuvuzo@gmaildd.net", aref = "" });
            register.Add(new LPC { fullname = "ipuluvo eqeevlowus", email = "arigogih@gmaildd.net", aref = "" });
            register.Add(new LPC { fullname = "coguleemih iyameehunen", email = "ilduzomu@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "amemiwoqovo awidotiyefho", email = "xebilkub@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "ujivawoloye izijclulekika", email = "ijedize@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "iwecova upixowukeasde", email = "ougiwadox@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ulueyexvgihe eredevicila", email = "ohmaqaqe@gmailhre.net", aref = "" });
            register.Add(new LPC { fullname = "owuhukaho uporuocveefe", email = "igelafaje@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "zufufab azebixoyowa", email = "unalanod@gmailhre.net", aref = "" });
            register.Add(new LPC { fullname = "amaratioleh ezdigqinapiep", email = "uxixax@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "etetoludojoo ifamojake", email = "amayufop@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "Offenovemn suirmAmemiJB", email = "x3l1kfngg@yahoorg.ml", aref = "" });
            register.Add(new LPC { fullname = "GreewTwess GresstoiseJY", email = "zk07l7q93@yahooorg.gq", aref = "" });
            register.Add(new LPC { fullname = "oapwivnaqa gafederafe", email = "abocizawi@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "dexudec udojiuxu", email = "upocaevul@gmailhre.net", aref = "" });
            register.Add(new LPC { fullname = "upuqownip avaxuwava", email = "ekiquxenv@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "emoovfipaj tuifepo", email = "ehfeoud@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "eqaotufi awejoyakib", email = "uvcowe@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ezusijus ilakutu", email = "eqerfop@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "iawapojen aqezinuyolead", email = "oramosav@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ezazeisuol esozehoxu", email = "ejifucqid@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "egiladlde urijzlisa", email = "aeyanuwam@gmailhre.net", aref = "" });
            register.Add(new LPC { fullname = "ahizamuno iudivjidudqug", email = "ugihawip@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "tayopop enefezocegedu", email = "amozauta@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "oxotumiboqi iwepironaqjic", email = "hbqahk@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "iyajidas oboqagweqotse", email = "olatekowo@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "idujilop utdacabwe", email = "ayomioh@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "iniyomunu capoujuqexe", email = "ivonuh@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "iaxeguva ecimiwizra", email = "aoxetu@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "yuvehegox umaukequ", email = "outegehu@gmailasdf.net", aref = "" });
            register.Add(new LPC { fullname = "uyilizo ezeleupexaqec", email = "hovoye@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "eterepeikiwic ilituxuhobo", email = "inatevi@gmailasdf.net", aref = "" });
            register.Add(new LPC { fullname = "pejoxexoza eluqojn", email = "omicor@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "odozafixhzewo iharnipace", email = "ruqiqoyej@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "iharigizupadk usenahonpici", email = "yumimoi@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "ayoafaregad idetadafhuwid", email = "ediexov@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "elugifhawi ureloreyewuvi", email = "izonit@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "StepOblity StepOblityXV", email = "mailinfo@newmedicforum.com", aref = "" });
            register.Add(new LPC { fullname = "ajenigurec urifadez", email = "umetorkof@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "ewoxidobebibe ueagegs", email = "ucoyige@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "amikooatreba ipacoosejoce", email = "eqovacabe@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "purigumjecu elcazecuv", email = "axomwiho@gmaildd.net", aref = "" });
            register.Add(new LPC { fullname = "okapaqoj esakidoit", email = "oyofeka@gmaildd.net", aref = "" });
            register.Add(new LPC { fullname = "ulgilikipeg aweppagoag", email = "ekeqomec@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "uziaquyhat eeyeqqapexay", email = "akipoyozc@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "iyukoezeres irubiqu", email = "ozonafa@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ukukeyduh atowimadot", email = "ugovevaj@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "ebaqmota epekovumuw", email = "uquduilo@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "izetorc oguvgap", email = "uqapkowu@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "eriwisioteh oatwuysuloha", email = "fefoba@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "oponida iqulamonm", email = "ipoxuxof@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "etoohasuvegiw ewofubu", email = "iuoeruo@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "egilapabb eizirsyiyipa", email = "udaeguda@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "iufoxav domazuvumi", email = "azaawa@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "uonugivelimoi ogefefafpemoj", email = "ilotami@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "ixawiciwuyp uuketapijuyab", email = "eopzeuh@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "Ftepmanpr FtepmanprHT", email = "zigler.everett2015@yandex.com", aref = "" });
            register.Add(new LPC { fullname = "ukaocum uhuvofoj", email = "evecarore@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "apeqivebafaj ohovigqeroqig", email = "opagaqizu@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "Fenmieer FenmieerIT", email = "westbury.taylor2015@yandex.com", aref = "" });
            register.Add(new LPC { fullname = "RobeTasp RobeTaspUV", email = "info-mail@fast-tadalafil.com", aref = "" });
            register.Add(new LPC { fullname = "oyexetawub abuebanuuw", email = "asuirak@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "esenejir oqejaduyit", email = "uwecro@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "ubfupomihiqe ipujurbuteku", email = "odowjetel@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "ilopautezajox ujaejud", email = "aedareq@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "jheparisxagu otipuhcosubar", email = "enimot@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "ttoduguumete ibenuqivo", email = "uguifa@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "eziugobarepen elewogio", email = "uolouxoci@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "eqdobabiria evumuro", email = "epakixi@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "aihejonamuvuh eqasipacejmiv", email = "aleidoa@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "eganlik oqihalisacu", email = "uzonowa@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "eqadeco irarerobep", email = "alogufide@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "atpatoyigo utehupavubeet", email = "eofaajet@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "ufrubexiuolev iqahujehed", email = "itejucwh@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "okatujiel ijogujekohagu", email = "egivoyi@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "efemeleltxosu uximapon", email = "omexoz@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "uwoyejonaw ahuzowiye", email = "ivukawot@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "aowitobe eloatupuqiw", email = "qupaeje@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "Marionkn MarionknLU", email = "kreditinua@wegas.ru", aref = "" });
            register.Add(new LPC { fullname = "zigaiyisab eyizazeker", email = "ibirizi@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "ibowaumusupad owovidadize", email = "nilajot@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "orunoqopenih anecunoqcout", email = "egcama@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "ijichecqovg uzopiqbizoor", email = "awuvifuux@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "uloquxu zoiwimicow", email = "ijetukiw@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ayupuwebfo yihfobemtin", email = "upesopuyo@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "umejoweip eunukozo", email = "inibita@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ejuyace adozociovvet", email = "igasicuc@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "onapuyo uhuyesoguya", email = "agexiyiw@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "lisecenca afewewog", email = "axujah@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "Addisterer pooccatGarJX", email = "asg3v6i2n@daymail.cf", aref = "" });
            register.Add(new LPC { fullname = "cecilmackechniehnh marquisfialhodbx", email = "mautzzwr@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "Slanievien KapoccanceJX", email = "yyyomr3vs@bitrixmail.tk", aref = "" });
            register.Add(new LPC { fullname = "useqefhikos uvjanadunadu", email = "alejlona@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "ewukatanupipk odogupue", email = "eqicika@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ijepekakakuqo elimigu", email = "arurpel@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "eyaoqoulowam oconisij", email = "icusjunu@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "pbeqimwokuxuh axiwufifu", email = "euseeg@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ejufapmavon cyinirsanud", email = "apewyu@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "odefucuciror iedajoqeyu", email = "imavub@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "irucivo ebuwohe", email = "fiayuzuef@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "pjoiewpulov efivuuru", email = "oqqodada@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "unuzacet ipexuazafapt", email = "xujiqot@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "adarwekianahu iregiejisud", email = "elahugow@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ogeluho ijoxoxugax", email = "rujufau@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "umuogewede inahovorij", email = "uzeivori@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "ohitapoclo okojime", email = "zupioqovy@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "enadixax ipeloievajo", email = "anunewa@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "MichaelPef MichaelPefCX", email = "pro773@wegas.ru", aref = "" });
            register.Add(new LPC { fullname = "efeubetuxor ocizotudew", email = "ahiwjejep@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "kozizaliom comfiaxof", email = "evasuw@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "obaruhieoni otigutouvig", email = "ahaqol@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "ikanecav edudjoti", email = "oviohaq@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "Abdul rasheed  matemilola", email = "matemilolar@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "uyogihu uupoleriwiye", email = "uberjif@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "uxuosiroseoc ujibuvc", email = "atequduqo@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "usxofat olohiqmoqi", email = "ajeerusav@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "igesofose jeriyukezunar", email = "uhaquvepe@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "udafeiro naoagoejehijo", email = "ugudocemi@mailsdfsdf.com", aref = "" });
            register.Add(new LPC { fullname = "eonpega ukzulaciy", email = "ocbilu@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "toyuxumu ucevehik", email = "eyopufji@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "izolijuyubauy iduzodey", email = "uuajoe@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "ehivemuyupa oyqaasaseide", email = "eheuak@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "cefuofuwu ibdguzuul", email = "erejasilu@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "redamentee addisegegoJX", email = "Palsheat@academmail.info", aref = "" });
            register.Add(new LPC { fullname = "InnogsHago PealglanceJX", email = "MAYPOZY@acixmail.info", aref = "" });
            register.Add(new LPC { fullname = "Ibrahim adebayor", email = "darethompson37@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "AdalinBub AdalinBubZI", email = "shaecaveneyvpbu@hotmail.com", aref = "" });
            register.Add(new LPC { fullname = "ojixboclexo iceaacohe", email = "abolidayi@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "ioyojihoabiti ugpeepanir", email = "olubewenq@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ivetikivavine ixecuofebusux", email = "ecatan@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "ahrodenoj eguxuhosun", email = "isopira@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "ibwenazuh obeyokojihix", email = "cemupzeke@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "acabaitan usuyeno", email = "awasuv@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "eciwsuh duyitoqae", email = "awiedudu@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "otizodehifasa adiqorefipuhu", email = "eqixuy@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "ufukticuwew adapoje", email = "acudaqov@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "exulugyaqajo eyobazxi", email = "opugauhu@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ofocuqirede ahiunaqewo", email = "ocolele@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "oequqig ixusotoyec", email = "amipen@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "ukoweqoroku emutuvaj", email = "ixzirirom@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "aqolsilip zijegaslime", email = "eyeamehig@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "vexonoloeye uawahug", email = "omovapao@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "litzialahesij usotelimehok", email = "oygabutok@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "EL-MONSOR Shobowale-Bakre", email = "monsor@hotmail.co.uk", aref = "" });
            register.Add(new LPC { fullname = "niducysex ouhusawilo", email = "daleemp@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "alipikeqac ukelrfeka", email = "omajuc@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "ebixapitu ebeyalula", email = "ubmuxoeuq@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "avetezehw ootezaniganas", email = "amvugen@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "egejadoyojril uxolihu", email = "uqezoiy@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "eciuvefu erohigetiawe", email = "afiecugem@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "acooliqa ejataritalapi", email = "udoyuk@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "ovahqluuabus oguozawtu", email = "ayepviyu@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "iwujohez amizifoqenit", email = "acezuwe@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "eyaqoxeqapa inzarewtaz", email = "ngenam@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ibezusibope avtewanm", email = "eyazesq@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "itinoxec ebicoki", email = "uvuumolum@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "oridabo ejodofulo", email = "equseni@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "izifoquw utisidego", email = "reguveci@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "asedohaxeioy aniuwecej", email = "qznasac@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "afoziej amozocoisi", email = "utohib@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "ilijiqaqiele veziworaele", email = "ejibyih@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "uwotigbok epukusipovoce", email = "omwiqeyob@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "Sypeenape Issuextfef", email = "Illuctpum@mail-jim.ga", aref = "" });
            register.Add(new LPC { fullname = "Onenenfops caginvins", email = "Cucheli@mail-jim.gq", aref = "" });
            register.Add(new LPC { fullname = "uecaxiqihez uxatirutukah", email = "udcjaun@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "udretamelo ixuwogeaen", email = "vuuvdovo@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "aeragila ahoyidikfai", email = "evoezipeg@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "otingjouqu ilhdomugamem", email = "esovenukc@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "ohavoquxe egekugapezeg", email = "oxixebako@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "elelokiyenig obuvusut", email = "ipajegtc@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "esinosixeq afhiqute", email = "ujewamu@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "papijumiila usoxobekakabi", email = "ikinbly@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "epgvovadk okayudepukumu", email = "pzafokup@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "ocuxihodizutr ezocebadi", email = "owogoko@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "nueiqoi jiwaazadoquya", email = "ilidiw@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "pirouyyej isorafil", email = "ameqenobz@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "eckatobibun ugoxesul", email = "ufemite@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "iusurosej iuoweviidacu", email = "owojax@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "etokgiaro ineyaxem", email = "iodauq@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "itotoha uhoxaikugi", email = "uwinin@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "isefoqaaxooup tadihaejofa", email = "okayino@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "uibibimafifuj culafofz", email = "ejiceyib@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "eqouyiopep okihuacayacuw", email = "ewuippi@mailapso.com", aref = "" });
            register.Add(new LPC { fullname = "azacuporowt iroxcii", email = "uzaboa@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "ofakizoyevv ostoaonet", email = "uujxizu@mailasdkr.com", aref = "" });
            register.Add(new LPC { fullname = "uzugaziyit uqojefuoqap", email = "ehuken@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "awonodijero asamoece", email = "unemafufo@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "azerovem aosecibxeneu", email = "veeoee@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "einowiyerumin oyumefovue", email = "otuxijahr@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "ibuiqomoeo ayewuru", email = "ucizoj@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "osuzamuteano eohusru", email = "orexove@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "eulocenkov ioecnpeduxoje", email = "oshowuga@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "uvisezizuj evataqagayz", email = "awayon@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "oganojaloxapi iyerakuwoxey", email = "axuliv@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "asuofetoch ihurikad", email = "alehihih@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "zituniet pejikiyozezeg", email = "epanuva@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "uzeziwicupev ucudurojepum", email = "eydourux@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "eiegideti imeximyuriri", email = "enarecaos@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ekamurlaxobok olovaureqam", email = "ukovyoyum@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "uodizijupo icuwouqeli", email = "uputepag@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "obejimnud uudvunulitaiy", email = "acibaho@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "aboguju ojulofoufamo", email = "rirupihce@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "eyabednag acokeja", email = "ojuyaiw@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "ibuounem egapefe", email = "otusicok@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ecihuqi konumibev", email = "qituxopag@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "vincenzokonnop8s gusmorock57e", email = "mautzzwr@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "ifokoko aceegumolu", email = "enlcipo@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "aqijtummif hitocalusoj", email = "ozoxag@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "edolohewatiko ucelenab", email = "anulojo@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "uwipoviuvacga unoheqe", email = "isoyukw@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "aguyeibew idudajago", email = "afufiu@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "iahogehif eazucogud", email = "afomao@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "izupdebezu izuravoroc", email = "upkvis@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "araecopuxa uhafugozaliyq", email = "avurayege@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "ekujolucula buqeexaxo", email = "azuqkuvog@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "azaqixusitar iyejaxuhuwili", email = "eirnatixu@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "emogibofapw iwozopaw", email = "esocporo@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "erhuilabivuw inuzesuifeh", email = "umaquv@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "ajofapasuake oluhrimcadihi", email = "obujae@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "igivaifuofe zoburikej", email = "oterapsuv@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "afiuqaruta ajuzqaxufubu", email = "ocoamy@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "egicebuhaliki ikofievaso", email = "ecuwanu@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "oqjekay uoyonimaki", email = "tilidedob@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "inaazojaje ufdpamoyixugo", email = "orohaw@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ouwuguwahtuck uebosiv", email = "umakadoo@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "apuzogoba uyawumoz", email = "ixoruha@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ialiwenryatua ejorohiwoocq", email = "oqupiu@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "orecousav izaeqqeuguq", email = "ehowule@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "afexoge uefimevucilu", email = "ufimerii@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "iodugomis ojofabib", email = "zozuso@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "oquzepucewe ibutorus", email = "kekawi@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "ewixilidiyy ufuhivuse", email = "ofotatek@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "eqeqeqe eqeqeqe", email = "eqeqeq@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "udaepona olahejozhuqah", email = "ipexipofo@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "usilujusf uruquzu", email = "asevavt@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "izidhavetaq risesap", email = "dkanni@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "eokofuu exsaslbwi", email = "eazeyebub@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "foejaxi eidabor", email = "axivepixz@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "izaguguho udamedi", email = "oaparocz@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "efitivuyaaetw apubuvkasiu", email = "azzoym@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "Fatima Salaam", email = "busfat01@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "Fatima salaam", email = "busfat01@yahoo.co.uk", aref = "" });
            register.Add(new LPC { fullname = "uqiutafs ajumohef", email = "zarina@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ihelpoxa eapiroleb", email = "avucef@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ilovagbupu ivixogar", email = "emasaw@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "erahenaju ejioxedawo", email = "ofabodl@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "aharaka azkufavik", email = "widoyota@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "kjodeivel tudubue", email = "eioyir@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "Jamal  Hassen", email = "amanbella@hotmail.com", aref = "" });
            register.Add(new LPC { fullname = "Jamal  Hassen", email = "amanbella@hotmail.com", aref = "" });
            register.Add(new LPC { fullname = "rtadiaq epuyeruu", email = "ebuzifuo@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "efumali ikoyikig", email = "uavaspon@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "urayawdutow ajenapast", email = "eibimet@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "ehezeje ofoixaqobo", email = "ogykap@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "mimepugiface oaigunauwapop", email = "exeyefgov@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ikaxahuqo nanoorunu", email = "icequinax@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "alaynayamatqaa katlynmacclairtyzg0", email = "weibelsph@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "etekukecisubu uujieluyviuhm", email = "owipioqee@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "nobuxgepeb qbexozk", email = "oritcob@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "areyolidemomo afvugeyoucu", email = "oveiowiha@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "eteroutazek ahoumod", email = "ojowinih@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "emruxiwule oyunanuno", email = "ivuamugar@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "utodeqekun ipenojowi", email = "isicago@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "iqiveawel irafehofanoy", email = "uxelalil@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "ugikosufi upusaowidugo", email = "oyebojuvo@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "ikirafufikib alazeluoya", email = "uhiqey@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "okjideliyemi uqmuuxzudu", email = "eenepewal@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "aefikadowic evumaxazi", email = "icuqoj@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "ixaliti ubuyayat", email = "jejofezjo@rtotlmail.com", aref = "" });
            register.Add(new LPC { fullname = "corriemebaneftb mauriceworsfold635", email = "glaeserapb@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "aqoneficex eevugah", email = "iriyuta@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "egeditoy ooliloqewele", email = "oluvag@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "iyiyejletab aavolebiceifo", email = "iquyoquse@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ofuvatikufli axeviyufey", email = "oiyuwuiuy@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "laurindabyrerg94 melidahabermanxlu", email = "barksdalehz@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "igegoqow epefoniqexov", email = "oduzabone@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "omibakuza iisoigqaz", email = "ijuqoyu@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "emafapofoqoa ewoquqe", email = "usuiwoc@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "iyasipeyobik urgijxuw", email = "ofdalavin@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "nitoqupepoyo oquqelel", email = "utpauu@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "igifecanifo iyilaloziyef", email = "afiymo@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "obuderellue unuuhitunubic", email = "otayedi@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "oropega iwexapun", email = "agoniha@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ilalizorapsir ogifumn", email = "sonuiw@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "fosxoupo xipujumihue", email = "eahogec@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "ozoafupemed gzapudezelu", email = "uqocas@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "ayipojux amafehuwonuc", email = "emijax@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "uyokofxuze ecifgubu", email = "umocoxed@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ipobexu ipufisoyuoqe", email = "iyoluyaq@mailsdfsdf.net", aref = "" });
            register.Add(new LPC { fullname = "uvoharvama onaxepozunu", email = "tanupa@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ufexuberofa uiqakaluamai", email = "imahodi@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "uxapaiyefj oyuvupewcumi", email = "igafipisi@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "Muhammad ahmad", email = "mam.7472@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "oqqepoikral uwacelefuc", email = "yogewo@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "ilmjufofdub iwnivisukae", email = "ecemxu@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "abufeqhuon urxencosun", email = "uduffime@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "ufnazovgti ezenexazubor", email = "akafisag@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "oxasikede oluyuadeqavu", email = "aloqkabog@qwkcmail.net", aref = "" });
            register.Add(new LPC { fullname = "iwobeyusufu uobifidiyi", email = "abovuqafu@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "theabertic0c claudiedurkeedp9", email = "borgheserx@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "paulinehillwig4q1 ericnaysmithlsj", email = "sealsoite@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "ozoculpiujug aegojim", email = "ihovufe@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "uvenaxamobium ogugasuhu", email = "ahuziz@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "eaqumuitapiri ovweguzanwoc", email = "efifeboq@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "uezidujala onovfer", email = "efugera@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "ewezexinhucex ukumijzjuca", email = "ixeviopu@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "izuzotefi acivodeyocov", email = "ubikofa@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "ojorebeo esorleten", email = "epujnuwe@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "Elderrard Fotretedob", email = "Fupplold@mail-vix.ml", aref = "" });
            register.Add(new LPC { fullname = "virgenchladek13r emilioarmorl1l", email = "barksdalehz@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "Geangenide Engeriecig", email = "pseumMem@mail-vix.ml", aref = "" });
            register.Add(new LPC { fullname = "Fodertvah FodertvahLW", email = "dankufoz@yandex.com", aref = "" });
            register.Add(new LPC { fullname = "Ibraheem Abdul fatah", email = "ojediranibraheemadekunle@yahoo.com", aref = "" });
            register.Add(new LPC { fullname = "Ramon  Emiabata", email = "emiabata@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "utujazu ekukacjahizx", email = "uveyak@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ulivisivuk afacadbukez", email = "adosokja@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "MNatoshaKt MNatoshaKtEE", email = "fredquimbyx235+10@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "FaylondQuop FaylondQuopKD", email = "kubrafit@yandex.com", aref = "" });
            register.Add(new LPC { fullname = "RichardSed RichardSedSJ", email = "jdanadorofeeva@mail.ru", aref = "" });
            register.Add(new LPC { fullname = "ukafowva fimafarani", email = "onemfixur@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "aeuxuqugixo auraficepebam", email = "irujageg@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "izkubuji tinividazfi", email = "izupox@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "afecuza ofujefat", email = "eqeyoy@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "idayuduqa obagihiero", email = "ewomicaco@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "egaineq ajikamapo", email = "uqagoju@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "oojozol imusegedul", email = "igiqiq@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "iqepirezesin ofexjeyod", email = "okemooho@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ikoqsagezams mutijeka", email = "aberaci@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "aviyufehan uiqabaituihl", email = "ojofoya@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "uccugicivahi izugehokasu", email = "ukdiro@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "relolijixuqe ihoavaja", email = "iseyuaav@apoimail.com", aref = "" });
            register.Add(new LPC { fullname = "ulugaevuxagu ikejudipcer", email = "ozojeijeg@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "etrsuzo uzuzuyc", email = "uvijsari@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "itujipaadon ozaxlecc", email = "eyemlu@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "iezziwigu evibithie", email = "ivijecoxi@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "azukosan ebijenoso", email = "udubanox@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "eneyelsabap iufufernuvi", email = "iziqodoms@fghmail.net", aref = "" });
            register.Add(new LPC { fullname = "efohowo ugbofaftigzuo", email = "amwixeagu@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "evevufe ererutubutaz", email = "wasawul@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "upigamefidu ivedudis", email = "ovofujud@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "acyesipi onaxesijujoya", email = "oesuyi@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "unefacil azicaweta", email = "unezoig@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ucixegie ahasejuruti", email = "epivoi@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ehebeguuju icojihuaha", email = "auqipequl@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "ioxudojofxe orexediasevuh", email = "uciyex@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "udekoquvemui ekeraideuxo", email = "okabonomo@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "odduhewaqa eniyijomikeza", email = "afijeutar@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "luisezirin1gq carripetkafna", email = "glaeserapb@outlook.com", aref = "" });
            register.Add(new LPC { fullname = "uwosicosobibi ifipaxupe", email = "epbawu@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "aruiwizi ihivujo", email = "uaqudal@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "iihokovuzenev ecekumozuxa", email = "apavajka@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "uqgomixoiy amuezejepuy", email = "izimifepo@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "owopoomutoq ahonuxak", email = "emaulzuv@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "dohekqocoi atuzacawuq", email = "bouhoqi@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "orepoceh uzelamu", email = "iepuanic@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "onuracp idewoyonus", email = "ubimiera@asdfasdfmail.com", aref = "" });
            register.Add(new LPC { fullname = "ubuloin aleheyfeyavo", email = "avkupauqo@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "omiceyi uvogpuwipunj", email = "aidazaroc@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "ibirogtiru eeebuob", email = "unotadu@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "akumupeye uzauyxas", email = "ofdiujiw@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "aqisiweyesiea exulaemo", email = "zedoriti@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "irojiyuol utisajotgoh", email = "ikiputulb@asdooeemail.com", aref = "" });
            register.Add(new LPC { fullname = "obimarayoy eceokuqoda", email = "itqisex@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ucodelee eboguabziciqa", email = "amenezik@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ejinaqonep ojqakegoq", email = "aduwzup@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "ekaoexinupu aawogosicezi", email = "genuyaves@dfoofmail.com", aref = "" });
            register.Add(new LPC { fullname = "opolveobak efuxediwacu", email = "azasog@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "eudipiafnu afxemziqadun", email = "epibexet@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "usumohopa agebanirohuqu", email = "ecukopike@asdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "idivimahihi epyidefo", email = "iyacoqa@asooemail.net", aref = "" });
            register.Add(new LPC { fullname = "amtemupulcad egewamiga", email = "fegewuj@toerkmail.com", aref = "" });
            register.Add(new LPC { fullname = "isma'il ibrahim", email = "abbahgafai@gmail.com", aref = "" });
            register.Add(new LPC { fullname = "igairiveluj umemoyvoz", email = "isuwew@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "iufizeruige uginepiiuye", email = "aoginoqam@asooemail.com", aref = "" });
            register.Add(new LPC { fullname = "uwutijefom rihirucub", email = "okecow@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "eziveala uyaqakipli", email = "acuera@qwkcmail.com", aref = "" });
            register.Add(new LPC { fullname = "izeevali zaxeqevetif", email = "egaefapo@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "opinugupfayol unujupihax", email = "exesucuuw@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "iyovsisud arivacowdib", email = "ifadira@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "asikieyiduyim ihagapish", email = "awtuza@rtotlmail.net", aref = "" });
            register.Add(new LPC { fullname = "uqbovus anequfbouehil", email = "eguqoso@asdfasdfmail.net", aref = "" });
            register.Add(new LPC { fullname = "ubewoyadep ovamuqejzeze", email = "atekeres@asdfasdfmail.net", aref = "" });
            return register;

        }

        public string test()
        {

            YouTubeModel ytm = new YouTubeModel();
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
                ytm = serialise.Deserialize<YouTubeModel>(jsonResult);

            }
            catch (Exception ex)
            {

            }
            StringBuilder ysb = new StringBuilder();
            foreach (var item in ytm.items)
            {
                //ysb.AppendFormat("<a target=\"_blank\" href=\"https://www.youtube.com/watch?v={0}\"><span class=\"play\">&nbsp;</span><img src=\"{1}\" /></a>&nbsp;&nbsp;&nbsp;", sytm.snippet.resourceId.videoId, sytm.snippet.thumbnails.medium.url);
                string imgVideoLnk = item.snippet.thumbnails.medium.url;
                string videolink = "https://www.youtube.com/watch?v=" + item.snippet.resourceId.videoId;

                //"//img.youtube.com/vi/" + item.videolink.Split('=')[1] + "/0.jpg";

                ysb.Append("<div class=\"col-md-3\">");
                ysb.Append("<div class=\"panel panel-info\">");
                ysb.Append("<div class=\"panel-body\">");
                ysb.AppendFormat("<a target=\"_blank\" href=\"https://www.youtube.com/watch?v={0}\"><img src=\"{1}\" /></a>&nbsp;&nbsp;&nbsp;", item.snippet.resourceId.videoId, item.snippet.thumbnails.medium.url);
                ysb.AppendFormat("<a target=\"_blank\" href=\"https://www.youtube.com/watch?v={0}\">{1}</a>", item.snippet.resourceId.videoId, item.snippet.title);
                ysb.Append("</div>");
                ysb.Append("</div>");
                ysb.Append("</div>");
            }




            var strLogo = Path.Combine(Config.website + "/EmailTemp/image/logo.png");
            var strYT = Path.Combine(Config.website + "/EmailTemp/image/48x48_youtube.png");
            var strEidinpark = Path.Combine(Config.website + "/EmailTemp/image/slide1.png");
            var strAppeal = Path.Combine(Config.website + "/EmailTemp/image/new_masjid_slide.jpg");
            var strTwitter = Path.Combine(Config.website + "/EmailTemp/image/48x48_twitter.png");
            var strFacebook = Path.Combine(Config.website + "/EmailTemp/image/48x48_facebook.png");



            //string[] ema = { "jamiuaz@yahoo.co.uk", "contact@jamiuaz.net" };
            //string[] eman = { "Jamiu Ayinde", "Jamiu Ayinde" };

            //try
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        strBody = strBody.Replace("%FULLNAME%", eman[i]);
            //        Email.Send("mailing@manuk.org", "MANUK Mailing List", ema[i], "Mailing List Notification", strBody, true);
            //    }

            //    return "Success";
            //}
            //catch (Exception ex)
            //{
            //    return "Fail " + ex.ToString();
            //}

            StringBuilder msb = new StringBuilder();

            List<UpdateList> lupList = db.UpdateLists.ToList();

            try
            {
                foreach (var xx in lupList)
                {
                    var strBody = System.IO.File.ReadAllText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~/EmailTemp/email.txt")));
                    strBody = strBody.Replace("%strLogo%", strLogo);
                    strBody = strBody.Replace("%VIDEOS%", ysb.ToString());
                    strBody = strBody.Replace("%EIDINPARK%", strEidinpark);
                    strBody = strBody.Replace("%APPEAL%", strAppeal);
                    strBody = strBody.Replace("%strYT%", strYT);
                    strBody = strBody.Replace("%strTwitter%", strTwitter);
                    strBody = strBody.Replace("%strFacebook%", strFacebook);
                    strBody = strBody.Replace("%FULLNAME%", xx.fullname);
                    msb.Append(xx.email + "<br/>" + strBody + "<br/><br/><br/>");
                    // Email.Send("mailing@manuk.org", "MANUK Mailing List", xx.email, "Mailing List Notification", strBody, true);
                }

                return "Success" + "<br/>" + msb.ToString();
            }
            catch (Exception ex)
            {
                return "Fail " + ex.ToString();
            }





            //// IEnumerable<string> lstStr = System.IO.File.ReadLines(path);

            //StringBuilder sb = new StringBuilder();
            //try
            //{
            //    foreach (string xx in System.IO.File.ReadLines(path))
            //    {
            //        string[] yy = xx.Split('/');
            //        string names = string.Empty;
            //        string mail = string.Empty;

            //        if (yy.Length > 0 && !yy[0].Contains("@"))
            //        {
            //            names = yy[0];
            //        }
            //        else if (yy.Length > 0 && yy[0].Contains("@"))
            //        {
            //            mail = yy[0];
            //        }

            //        if (yy.Length > 1 && !yy[1].Contains("@"))
            //        {
            //            names = names + " " + yy[1];
            //        }
            //        else if (yy.Length > 1 && yy[1].Contains("@"))
            //        {
            //            mail = yy[1];
            //        }

            //        if (yy.Length > 2 && !yy[2].Contains("@"))
            //        {
            //            names = names + " " + yy[2];
            //        }
            //        else if (yy.Length > 2 && yy[2].Contains("@"))
            //        {
            //            mail = yy[2];
            //        }

            //        if (yy.Length > 3 && !yy[3].Contains("@"))
            //        {
            //            names = names + " " + yy[3];
            //        }
            //        else if (yy.Length > 3 && yy[3].Contains("@"))
            //        {
            //            mail = yy[3];
            //        }

            //        if (yy.Length > 4 && !yy[4].Contains("@"))
            //        {
            //            names = names + " " + yy[4];
            //        }
            //        else if (yy.Length > 4 && yy[4].Contains("@"))
            //        {
            //            mail = yy[4];
            //        }
            //        //sb.AppendFormat("name : {0} ", names + "<br/>");
            //        //sb.AppendFormat("email : {0} ", mail + "<br/><br/>");

            //        List<UpdateList> lsUp = db.UpdateLists.Where(x => x.email == mail).ToList();

            //        if (lsUp.Count < 1)
            //        {
            //            db.UpdateLists.Add(new UpdateList { fullname = names, email = mail, subscribe = true, datecreated = DateTime.Now });
            //            db.SaveChanges();
            //            sb.Append( "added : " + mail + "<br/>");
            //        }
            //        else
            //        {
            //            sb.Append( "exists : " + mail + "<br/>");
            //        }
            //    }


            //    return sb.ToString();

            //    //db.UpdateLists.Add(new UpdateList { fullname = us.FirstName + " " + us.LastName, email = us.Email, subscribe = true, datecreated = DateTime.Now });
            //    //db.SaveChanges();

            //    //strBody = strBody.Replace("%FULLNAME%", eman[i]);
            //    //Email.Send("mailing@manuk.org", "MANUK Mailing List", ema[i], "Mailing List Notification", strBody, true);



            //}
            //catch (Exception ex)
            //{
            //    return "Fail " + ex.ToString();
            //}


            //try
            //{
            //    List<UserProfile> lstx = db.UserProfiles.ToList();

            //    foreach (var us in lstx)
            //    {
            //        db.UpdateLists.Add(new UpdateList { fullname = us.FirstName + " " + us.LastName, email = us.Email, subscribe = true, datecreated = DateTime.Now });
            //        db.SaveChanges();
            //    }

            //    return "success";
            //}
            //catch(Exception ex)
            //{
            //    return "fail " + ex.ToString();
            //}




            //for(int i = 0; i < 10; i++)
            //{
            //    //var xx = new content_news();
            //    //xx.newstitle = "New " + i;
            //    //xx.news = "orem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Typi non habent claritatem insitam; est usus legentis in iis qui facit eorum claritatem. Investigationes demonstraverunt lectores legere me lius quod ii legunt saepius. Claritas est etiam processus dynamicus, qui sequitur mutationem consuetudium lectorum. Mirum est notare quam littera gothica, quam nunc putamus parum claram, anteposuerit litterarum formas humanitatis per seacula quarta decima et quinta decima. Eodem modo typi, qui nunc nobis videntur parum clari, fiant sollemnes in futurum.";
            //    //xx.newsdate = DateTime.Now;
            //    //xx.enable = true;
            //    //xx.datecreated = DateTime.Now;
            //    //db.content_news.Add(xx);
            //    //db.SaveChanges();

            //    var yy = new content_article();
            //    yy.articletitle = "Article " + i;
            //    yy.article = "orem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Typi non habent claritatem insitam; est usus legentis in iis qui facit eorum claritatem. Investigationes demonstraverunt lectores legere me lius quod ii legunt saepius. Claritas est etiam processus dynamicus, qui sequitur mutationem consuetudium lectorum. Mirum est notare quam littera gothica, quam nunc putamus parum claram, anteposuerit litterarum formas humanitatis per seacula quarta decima et quinta decima. Eodem modo typi, qui nunc nobis videntur parum clari, fiant sollemnes in futurum.";
            //    yy.articleby = "Article By " + i;
            //    yy.articledate = DateTime.Now;
            //    yy.enable = true;
            //    yy.datecreated = DateTime.Now;
            //    db.content_article.Add(yy);
            //    db.SaveChanges();
            //}

            //var xx = Config.name + "<br>" + Config.email +
            //    "<br>" + Config.address + "<br>" + Config.registerno +
            //    "<br>" + Config.telephone + "<br>" + Config.website +
            //    "<br>" + Config.postcode + "<br>" + Config.nameAbr;
            //return "result " + xx;
        }
    }

    public class LPC
    {
        public string fullname { get; set; }
        public string email { get; set; }
        public string aref { get; set; }
    }
}
