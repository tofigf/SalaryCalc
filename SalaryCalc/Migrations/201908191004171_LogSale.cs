namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogSale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "SaleId", c => c.Int());
            CreateIndex("dbo.Logs", "SaleId");
            AddForeignKey("dbo.Logs", "SaleId", "dbo.Sales", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "SaleId", "dbo.Sales");
            DropIndex("dbo.Logs", new[] { "SaleId" });
            DropColumn("dbo.Logs", "SaleId");
        }
    }
}
