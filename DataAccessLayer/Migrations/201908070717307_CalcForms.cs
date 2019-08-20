namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CalcForms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CalcForums",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Formula = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CalcForums");
        }
    }
}
