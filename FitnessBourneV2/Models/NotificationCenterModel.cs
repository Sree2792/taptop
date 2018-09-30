using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitnessBourneV2.Models
{
    public class NotificationCenterModel
    {
        public List<NotificationObject> listOfNotif { get; set; }
    }

    public class NotificationObject
    {
        public string notifMessage { get; set; }
        public string eventType { get; set; }
        public string startLoc { get; set; }
        public string stopLoc { get; set; }
        public string startDT { get; set; }
        public string stopDT { get; set; }
        public bool isEventAdmin { get; set; }
        public string youAre { get; set; }
    }
}