namespace SalaryCalc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ButtonsStatic : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ButtonsStatics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        Key = c.String(nullable: false, maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ButtonsStatics");
        }
    }
}
