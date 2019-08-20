namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Phone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Phone", c => c.String(nullable: false, maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Phone");
        }
    }
}
