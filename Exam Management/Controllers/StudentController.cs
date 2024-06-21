using Exam_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exam_Management.Controllers
{
    [Authorize] // Requires authorization to access this controller
    public class StudentController : Controller
    {
        ExamManagementEntities db = new ExamManagementEntities(); // Database context



        // Action method for displaying the student dashboard
        public ActionResult Dashboard()
        {
            return View(); // Returns the Dashboard view
        }



        // Action method for logging out the student
        public ActionResult Logout()
        {
            Session.Clear(); // Clears session data
            return RedirectToAction("Login", "Registration"); // Redirects to login page
        }



        // Action method for displaying the paper manager for the student
        public ActionResult PaperManager()
        {
            var papers = db.Papers.Where(model => model.Status == "Approved").ToList(); // Retrieves approved papers
            return View(papers); // Returns the PaperManager view with the list of papers
        }



        // Action method for showing a specific paper to the student
        public ActionResult ShowPaper(int id)
        {
            Paper p = db.Papers.Find(id); // Finds the paper by ID
            if (p == null)
            {
                return RedirectToAction("PaperManager", "Student"); // Redirects to PaperManager if paper is not found
            }
            var questions = db.Questions.Where(q => q.PaperId == id).ToList(); // Retrieves questions related to the paper
            ViewBag.ques = questions; // Passes questions to the view
            return View(p); // Returns the ShowPaper view with the paper and questions
        }



        // Action method for managing attempts for a paper by the student
        public ActionResult AttemptManager(int id)
        {
            Paper p = db.Papers.Find(id); // Finds the paper by ID
            if (p == null)
            {
                return RedirectToAction("PaperManager", "Student"); // Redirects to PaperManager if paper is not found
            }
            var questions = db.Questions.Where(q => q.PaperId == id).ToList(); // Retrieves questions related to the paper
            ViewBag.ques = questions; // Passes questions to the view
            return View(p); // Returns the AttemptManager view with the paper and questions
        }



        // POST: Action method for submitting attempts by the student
        [HttpPost]
        public ActionResult AttemptManager(int id, FormCollection data)
        {
            // Fetching the Correct Answers to store
            var questions = db.Questions.Where(q => q.PaperId == id).ToList();
            string[] answerArray = new string[questions.Count];
            var index = 0;
            foreach (var q in questions)
            {
                answerArray[index] = q.CorrectAnswer;
                index++;
            }

            // Iterates through submitted form data
            for (int i = 1; i < data.Count; i++)
            {
                int qid = Convert.ToInt32(data.GetKey(i)); // Gets question ID
                string value = data[i]; // Gets attempted answer
                Answer ans = new Answer(); // Creates new Answer object
                ans.PaperId = id; // Sets PaperId
                ans.AttempterId = Convert.ToInt32(Session["Id"]); // Sets AttempterId from session
                ans.QuestionId = qid; // Sets QuestionId
                ans.CorrectAnswer = Convert.ToString(answerArray[i-1]); // Initializes CorrectAnswer as null
                ans.AttemptedAnswer = value; // Sets AttemptedAnswer
                ans.AttemptDate = DateTime.Now; // Sets AttemptDate to current date and time
                db.Answers.Add(ans); // Adds answer to database
                db.SaveChanges(); // Saves changes to database
            }
            TempData["PaperSubmitMsg"] = true;
            return RedirectToAction("ShowResults", new { id = id }); // Redirects to ShowResults action
        }



        // Action method for showing results of attempts by the student
        public ActionResult ShowResults(int id)
        {
            int UserId = Convert.ToInt32(Session["Id"]); // Retrieves user ID from session
            var answers = db.Answers.Where(model => model.PaperId == id && model.User.Id == UserId).ToList(); // Retrieves answers for the paper and user
            return View(answers); // Returns the ShowResults view with answers
        }


        // Action method for showing results of attempts by the student
        public ActionResult ShowSubmittedAnswers(int id)
        {
            var answers = db.Answers.Where(model => model.AttempterId == id).ToList(); // Retrieves answers for the paper and user
            return View(answers); // Returns the ShowResults view with answers
        }
    }
}
