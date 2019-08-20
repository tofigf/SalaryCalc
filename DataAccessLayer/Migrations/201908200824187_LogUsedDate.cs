namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogUsedDate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Logs", name: "CalcForumId", newName: "CalcForum_Id");
            RenameColumn(table: "dbo.Logs", name: "SaleId", newName: "Sale_Id");
            RenameIndex(table: "dbo.Logs", name: "IX_SaleId", newName: "IX_Sale_Id");
            RenameIndex(table: "dbo.Logs", name: "IX_CalcForumId", newName: "IX_CalcForum_Id");
            AddColumn("dbo.Logs", "UsedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Logs", "Action", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.Logs", "Controller", c => c.String());
            DropColumn("dbo.Logs", "Method");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Logs", "Method", c => c.String(nullable: false, maxLength: 250));
            DropColumn("dbo.Logs", "Controller");
            DropColumn("dbo.Logs", "Action");
            DropColumn("dbo.Logs", "UsedDate");
            RenameIndex(table: "dbo.Logs", name: "IX_CalcForum_Id", newName: "IX_CalcForumId");
            RenameIndex(table: "dbo.Logs", name: "IX_Sale_Id", newName: "IX_SaleId");
            RenameColumn(table: "dbo.Logs", name: "Sale_Id", newName: "SaleId");
            RenameColumn(table: "dbo.Logs", name: "CalcForum_Id", newName: "CalcForumId");
        }
    }
}
