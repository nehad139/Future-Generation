SELECT[courseName],[startDate],[endDate],[cost],[Capacity],[Status],[courseSyllaby] FROM [dbo].[courses] WHERE [UserID]=3024AND[OnlyMe] =0
ALTER DATABASE courses
SET ENABLE_Broker WITH ROLLBACK IMMEDIATE