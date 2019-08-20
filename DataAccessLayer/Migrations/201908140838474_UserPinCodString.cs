namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPinCodString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "PinCod", c => c.String(nullable: false, maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "PinCod", c => c.Int(nullable: false));
        }
    }
}
