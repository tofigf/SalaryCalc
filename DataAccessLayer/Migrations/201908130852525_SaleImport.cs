namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaleImport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SaleImports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(maxLength: 250),
                        FileUrl = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Sales", "IsComfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sales", "IsImported", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sales", "SaleImportId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sales", "SaleImportId");
            AddForeignKey("dbo.Sales", "SaleImportId", "dbo.SaleImports", "Id");
            DropColumn("dbo.Sales", "FileUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sales", "FileUrl", c => c.String(maxLength: 250));
            DropForeignKey("dbo.Sales", "SaleImportId", "dbo.SaleImports");
            DropIndex("dbo.Sales", new[] { "SaleImportId" });
            DropColumn("dbo.Sales", "SaleImportId");
            DropColumn("dbo.Sales", "IsImported");
            DropColumn("dbo.Sales", "IsComfirmed");
            DropTable("dbo.SaleImports");
        }
    }
}
