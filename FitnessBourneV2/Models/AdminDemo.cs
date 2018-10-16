using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitnessBourneV2.Models
{
    public class AdminDemo
    {
        public List<memberObject> memberList { get; set; }
    }

    public class memberObject
    {
        public string mem_EmailID { get; set; }
        public string mem_ContactNum { get; set; }
        public Int32 mem_id { get; set; }
    }
}