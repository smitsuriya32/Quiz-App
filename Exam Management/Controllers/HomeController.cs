/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exam_Management.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exam_Management.Controllers
{
    // Controller for handling home-related actions
    public class HomeController : Controller
    {
        // Action method for rendering the home page
        public ActionResult Index()
        {
            return View(); // Returns the Index view
        }

        // Action method for rendering the about page
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page."; // Sets a message for the view

            return View(); // Returns the About view
        }

        // Action method for rendering the contact page
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page."; // Sets a message for the view

            return View(); // Returns the Contact view
        }
    }
}
