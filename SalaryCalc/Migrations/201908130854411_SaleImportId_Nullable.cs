namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaleImportId_Nullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Sales", new[] { "SaleImportId" });
            AlterColumn("dbo.Sales", "SaleImportId", c => c.Int());
            CreateIndex("dbo.Sales", "SaleImportId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Sales", new[] { "SaleImportId" });
            AlterColumn("dbo.Sales", "SaleImportId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sales", "SaleImportId");
        }
    }
}
