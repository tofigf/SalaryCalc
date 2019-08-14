namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPinCodInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "PinCod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "PinCod", c => c.String(nullable: false, maxLength: 250));
        }
    }
}
