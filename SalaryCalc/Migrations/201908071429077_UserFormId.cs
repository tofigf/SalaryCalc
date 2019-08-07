namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserFormId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CalcFormId", c => c.Int());
            AddColumn("dbo.Users", "CalcForum_Id", c => c.Int());
            CreateIndex("dbo.Users", "CalcForum_Id");
            AddForeignKey("dbo.Users", "CalcForum_Id", "dbo.CalcForums", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "CalcForum_Id", "dbo.CalcForums");
            DropIndex("dbo.Users", new[] { "CalcForum_Id" });
            DropColumn("dbo.Users", "CalcForum_Id");
            DropColumn("dbo.Users", "CalcFormId");
        }
    }
}
