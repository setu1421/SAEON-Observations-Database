using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("UserQueries")]
    public class UserQueriesController : BaseController
    {
        // GET: UserQueries
        [Route]
        public ActionResult Index()
        {
            return View(db.UserQueries.Where(i => i.UserId == User.Identity.Name).OrderBy(i => i.Name).ToList());
        }

        // GET: UserQueries/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserQueries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserQueries/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserQueries/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserQueries/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserQueries/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserQueries/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
