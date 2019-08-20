namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaleCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "Count", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "Count");
        }
    }
}
