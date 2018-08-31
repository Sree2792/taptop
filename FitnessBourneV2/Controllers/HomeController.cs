using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnessBourneV2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult HomeIndex()
        {
            return View();
        }
    }
}