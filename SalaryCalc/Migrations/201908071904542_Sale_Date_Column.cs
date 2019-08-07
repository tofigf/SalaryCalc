namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sale_Date_Column : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "Date");
        }
    }
}
