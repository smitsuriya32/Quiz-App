using Exam_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Exam_Management.Controllers
{
    // This is the admin controller which will be doing all the task in the app by their corresponding names of functions.

    [Authorize]
    public class AdminController : Controller
    {
        ExamManagementEntities db = new ExamManagementEntities();



        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }



        public ActionResult UserManager()
        {
            return View(db.Users.ToList());
        }



        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User u)
        {
            if (ModelState.IsValid == true)
            {

                u.PasswordHash = EncryptString(u.PasswordHash);
                db.Users.Add(u);
                int check = db.SaveChanges();
                if (check > 0)
                {
                    TempData["UserCreateMsg"] = true;
                    ModelState.Clear();
                    return RedirectToAction("UserManager", "Admin");
                }
                else
                {
                    TempData["UserCreateMsg"] = false;
                }
            }
            return View();
        }



        public ActionResult Edit(int id)
        {
            var row = db.Users.Where(model => model.Id == id).FirstOrDefault();
            return View(row);
        }



        [HttpPost]
        public ActionResult Edit(User u)
        {
            db.Entry(u).State = EntityState.Modified;
            int a = db.SaveChanges();
            if (a > 0)
            {
                TempData["UserUpdatedMsg"] = true;
                return RedirectToAction("UserManager");
            }
            else
            {
                TempData["UserUpdatedMsg"] = false;
            }
            return RedirectToAction("UserManager");
        }



        public ActionResult Delete(int id)
        {
            var row = db.Users.Where(model => model.Id == id).FirstOrDefault();
            return View(row);
        }



        [HttpPost]
        public ActionResult Delete(User user)
        {
            var delpapers = db.Papers.Where(model => model.CreatedBy == user.Id).ToList();
            foreach (var p in delpapers)
            {
                var delans = db.Answers.Where(model => model.PaperId == p.Id).ToList();
                db.Answers.RemoveRange(delans);
                db.SaveChanges();

                var delapproval = db.Approvals.Where(model => model.PaperID == p.Id).ToList();
                db.Approvals.RemoveRange(delapproval);
                db.SaveChanges();

                var delQuestions = db.Questions.Where(model => model.PaperId == p.Id).ToList();
                db.Questions.RemoveRange(delQuestions); 
                db.SaveChanges();

                db.Papers.Remove(p);
                db.SaveChanges();
            }

            var deleteans = db.Answers.Where(model => model.AttempterId == user.Id).ToList();
            db.Answers.RemoveRange(deleteans);
            db.SaveChanges();

            db.Entry(user).State = EntityState.Deleted;
            int a = db.SaveChanges();
            if (a > 0)
            {
                TempData["UserDeletedMsg"] = true;
                return RedirectToAction("UserManager");
            }
            else
            {
                TempData["UserDeletedMsg"] = true;
            }
            return RedirectToAction("UserManager");
        }



        public ActionResult PaperManager()
        {
            return View(db.Papers.ToList());
        }



        public ActionResult CreatePaper()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePaper(Paper p)
        {
            if (ModelState.IsValid == true)
            {
                p.CreationDate = DateTime.Now;
                p.Status = "Pending";
                p.CreatedBy = Convert.ToInt32(Session["Id"]);
                db.Papers.Add(p);
                int check = db.SaveChanges();
                if (check > 0)
                {
                    TempData["PaperCreateMsg"] = true;
                    ModelState.Clear();
                    return RedirectToAction("PaperManager", "Admin");
                }
                else
                {
                    TempData["PaperCreateMsg"] = false;
                }
            }
            return View();
        }



        public ActionResult ShowPaper(int id)
        {
            TempData["PId"] = id;
            Paper p = db.Papers.Find(id);
            if (p == null)
            {
                return RedirectToAction("PaperManager");
            }
            var questions = db.Questions.Where(q => q.PaperId == id).ToList();
            ViewBag.ques = questions;
            return View(p);
        }



        public ActionResult EditPaper(int id)
        {
            var row = db.Papers.Where(model => model.Id == id).FirstOrDefault();
            if (row == null)
            {
                return RedirectToAction("PaperManager");
            }
            return View(row);
        }



        [HttpPost]
        public ActionResult EditPaper(Paper p)
        {
            db.Entry(p).State = EntityState.Modified;
            int a = db.SaveChanges();
            if (a > 0)
            {
                TempData["PaperUpdateMsg"] = true;
                return RedirectToAction("PaperManager");
            }
            else
            {
                TempData["PaperUpdateMsg"] = false;
            }
            return RedirectToAction("PaperManager");
        }



        public ActionResult DeletePaper(int id)
        {
            var row = db.Papers.Where(model => model.Id == id).FirstOrDefault();
            return View(row);
        }

        [HttpPost]
        public ActionResult DeletePaper(Paper p)
        {
            var delans = db.Answers.Where(model => model.PaperId == p.Id).ToList();
            db.Answers.RemoveRange(delans);
            db.SaveChanges();

            var delapproval = db.Approvals.Where(model => model.PaperID == p.Id).ToList();
            db.Approvals.RemoveRange(delapproval);
            db.SaveChanges();

            var questions = db.Questions.Where(q => q.PaperId == p.Id).ToList();
            foreach (var question in questions)
            {
                db.Questions.Remove(question);
            }
            db.SaveChanges();
            db.Entry(p).State = EntityState.Deleted;
            int a = db.SaveChanges();
            if (a > 0)
            {
                TempData["PaperDeleteMsg"] = true;
                return RedirectToAction("PaperManager");
            }
            else
            {
                TempData["PaperDeleteMsg"] = false;
            }
            return RedirectToAction("PaperManager");
        }



        public ActionResult CreateQuestion(int? id)
        {
            return View();
        }



        [HttpPost]
        public ActionResult CreateQuestion(Question q, int id)
        {
            if (ModelState.IsValid == true)
            {
                q.PaperId = Convert.ToInt32(id);
                db.Questions.Add(q);
                int check = db.SaveChanges();
                if (check > 0)
                {
                    TempData["QuesCreateMsg"] = true;
                    ModelState.Clear();
                    return RedirectToAction("PaperManager", "Admin");
                }
                else
                {
                    TempData["QuesCreateMsg"]=false;
                }
            }
            return View();
        }



        public ActionResult EditQuestion(int id)
        {
            var row = db.Questions.Where(model => model.Id == id).FirstOrDefault();
            if (row == null)
            {
                return RedirectToAction("PaperManager");
            }
            return View(row);
        }



        [HttpPost]
        public ActionResult EditQuestion(Question q)
        {
            q.PaperId = Convert.ToInt32(TempData["PId"]);
            db.Entry(q).State = EntityState.Modified;
            int a = db.SaveChanges();
            if (a > 0)
            {
                TempData["QuesUpdateMsg"] = true;
                return RedirectToAction("PaperManager");
            }
            else
            {
                TempData["QuesUpdateMsg"] = true;
            }
            return RedirectToAction("PaperManager");
        }



        public ActionResult DeleteQuestion(int id)
        {
            var row = db.Questions.Where(model => model.Id == id).FirstOrDefault();
            return View(row);
        }



        [HttpPost]
        public ActionResult DeleteQuestion(Question q)
        {
            db.Entry(q).State = EntityState.Deleted;
            int a = db.SaveChanges();
            if (a > 0)
            {
                TempData["QuesDeleteMsg"] = true;
                return RedirectToAction("PaperManager");
            }
            else
            {
                TempData["QuesDeleteMsg"] = false;
            }
            return RedirectToAction("PaperManager");
        }



        public ActionResult ApprovePaper(int id)
        {
            var paper = db.Papers.Find(id);
            if (paper != null)
            {
                paper.Status = "Approved";
            }
            Approval approval = new Approval();
            approval.ApproverID = Convert.ToInt32(Session["Id"]);
            approval.PaperID = id;
            approval.ApprovalDate = DateTime.Now;
            db.Approvals.Add(approval);
            db.SaveChanges();
            return RedirectToAction("PaperManager");
        }



        public ActionResult RejectPaper(int id)
        {
            var paper = db.Papers.Find(id);
            if (paper != null)
            {
                paper.Status = "Rejected";
            }
            db.SaveChanges();
            return RedirectToAction("PaperManager");
        }



        // Action method for showing results of attempts by all the students
        public ActionResult ShowResults()
        {
            return View(db.Answers.ToList());
        }



        private string EncryptString(string plainText)
        {
            const string EncryptionKey = "qWE7&5pZ@2#9Df!1gH*3sKl$8oP5mN^0";

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[16];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }
}