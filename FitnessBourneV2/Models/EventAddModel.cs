using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FitnessBourneV2.Models
{
    public class EventAddModel
    {
        public int eventID { get; set; }

        [Required]
        [Display(Name = "Event Type")]
        public string eventTypeName { get; set; }

        [Display(Name = "Is the event private to your club members?")]
        public bool isPrivate { get; set; }
        public bool isCheckIn { get; set; }

        [Required]
        [Display(Name = "Event start time")]
        public String startDateTime { get; set; }

        [Required]
        [Display(Name = "Event end time")]
        public String endDateTime { get; set; }

        [Required]
        [Display(Name = "Event capacity")]
        public string mem_capacity { get; set; }

        [Display(Name = "Club Name")]
        public Int32 clubName { get; set; }

        public MemberTable eventAdmin { get; set; }
        public EventType eventType { get; set; }
        public EventEdit editedEvent { get; set; }
        public bool isEditMode { get; set; } 
        public FitnessClub clubList { get; set; }
        public List<EventType> eventTypeOptions { get; set; }

    }
}