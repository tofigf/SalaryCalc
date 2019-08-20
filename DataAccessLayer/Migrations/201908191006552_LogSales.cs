namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogSales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogSales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldPrice = c.Double(nullable: false),
                        OldName = c.String(maxLength: 150),
                        OLdVip = c.Boolean(nullable: false),
                        OLdDisCount = c.Boolean(nullable: false),
                        OldCount = c.Int(nullable: false),
                        OldIsComfirmed = c.Boolean(nullable: false),
                        OldIsImported = c.Boolean(nullable: false),
                        LogId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Logs", t => t.LogId)
                .Index(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogSales", "LogId", "dbo.Logs");
            DropIndex("dbo.LogSales", new[] { "LogId" });
            DropTable("dbo.LogSales");
        }
    }
}
