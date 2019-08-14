namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPinCod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "PinCod", c => c.String(nullable: false, maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "PinCod");
        }
    }
}
