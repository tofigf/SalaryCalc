namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Calcforum_Name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CalcForums", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.CalcForums", "Name", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CalcForums", "Name");
            DropColumn("dbo.CalcForums", "Date");
        }
    }
}
