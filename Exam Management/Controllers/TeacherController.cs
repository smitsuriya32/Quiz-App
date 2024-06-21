
using Exam_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exam_Management.Controllers
{
    [Authorize] // Requires authorization to access this controller
    public class TeacherController : Controller
    {
        ExamManagementEntities db = new ExamManagementEntities(); // Database context



        // Action method for displaying the teacher dashboard
        public ActionResult Dashboard()
        {
            return View(); // Returns the Dashboard view
        }



        // Action method for managing papers created by the teacher
        public ActionResult PaperManager()
        {
            int userId = Convert.ToInt32(Session["Id"]); // Retrieves user ID from session
            var papers = db.Papers.Where(model => model.CreatedBy == userId).ToList(); // Retrieves papers created by the teacher
            return View(papers); // Returns the PaperManager view with the list of papers
        }



        // Action method for creating a new paper by the teacher
        public ActionResult CreatePaper()
        {
            return View(); // Returns the CreatePaper view
        }



        // POST: Action method for submitting paper creation by the teacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePaper(Paper p)
        {
            if (ModelState.IsValid == true)
            {
                p.CreationDate = DateTime.Now; // Sets CreationDate to current date and time
                p.Status = "Pending"; // Sets initial status of paper
                p.CreatedBy = Convert.ToInt32(Session["Id"]); // Sets CreatedBy to the current user's ID from session
                db.Papers.Add(p); // Adds paper to database
                int check = db.SaveChanges(); // Saves changes to database
                if (check > 0)
                {
                    TempData["PaperCreateMsg"] = true; // Sets success message
                    ModelState.Clear(); // Clears model state
                    return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager action
                }
                else
                {
                    TempData["PaperCreateMsg"] = false; // Sets error message
                }
            }
            return View(); // Returns CreatePaper view
        }



        // Action method for showing a specific paper to the teacher
        public ActionResult ShowPaper(int id)
        {
            TempData["PId"] = id; // Stores paper ID in TempData
            Paper p = db.Papers.Find(id); // Finds the paper by ID
            if (p == null)
            {
                return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager if paper is not found
            }
            var questions = db.Questions.Where(q => q.PaperId == id).ToList(); // Retrieves questions related to the paper
            ViewBag.ques = questions; // Passes questions to the view
            return View(p); // Returns the ShowPaper view with the paper and questions
        }



        // Action method for editing a paper by the teacher
        public ActionResult EditPaper(int id)
        {
            var row = db.Papers.Where(model => model.Id == id).FirstOrDefault(); // Retrieves paper by ID
            if (row == null)
            {
                return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager if paper is not found
            }
            return View(row); // Returns the EditPaper view with the paper details
        }



        // POST: Action method for submitting paper edit by the teacher
        [HttpPost]
        public ActionResult EditPaper(Paper p)
        {
            db.Entry(p).State = EntityState.Modified; // Sets the state of the entity to modified
            int a = db.SaveChanges(); // Saves changes to the database
            if (a > 0)
            {
                TempData["PaperUpdateMsg"] = true; // Sets success message
                return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager action
            }
            else
            {
                    TempData["PaperUpdateMsg"] = true; // Sets error message
            }
            return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager action
        }



        // Action method for deleting a paper by the teacher
        public ActionResult DeletePaper(int id)
        {
            var row = db.Papers.Where(model => model.Id == id).FirstOrDefault(); // Retrieves paper by ID
            return View(row); // Returns the DeletePaper view with the paper details
        }



        // POST: Action method for submitting paper deletion by the teacher
        [HttpPost]
        public ActionResult DeletePaper(Paper p)
        {
            var questions = db.Questions.Where(q => q.PaperId == p.Id).ToList(); // Retrieves questions related to the paper
            foreach (var question in questions)
            {
                db.Questions.Remove(question); // Removes questions related to the paper
            }
            db.SaveChanges(); // Saves changes to the database
            db.Entry(p).State = EntityState.Deleted; // Sets the state of the entity to deleted
            int a = db.SaveChanges(); // Saves changes to the database
            if (a > 0)
            {
                TempData["PaperDeleteMsg"] = true; // Sets success message
                return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager action
            }
            else
            {
                TempData["PaperDeleteMsg"] = false; // Sets error message
            }
            return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager action
        }



        // Action method for creating a question for a paper by the teacher
        public ActionResult CreateQuestion(int? id)
        {
            return View(); // Returns the CreateQuestion view
        }



        // POST: Action method for submitting question creation for a paper by the teacher
        [HttpPost]
        public ActionResult CreateQuestion(Question q, int id)
        {
            if (ModelState.IsValid == true)
            {
                q.PaperId = Convert.ToInt32(id); // Sets PaperId for the question
                db.Questions.Add(q); // Adds question to the database
                int check = db.SaveChanges(); // Saves changes to the database
                if (check > 0)
                {
                    TempData["QuesCreateMsg"] = true; // Sets success message
                    ModelState.Clear(); // Clears model state
                    return RedirectToAction("PaperManager", "Teacher"); // Redirects to PaperManager action
                }
                else
                {
                    TempData["QuesDeleteMsg"] = false; // Sets error message
                }
            }
            return View(); // Returns CreateQuestion view
        }



        // Action method for editing a question by the teacher
        public ActionResult EditQuestion(int id)
        {
            var row = db.Questions.Where(model => model.Id == id).FirstOrDefault(); // Retrieves question by ID
            if (row == null)
            {
                return RedirectToAction("PaperManager"); // Redirects to PaperManager if question is not found
            }
            return View(row); // Returns the EditQuestion view with the question details
        }



        // POST: Action method for submitting question edit by the teacher
        [HttpPost]
        public ActionResult EditQuestion(Question q)
        {
            q.PaperId = Convert.ToInt32(TempData["PId"]); // Retrieves paper ID from TempData
            db.Entry(q).State = EntityState.Modified; // Sets the state of the entity to modified
            int a = db.SaveChanges(); // Saves changes to the database
            if (a > 0)
            {
                TempData["QuesUpdateMsg"] = true; // Sets success message
                return RedirectToAction("PaperManager"); // Redirects to PaperManager action
            }
            else
            {
                TempData["QuesUpdateMsg"] = false; // Sets error message
            }
            return RedirectToAction("PaperManager"); // Redirects to PaperManager action
        }



        // Action method for deleting a question by the teacher
        public ActionResult DeleteQuestion(int id)
        {
            var row = db.Questions.Where(model => model.Id == id).FirstOrDefault(); // Retrieves question by ID
            return View(row); // Returns the DeleteQuestion view with the question details
        }



        // POST: Action method for submitting question deletion by the teacher
        [HttpPost]
        public ActionResult DeleteQuestion(Question q)
        {
            db.Entry(q).State = EntityState.Deleted; // Sets the state of the entity to deleted
            int a = db.SaveChanges(); // Saves changes to the database
            if (a > 0)
            {
                TempData["QuesDeleteMsg"] = true; // Sets success message
                return RedirectToAction("PaperManager"); // Redirects to PaperManager action
            }
            else
            {
                TempData["QuesDeleteMsg"] = false; // Sets error message
            }
            return RedirectToAction("PaperManager"); // Redirects to PaperManager action
        }

        // Action method for showing results of attempts by all the students
        public ActionResult ShowResults()
        {
            return View(db.Answers.ToList());
        }
    }
}
