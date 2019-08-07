namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_positionId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Users", name: "CalcForum_Id", newName: "CalcForumId");
            RenameIndex(table: "dbo.Users", name: "IX_CalcForum_Id", newName: "IX_CalcForumId");
            DropColumn("dbo.Users", "CalcFormId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CalcFormId", c => c.Int());
            RenameIndex(table: "dbo.Users", name: "IX_CalcForumId", newName: "IX_CalcForum_Id");
            RenameColumn(table: "dbo.Users", name: "CalcForumId", newName: "CalcForum_Id");
        }
    }
}
