namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserAdminColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "IsAdmin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "IsAdmin", c => c.Byte(nullable: false));
        }
    }
}
