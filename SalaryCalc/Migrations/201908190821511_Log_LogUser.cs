namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Log_LogUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrentUserId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Method = c.String(nullable: false, maxLength: 250),
                        ActionedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ActionedUserId)
                .ForeignKey("dbo.Users", t => t.CurrentUserId)
                .Index(t => t.CurrentUserId)
                .Index(t => t.ActionedUserId);
            
            CreateTable(
                "dbo.LogUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldUserName = c.String(maxLength: 150),
                        OldFullName = c.String(maxLength: 150),
                        OldEmail = c.String(maxLength: 150),
                        OldPhone = c.String(maxLength: 150),
                        OldPinCod = c.String(maxLength: 150),
                        LogId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Logs", t => t.LogId)
                .Index(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "CurrentUserId", "dbo.Users");
            DropForeignKey("dbo.Logs", "ActionedUserId", "dbo.Users");
            DropForeignKey("dbo.LogUsers", "LogId", "dbo.Logs");
            DropIndex("dbo.LogUsers", new[] { "LogId" });
            DropIndex("dbo.Logs", new[] { "ActionedUserId" });
            DropIndex("dbo.Logs", new[] { "CurrentUserId" });
            DropTable("dbo.LogUsers");
            DropTable("dbo.Logs");
        }
    }
}
