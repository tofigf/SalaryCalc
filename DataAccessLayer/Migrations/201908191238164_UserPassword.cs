namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPassword : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 150));
        }
    }
}
