using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using Portfolio_Blog.Helpers;
using Portfolio_Blog.Models;

using PagedList.Mvc;

namespace Portfolio_Blog.Controllers
{

    [Authorize(Roles = "Admin,Moderator")]


    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Search search = new Search();

        // GET: BlogPosts
        //? after int makes it nullable, ints not nullable by default
        [AllowAnonymous]
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = search.IndexSearch(searchStr);

            int pageSize = 3; //displays 3 blogposts at a time on this page
            int pageNumber = (page ?? 1); //means that if page comes in as null, it sets the value to 1

    
            //return View(db.Posts.Where(foo => foo.IsPublished).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
            return View(blogList.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult PublishedIndex()
        {
            return View("Index", db.Posts.Where(foo => foo.IsPublished).OrderByDescending(b => b.Created).ToList());
        }


        //public IQueryable<BlogPost> IndexSearch(string searchStr)
        //{
        //    IQueryable<BlogPost> result = null;
        //    if (searchStr != null)
        //    {
        //        result = db.Posts.AsQueryable();
        //        result = result.Where(p => p.Title.Contains(searchStr) ||
        //                                p.Body.Contains(searchStr) ||
        //                                p.Comments.Any(c => c.Body.Contains(searchStr) ||
        //                                c.Author.FirstName.Contains(searchStr) ||
        //                                c.Author.LastName.Contains(searchStr) ||
        //                                c.Author.DisplayName.Contains(searchStr) ||
        //                                c.Author.Email.Contains(searchStr)));

        //    }
        //    else
        //    {
        //        result = db.Posts.AsQueryable();
        //    }
        //    return result.OrderByDescending(p => p.Created);
        //}











        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public ActionResult Details(string Slug)
        {

            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);

        }

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Abstract,Body,MediaUrl,IsPublished")] BlogPost blogPost, HttpPostedFileBase image)
        {
            try
            {

                
                if (ModelState.IsValid)
                {
                    //How I instantiate an object of type StringUtilities
                    if (ImageUploadValidator.IsWebFriendlyImage(image))
                    {
                        //isolate file name
                        var justFileName = Path.GetFileNameWithoutExtension(image.FileName);
                        //run filename without ext through slugger
                        justFileName = StringUtilities.URLFriendly(justFileName);

                        //finally add unique date stamp
                        justFileName = $"{justFileName}-{DateTime.Now.Ticks}";

                        justFileName = $"{justFileName}{Path.GetExtension(image.FileName)}";



                        //var fileName = Path.GetFileName(image.FileName);
                        image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), justFileName));

                        blogPost.MediaUrl = "/Uploads/" + justFileName;
                    }





                    var Slug = StringUtilities.URLFriendly(blogPost.Title);
                    if (String.IsNullOrWhiteSpace(Slug))
                    {
                        ModelState.AddModelError("title", "invalid title");
                        return View(blogPost);
                    }
                    if (db.Posts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("title", "the title must be unique");
                        return View(blogPost);
                    }
                    
                    blogPost.Slug = Slug;
                    blogPost.Created = DateTime.Now;
                    db.Posts.Add(blogPost);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system admin.");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Abstract,Body,MediaUrl,IsPublished")] BlogPost blogPost, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    //isolate file name
                    var justFileName = Path.GetFileNameWithoutExtension(image.FileName);
                    //run filename without ext through slugger
                    justFileName = StringUtilities.URLFriendly(justFileName);

                    //finally add unique date stamp
                    justFileName = $"{justFileName}-{DateTime.Now.Ticks}";

                    justFileName = $"{justFileName}{Path.GetExtension(image.FileName)}";



                    //var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), justFileName));

                    blogPost.MediaUrl = "/Uploads/" + justFileName;
                }


                var slug = StringUtilities.URLFriendly(blogPost.Title);
                if (blogPost.Slug != slug)
                {
                    if (string.IsNullOrWhiteSpace(slug))
                    {
                        ModelState.AddModelError("Title", "Oops, the Title cannot be empty!");
                        return View(blogPost);
                    }
                    if (db.Posts.Any(p => p.Slug == slug))
                    {
                        ModelState.AddModelError("title", "the title must be unique");
                        return View(blogPost);
                    }
                }


                blogPost.Updated = DateTime.Now;

                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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
