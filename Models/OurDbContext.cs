using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
namespace CustomLogin.Models
{
    public class OurDbContext : DbContext
    { 
        public DbSet<UserAccount> userAccount { get; set; }
        public DbSet<courses> course { get; set; }
        public DbSet<Admin> admin { get; set; }
        public DbSet<student_course> sub { get; set; }


    }
}