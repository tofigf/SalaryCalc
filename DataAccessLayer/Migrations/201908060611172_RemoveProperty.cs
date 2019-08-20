namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProperty : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Sales", "MyProperty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sales", "MyProperty", c => c.Int(nullable: false));
        }
    }
}
