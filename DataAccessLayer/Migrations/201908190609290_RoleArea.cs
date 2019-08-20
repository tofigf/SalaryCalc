namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleArea : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Roles", "Area");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "Area", c => c.String(maxLength: 150));
        }
    }
}
