namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPinCodStringlength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "PinCod", c => c.String(nullable: false, maxLength: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "PinCod", c => c.String(nullable: false, maxLength: 250));
        }
    }
}
