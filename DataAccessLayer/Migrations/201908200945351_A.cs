namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class A : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogCalcSalaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldSalary = c.Double(nullable: false),
                        UserId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        LogId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Logs", t => t.LogId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.LogId);
            
            AlterColumn("dbo.Logs", "UsedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogCalcSalaries", "UserId", "dbo.Users");
            DropForeignKey("dbo.LogCalcSalaries", "LogId", "dbo.Logs");
            DropIndex("dbo.LogCalcSalaries", new[] { "LogId" });
            DropIndex("dbo.LogCalcSalaries", new[] { "UserId" });
            AlterColumn("dbo.Logs", "UsedDate", c => c.DateTime(nullable: false));
            DropTable("dbo.LogCalcSalaries");
        }
    }
}
