namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PositionIsAdmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Postions", "IsAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Postions", "IsAdmin");
        }
    }
}
