namespace CustomLogin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class future : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.courses",
                c => new
                    {
                        courseID = c.Int(nullable: false, identity: true),
                        courseName = c.String(nullable: false),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        cost = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Status = c.String(nullable: false),
                        CourseSyllabus = c.String(),
                    })
                .PrimaryKey(t => t.courseID);
            
            CreateTable(
                "dbo.UserAccount",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Username = c.String(nullable: false),
                        City = c.String(),
                        Country = c.String(),
                        mobile = c.Int(nullable: false),
                        image = c.String(),
                        Password = c.String(nullable: false),
                        ConfirmPassword = c.String(),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.student_course",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        courseID = c.Int(nullable: false),
                        studentId = c.Int(nullable: false),
                        student_UserID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.courses", t => t.courseID, cascadeDelete: true)
                .ForeignKey("dbo.UserAccount", t => t.student_UserID)
                .Index(t => t.courseID)
                .Index(t => t.student_UserID);
            
            CreateTable(
                "dbo.UserAccountcourses",
                c => new
                    {
                        UserAccount_UserID = c.Int(nullable: false),
                        courses_courseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserAccount_UserID, t.courses_courseID })
                .ForeignKey("dbo.UserAccount", t => t.UserAccount_UserID, cascadeDelete: true)
                .ForeignKey("dbo.courses", t => t.courses_courseID, cascadeDelete: true)
                .Index(t => t.UserAccount_UserID)
                .Index(t => t.courses_courseID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.student_course", "student_UserID", "dbo.UserAccount");
            DropForeignKey("dbo.student_course", "courseID", "dbo.courses");
            DropForeignKey("dbo.UserAccountcourses", "courses_courseID", "dbo.courses");
            DropForeignKey("dbo.UserAccountcourses", "UserAccount_UserID", "dbo.UserAccount");
            DropIndex("dbo.UserAccountcourses", new[] { "courses_courseID" });
            DropIndex("dbo.UserAccountcourses", new[] { "UserAccount_UserID" });
            DropIndex("dbo.student_course", new[] { "student_UserID" });
            DropIndex("dbo.student_course", new[] { "courseID" });
            DropTable("dbo.UserAccountcourses");
            DropTable("dbo.student_course");
            DropTable("dbo.UserAccount");
            DropTable("dbo.courses");
            DropTable("dbo.Admins");
        }
    }
}
