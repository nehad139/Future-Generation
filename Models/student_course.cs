using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomLogin.Models
{
    public class student_course
    {
        [Key]
        public int ID { get; set; }
     
        public int courseID { get; set; }
        public virtual courses course { get; set; }


        public int studentId { get; set; }
        public virtual UserAccount student { get; set; }

    }
}