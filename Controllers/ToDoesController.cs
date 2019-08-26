using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDoList.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;


using System.Web.UI.WebControls;

using Hangfire;
using System.Web.UI;

namespace ToDoList.Controllers
{
    public class ToDoesController : Controller
    {
        static int DeadlineExpired = 0;
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: ToDoes
        public ActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("Index");

            if (DeadlineExpired == 1){
                ViewBag.Message = "Your item expired !!!";
                DeadlineExpired = 0;
            }
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(
                x => x.Id == currentUserId);
            return View(db.Todos.ToList().Where(x => x.User == currentUser));
        }

        private IEnumerable<ToDo> GetMyTodoes()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(
                x => x.Id == currentUserId);
            return db.Todos.ToList().Where(x => x.User == currentUser);
        }

        public ActionResult BuildToDoTable()
        {
            return PartialView("_ToDoTable", GetMyTodoes());
        }

        // GET: ToDoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.Todos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // GET: ToDoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Deadline")] ToDo toDo)
        {

            if (ModelState.IsValid)
            {
                string currentUserId = (User.Identity.GetUserId());

                ApplicationUser currentUser = db.Users.FirstOrDefault(
                    x => x.Id == currentUserId);
                toDo.User = currentUser;
                toDo.IsDone = false;
                db.Todos.Add(toDo);
                db.SaveChanges();

                //Enqueue Hangfire Message 
                var jobId = BackgroundJob.Schedule(() => SendDeadlineMessage(toDo.Id), TimeSpan.Parse(GetTimeSpan(DateTime.Now, toDo.Deadline)));
                return RedirectToAction("Index");
            }


            return View(toDo);
        }
        

        public ActionResult SendDeadlineMessage(int Id)
        {
            DeadlineExpired = 1;
            System.Diagnostics.Debug.WriteLine("SendDeadlineMessage - ToDo Id : "+ Id);
            return RedirectToAction("Index");
        }

        public String GetTimeSpan(DateTime now, DateTime deadline)
        {
            System.Diagnostics.Debug.WriteLine("Now : " + now);
            System.Diagnostics.Debug.WriteLine("Deadline : " + deadline);
            TimeSpan diff = deadline - now;
            string timeSpan = diff.Days + "." + diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds;
            System.Diagnostics.Debug.WriteLine("TimeSpan : " + timeSpan);

            return timeSpan;
        }



        // GET: ToDoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.Todos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,IsDone,Deadline")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDo);
        }

        [HttpPost]
        public ActionResult AJAXEdit(int? id, bool value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.Todos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else
            {
                toDo.IsDone = value;
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
            }
            return PartialView("_ToDoTable", GetMyTodoes());
        }

        // GET: ToDoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.Todos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToDo toDo = db.Todos.Find(id);
            db.Todos.Remove(toDo);
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
