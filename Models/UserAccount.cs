using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Data.Entity;

namespace CustomLogin.Models
{
    [Table("UserAccount")]
    public class UserAccount
    {
        [Key]
        public int UserID { get; set; }
        [Required(ErrorMessage ="First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
      
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        
        public string City { get; set; }
       
        public string Country { get; set; }
       
        public int mobile { get; set; }
        
        [DisplayName("my image")]

        public string image { get; set; }

      

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        [Compare("Password",ErrorMessage="please confirm your password.")]
        [DataType(dataType: DataType.Password)]
        public string ConfirmPassword { get; set; }

        public virtual ICollection<courses> posts { get; set; }
        
      
    }
   
   


   


}