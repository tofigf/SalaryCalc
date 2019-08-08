namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserCalcForum : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CalculatedSalaryByUsers", "CalcForumId", "dbo.CalcForums");
            DropIndex("dbo.CalculatedSalaryByUsers", new[] { "CalcForumId" });
            AddColumn("dbo.Users", "CalcForumId", c => c.Int());
            CreateIndex("dbo.Users", "CalcForumId");
            AddForeignKey("dbo.Users", "CalcForumId", "dbo.CalcForums", "Id");
            DropColumn("dbo.CalculatedSalaryByUsers", "CalcForumId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CalculatedSalaryByUsers", "CalcForumId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Users", "CalcForumId", "dbo.CalcForums");
            DropIndex("dbo.Users", new[] { "CalcForumId" });
            DropColumn("dbo.Users", "CalcForumId");
            CreateIndex("dbo.CalculatedSalaryByUsers", "CalcForumId");
            AddForeignKey("dbo.CalculatedSalaryByUsers", "CalcForumId", "dbo.CalcForums", "Id");
        }
    }
}
