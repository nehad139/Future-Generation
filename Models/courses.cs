using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CustomLogin.Models
{
    public class courses
    {
        [Key]
        public int courseID { get; set; }

        [Required(ErrorMessage = "Course Name is required")]
        public string courseName { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        public System.DateTime startDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public System.DateTime endDate { get; set; }

        [Required(ErrorMessage = "Cost is required")]
        public int cost { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        [DisplayName("Course syllaby")]
        public string CourseSyllabus  { get; set; }

        public virtual List<UserAccount> UserAccount { get; set; }


    }
}