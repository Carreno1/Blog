using Portfolio_Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Portfolio_Blog.Controllers
{

    [RequireHttps]
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {

            //get all the BlogPosts that have been marked as published

            //return View(db.BlogPosts.Where(b => b.IsPublished).ToList());
            var model = db.Posts.Where(foo => foo.IsPublished).OrderByDescending(b => b.Created).ToList();


            return View(model);


        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold> ({1})</p><p>Message:</p><p>{2}</p>";

                    var from = $"Michael's Blog <{ WebConfigurationManager.AppSettings["emailfrom"]}/>";



                    model.Body = "This is a message from your portfolio site.  The name and the email of the contacting person is above.";


                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Portfolio Contact Email",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return View(new EmailModel());

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }

            }


            return View(model);
        }




    }
}