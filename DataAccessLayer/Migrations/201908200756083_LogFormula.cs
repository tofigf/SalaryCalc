namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogFormula : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogCalcForums",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldFormula = c.String(maxLength: 500),
                        OldName = c.String(maxLength: 200),
                        LogId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Logs", t => t.LogId)
                .Index(t => t.LogId);
            
            AddColumn("dbo.Logs", "CalcForumId", c => c.Int());
            CreateIndex("dbo.Logs", "CalcForumId");
            AddForeignKey("dbo.Logs", "CalcForumId", "dbo.CalcForums", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogCalcForums", "LogId", "dbo.Logs");
            DropForeignKey("dbo.Logs", "CalcForumId", "dbo.CalcForums");
            DropIndex("dbo.LogCalcForums", new[] { "LogId" });
            DropIndex("dbo.Logs", new[] { "CalcForumId" });
            DropColumn("dbo.Logs", "CalcForumId");
            DropTable("dbo.LogCalcForums");
        }
    }
}
