
using Exam_Management.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Exam_Management.Controllers
{
    [AllowAnonymous] // Allows anonymous access to this controller
    public class RegistrationController : Controller
    {
        ExamManagementEntities db = new ExamManagementEntities(); // Database context

        // Action method for logging out
        public ActionResult Logout()
        {
            Session.Clear(); // Clears session data
            FormsAuthentication.SignOut(); // Signs out from Forms Authentication
            TempData["LogoutMsg"] = true; // Sets success message
            return RedirectToAction("Login", "Registration"); // Redirects to login page
        }



        // Action method for displaying registration form
        public ActionResult Create()
        {
            return View(); // Returns the Create view
        }




        // POST: Create a new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User u)
        {
            if (Regex.IsMatch(u.Username, @"[^a-zA-Z0-9 ]"))
            {
                TempData["UsernameInvalid"] = true;
                return View();
            }

            User record = db.Users.FirstOrDefault(model => model.Email == u.Email);
            if(record != null)
            {
                TempData["Registration"] = false;
                return View();
            }

            u.PasswordHash = EncryptString(u.PasswordHash); // Encrypts the password
            db.Users.Add(u); // Adds the user to the database
            int check = db.SaveChanges(); // Saves changes to the database
            if (check > 0)
            {
                TempData["Registration"] = true; // Sets success message
                ModelState.Clear(); // Clears the model state
                return View(); // Returns the Create view
            }
            else
            {
                TempData["Registration"] = false; // Error message
            }
            return View(); // Returns the Create view
        }



        // Action method for displaying login form
        public ActionResult Login()
        {
            return View(); // Returns the Login view
        }



        // POST: Perform login
        [HttpPost]
        public ActionResult Login(User u)
        {
            User record = db.Users.FirstOrDefault(model => model.Email == u.Email); // Retrieves user record from the database
            if (record != null)
            {
                if (u.Email == record.Email && record.PasswordHash == EncryptString(u.PasswordHash))
                {
                    TempData["LoginMsg"] = true; // Sets success message
                    ModelState.Clear(); // Clears the model state
                    Session["Id"] = record.Id; // Stores user ID in session
                    Session["Role"] = record.Role; // Stores user role in session
                    FormsAuthentication.SetAuthCookie(record.Id.ToString(), false); // Sets authentication cookie

                    // Redirects based on user role
                    if (record.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (record.Role == "Teacher")
                    {
                        return RedirectToAction("Dashboard", "Teacher");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Student");
                    }
                }
            }
            TempData["LoginMsg"] = false; // Sets success message
            return View(); // Returns the Login view
        }



        // Method to encrypt string using AES encryption
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
                            swEncrypt.Write(plainText); // Writes encrypted text to stream
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray()); // Converts encrypted data to base64 string
                }
            }
        }
    }
}

