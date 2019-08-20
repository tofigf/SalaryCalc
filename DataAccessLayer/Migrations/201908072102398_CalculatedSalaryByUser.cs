namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CalculatedSalaryByUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "CalcForumId", "dbo.CalcForums");
            DropIndex("dbo.Users", new[] { "CalcForumId" });
            CreateTable(
                "dbo.CalculatedSalaryByUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Salary = c.Double(nullable: false),
                        UserId = c.Int(nullable: false),
                        CalcForumId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CalcForums", t => t.CalcForumId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CalcForumId);
            
            DropColumn("dbo.Users", "CalcForumId");
        }
        
        public override void Down()
        {
          
            DropForeignKey("dbo.CalculatedSalaryByUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.CalculatedSalaryByUsers", "CalcForumId", "dbo.CalcForums");
            DropIndex("dbo.CalculatedSalaryByUsers", new[] { "CalcForumId" });
            DropIndex("dbo.CalculatedSalaryByUsers", new[] { "UserId" });
            DropTable("dbo.CalculatedSalaryByUsers");
    
           
        }
    }
}
