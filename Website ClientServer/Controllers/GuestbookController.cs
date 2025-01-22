using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class GuestbookController : Controller
    {
        private readonly GuestBookContext _db = new GuestBookContext();
        // GET: Guestbook
        
        public ActionResult Index()
        {

     
            {
                var mostRecentEntries = (from entry in _db.Entries
                                         orderby entry.DateAdded descending
                                         select entry).Take(20).ToList();

                ViewBag.Entries = mostRecentEntries;
                return View();
            }
        }
        [Authorize]
        public ActionResult Create()
        {

            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create(GuestBookEntry entry)
        {
            if (ModelState.IsValid)
            {
                entry.DateAdded = DateTime.Now;
                entry.Email=User.Identity.Name;
                _db.Entries.Add(entry);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entry);
        }
        [AllowAnonymous]
        public ActionResult CommentsSummary()
        {
            var entries = from entry in _db.Entries
                          group entry by entry.Name into groupedByName
                          orderby groupedByName.Count() descending
                          select new WebApplication1.Models.CommentsSummary
                          {
                              numComments = groupedByName.Count(),
                              username = groupedByName.Key
                          };
            return View(entries.ToList());

        }
     
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var entry = _db.Entries.Find(id);
           

            if (entry == null)
            {
                return HttpNotFound("The requested entry does not exist.");
            }

            if (entry.Email != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorized to edit this entry.");
            }

            return View(entry);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(GuestBookEntry model)
        {
      

            var entry = _db.Entries.Find(model.Id);
            //check if the entry exists
            if (entry == null)
            {
                System.Diagnostics.Debug.WriteLine("Entry not found.");
                return HttpNotFound("The requested comment does not exist.");
            }

            if (entry.Email != User.Identity.Name)
            {
                System.Diagnostics.Debug.WriteLine("Unauthorized edit attempt.");
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorized to edit this comment.");
            }

            if (ModelState.IsValid)
            {
                //update the entry values

                entry.Name = model.Name;
                entry.Message = model.Message;
                entry.DateAdded = DateTime.Now;
                _db.SaveChanges();

                //redirect to the MyComments 
                return RedirectToAction("MyComments", "Guestbook");
            }

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entry = _db.Entries.Find(id);

            if (entry == null)
            {
                return HttpNotFound("The requested comment does not exist.");
            }

            if (entry.Email != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorized to delete this comment.");
            }

            _db.Entries.Remove(entry);
            _db.SaveChanges();

            return RedirectToAction("MyComments");
        }

        [Authorize]
        public ActionResult MyComments()
        {
            var userEmail = User.Identity.Name;

            var userEntries = _db.Entries
                                 .Where(entry => entry.Email == userEmail)
                                 .OrderByDescending(entry => entry.DateAdded)
                                 .ToList();

            return View(userEntries);
        }

        [AllowAnonymous]
        public ViewResult Show(int id)
        {
            var entry = _db.Entries.Find(id);
            bool hasPermission = User.Identity.Name == entry.Email;
            ViewBag.hasPermission = hasPermission;
            return View(entry);
        }

        [AllowAnonymous]
        public ViewResult ShowAll()
        {
            var allEntries = _db.Entries.OrderByDescending(entry => entry.DateAdded).ToList();
            return View("ShowAll", allEntries);
        }






        [AllowAnonymous]
    public ActionResult Comments(string userName)
        {
            // return all comments
            if (string.IsNullOrEmpty(userName))
            {
                var allComments = _db.Entries.OrderByDescending(entry => entry.DateAdded).ToList();
                return View(allComments);
            }

            // Filter comments by username 
            var userComments = _db.Entries
                                  .Where(entry => entry.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
                                  .OrderByDescending(entry => entry.DateAdded)
                                  .ToList();

            return View(userComments);
        }
        [AllowAnonymous]
        public ActionResult CommentsByDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime selectedDate))
            {
                ViewBag.SelectedDate = selectedDate;
                var filteredComments = _db.Entries
                                          .Where(entry => DbFunctions.TruncateTime(entry.DateAdded) == selectedDate.Date)
                                          .OrderByDescending(entry => entry.DateAdded)
                                          .ToList();

                return View(filteredComments);
            }


            return RedirectToAction("Index");

        }


        }
}
