using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessBourneV2.Models;

namespace FitnessBourneV2.Controllers
{
    public class AdminController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();
        // GET: Admin
        public ActionResult Index()
        {
            List<memberObject> objList = new List<memberObject>();
            foreach(MemberTable tableObj in db.MemberTables.ToList())
            {
                memberObject obj = new memberObject()
                {
                    mem_EmailID = tableObj.Mem_Email_Id,
                    mem_ContactNum = tableObj.Mem_Contact_No,
                    mem_id = tableObj.Mem_Id
                };
                objList.Add(obj);
            }

            AdminDemo modelToPass = new AdminDemo()
            {
                memberList = objList
            };

            return View(modelToPass);
        }

        public ActionResult deleteMember(Int32 memIDVal)
        {
            return RedirectToAction("Index", "Admin");
        }
    }
}