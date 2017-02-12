using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FitnessFrog.Data;
using FitnessFrog.Models;

namespace FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private readonly EntriesRepository entriesRepository;

        public EntriesController()
        {
            entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / numberOfActiveDays);
            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry
            {
                Date = DateTime.Today
            };
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            if (ModelState.IsValid)
            {
                entriesRepository.AddEntry(entry);
                TempData["Message"] = "Your entry was successfully added!";
                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = entriesRepository.GetEntry((int)id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            if (ModelState.IsValid)
            {
                entriesRepository.UpdateEntry(entry);
                TempData["Message"] = "Your entry was successfully updated!";
                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Entry entry = entriesRepository.GetEntry((int)id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            entriesRepository.DeleteEntry(id);
            TempData["Message"] = "Your entry was successfully deleted!";
            return RedirectToAction("Index");
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}